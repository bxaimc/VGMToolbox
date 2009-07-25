using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format.util;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.xsf
{
    public struct PsfStubMakerStruct : IVgmtWorkerStruct
    {
        private string[] sourcePaths;
        public string[] SourcePaths
        {
            get { return sourcePaths; }
            set { sourcePaths = value; }
        }
    }
    
    class PsfStubMakerWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {        
        public PsfStubMakerWorker() : base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPsfStubMakerStruct, DoWorkEventArgs e)
        {                        
            // call sigfind

            // call sigfind2

            // get psyQAddresses

            // compile stub
        }
    }
}
