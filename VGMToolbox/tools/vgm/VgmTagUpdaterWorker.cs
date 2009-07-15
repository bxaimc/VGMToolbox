using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.vgm
{
    class VgmTagUpdaterWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct VgmTagUpdaterStruct : IVgmtWorkerStruct
        {
            public bool IsBatchMode;

            public string TitleTagEn;
            public string TitleTagJp;
            public string GameTagEn;
            public string GameTagJp;
            public string SystemTagEn;
            public string SystemTagJp;
            public string ArtistTagEn;
            public string ArtistTagJp;
            public string DateTag;
            public string RipperTag;
            public string CommentTag;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public VgmTagUpdaterWorker() : base() { }

        protected override void DoTaskForFile(string pPath,
            IVgmtWorkerStruct pVgmTagUpdaterStruct, DoWorkEventArgs e)
        {
            VgmTagUpdaterStruct vgmTagUpdaterStruct = (VgmTagUpdaterStruct)pVgmTagUpdaterStruct;

            Type formatType = null;
            IGd3TagFormat vgmData = null;

            using (FileStream fs = File.OpenRead(pPath))
            {
                formatType = FormatUtil.getObjectType(fs);
                if (formatType != null)
                {
                    vgmData = (IGd3TagFormat)Activator.CreateInstance(formatType);
                    vgmData.Initialize(fs, pPath);
                }
            }

            if (vgmData != null)
            {
                vgmData.SetGameTagEn(vgmTagUpdaterStruct.GameTagEn);
                vgmData.SetGameTagJp(vgmTagUpdaterStruct.GameTagJp);
                vgmData.SetSystemTagEn(vgmTagUpdaterStruct.SystemTagEn);
                vgmData.SetSystemTagJp(vgmTagUpdaterStruct.SystemTagJp);
                vgmData.SetArtistTagEn(vgmTagUpdaterStruct.ArtistTagEn);
                vgmData.SetArtistTagJp(vgmTagUpdaterStruct.ArtistTagJp);
                vgmData.SetDateTag(vgmTagUpdaterStruct.DateTag);
                vgmData.SetRipperTag(vgmTagUpdaterStruct.RipperTag);
                vgmData.SetCommentTag(vgmTagUpdaterStruct.CommentTag);

                if (!vgmTagUpdaterStruct.IsBatchMode)
                {
                    vgmData.SetTitleTagEn(vgmTagUpdaterStruct.TitleTagEn);
                    vgmData.SetTitleTagJp(vgmTagUpdaterStruct.TitleTagJp);
                }

                vgmData.UpdateTags();
            }
        }
    }
}
