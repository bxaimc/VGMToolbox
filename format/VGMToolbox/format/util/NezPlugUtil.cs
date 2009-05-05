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

            char[] charDelimtier = new char[] { ',' };
            string[] splitLine = pString.Split(charDelimtier, StringSplitOptions.None);

            string[] stringDelimtier = new string[] { "::" };
            string[] splitFirstChunk = splitLine[0].Split(stringDelimtier, StringSplitOptions.None);

            if (splitFirstChunk.Length > 0)
            {
                m3uEntry.filename = splitFirstChunk[0];
            }

            if (splitFirstChunk.Length > 1)
            {
                m3uEntry.format = splitFirstChunk[1];
            }

            if (splitLine.Length > 1)
            {
                m3uEntry.songNumber = (int)VGMToolbox.util.Encoding.GetIntFromString(splitLine[1].Replace("$", "0x"));
            }
            else
            {
                m3uEntry.songNumber = EMPTY_COUNT;
            }
            
            if (splitLine.Length > 2)
            {
                m3uEntry.title = splitLine[2];
            }

            if (splitLine.Length > 3)
            {
                m3uEntry.time = splitLine[3];
            }

            if (splitLine.Length > 4)
            {
                m3uEntry.fade = splitLine[4];
            }

            if (splitLine.Length > 5)
            {
                m3uEntry.loopCount = splitLine[5];
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
                            m3uList.Add(m3uEntry);
                        }
                    }
                    m3uEntries = (NezPlugM3uEntry[])m3uList.ToArray(typeof(NezPlugM3uEntry));
                }
            }
            else
            {
                throw new IOException(String.Format("File not found: <{0}>", pPath));
            }

            return m3uEntries;
        }
    }
}
