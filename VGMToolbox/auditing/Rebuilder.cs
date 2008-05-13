using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

using VGMToolbox.format;
using VGMToolbox.tools;
using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.auditing
{
    class Rebuilder
    {
        // Fields
        // private Hashtable checksumHash;
        private Dictionary<string, ByteArray> libHash;
        private const string CACHE_DB_FILENAME = "cache.db";
        private ArrayList libFilesForDeletion = new ArrayList();

        // Methods
        private string buildFilePath(string pSetName, string pFilePath)
        {
            return (pSetName + Path.DirectorySeparatorChar + pFilePath);
        }

        private void deleteQueuedLibFiles()
        {
            foreach (string f in libFilesForDeletion)
            {
                File.Delete(f);
            }
            libFilesForDeletion.Clear();
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
                MessageBox.Show("Error processing <" + pSourceName + "> \n" + exception.Message);
            }
        }

        private void rebuildFile(string pFilePath, string pDestination, bool pRemoveSource,
            bool pOverwriteExisting, ToolStripProgressBar pToolStripProgressBar, AuditingUtil pAuditingUtil,
            bool pStreamInput, ref string pOutputMessage, bool pCompressOutput)
        {
            ArrayList pDestinationFiles = new ArrayList();
            bool isFileLibrary = false;

            Application.DoEvents();

            FileStream fs = File.OpenRead(pFilePath);
            Type formatType = FormatUtil.getObjectType(fs);
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

                    if (!pStreamInput)
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
                        false, pStreamInput);
                    vgmData = null;
                }
                catch (EndOfStreamException e)
                {
                    pOutputMessage += String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFilePath, formatType.Name, e.Message) + Environment.NewLine + Environment.NewLine;
                    // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator,
                    //    ref md5CryptoStream, ref sha1CryptoStream);
                    ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                }
                catch (System.OutOfMemoryException e)
                {
                    pOutputMessage += String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFilePath, formatType.Name, e.Message) + Environment.NewLine + Environment.NewLine;
                    // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator,
                    //    ref md5CryptoStream, ref sha1CryptoStream);
                    ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                }
                catch (IOException e)
                {
                    pOutputMessage += String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFilePath, formatType.Name, e.Message) + Environment.NewLine + Environment.NewLine;
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
                this.moveFile(pDestination, pDestinationFiles, fs, pFilePath, pOverwriteExisting, 
                    pAuditingUtil, pCompressOutput);
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
            if (pDestinationFiles != null && pRemoveSource && File.Exists(pFilePath))
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
        }
        
        private void rebuildSets(string pSourceDir, string pDestination, int pDepth, bool pRemoveSource,
            bool pOverwriteExisting, ToolStripProgressBar pToolStripProgressBar, AuditingUtil pAuditingUtil,
            bool pStreamInput, ref string pOutputMessage, bool pCompressOutput)
        {
            try
            {
                if (pDepth++ == 0)
                {
                    // process top level files
                    libFilesForDeletion.Clear();
                    foreach (string f in Directory.GetFiles(pSourceDir))
                    {
                        pToolStripProgressBar.PerformStep();
                        Application.DoEvents();
                        this.rebuildFile(f, pDestination, pRemoveSource, pOverwriteExisting, pToolStripProgressBar, pAuditingUtil, pStreamInput, ref pOutputMessage, pCompressOutput);
                    }
                    this.deleteQueuedLibFiles();
                }

                // process subdirs                                
                foreach (string d in Directory.GetDirectories(pSourceDir))
                {
                    /*
                    var sortedFileList = from f in Directory.GetFiles(d)
                                         let fs = File.OpenRead(f)     
                                         orderby fs.Length
                                         select f;
                    string[] fileList = sortedFileList.ToArray<string>();
                    sortedFileList = null;

                    foreach (string f in fileList)
                    */
                    libFilesForDeletion.Clear();
                    
                    foreach (string f in Directory.GetFiles(d))
                    {                        
                        try
                        {
                            pToolStripProgressBar.PerformStep();
                            Application.DoEvents();
                            this.rebuildFile(f, pDestination, pRemoveSource, pOverwriteExisting, 
                                pToolStripProgressBar, pAuditingUtil, pStreamInput, 
                                ref pOutputMessage, pCompressOutput);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("[" + f + "] " + e.Message);
                        }
                    }

                    this.rebuildSets(d, pDestination, pDepth, pRemoveSource, 
                        pOverwriteExisting, pToolStripProgressBar, pAuditingUtil, pStreamInput, 
                        ref pOutputMessage, pCompressOutput);
                    
                    // Remove Queued Lib Files
                    this.deleteQueuedLibFiles();

                    // remove empty directory
                    if (pRemoveSource &&
                        Directory.GetFiles(d, "*.*", SearchOption.AllDirectories).Length == 0)
                    {
                        Directory.Delete(d);
                    }
                }
            }
            
            catch (Exception exception2)
            {
                MessageBox.Show(exception2.Message);
            }
             
        }

        public void rebuildSets(string pSourceDir, string pDestinationDir, Datafile pDatFile, 
            bool pRemoveSource, bool pOverwriteExisting, ToolStripProgressBar pToolStripProgressBar,
            bool pStreamInput, bool pCompressOutput, ref string pOutputMessage)
        {
            // Setup output directory
            pDestinationDir = pDestinationDir + Path.DirectorySeparatorChar;
            
            // Setup AuditingUtil
            AuditingUtil auditingUtil = new AuditingUtil(pDatFile);

            // Load/Create cache db
            string cacheDbPath = pDestinationDir + CACHE_DB_FILENAME;
            FileStream cacheFileStream = new FileStream(cacheDbPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            auditingUtil.ReadChecksumHashFromFile(cacheFileStream);
            cacheFileStream.Close();
            cacheFileStream.Dispose();

            // Rebuild
            this.rebuildSets(pSourceDir, pDestinationDir, 0, pRemoveSource, pOverwriteExisting,
                pToolStripProgressBar, auditingUtil, pStreamInput, ref pOutputMessage, pCompressOutput);

            // Finish rebuilding
            cacheFileStream = new FileStream(cacheDbPath, FileMode.Open, FileAccess.ReadWrite);
            auditingUtil.WriteChecksumHashToFile(cacheFileStream);
            auditingUtil.WriteHaveMissLists(pDestinationDir);

            cacheFileStream.Close();
            cacheFileStream.Dispose();
        }
    }
}