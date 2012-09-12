using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    public class UnpackNintendoWadWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct WadUnpackerStruct : IVgmtWorkerStruct
        {
            public bool UnpackExtractedU8Files;
            
            public string[] SourcePaths {set;get;}
        }

        public UnpackNintendoWadWorker() : base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct,
            DoWorkEventArgs e)
        {
            WadUnpackerStruct ws = (WadUnpackerStruct)pTaskStruct;

            NintendoWad wad;
            string[] extractedFiles;

            if (NintendoWad.IsWadFile(pPath))
            {
                wad = new NintendoWad(pPath);
                extractedFiles = wad.ExtractAll();
            }

            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = String.Format("[{0}]{1}", Path.GetFileName(pPath), Environment.NewLine);
            ReportProgress(this.progress, this.progressStruct);            
        }   
    }
}
