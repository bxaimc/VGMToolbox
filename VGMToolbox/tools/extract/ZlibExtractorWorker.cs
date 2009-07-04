using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class ZlibExtractorWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public static string OUTPUT_EXTENSION = ".zlibx";
        
        public struct ZlibExtractorStruct : IVgmtWorkerStruct
        {
            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public ZlibExtractorWorker() : base() { }

        protected override void DoTaskForFile(string pPath,
            IVgmtWorkerStruct pZlibExtractorStruct, DoWorkEventArgs e)
        {
            ZlibExtractorStruct zlibExtractorStruct =
                (ZlibExtractorStruct)pZlibExtractorStruct;

            this.progressStruct.Clear();
            progressStruct.GenericMessage = String.Format("Extracting from <{0}>{1}", pPath, Environment.NewLine);
            ReportProgress(this.Progress, progressStruct);

            try
            {
                string outputFileName = Path.ChangeExtension(pPath, OUTPUT_EXTENSION);

                using (FileStream fs = File.OpenRead(pPath))
                {
                    CompressionUtil.DecompressZlibStreamToFile(fs, outputFileName);
                }

                this.progressStruct.Clear();
                progressStruct.GenericMessage = String.Format("    {0} extracted.{1}", Path.GetFileName(outputFileName), Environment.NewLine);
                ReportProgress(this.Progress, progressStruct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
