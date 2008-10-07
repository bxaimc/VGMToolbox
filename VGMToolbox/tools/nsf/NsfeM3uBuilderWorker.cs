using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.auditing;
using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools.nsf
{
    class NsfeM3uBuilderWorker :BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct NsfeM3uBuilderStruct
        {
            public string[] pPaths;
            public int totalFiles;
            public bool onePlaylistPerFile;
        }

        public NsfeM3uBuilderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }


        private void buildM3uFiles(NsfeM3uBuilderStruct pNsfeM3uBuilderStruct, DoWorkEventArgs e)
        {
            foreach (string path in pNsfeM3uBuilderStruct.pPaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.buildM3uForFile(path, pNsfeM3uBuilderStruct.onePlaylistPerFile, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.buildM3usForDirectory(path, pNsfeM3uBuilderStruct.onePlaylistPerFile, e);

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
                    // fileCount++;
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
            AuditingUtil.ProgressStruct vProgressStruct = new AuditingUtil.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);
          
            try
            {
                FileStream fs = File.OpenRead(pPath);
                Type dataType = FormatUtil.getObjectType(fs);
                System.Text.Encoding enc = System.Text.Encoding.ASCII;

                if (dataType != null && dataType.Name.Equals("Nsfe"))
                {
                    StreamWriter sw;
                    string filename = Path.GetFileName(pPath);
                    string trackItem = String.Empty;

                    Nsfe nsfeData = new Nsfe();
                    fs.Seek(0, SeekOrigin.Begin);
                    nsfeData.Initialize(fs);

                    string outputFile = Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar +
                        Path.GetFileNameWithoutExtension(pPath) + ".m3u";
                    sw = File.CreateText(outputFile);

                    string[] playlist = nsfeData.Playlist.Split(',');

                    sw.WriteLine("#######################################################");
                    sw.WriteLine("#");
                    sw.WriteLine("# Game: " + nsfeData.SongName);
                    sw.WriteLine("# Artist: " + nsfeData.SongArtist);
                    sw.WriteLine("# Copyright: " + nsfeData.SongCopyright);
                    sw.WriteLine("# Ripper: " + nsfeData.NsfRipper);
                    sw.WriteLine("#");
                    sw.WriteLine("#######################################################");
                    sw.WriteLine();

                    // Build by playlist if it exists            
                    if (!String.IsNullOrEmpty(nsfeData.Playlist))
                    {
                        int j = 1;
                        foreach (string s in nsfeData.Playlist.Split(','))
                        {
                            int index = int.Parse(s.Trim());
                            trackItem = buildTrackItem(index, nsfeData, pPath);
                            sw.WriteLine(trackItem);

                            buildSingleFileM3u(pPath, nsfeData, trackItem, j, index);
                            j++;
                        }
                    }
                    // Use default order if playlist does not exist
                    else
                    {
                        // !!! CHANGE TO START FROM nsfeData.StartingSong???????
                        for (int i = 0; i < nsfeData.TotalSongs[0]; i++)
                        {
                            trackItem = buildTrackItem(i, nsfeData, pPath);
                            sw.WriteLine(trackItem);
                            buildSingleFileM3u(pPath, nsfeData, trackItem, i, i);
                        }
                    }

                    sw.Close();
                    sw.Dispose();

                    NsfTools.NsfeToNsf(nsfeData, pPath);
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

        private string buildTrackItem(int pIndex, Nsfe pNsfeData, string pPath)
        {
            int tempTime;
            int timeMinutes = 0;
            int timeSeconds = 0;
            string timeTotal = String.Empty;
            int fadeMinutes = 0;
            int fadeSeconds = 0;
            string fadeTotal = String.Empty;
            string title;
            string entry;

            title = ParseTitle(pNsfeData, pIndex);

            if (pNsfeData.Times != null)
            {
                tempTime = pNsfeData.Times[pIndex];

                if (tempTime >= 0)
                {
                    timeMinutes = tempTime / 60000;
                    timeSeconds = ((tempTime - (timeMinutes * 60000)) % 60000) / 1000;
                    timeTotal = timeMinutes + ":" + timeSeconds.ToString("d2");
                }
            }

            if (pNsfeData.Fades != null)
            {
                tempTime = pNsfeData.Fades[pIndex];

                if (tempTime >= 0)
                {
                    fadeMinutes = tempTime / 60000;
                    fadeSeconds = ((tempTime - (fadeMinutes * 60000)) % 60000) / 1000;
                    fadeTotal = fadeMinutes + ":" + fadeSeconds.ToString("d2");
                }
            }

            entry = NezPlug.BuildPlaylistEntry(NezPlug.FORMAT_NSF,
                Path.GetFileNameWithoutExtension(pPath) + ".nsf",
                (pIndex + 1).ToString(),
                title.Trim(),
                timeTotal,
                String.Empty,
                fadeTotal,
                String.Empty);
            return entry;
        }

        private string ParseTitle(Nsfe pNsfeData, int pIndex)
        {
            string title;

            if ((pNsfeData.TrackLabels.Length > pIndex) && (!String.IsNullOrEmpty(pNsfeData.TrackLabels[pIndex])))
            {
                title = pNsfeData.TrackLabels[pIndex];
            }
            else
            {
                title = "Track " + pIndex;
            }
            return title;
        }

        private void buildSingleFileM3u(string pPath, Nsfe pNsfeData, string pTrackData, int pIndex, int pTrackIndex)
        {
            string outputSingleFile = Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(pPath) + " - " + pIndex.ToString().PadLeft(2, '0') +
                " - " + ParseTitle(pNsfeData, pTrackIndex) + ".m3u";
            
            StreamWriter singleSW = File.CreateText(outputSingleFile);
            
            singleSW.WriteLine("#######################################################");
            singleSW.WriteLine("#");
            singleSW.WriteLine("# Game: " + pNsfeData.SongName);
            singleSW.WriteLine("# Artist: " + pNsfeData.SongArtist);
            singleSW.WriteLine("# Copyright: " + pNsfeData.SongCopyright);
            singleSW.WriteLine("# Ripper: " + pNsfeData.NsfRipper);
            singleSW.WriteLine("# Song: " + ParseTitle(pNsfeData, pIndex));
            singleSW.WriteLine("#");
            singleSW.WriteLine("#######################################################");
            singleSW.WriteLine();
            singleSW.WriteLine(pTrackData);
            
            singleSW.Close();
            singleSW.Dispose();
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            NsfeM3uBuilderStruct nsfeM3uBuilderStruct = (NsfeM3uBuilderStruct)e.Argument;
            maxFiles = nsfeM3uBuilderStruct.totalFiles;

            this.buildM3uFiles(nsfeM3uBuilderStruct, e);
        }            
    }
}
