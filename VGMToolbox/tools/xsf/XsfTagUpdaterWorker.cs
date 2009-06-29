using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.xsf
{
    class XsfTagUpdaterWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        private delegate void XsfTagSetter(string pValue);
        
        public struct XsfTagUpdaterStruct : IVgmtWorkerStruct
        {
            public bool RemoveEmptyTags;
            public bool IsBatchMode;

            public string TitleTag;
            public string ArtistTag;
            public string GameTag;
            public string YearTag;
            public string GenreTag;
            public string CommentTag;
            public string CopyrightTag;
            public string XsfByTag;
            public string VolumeTag;
            public string LengthTag;
            public string FadeTag;
            public string SystemTag;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public XsfTagUpdaterWorker() : base() { }

        protected override void DoTaskForFile(string pPath,
            IVgmtWorkerStruct pXsfTagUpdaterStruct, DoWorkEventArgs e)
        {
            XsfTagUpdaterStruct xsfTagUpdaterStruct = (XsfTagUpdaterStruct)pXsfTagUpdaterStruct;

            Type formatType = null;
            IXsfTagFormat vgmData = null;

            using (FileStream fs = File.OpenRead(pPath))
            {
                formatType = FormatUtil.getObjectType(fs);
                if (formatType != null)
                {
                    vgmData = (IXsfTagFormat)Activator.CreateInstance(formatType);
                    vgmData.Initialize(fs, pPath);
                }                
            }

            if (vgmData != null)
            {
                XsfTagSetter xts;

                xts = new XsfTagSetter(vgmData.SetArtistTag);
                updateXsfTag(xts, xsfTagUpdaterStruct.ArtistTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                xts = new XsfTagSetter(vgmData.SetGameTag);
                updateXsfTag(xts, xsfTagUpdaterStruct.GameTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                xts = new XsfTagSetter(vgmData.SetYearTag);
                updateXsfTag(xts, xsfTagUpdaterStruct.YearTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                xts = new XsfTagSetter(vgmData.SetGenreTag);
                updateXsfTag(xts, xsfTagUpdaterStruct.GenreTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                xts = new XsfTagSetter(vgmData.SetCommentTag);
                updateXsfTag(xts, xsfTagUpdaterStruct.CommentTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                xts = new XsfTagSetter(vgmData.SetCopyrightTag);
                updateXsfTag(xts, xsfTagUpdaterStruct.CopyrightTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                xts = new XsfTagSetter(vgmData.SetXsfByTag);
                updateXsfTag(xts, xsfTagUpdaterStruct.XsfByTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                xts = new XsfTagSetter(vgmData.SetSystemTag);
                updateXsfTag(xts, xsfTagUpdaterStruct.SystemTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                
                if (!xsfTagUpdaterStruct.IsBatchMode)
                {
                    xts = new XsfTagSetter(vgmData.SetVolumeTag);
                    updateXsfTag(xts, xsfTagUpdaterStruct.VolumeTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                    xts = new XsfTagSetter(vgmData.SetLengthTag);
                    updateXsfTag(xts, xsfTagUpdaterStruct.LengthTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                    xts = new XsfTagSetter(vgmData.SetFadeTag);
                    updateXsfTag(xts, xsfTagUpdaterStruct.FadeTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                    xts = new XsfTagSetter(vgmData.SetTitleTag);
                    updateXsfTag(xts, xsfTagUpdaterStruct.TitleTag, xsfTagUpdaterStruct.RemoveEmptyTags);
                }

                vgmData.UpdateTags();
            }
        }

        private void updateXsfTag(XsfTagSetter pXsfTagSetter, string pValue, 
            bool pRemoveEmptyTags)
        {
            if (pRemoveEmptyTags)
            {
                pXsfTagSetter(pValue);
            }
            else if ((!String.IsNullOrEmpty(pValue)) && (!String.IsNullOrEmpty(pValue.Trim())))
            {
                pXsfTagSetter(pValue);
            }
        }
    }
}
