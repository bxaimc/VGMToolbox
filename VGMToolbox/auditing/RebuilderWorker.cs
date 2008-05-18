using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using System.Windows.Forms;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

using VGMToolbox.format;
using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.auditing
{
    class RebuilderWorker : BackgroundWorker
    {
        private Dictionary<string, ByteArray> libHash;
        private const string CACHE_DB_FILENAME = "cache.db";
        private ArrayList libFilesForDeletion = new ArrayList();
        private ArrayList tempDirsForDeletion = new ArrayList();
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct RebuildSetsStruct
        { 
            public string pSourceDir; 
            public string pDestinationDir; 
            public datafile pDatFile;
            public bool pRemoveSource; 
            public bool pOverwriteExisting;
            public bool pStreamInput; 
            public bool pCompressOutput;
            public int totalFiles;

        }

        public RebuilderWorker()
        {
            fileCount = 0;
            maxFiles = 0;

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private string buildFilePath(string pSetName, string pFilePath)
        {
            return (pSetName + Path.DirectorySeparatorChar + pFilePath);
        }

        private void deleteQueuedLibFiles()
        {
            foreach (string f in libFilesForDeletion.ToArray(typeof(string)))
            {
                File.Delete(f);
                libFilesForDeletion.Remove(f);
            }
        }

        private void deleteQueuedTempDirs()
        {
            foreach (string d in tempDirsForDeletion.ToArray(typeof(string)))
            {
                Directory.Delete(d, true);
                tempDirsForDeletion.Remove(d);
            }
        }

        private void moveFile(string pDestination, ArrayList pDestinationFiles, FileStream pSourceFile,
            string pSourceName, bool pOverwriteExisting, AuditingUtil pAuditingUtil, bool pCompressOutput)
        {
            int readSize = 0;
            byte[] data = new byte[4096];
            string pZipFilePath = String.Empty;

            try
            {
                foreach (AuditingUtil.ChecksumStruct cs in pDestinationFiles)
                {
                    Application.DoEvents();
                    string filePath = buildFilePath(cs.game, cs.rom);
                    string path = pDestination + filePath.Substring(0, filePath.LastIndexOf(Path.DirectorySeparatorChar));

                    if (pCompressOutput)
                    {
                        pZipFilePath = pDestination + cs.game + ".zip";
                        pSourceFile.Seek(0, SeekOrigin.Begin);

                        ZipFile zf;
                        if (File.Exists(pZipFilePath))
                        {
                            zf = new ZipFile(pZipFilePath);
                        }
                        else
                        {
                            zf = ZipFile.Create(pZipFilePath);
                        }

                        zf.BeginUpdate();
                        zf.Add(new FileDataSource(pSourceName), cs.rom);
                        zf.CommitUpdate();
                        zf.Close();
                    }
                    else
                    {
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        if (pOverwriteExisting || !File.Exists(pDestination + filePath))
                        {
                            File.Delete(pDestination + filePath);
                            FileStream streamWriter = File.Create(pDestination + filePath);
                            pSourceFile.Seek(0, SeekOrigin.Begin);

                            while (true)
                            {
                                readSize = pSourceFile.Read(data, 0, data.Length);
                                if (readSize > 0)
                                {
                                    streamWriter.Write(data, 0, readSize);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            streamWriter.Close();
                            streamWriter.Dispose();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AuditingUtil.ProgressStruct exProgressStruct = new AuditingUtil.ProgressStruct();
                exProgressStruct.filename = pSourceName;
                exProgressStruct.errorMessage = "Error processing <" + pSourceName + "> " + exception.Message;
                ReportProgress(AuditingUtil.IGNORE_PROGRESS, exProgressStruct);

            }
        }

        private void rebuildFile(string pFilePath, RebuildSetsStruct pRebuildSetsStruct, AuditingUtil pAuditingUtil, 
            DoWorkEventArgs ea)
        {
            ArrayList pDestinationFiles = new ArrayList();
            bool isFileLibrary = false;
            Type formatType = null;

            if (FormatUtil.IsZipFile(pFilePath))
            {
                FastZip fz = new FastZip();
                string tempDir = System.IO.Path.GetTempPath() + Path.GetFileNameWithoutExtension(pFilePath);
                fz.ExtractZip(pFilePath, tempDir, String.Empty);

                tempDirsForDeletion.Add(tempDir);           // Add in case of cancel button
                
                RebuildSetsStruct zipRebuildSetsStruct = pRebuildSetsStruct;
                zipRebuildSetsStruct.pSourceDir = tempDir;
                zipRebuildSetsStruct.totalFiles = Directory.GetFiles(tempDir, "*.*", SearchOption.AllDirectories).Length;
                this.rebuildSets(zipRebuildSetsStruct, pAuditingUtil, 0, ea);

                if (!CancellationPending)
                {
                    Directory.Delete(tempDir, true);
                    tempDirsForDeletion.Remove(tempDir);
                }
            }
            else
            {
                FileStream fs = File.OpenRead(pFilePath);
                formatType = FormatUtil.getObjectType(fs);
                fs.Seek(0, SeekOrigin.Begin);

                // CRC32
                string crc32Value = String.Empty;
                Crc32 crc32Generator = new Crc32();

                // @TODO - Change to file streams?  Out of memory errors on repeat runs

                /*
                // MD5
                string md5FileName = Path.GetTempFileName();
                MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider();
                //MemoryStream md5MemoryStream = new MemoryStream();
                FileStream md5MemoryStream = new FileStream(md5FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                CryptoStream md5CryptoStream = new CryptoStream(md5MemoryStream, md5Hash, CryptoStreamMode.Write);

                // SHA1
                string sha1FileName = Path.GetTempFileName();
                SHA1CryptoServiceProvider sha1Hash = new SHA1CryptoServiceProvider();
                //MemoryStream sha1MemoryStream = new MemoryStream();
                FileStream sha1MemoryStream = new FileStream(sha1FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                CryptoStream sha1CryptoStream = new CryptoStream(sha1MemoryStream, sha1Hash, CryptoStreamMode.Write);
                */


                if (formatType != null)
                {
                    try
                    {
                        IFormat vgmData = (IFormat)Activator.CreateInstance(formatType);

                        if (!pRebuildSetsStruct.pStreamInput)
                        {
                            int dataArrayindex = -1;
                            ByteArray dataArray = ObjectPooler.Instance.GetFreeByteArray(ref dataArrayindex);

                            ParseFile.ReadWholeArray(fs, dataArray.ByArray, (int)fs.Length);
                            dataArray.ArrayLength = (int)fs.Length;

                            vgmData.initialize(dataArray);

                            ObjectPooler.Instance.DoneWithByteArray(dataArrayindex);
                        }
                        else
                        {
                            vgmData.initialize(fs);
                        }

                        isFileLibrary = vgmData.IsFileLibrary(pFilePath);
                        // vgmData.getDatFileCrc32(pFilePath, ref libHash, ref crc32Generator,
                        //    ref md5CryptoStream, ref sha1CryptoStream, false, pStreamInput);                    
                        vgmData.getDatFileCrc32(pFilePath, ref libHash, ref crc32Generator,
                            false, pRebuildSetsStruct.pStreamInput);
                        vgmData = null;
                    }
                    catch (EndOfStreamException e)
                    {
                        AuditingUtil.ProgressStruct exProgressStruct = new AuditingUtil.ProgressStruct();
                        exProgressStruct.filename = pFilePath;
                        exProgressStruct.errorMessage = String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: {2}", pFilePath, formatType.Name, e.Message) + Environment.NewLine + Environment.NewLine;
                        ReportProgress(AuditingUtil.IGNORE_PROGRESS, exProgressStruct);

                        // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator,
                        //    ref md5CryptoStream, ref sha1CryptoStream);
                        ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                    }
                    catch (System.OutOfMemoryException e)
                    {
                        AuditingUtil.ProgressStruct exProgressStruct = new AuditingUtil.ProgressStruct();
                        exProgressStruct.filename = pFilePath;
                        exProgressStruct.errorMessage = String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: {2}", pFilePath, formatType.Name, e.Message) + Environment.NewLine + Environment.NewLine;
                        ReportProgress(AuditingUtil.IGNORE_PROGRESS, exProgressStruct);

                        // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator,
                        //    ref md5CryptoStream, ref sha1CryptoStream);
                        ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                    }
                    catch (IOException e)
                    {
                        AuditingUtil.ProgressStruct exProgressStruct = new AuditingUtil.ProgressStruct();
                        exProgressStruct.filename = pFilePath;
                        exProgressStruct.errorMessage = String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: {2}", pFilePath, formatType.Name, e.Message) + Environment.NewLine + Environment.NewLine;
                        ReportProgress(AuditingUtil.IGNORE_PROGRESS, exProgressStruct);

                        // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator,
                        //    ref md5CryptoStream, ref sha1CryptoStream);
                        ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                    }
                }
                else
                {
                    // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator,
                    //    ref md5CryptoStream, ref sha1CryptoStream);
                    ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                }

                // @TODO Add MD5/SHA1 to make checksum hash correct String(CRC32 + MD5 + SHA1)
                crc32Value = crc32Generator.Value.ToString("X2");

                pDestinationFiles = (ArrayList)pAuditingUtil.ChecksumHash[crc32Value];
                if (pDestinationFiles != null)
                {
                    this.moveFile(pRebuildSetsStruct.pDestinationDir, pDestinationFiles, fs, pFilePath, pRebuildSetsStruct.pOverwriteExisting,
                        pAuditingUtil, pRebuildSetsStruct.pCompressOutput);
                    pAuditingUtil.AddChecksumToCache(crc32Value);
                }

                fs.Close();
                fs.Dispose();

                /*
                md5CryptoStream.Close();
                md5CryptoStream.Dispose();
                sha1CryptoStream.Close();
                sha1CryptoStream.Dispose();
            
                md5MemoryStream.Close();
                md5MemoryStream.Dispose();
                sha1MemoryStream.Close();
                sha1MemoryStream.Dispose();
           
                File.Delete(md5FileName);
                File.Delete(sha1FileName);
                */

                // Remove Source only if copied
                if (pDestinationFiles != null && pRebuildSetsStruct.pRemoveSource && File.Exists(pFilePath))
                {
                    if (!isFileLibrary)
                    {
                        File.Delete(pFilePath);
                    }
                    else // Add to List for deletion later
                    {
                        libFilesForDeletion.Add(pFilePath);
                    }
                }
            } // (FormatUtil.IsZipFile(pFilePath))
        }

        private void rebuildSets(RebuildSetsStruct pRebuildSetsStruct, AuditingUtil pAuditingUtil,
            uint pDepth, DoWorkEventArgs e)
        {
            try
            {
                if (pDepth++ == 0)
                {
                    
                    // process top level files
                    libFilesForDeletion.Clear();
                    foreach (string f in Directory.GetFiles(pRebuildSetsStruct.pSourceDir))
                    {
                        if (!CancellationPending)
                        {
                            int progress = (++fileCount * 100) / maxFiles;
                            AuditingUtil.ProgressStruct vProgressStruct = new AuditingUtil.ProgressStruct();
                            vProgressStruct.filename = f;
                            ReportProgress(progress, vProgressStruct);

                            this.rebuildFile(f, pRebuildSetsStruct, pAuditingUtil, e);
                        }
                        else
                        {
                            e.Cancel = true;
                            doCancelCleanup(pAuditingUtil, pRebuildSetsStruct.pDestinationDir);
                            return;
                        }
                    }
                    if (!CancellationPending)
                    {
                        this.deleteQueuedLibFiles();
                    }
                }

                // process subdirs                                
                if (!CancellationPending)
                {
                    foreach (string d in Directory.GetDirectories(pRebuildSetsStruct.pSourceDir))
                    {
                        libFilesForDeletion.Clear();

                        foreach (string f in Directory.GetFiles(d))
                        {
                            if (!CancellationPending)
                            {
                                try
                                {
                                    int progress = (++fileCount * 100) / maxFiles;
                                    AuditingUtil.ProgressStruct vProgressStruct = new AuditingUtil.ProgressStruct();
                                    vProgressStruct.filename = f;
                                    ReportProgress(progress, vProgressStruct);

                                    this.rebuildFile(f, pRebuildSetsStruct, pAuditingUtil, e);
                                }
                                catch (Exception ex)
                                {
                                    AuditingUtil.ProgressStruct exProgressStruct = new AuditingUtil.ProgressStruct();
                                    exProgressStruct.filename = f;
                                    exProgressStruct.errorMessage = "[" + f + "] " + ex.Message;
                                    ReportProgress(AuditingUtil.IGNORE_PROGRESS, exProgressStruct);
                                }
                            }
                            else
                            {
                                e.Cancel = true;
                                doCancelCleanup(pAuditingUtil, pRebuildSetsStruct.pDestinationDir);
                                return;
                            }
                        }

                        if (!CancellationPending)
                        {
                            RebuildSetsStruct subdirRebuildSetsStruct = pRebuildSetsStruct;
                            subdirRebuildSetsStruct.pSourceDir = d;
                            this.rebuildSets(subdirRebuildSetsStruct, pAuditingUtil, pDepth, e);

                            // Remove Queued Lib Files
                            this.deleteQueuedLibFiles();

                            // remove empty directory
                            if (pRebuildSetsStruct.pRemoveSource &&
                                Directory.GetFiles(d, "*.*", SearchOption.AllDirectories).Length == 0)
                            {
                                Directory.Delete(d);
                            }                                                
                        }
                        else 
                        {
                            e.Cancel = true;
                            doCancelCleanup(pAuditingUtil, pRebuildSetsStruct.pDestinationDir);
                            return;
                        }                        
                    } // (string d in Directory.GetDirectories(pRebuildSetsStruct.pSourceDir))
                } //if (!CancellationPending)
                else
                {
                    e.Cancel = true;
                    doCancelCleanup(pAuditingUtil, pRebuildSetsStruct.pDestinationDir);
                    return;
                }
            }
            catch (Exception exception2)
            {
                AuditingUtil.ProgressStruct exProgressStruct = new AuditingUtil.ProgressStruct();
                exProgressStruct.filename = null;
                exProgressStruct.errorMessage = exception2.Message;
                ReportProgress(AuditingUtil.IGNORE_PROGRESS, exProgressStruct);
            }
        }

        public void rebuildSets(RebuildSetsStruct pRebuildSetsStruct, DoWorkEventArgs e)
        {
            // Set Max sets
            maxFiles += pRebuildSetsStruct.totalFiles;
            
            // Setup output directory
            pRebuildSetsStruct.pDestinationDir += Path.DirectorySeparatorChar;

            // Setup AuditingUtil
            AuditingUtil auditingUtil = new AuditingUtil(pRebuildSetsStruct.pDatFile);

            // Load/Create cache db
            string cacheDbPath = pRebuildSetsStruct.pDestinationDir + CACHE_DB_FILENAME;
            FileStream cacheFileStream = new FileStream(cacheDbPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            auditingUtil.ReadChecksumHashFromFile(cacheFileStream);
            cacheFileStream.Close();
            cacheFileStream.Dispose();

            // Rebuild
            this.rebuildSets(pRebuildSetsStruct, auditingUtil, 0, e);

            // Finish rebuilding
            cacheFileStream = new FileStream(cacheDbPath, FileMode.Open, FileAccess.ReadWrite);
            auditingUtil.WriteChecksumHashToFile(cacheFileStream);
            auditingUtil.WriteHaveMissLists(pRebuildSetsStruct.pDestinationDir);

            cacheFileStream.Close();
            cacheFileStream.Dispose();
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            RebuildSetsStruct vRebuildSetsStruct = (RebuildSetsStruct)e.Argument;
            this.rebuildSets(vRebuildSetsStruct, e);
        }

        private void doCancelCleanup(AuditingUtil pAuditingUtil, string pDestinationDir)
        {
            this.deleteQueuedTempDirs();            
        }
    }
}
