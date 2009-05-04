using System;
using System.Collections.Generic;
using System.Text;

using VGMToolbox.format.util;

namespace VGMToolbox.format
{
    public interface IHootM3uFormat : IFormat
    {
        NezPlugM3uEntry[] GetPlaylistEntries();

        string GetHootDriverAlias();
        string GetHootDriverType();
        string GetHootDriver();
    }
}