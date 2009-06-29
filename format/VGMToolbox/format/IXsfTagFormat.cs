using System;
using System.Collections.Generic;
using System.Text;

namespace VGMToolbox.format
{
    public interface IXsfTagFormat: IFormat
    {       
        string GetTitleTag();
        string GetArtistTag();
        string GetGameTag();
        string GetYearTag();
        string GetGenreTag();
        string GetCommentTag();
        string GetCopyrightTag();
        string GetXsfByTag();
        string GetVolumeTag();
        string GetLengthTag();
        string GetFadeTag();
        string GetSystemTag();

        void SetTitleTag(string pNewValue);
        void SetArtistTag(string pNewValue);
        void SetGameTag(string pNewValue);
        void SetYearTag(string pNewValue);
        void SetGenreTag(string pNewValue);
        void SetCommentTag(string pNewValue);
        void SetCopyrightTag(string pNewValue);
        void SetXsfByTag(string pNewValue);
        void SetVolumeTag(string pNewValue);
        void SetLengthTag(string pNewValue);
        void SetFadeTag(string pNewValue);
        void SetSystemTag(string pNewValue);

        void UpdateTags();
    }
}
