using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.genh
{
    public class GenhCreatorWorker : BackgroundWorker, IVgmtBackgroundWorker
    {
        private int fileCount;
        private int maxFiles;
        ProgressStruct progressStruct = new ProgressStruct();
        
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

        public GenhCreatorWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            this.progressStruct = new VGMToolbox.util.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;        
        }

        private void createGenhs(GenhCreatorStruct pGenhCreatorStruct, DoWorkEventArgs e)
        {
            int progress;

            this.maxFiles = pGenhCreatorStruct.SourcePaths.Length;

            foreach (string file in pGenhCreatorStruct.SourcePaths)
            {
                progress = (++this.fileCount * 100) / this.maxFiles;
                this.progressStruct.Clear();
                this.progressStruct.Filename = file;
                ReportProgress(progress, this.progressStruct);
                
                if (File.Exists(file))
                {
                    GenhCreationStruct genhCreationStruct = new GenhCreationStruct();
                    genhCreationStruct.Format = pGenhCreatorStruct.Format;
                    genhCreationStruct.HeaderSkip = pGenhCreatorStruct.HeaderSkip;
                    genhCreationStruct.Interleave = pGenhCreatorStruct.Interleave;
                    genhCreationStruct.Channels = pGenhCreatorStruct.Channels;
                    genhCreationStruct.Frequency = pGenhCreatorStruct.Frequency;
                    genhCreationStruct.LoopStart = pGenhCreatorStruct.LoopStart;
                    genhCreationStruct.LoopEnd = pGenhCreatorStruct.LoopEnd;
                    genhCreationStruct.NoLoops = pGenhCreatorStruct.NoLoops;
                    genhCreationStruct.UseFileEnd = pGenhCreatorStruct.UseFileEnd;
                    genhCreationStruct.FindLoop = pGenhCreatorStruct.FindLoop;
                    genhCreationStruct.CoefRightChannel = pGenhCreatorStruct.CoefRightChannel;
                    genhCreationStruct.CoefLeftChannel = pGenhCreatorStruct.CoefLeftChannel;
                    genhCreationStruct.CapcomHack = pGenhCreatorStruct.CapcomHack;
                    genhCreationStruct.OutputHeaderOnly = pGenhCreatorStruct.OutputHeaderOnly;
                    genhCreationStruct.SourcePaths = pGenhCreatorStruct.SourcePaths;

                    string genhFilePath = GenhUtil.CreateGenhFile(file, genhCreationStruct);

                    this.progressStruct.Clear();
                    this.progressStruct.GenericMessage = String.Format("{0} Created.{1}", genhFilePath, Environment.NewLine);
                    ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
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
