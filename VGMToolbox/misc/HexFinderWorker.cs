using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using VGMToolbox.auditing;

namespace VGMToolbox.misc
{
    class HexFinderWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct HexFinderStruct
        {
            public string[] pPaths;
            public int totalFiles;
        }

        public HexFinderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void findHexValues(HexFinderStruct pHexFinderStruct, DoWorkEventArgs e)
        {
            AuditingUtil.ProgressStruct vProgressStruct;

            foreach (string path in pHexFinderStruct.pPaths)
            { 
            
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            HexFinderStruct hexFinderStruct = (HexFinderStruct)e.Argument;
            maxFiles = hexFinderStruct.totalFiles;
            this.findHexValues(hexFinderStruct, e);
        } 
    }
}
