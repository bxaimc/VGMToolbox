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
    public struct GenhCreatorStruct : IVgmtWorkerStruct
    {
        private string format;
        private string headerSkip;
        private string interleave;
        private string channels;
        private string frequency;
        private string loopStart;
        private string loopEnd;
        private bool noLoops;
        private bool useFileEnd;
        private bool findLoop;
        private string coefRightChannel;
        private string coefLeftChannel;
        private bool capcomHack;
        private bool outputHeaderOnly;
        private string[] sourcePaths;
        private bool doCreation;
        private bool doEdit;
        private bool doExtract;

        public string Format
        {
            set { format = value; }
            get { return format; }
        }
        public string HeaderSkip
        {
            set { headerSkip = value; }
            get { return headerSkip; }
        }
        public string Interleave
        {
            set { interleave = value; }
            get { return interleave; }
        }
        public string Channels
        {
            set { channels = value; }
            get { return channels; }
        }
        public string Frequency
        {
            set { frequency = value; }
            get { return frequency; }
        }
        public string LoopStart
        {
            set { loopStart = value; }
            get { return loopStart; }
        }
        public string LoopEnd
        {
            set { loopEnd = value; }
            get { return loopEnd; }
        }
        public bool NoLoops
        {
            set { noLoops = value; }
            get { return noLoops; }
        }
        public bool UseFileEnd
        {
            set { useFileEnd = value; }
            get { return useFileEnd; }
        }
        public bool FindLoop
        {
            set { findLoop = value; }
            get { return findLoop; }
        }
        public string CoefRightChannel
        {
            set { coefRightChannel = value; }
            get { return coefRightChannel; }
        }
        public string CoefLeftChannel
        {
            set { coefLeftChannel = value; }
            get { return coefLeftChannel; }
        }
        public bool CapcomHack
        {
            set { capcomHack = value; }
            get { return capcomHack; }
        }
        public bool OutputHeaderOnly
        {
            set { outputHeaderOnly = value; }
            get { return outputHeaderOnly; }
        }
        public string[] SourcePaths
        {
            get { return sourcePaths; }
            set { sourcePaths = value; }
        }
        public bool DoCreation
        {
            set { doCreation = value; }
            get { return doCreation; }
        }
        public bool DoEdit
        {
            set { doEdit = value; }
            get { return doEdit; }
        }
        public bool DoExtract
        {
            set { doExtract = value; }
            get { return doExtract; }
        }

        public GenhCreationStruct ToGenhCreationStruct()
        {
            GenhCreationStruct genhCreationStruct = new GenhCreationStruct();

            genhCreationStruct.Format = this.Format;
            genhCreationStruct.HeaderSkip = this.HeaderSkip;
            genhCreationStruct.Interleave = this.Interleave;
            genhCreationStruct.Channels = this.Channels;
            genhCreationStruct.Frequency = this.Frequency;
            genhCreationStruct.LoopStart = this.LoopStart;
            genhCreationStruct.LoopEnd = this.LoopEnd;
            genhCreationStruct.NoLoops = this.NoLoops;
            genhCreationStruct.UseFileEnd = this.UseFileEnd;
            genhCreationStruct.FindLoop = this.FindLoop;
            genhCreationStruct.CoefRightChannel = this.CoefRightChannel;
            genhCreationStruct.CoefLeftChannel = this.CoefLeftChannel;
            genhCreationStruct.CapcomHack = this.CapcomHack;
            genhCreationStruct.OutputHeaderOnly = this.OutputHeaderOnly;
            genhCreationStruct.SourcePaths = this.SourcePaths;

            return genhCreationStruct;
        }
    }
    
    public class GenhCreatorWorker : BackgroundWorker, IVgmtBackgroundWorker
    {
        private int fileCount;
        private int maxFiles;
        ProgressStruct progressStruct = new ProgressStruct();
        
        public GenhCreatorWorker()
        {
            this.progressStruct = new VGMToolbox.util.ProgressStruct();
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void createGenhs(GenhCreatorStruct pGenhCreatorStruct, DoWorkEventArgs e)
        {
            int progress;

            this.maxFiles = pGenhCreatorStruct.SourcePaths.Length;
            
            string outputFilePath = String.Empty;
            string outputMessageAction = String.Empty;

            foreach (string file in pGenhCreatorStruct.SourcePaths)
            {
                progress = (++this.fileCount * 100) / this.maxFiles;
                this.progressStruct.Clear();
                this.progressStruct.FileName = file;
                ReportProgress(progress, this.progressStruct);
                
                if (File.Exists(file))
                {
                    if (pGenhCreatorStruct.DoExtract)
                    {
                        outputFilePath = GenhUtil.ExtractGenhFile(file, true, true);
                        outputMessageAction = "Extracted";
                    }
                    else
                    {
                        GenhCreationStruct genhCreationStruct = pGenhCreatorStruct.ToGenhCreationStruct();

                        if (pGenhCreatorStruct.DoCreation)
                        {
                            outputFilePath = GenhUtil.CreateGenhFile(file, genhCreationStruct);
                            outputMessageAction = "Created";
                        }
                    }

                    if (!String.IsNullOrEmpty(outputFilePath))
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.GenericMessage = String.Format("{0} {1}.{2}", outputFilePath, outputMessageAction, Environment.NewLine);
                        ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                    }
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
