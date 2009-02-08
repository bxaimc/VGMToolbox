using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.tools
{
    class NezPlug
    {
        public const string FORMAT_NSF = "NSF";
        public const string FORMAT_GBS = "GBS";
        
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
    
    }
}
