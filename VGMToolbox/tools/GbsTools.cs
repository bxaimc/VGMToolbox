using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools
{
    class GbsTools
    {
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
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            if (formatType != null && formatType.Name.Equals("Gbs"))
            {
                StreamWriter sw;
                string filename = Path.GetFileName(pPath);
                string trackItem = String.Empty;

                Gbs gbsData = new Gbs();
                fs.Seek(0, SeekOrigin.Begin);
                gbsData.Initialize(fs);

                string outputFile = pPath.Substring(0, pPath.Length - Path.GetExtension(pPath).Length) + ".m3u";
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
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
        }

        private static string buildTrackItem(int pIndex, Gbs pGbsData, string pPath)
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
    }
}
