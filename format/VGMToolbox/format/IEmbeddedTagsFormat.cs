using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.format
{
    public interface IEmbeddedTagsFormat : IFormat
    {
        void UpdateSongName(string pFilePath, string pNewValue);
        void UpdateArtist(string pFilePath, string pNewValue);
        void UpdateCopyright(string pFilePath, string pNewValue);

        string GetSongNameAsText();
        string GetArtistAsText();
        string GetCopyrightAsText();
    }
}
