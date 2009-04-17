using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.genh
{
    public class GenhCreatorWorker : BackgroundWorker, IVgmtBackgroundWorker
    {
        public struct GenhCreatorStruct : IVgmtWorkerStruct
        {
            public string Format;
            public string HeaderSkip;
            public string Interleave;
            public string Channels;
            public string Frequency;

            public string LoopStart;
            public string LoopEnd;
            public bool NoLoops;
            public bool UseFileEnd;
            public bool FindLoop;

            public string CoefRightChannel;
            public string CoefLeftChannel;
            public bool CapcomHack;

            public bool OutputHeaderOnly;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        private void createGenhs(GenhCreatorStruct pGenhCreatorStruct, DoWorkEventArgs e)
        {
            foreach (string file in pGenhCreatorStruct.SourcePaths)
            {
                if (File.Exists(file))
                { 
                
                }
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            GenhCreatorStruct genhCreatorStruct = (GenhCreatorStruct)e.Argument;
            this.createGenhs(genhCreatorStruct, e);
        }
    }
}
