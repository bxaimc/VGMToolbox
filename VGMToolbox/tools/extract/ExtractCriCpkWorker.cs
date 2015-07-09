using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class ExtractCriCpkWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct ExtractCriCpkStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
        }

        public ExtractCriCpkWorker() :
            base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pExtractCriCpkStruct, DoWorkEventArgs e)
        {
            ExtractCriCpkStruct extractCriCpkStruct = (ExtractCriCpkStruct)pExtractCriCpkStruct;

            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                CriUtfTable cpkUtf = new CriUtfTable();
                cpkUtf.Initialize(fs, 0x10);

                CriUtfTable tocUtf = new CriUtfTable();
                tocUtf.Initialize(fs, 0x810);

            
            }
        }
    }
}
