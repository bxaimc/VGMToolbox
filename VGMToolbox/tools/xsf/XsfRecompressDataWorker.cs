using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using VGMToolbox.format.util;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.xsf
{
    class XsfRecompressDataWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct XsfRecompressDataStruct : IVgmtWorkerStruct
        {
            public int CompressionLevel;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public XsfRecompressDataWorker() : base() { }

        protected override void DoTaskForFile(string pPath,
            IVgmtWorkerStruct pXsfRecompressDataStruct, DoWorkEventArgs e)
        {
            XsfRecompressDataStruct xsfRecompressDataStruct =
                (XsfRecompressDataStruct)pXsfRecompressDataStruct;

            XsfUtil.XsfRecompressStruct xsfRecompressStruct = new XsfUtil.XsfRecompressStruct();
            xsfRecompressStruct.CompressionLevel = xsfRecompressDataStruct.CompressionLevel;

            string outputPath = XsfUtil.ReCompressDataSection(pPath, xsfRecompressStruct);
        }    
    }
}
