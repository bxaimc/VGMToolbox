using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class UnpkPsf2Worker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public struct UnpkPsf2Struct : IVgmtWorkerStruct
        {
            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public UnpkPsf2Worker() : base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pUnpkPsf2Struct, 
            DoWorkEventArgs e)
        {
            string stdOutput;
            string stdError;
            string unpackedDir = XsfUtil.UnpackPsf2(pPath, out stdOutput, out stdError);

            if (!String.IsNullOrEmpty(stdError))
            {
                throw new ApplicationException(String.Format("Error unpacking <{0}>: {1}{2}", pPath, stdError,
                    Environment.NewLine));
            }
            //else
            //{
            //    this.progressStruct.Clear();
            //    this.progressStruct.GenericMessage = String.Format("{0}{1}", stdOutput, Environment.NewLine);
            //    ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
            //}


            //string xsfString = XsfUtil.GetXsfFormatString(pPath);

            //if (!String.IsNullOrEmpty(xsfString) &&
            //    xsfString.Equals(Psf2.FORMAT_NAME_PSF2))
            //{
            //    Psf2 vgmData = null;
            //    using (FileStream fs = File.OpenRead(pPath))
            //    {
            //        vgmData = new Psf2();
            //        vgmData.Initialize(fs, pPath);
            //    }
            //    vgmData.Unpack(Path.Combine(Path.GetDirectoryName(pPath), Path.GetFileNameWithoutExtension(pPath)));
            //}             
        }            
    }
}
