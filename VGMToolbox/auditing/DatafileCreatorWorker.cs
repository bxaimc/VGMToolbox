using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpZipLib.Checksums;

using VGMToolbox.format;
using VGMToolbox.util;
using VGMToolbox.util.ObjectPooling;

namespace VGMToolbox.auditing
{
    class DatafileCreatorWorker : BackgroundWorker
    {
        private string dir;
        private ArrayList romList;
        private Dictionary<string, ByteArray> libHash;
        private int fileCount = 0;

        public struct GetGameParamsStruct
        { 
            public string pDir; 
            public string pOutputMessage; 
            public bool pUseLibHash;
            public bool pStreamInput;
            public int totalFiles;
        }
        
        public DatafileCreatorWorker()
        {
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

        public rom buildRom(string pDirectory, string pFileName, bool pUseLibHash, bool pStreamInput)
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

                    // vgmData.getDatFileCrc32(pFileName, ref libHash, ref crc32Generator,
                    //    ref md5CryptoStream, ref sha1CryptoStream, pUseLibHash, pStreamInput);
                    vgmData.getDatFileCrc32(pFileName, ref libHash, ref crc32Generator,
                        pUseLibHash, pStreamInput);
                    vgmData = null;
                }
                catch (EndOfStreamException _es)
                {
                    // pOutputMessage += String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFileName, formatType.Name, _es.Message) + Environment.NewLine;
                    MessageBox.Show(String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFileName, formatType.Name, _es.Message) + Environment.NewLine);
                    // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator, 
                    //    ref md5CryptoStream, ref sha1CryptoStream);
                    ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                }
                catch (System.OutOfMemoryException _es)
                {
                    //pOutputMessage += String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFileName, formatType.Name, _es.Message) + Environment.NewLine;
                    MessageBox.Show(String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFileName, formatType.Name, _es.Message) + Environment.NewLine);
                    // ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator,
                    //    ref md5CryptoStream, ref sha1CryptoStream);
                    ParseFile.AddChunkToChecksum(fs, 0, (int)fs.Length, ref crc32Generator);
                }
                catch (IOException _es)
                {
                    //pOutputMessage += String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFileName, formatType.Name, _es.Message) + Environment.NewLine;
                    MessageBox.Show(String.Format("Error processing <{0}> as type [{1}], falling back to full file cheksum.  Error received: [{2}]", pFileName, formatType.Name, _es.Message) + Environment.NewLine);
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

                /*
                byte[] data = new byte [4096];
                int read = 0;
                
                fs.Seek(0, SeekOrigin.Begin); 
                 
                while ((read = fs.Read(data, 0, 4096)) > 0)
                {
                    md5CryptoStream.Write(data, 0, read);
                }                                
                md5CryptoStream.FlushFinalBlock();
                */
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
                            this.libHash = new Dictionary<string, ByteArray>();
                            this.dir = d;
                        }

                        game set = new game();

                        foreach (string f in Directory.GetFiles(d))
                        {
                            if (!CancellationPending)
                            {
                                int progress = (++fileCount * 100) / pGetGameParamsStruct.totalFiles;
                                ReportProgress(progress);

                                try
                                {
                                    rom romfile = buildRom(d, f, pGetGameParamsStruct.pUseLibHash, pGetGameParamsStruct.pStreamInput);
                                    if (romfile.name != null)
                                    {
                                        // Convert to use Array of rom?
                                        romList.Add(romfile);
                                    }
                                }
                                catch (Exception _ex)
                                {
                                    MessageBox.Show(_ex.Message);
                                    //pOutputMessage += "Error processing <" + f + "> (" + _ex.Message + ")" + "...Skipped" + Environment.NewLine;
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
                                gameList.Add(set);

                                if (pGetGameParamsStruct.pUseLibHash)
                                {
                                    for (int i = 1; i < 5; i++)
                                        ObjectPooler.Instance.DoneWithByteArray(i);
                                }
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
                MessageBox.Show(e1.Message);
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
