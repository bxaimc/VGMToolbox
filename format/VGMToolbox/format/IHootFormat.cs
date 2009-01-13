using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.format
{
    public interface IHootFormat : IFormat
    {
        int GetStartingSong();
        int GetTotalSongs();
        string GetSongName();
        
        string GetHootDriverAlias();
        string GetHootDriverType();
        string GetHootDriver();    
    }
}
