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
        public enum IsoFormatType
        {
            Iso9660
        };
        
        public struct IsoExtractorStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
            public string DestinationFolder;

            public IsoFormatType IsoFormat { set; get; }

            public IFileStructure[] Files { set; get; }
            public IDirectoryStructure[] Directories { set; get; }
        }
        
        public IsoExtractorWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e)
        {
            IsoExtractorStruct taskStruct = (IsoExtractorStruct)pTaskStruct;

            using (FileStream fs = File.OpenRead(pPath))
            {
                foreach (IDirectoryStructure d in taskStruct.Directories)
                {
                    if (!CancellationPending)
                    {
                        d.Extract(fs, taskStruct.DestinationFolder);
                    }
                    else
                    {
                        e.Cancel = false;
                        break;
                    }
                }

                foreach (IFileStructure f in taskStruct.Files)
                {
                    if (!CancellationPending)
                    {
                        f.Extract(fs, taskStruct.DestinationFolder);
                    }
                    else
                    {
                        e.Cancel = false;
                        break;
                    }
                }

            }
        }    
    }
}
