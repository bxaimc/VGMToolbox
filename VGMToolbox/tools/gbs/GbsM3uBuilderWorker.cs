using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.auditing;
using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools.gbs
{
    class GbsM3uBuilderWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct GbsM3uBuilderStruct
        {
            public string[] pPaths;
            public int totalFiles;
        }

        public GbsM3uBuilderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }


        private void buildM3uFiles(GbsM3uBuilderStruct pGbsM3uBuilderStruct, DoWorkEventArgs e)
        {
            foreach (string path in pGbsM3uBuilderStruct.pPaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.buildM3uForFile(path, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.buildM3usForDirectory(path, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void buildM3usForDirectory(string pPath, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.buildM3usForDirectory(d, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
            foreach (string f in Directory.GetFiles(pPath))
            {
                if (!CancellationPending)
                {
                    this.buildM3uForFile(f, e);
                    // fileCount++;
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void buildM3uForFile(string pPath, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            AuditingUtil.ProgressStruct vProgressStruct = new AuditingUtil.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);
          
            try
            {
                FileStream fs = File.OpenRead(pPath);
                Type dataType = FormatUtil.getObjectType(fs);
                System.Text.Encoding enc = System.Text.Encoding.ASCII;

                if (dataType != null && dataType.Name.Equals("Gbs"))
                {
                    StreamWriter sw;
                    string filename = Path.GetFileName(pPath);
                    string trackItem = String.Empty;

                    Gbs gbsData = new Gbs();
                    fs.Seek(0, SeekOrigin.Begin);
                    gbsData.Initialize(fs);

                    string outputFile = Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar + 
                        Path.GetFileNameWithoutExtension(pPath) + ".m3u";
                    sw = File.CreateText(outputFile);

                    sw.WriteLine("#######################################################");
                    sw.WriteLine("#");
                    sw.WriteLine("# Game: " + enc.GetString(FileUtil.ReplaceNullByteWithSpace(gbsData.SongName)).Trim());
                    sw.WriteLine("# Artist: " + enc.GetString(FileUtil.ReplaceNullByteWithSpace(gbsData.SongArtist)).Trim());
                    sw.WriteLine("# Copyright: " + enc.GetString(FileUtil.ReplaceNullByteWithSpace(gbsData.SongCopyright)).Trim());
                    sw.WriteLine("#");
                    sw.WriteLine("#######################################################");
                    sw.WriteLine();

                    for (int i = gbsData.StartingSong[0] - 1; i < gbsData.TotalSongs[0]; i++)
                    {
                        trackItem = buildTrackItem(i, gbsData, pPath);
                        sw.WriteLine(trackItem);
                    }

                    sw.Close();
                    sw.Dispose();
                }

                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                vProgressStruct = new AuditingUtil.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, vProgressStruct);
            }            
        }

        private string buildTrackItem(int pIndex, Gbs pGbsData, string pPath)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string title = enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongArtist)).Trim() + " - " +
                    enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongName)).Trim() + " - " +
                    "Track " + pIndex;

            string entry = NezPlug.BuildPlaylistEntry(NezPlug.FORMAT_GBS,
                Path.GetFileName(pPath),
                (pIndex).ToString(),
                title,
                String.Empty,
                String.Empty,
                String.Empty,
                String.Empty);
            return entry;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            GbsM3uBuilderStruct gbsM3uBuilderStruct = (GbsM3uBuilderStruct)e.Argument;
            maxFiles = gbsM3uBuilderStruct.totalFiles;

            this.buildM3uFiles(gbsM3uBuilderStruct, e);
        }        
    }
}
