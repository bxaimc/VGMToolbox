using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.auditing
{
    class DatafileCreatorWorker : BackgroundWorker
    {
        private string dir;
        private ArrayList romList;
        private int fileCount = 0;
        Constants.ProgressStruct progressStruct;

        public struct GetGameParamsStruct
        { 
            public string pDir; 
            public string pOutputMessage; 
            public int totalFiles;
        }
        
        public DatafileCreatorWorker()
        {
            fileCount = 0;
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        public static header buildHeader(string pAuthor, string pCategory, string pComment, string pDate, string pDescription,
            string pEmail, string pHomepage, string pName, string pUrl, string pVersion)
        {
            header datHeader = new header();

            datHeader.author = pAuthor;
            datHeader.category = pCategory;
            datHeader.comment = pComment;
            datHeader.date = pDate;
            datHeader.description = pDescription;
            datHeader.email = pEmail;
            datHeader.homepage = pHomepage;
            datHeader.name = pName;
            datHeader.url = pUrl;
            datHeader.version = pVersion;

            return datHeader;
        }

        public rom buildRom(string pDirectory, string pFileName)
        {
            string path = pFileName.Substring((pDirectory.LastIndexOf(this.dir) + this.dir.Length));
            FileStream fs = File.OpenRead(pFileName);
            Type formatType = FormatUtil.getObjectType(fs);

            // CRC32
            Crc32 crc32Generator = new Crc32();

            /*
            // MD5
            MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider();
            MemoryStream md5MemoryStream = new MemoryStream();
            CryptoStream md5CryptoStream = new CryptoStream(md5MemoryStream, md5Hash, CryptoStreamMode.Write);

            // SHA1
            SHA1CryptoServiceProvider sha1Hash = new SHA1CryptoServiceProvider();
            MemoryStream sha1MemoryStream = new MemoryStream();
            CryptoStream sha1CryptoStream = new CryptoStream(sha1MemoryStream, sha1Hash, CryptoStreamMode.Write);
            */

            fs.Seek(0, SeekOrigin.Begin);                  // Return to start of stream
            rom romfile = new rom();

            if (formatType != null)
            {
                try
                {
                    IFormat vgmData = (IFormat)Activator.CreateInstance(formatType);
                    vgmData.Initialize(fs, pFileName);

                    // vgmData.getDatFileCrc32(pFileName, ref libHash, ref crc32Generator,
                    //    ref md5CryptoStream, ref sha1CryptoStream, pUseLibHash, pStreamInput);
                    vgmData.GetDatFileCrc32(ref crc32Generator);
                    vgmData = null;
                }
                catch (EndOfStreamException _es)
                {
                    this.progressStruct.Clear();
                    this.progressStruct.filename = pFileName;
                    this.progressStruct.errorMessage = String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: {2}", pFileName, formatType.Name, _es.Message) + Environment.NewLine;
                    ReportProgress(Constants.IGNORE_PROGRESS, this.progressStruct);

                    crc32Generator.Reset();
                    // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator, 
                    //    ref md5CryptoStream, ref sha1CryptoStream);
                    ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                }
                catch (System.OutOfMemoryException _es)
                {
                    this.progressStruct.Clear();
                    this.progressStruct.filename = pFileName;
                    this.progressStruct.errorMessage = String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: {2}", pFileName, formatType.Name, _es.Message) + Environment.NewLine;
                    ReportProgress(Constants.IGNORE_PROGRESS, this.progressStruct);


                    crc32Generator.Reset();
                    // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator,
                    //    ref md5CryptoStream, ref sha1CryptoStream);
                    ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                }
                catch (IOException _es)
                {
                    this.progressStruct.Clear();
                    this.progressStruct.filename = pFileName;
                    this.progressStruct.errorMessage = String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: {2}", pFileName, formatType.Name, _es.Message) + Environment.NewLine;
                    ReportProgress(Constants.IGNORE_PROGRESS, this.progressStruct);

                    crc32Generator.Reset();
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

            /*
            md5CryptoStream.FlushFinalBlock();
            sha1CryptoStream.FlushFinalBlock();
            */

            romfile.crc = crc32Generator.Value.ToString("X2");
            // romfile.md5 = AuditingUtil.ByteArrayToString(md5Hash.Hash);
            // romfile.sha1 = AuditingUtil.ByteArrayToString(sha1Hash.Hash);
            
            romfile.name = pFileName.Substring((pDirectory.LastIndexOf(this.dir) + this.dir.Length + 1));
            // Cleanup
            crc32Generator.Reset();

            fs.Close();
            fs.Dispose();

            /*
            md5MemoryStream.Close();
            md5MemoryStream.Dispose();
            sha1MemoryStream.Close();
            sha1MemoryStream.Dispose();

            md5CryptoStream.Close();
            md5CryptoStream.Dispose();
            sha1CryptoStream.Close();
            sha1CryptoStream.Dispose();
            */

            return romfile;
        }

        public game[] buildGames(GetGameParamsStruct pGetGameParamsStruct, DoWorkEventArgs e)
        {
            return this.buildGames(pGetGameParamsStruct, 0, e);
        }

        private game[] buildGames(GetGameParamsStruct pGetGameParamsStruct, uint pDepth, DoWorkEventArgs e)
        {
            game[] gameArray = null;
            ArrayList gameList = new ArrayList();

            pDepth++;

            game set;
            int progress;
            rom romfile;

            try
            {                
                // Directories
                foreach (string d in Directory.GetDirectories(pGetGameParamsStruct.pDir))
                {
                    if (Directory.GetFiles(d, "*.*", SearchOption.AllDirectories).Length > 0)
                    {
                        if (pDepth == 1)
                        {
                            this.romList = new ArrayList();
                            this.dir = d;
                        }

                        set = new game();

                        foreach (string f in Directory.GetFiles(d))
                        {
                            if (!CancellationPending)
                            {
                                progress = (++fileCount * 100) / pGetGameParamsStruct.totalFiles;
                                this.progressStruct.Clear();
                                this.progressStruct.filename = f;
                                ReportProgress(progress, this.progressStruct);

                                try
                                {
                                    romfile = buildRom(d, f);
                                    if (romfile.name != null)
                                    {
                                        // Convert to use Array of rom?
                                        romList.Add(romfile);
                                    }
                                }
                                catch (Exception _ex)
                                {
                                    this.progressStruct.Clear();
                                    this.progressStruct.filename = f;
                                    this.progressStruct.errorMessage = "Error processing <" + f + "> (" + _ex.Message + ")" + "...Skipped" + Environment.NewLine;
                                    ReportProgress(Constants.IGNORE_PROGRESS, this.progressStruct);
                                }
                            }
                            else
                            {
                                e.Cancel = true;
                                break;
                            }
                        }

                        if (!CancellationPending)
                        {
                            GetGameParamsStruct subdirGetGameParamsStruct = pGetGameParamsStruct;
                            subdirGetGameParamsStruct.pDir = d;
                            this.buildGames(subdirGetGameParamsStruct, pDepth, e);

                            if (pDepth == 1 && romList.Count > 0)
                            {
                                set.rom = (rom[])this.romList.ToArray(typeof(rom));
                                set.name = d.Substring(d.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                                set.description = set.name;
                                gameList.Add(set);
                            }
                        }
                        else 
                        { 
                            break; 
                        }
                    } // if ((Directory.GetFiles(d, "*.*", SearchOption.AllDirectories).Length - 1) > 0)              
                } // foreach (string d in Directory.GetDirectories(pDir))

                if (gameList.Count > 0)
                {
                    gameArray = (game[])gameList.ToArray(typeof(game));
                }
            }
            catch (Exception e1)
            {
                this.progressStruct.Clear();
                this.progressStruct.filename = null;
                this.progressStruct.errorMessage = e1.Message;
                ReportProgress(Constants.IGNORE_PROGRESS, this.progressStruct);
            }
            return gameArray;            
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            GetGameParamsStruct getGameParamsStruct = (GetGameParamsStruct)e.Argument;
            e.Result = (game[])this.buildGames(getGameParamsStruct, e);
        }
    }
}
