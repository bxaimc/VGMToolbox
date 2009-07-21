using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VGMToolbox.format.util
{
    public struct NezPlugM3uEntry
    {
        public string filename;
        public string format;
        public int songNumber;
        public string title;
        public string time;
        public string loop;
        public string fade;
        public string loopCount;
    }
    
    public class NezPlugUtil
    {
        public const string FORMAT_NSF = "NSF";
        public const string FORMAT_GBS = "GBS";
        
        public const string COMMENT_MARKER = "#";
        public const string M3U_FILE_EXTENSION = ".m3u";
        public const int EMPTY_COUNT = -1;

        // convert to use struct
        public static string BuildPlaylistEntry(string pFileType, string pFileName, string pSongNumber,
            string pTitle, string pTime, string pLoop, string pFade, string pLoopCount)
        {
            StringBuilder sb = new StringBuilder();
            string playlistEntry = String.Empty;

            sb.Append(pFileName + "::");
            sb.Append(pFileType + ",");
            sb.Append(pSongNumber + ",");
            sb.Append("" + pTitle.Replace(",", "\\,") + ",");
            sb.Append(pTime + ",");
            sb.Append(pLoop + ",");
            sb.Append(pFade + ",");
            sb.Append(pLoopCount);

            return sb.ToString();
        }    

        public static NezPlugM3uEntry GetNezPlugM3uEntryFromString(string pString)
        {
            NezPlugM3uEntry m3uEntry = new NezPlugM3uEntry();

            string[] stringDelimtier = new string[] { "::" };
            string[] splitLine = pString.Split(stringDelimtier, StringSplitOptions.None);

            if (splitLine.Length > 1)
            {
                char[] charDelimtier = new char[] { ',' };
                string[] splitData = splitLine[1].Split(charDelimtier, StringSplitOptions.None);

                if (splitLine.Length > 0)
                {
                    m3uEntry.filename = splitLine[0];
                }

                if (splitData.Length > 0)
                {
                    m3uEntry.format = splitData[0];
                }

                if (splitData.Length > 1)
                {
                    m3uEntry.songNumber = (int)VGMToolbox.util.Encoding.GetLongFromString(splitData[1].Replace("$", "0x"));
                }
                else
                {
                    m3uEntry.songNumber = EMPTY_COUNT;
                }

                if (splitData.Length > 2)
                {
                    m3uEntry.title = splitData[2];
                }

                if (splitData.Length > 3)
                {
                    m3uEntry.time = splitData[3];
                }

                if (splitData.Length > 4)
                {
                    m3uEntry.loop = splitData[4];
                }

                if (splitData.Length > 5)
                {
                    m3uEntry.fade = splitData[5];
                }

                if (splitData.Length > 6)
                {
                    m3uEntry.loopCount = splitData[6];
                }
            }

            return m3uEntry;
        }

        public static NezPlugM3uEntry[] GetNezPlugM3uEntriesFromFile(string pPath)
        {
            NezPlugM3uEntry[] m3uEntries = null;
            ArrayList m3uList = new ArrayList();

            if (File.Exists(pPath))
            {
                using (StreamReader sr = new StreamReader(File.OpenRead(pPath)))
                {
                    string currentLine;

                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        if (!currentLine.StartsWith(COMMENT_MARKER) &&
                            (!String.IsNullOrEmpty(currentLine)))
                        {
                            NezPlugM3uEntry m3uEntry = new NezPlugM3uEntry();
                            m3uEntry = GetNezPlugM3uEntryFromString(currentLine);

                            if (!String.IsNullOrEmpty(m3uEntry.filename))
                            {
                                m3uList.Add(m3uEntry);
                            }
                        }
                    }
                    m3uEntries = (NezPlugM3uEntry[])m3uList.ToArray(typeof(NezPlugM3uEntry));
                }
            }
            else
            {
                throw new IOException(String.Format("File not found: <{0}>{1}", pPath, Environment.NewLine));
            }

            return m3uEntries;
        }
    }
}
