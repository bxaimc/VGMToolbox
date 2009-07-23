using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format.util
{
    public struct GbsM3uBuilderStruct
    {
        public string path;
        public bool onePlaylistPerFile;

        public string Path
        {
            set { path = value; }
            get { return path; }
        }
        public bool OnePlaylistPerFile
        {
            set { onePlaylistPerFile = value; }
            get { return onePlaylistPerFile; }
        }
    }
    
    public sealed class GbsUtil
    {
        private GbsUtil() { }
        
        public static string BuildPlaylistTrackItem(int index, Gbs pGbsData, string pPath)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string title = enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongArtist)).Trim() + " - " +
                    enc.GetString(FileUtil.ReplaceNullByteWithSpace(pGbsData.SongName)).Trim() + " - " +
                    "Track " + index.ToString().PadLeft(2, '0'); ;

            string entry = NezPlugUtil.BuildPlaylistEntry(NezPlugUtil.FORMAT_GBS,
                Path.GetFileName(pPath),
                (index).ToString(),
                title,
                String.Empty,
                String.Empty,
                String.Empty,
                String.Empty);
            return entry;
        }

        public static void BuildPlaylistForFile(GbsM3uBuilderStruct pGbsM3uBuilderStruct)
        {
            using (FileStream fs = File.OpenRead(pGbsM3uBuilderStruct.Path))
            {
                Type dataType = FormatUtil.getObjectType(fs);
                System.Text.Encoding enc = System.Text.Encoding.ASCII;

                if (dataType != null && dataType.Name.Equals("Gbs"))
                {
                    string filename = Path.GetFileName(pGbsM3uBuilderStruct.Path);
                    string trackItem = String.Empty;

                    Gbs gbsData = new Gbs();
                    fs.Seek(0, SeekOrigin.Begin);
                    gbsData.Initialize(fs, pGbsM3uBuilderStruct.Path);

                    string outputFile = Path.GetDirectoryName(pGbsM3uBuilderStruct.Path) + Path.DirectorySeparatorChar +
                        Path.GetFileNameWithoutExtension(pGbsM3uBuilderStruct.Path) + ".m3u";

                    using (StreamWriter sw = File.CreateText(outputFile))
                    {
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
                            trackItem = GbsUtil.BuildPlaylistTrackItem(i, gbsData, pGbsM3uBuilderStruct.Path);
                            sw.WriteLine(trackItem);

                            if (pGbsM3uBuilderStruct.OnePlaylistPerFile)
                            {
                                GbsUtil.BuildSingleFilePlaylist(pGbsM3uBuilderStruct.Path, gbsData, trackItem, i);
                            }
                        }
                    }
                }            
            }
        }

        public static void BuildSingleFilePlaylist(string pPath, Gbs pGbsData, string pTrackData, int pIndex)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string outputFileName = Path.GetFileNameWithoutExtension(pPath) + " - " + pIndex.ToString().PadLeft(2, '0') +
                " - " + GbsUtil.CreatePlaylistTitle(pIndex) + ".m3u";

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

        public static string CreatePlaylistTitle(int index)
        {
            return "Track " + index.ToString().PadLeft(2, '0');
        }
    }
}
