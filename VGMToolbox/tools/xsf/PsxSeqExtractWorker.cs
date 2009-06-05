using System;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    public partial class PsxSeqExtractWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        private const string BATCH_FILE_NAME = "!timing_batch.bat";
        
        public struct PsxSeqExtractStruct : IVgmtWorkerStruct
        {            
            public bool force2Loops;
            public bool forceSepType;
            public bool forceSeqType;
            public bool loopEntireTrack;

            private string[] sourcePaths;
            public string[] SourcePaths 
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public PsxSeqExtractWorker()
            : base() {}

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPsxSeqExtractStruct, 
            DoWorkEventArgs e)
        {
            PsxSeqExtractStruct psxSeqExtractStruct = (PsxSeqExtractStruct)pPsxSeqExtractStruct;
            
            int minutes;
            double seconds;

            StringBuilder batchFile = new StringBuilder();
            string batchFilePath;
              
            string extractedSq = XsfUtil.ExtractPsxSequences(pPath);

            if (!String.IsNullOrEmpty(extractedSq))
            {
                PsxSequence psxSeq = null;
                
                using (FileStream fs = File.OpenRead(extractedSq))
                {
                    PsxSequence.PsxSqInitStruct initStruct = new PsxSequence.PsxSqInitStruct();
                    initStruct.force2Loops = psxSeqExtractStruct.force2Loops;
                    initStruct.forceOppositeFormatType = false;
                    initStruct.forceSepType = psxSeqExtractStruct.forceSepType;
                    initStruct.forceSeqType = psxSeqExtractStruct.forceSeqType;

                    try
                    {
                        psxSeq = new PsxSequence(fs, initStruct);
                    }
                    catch (PsxSeqFormatException)
                    {
                        // sometimes the version number is wrong, (Devil Summoner 1-50b.minipsf)
                        initStruct.forceOppositeFormatType = true;
                        psxSeq = new PsxSequence(fs, initStruct);
                    }
                }

                if (psxSeq != null)
                {
                    double timingInfoSeconds = psxSeq.TimingInfo.TimeInSeconds;
                    int timingInfoFadeInSeconds = psxSeq.TimingInfo.FadeInSeconds;

                    // loop entire track
                    if (psxSeqExtractStruct.loopEntireTrack)
                    {
                        timingInfoSeconds = 2d * psxSeq.TimingInfo.TimeInSeconds;
                        timingInfoFadeInSeconds = 10;
                    }
                    
                    // Add line to batch file.
                    minutes = (int)(timingInfoSeconds / 60d);
                    seconds = timingInfoSeconds - (minutes * 60);
                    // seconds = Math.Ceiling(seconds);

                    // shouldn't be needed without Math.Ceiling call, but whatever 
                    if (seconds >= 60)
                    {
                        minutes++;
                        seconds -= 60d;
                    }

                    batchFile.AppendFormat("psfpoint.exe -length=\"{0}:{1}\" -fade=\"{2}\" \"{3}\"",
                        minutes.ToString(), seconds.ToString().PadLeft(2, '0'),
                        timingInfoFadeInSeconds.ToString(), Path.GetFileName(pPath));
                    batchFile.Append(Environment.NewLine);

                    batchFilePath = Path.Combine(Path.GetDirectoryName(pPath), BATCH_FILE_NAME);

                    if (!File.Exists(batchFilePath))
                    {
                        using (FileStream cfs = File.Create(batchFilePath)) { };
                    }

                    using (StreamWriter sw = new StreamWriter(File.Open(batchFilePath, FileMode.Append, FileAccess.Write)))
                    {
                        sw.Write(batchFile.ToString());
                    }

                    // report warnings
                    if (!String.IsNullOrEmpty(psxSeq.TimingInfo.Warnings))
                    {
                        this.progressStruct.Clear();
                        progressStruct.GenericMessage = String.Format("{0}{1}  WARNINGS{2}    {3}", pPath, Environment.NewLine, Environment.NewLine, psxSeq.TimingInfo.Warnings);
                        ReportProgress(this.Progress, progressStruct);
                    }
                }

                File.Delete(extractedSq);
            }
        }        
    }
}
