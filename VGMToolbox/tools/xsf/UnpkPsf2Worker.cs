using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class UnpkPsf2Worker : AVgmtDragAndDropWorker
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
        }            
    }
}
