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
        private Constants.ProgressStruct progressStruct;

        public struct GbsM3uBuilderStruct
        {
            public string[] pPaths;
            public int totalFiles;
            public bool onePlaylistPerFile;
        }

        public GbsM3uBuilderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();

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
                        this.buildM3uForFile(path, pGbsM3uBuilderStruct.onePlaylistPerFile, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.buildM3usForDirectory(path, pGbsM3uBuilderStruct.onePlaylistPerFile, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void buildM3usForDirectory(string pPath, bool pOnePlaylistPerFile, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.buildM3usForDirectory(d, pOnePlaylistPerFile, e);
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
                    this.buildM3uForFile(f, pOnePlaylistPerFile, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void buildM3uForFile(string pPath, bool pOnePlaylistPerFile, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.filename = pPath;
            ReportProgress(progress, this.progressStruct);
          
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
                    gbsData.Initialize(fs, pPath);

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

                        if (pOnePlaylistPerFile)
                        {
                            buildSingleFileM3u(pPath, gbsData, trackItem, i);
                        }
                    }

                    sw.Close();
                    sw.Dispose();
                }

                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, this.progressStruct);
            }            
        }

        private string buildTrackItem(int pIndex, Gbs pGbsData, string pPath)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string title = enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongArtist)).Trim() + " - " +
                    enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongName)).Trim() + " - " +
                    "Track " + pIndex.ToString().PadLeft(2, '0'); ;

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

        private string CreateTitle(int pIndex)
        {
            return "Track " + pIndex.ToString().PadLeft(2, '0');
        }
        
        private void buildSingleFileM3u(string pPath, Gbs pGbsData, string pTrackData, int pIndex)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string outputFileName = Path.GetFileNameWithoutExtension(pPath) + " - " + pIndex.ToString().PadLeft(2, '0') +
                " - " + CreateTitle(pIndex) + ".m3u";

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                outputFileName = outputFileName.Replace(c, '_');
            }

            string outputPath = Path.GetDirectoryName(pPath);

            StreamWriter singleSW = File.CreateText(outputPath + Path.DirectorySeparatorChar + outputFileName);

            singleSW.WriteLine("#######################################################");
            singleSW.WriteLine("#");
            singleSW.WriteLine("# Game: " + enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongName)).Trim());
            singleSW.WriteLine("# Artist: " + enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongArtist)).Trim());
            singleSW.WriteLine("# Copyright: " + enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongCopyright)).Trim());
            singleSW.WriteLine("#");
            singleSW.WriteLine("#######################################################");
            singleSW.WriteLine();
            singleSW.WriteLine(pTrackData);

            singleSW.Close();
            singleSW.Dispose();
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            GbsM3uBuilderStruct gbsM3uBuilderStruct = (GbsM3uBuilderStruct)e.Argument;
            maxFiles = gbsM3uBuilderStruct.totalFiles;

            this.buildM3uFiles(gbsM3uBuilderStruct, e);
        }        
    }
}
