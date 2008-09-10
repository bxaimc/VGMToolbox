using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;

namespace VGMToolbox.tools
{
    class NsfeToM3u
    {
        public NsfeToM3u() { }
        
        public static void BuildNezPlugM3uFiles(string[] pPaths)
        {
            foreach (string path in pPaths)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        buildNezPlugM3u(path);
                    }
                    catch (Exception _de)
                    {
                        MessageBox.Show(_de.Message);
                    }
                }
                else if (Directory.Exists(path))
                {
                    BuildNezPlugM3uFiles(Directory.GetDirectories(path));
                    BuildNezPlugM3uFiles(Directory.GetFiles(path));
                }
            }
        
        }

        private static void buildNezPlugM3u(string pPath)
        {
            FileStream fs = File.OpenRead(pPath);
            Type formatType = FormatUtil.getObjectType(fs);

            if (formatType != null && formatType.Name.Equals("Nsfe"))
            {
                StreamWriter sw;
                string filename = Path.GetFileName(pPath);
                string trackItem = String.Empty;
                
                Nsfe nsfeData = new Nsfe();
                fs.Seek(0, SeekOrigin.Begin);
                nsfeData.Initialize(fs);

                string[] playlist = nsfeData.Playlist.Split(',');                

                string outputFile = pPath.Substring(0, pPath.Length - Path.GetExtension(pPath).Length) + ".m3u";
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                sw = File.CreateText(outputFile);
                
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
                    foreach (string s in nsfeData.Playlist.Split(','))
                    {
                        int index = int.Parse(s.Trim());
                        trackItem = buildTrackItem(index, nsfeData, pPath);
                        sw.WriteLine(trackItem);
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
                    }
                }
                
                sw.Close();
                sw.Dispose();
            }
        }

        private static string buildTrackItem(int pIndex, Nsfe pNsfeData, string pPath)
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

            if ((pNsfeData.TrackLabels.Length > pIndex) && (!String.IsNullOrEmpty(pNsfeData.TrackLabels[pIndex])))
            {
                title = pNsfeData.TrackLabels[pIndex];
            }
            else
            {
                title = "Track " + pIndex;
            }

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
    }
}
