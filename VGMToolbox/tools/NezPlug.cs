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
            string playlistEntry = String.Empty;

            playlistEntry += pFileName + "::";
            playlistEntry += pFileType + ",";
            playlistEntry += pSongNumber + ",";
            playlistEntry += "" + pTitle.Replace(',', '-') + ",";
            playlistEntry += pTime + ",";
            playlistEntry += pLoop + ",";
            playlistEntry += pFade + ",";
            playlistEntry += pLoopCount;

            return playlistEntry;
        }    
    
    }
}
