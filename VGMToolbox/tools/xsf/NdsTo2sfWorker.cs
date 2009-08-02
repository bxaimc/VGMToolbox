using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.format.sdat;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class NdsTo2sfWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public const string TESTPACK_PATH = "external\\2sf\\testpack.nds";
        public const string TESTPACK_CRC32 = "FB16DF0E";
        public readonly string TESTPACK_FULL_PATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), TESTPACK_PATH);

        VGMToolbox.util.ProgressStruct progressStruct = new VGMToolbox.util.ProgressStruct();

        public struct NdsTo2sfStruct : IVgmtWorkerStruct
        {
            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public NdsTo2sfWorker()
        {
            this.progressStruct = new VGMToolbox.util.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pMk2sfStruct, DoWorkEventArgs e)
        {
            XsfUtil.NdsTo2sf(pPath, TESTPACK_FULL_PATH);
        }
    }
}
