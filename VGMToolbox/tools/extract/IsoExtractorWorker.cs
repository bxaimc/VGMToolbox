using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format.iso;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class IsoExtractorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct IsoExtractorStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
        }
        
        public IsoExtractorWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e)
        {
            IsoExtractorStruct taskStruct = (IsoExtractorStruct)pTaskStruct;

            Iso9660 iso9660 = new Iso9660(pPath);
            iso9660.Initialize();

            using (FileStream fs = File.OpenRead(pPath))
            {
                iso9660.ExtractAll(fs, Path.Combine(Path.GetDirectoryName(pPath), "ext"), 0);
            }
        }    
    }
}
