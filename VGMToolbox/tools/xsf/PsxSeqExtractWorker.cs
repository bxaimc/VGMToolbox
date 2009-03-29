using System;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    public partial class PsxSeqExtractWorker : BackgroundWorker
    {
        private const string BATCH_FILE_NAME = "!timing_batch.bat";
        
        private int fileCount = 0;
        private int maxFiles = 0;
        Constants.ProgressStruct progressStruct;

        public struct PsxSeqExtractStruct
        {
            public string[] sourcePaths;
            public bool force2Loops;
            public bool forceSepType;
            public bool forceSeqType;
        }

        public PsxSeqExtractWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void extractSeqs(PsxSeqExtractStruct pPsxSeqExtractStruct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pPsxSeqExtractStruct.sourcePaths);

            foreach (string path in pPsxSeqExtractStruct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.extractSeqsFromFile(path, pPsxSeqExtractStruct, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.extractSeqsFromDirectory(path, pPsxSeqExtractStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void extractSeqsFromFile(string pPath, PsxSeqExtractStruct pPsxSeqExtractStruct, 
            DoWorkEventArgs e)
        {
            int minutes;
            double seconds;

            StringBuilder batchFile = new StringBuilder();
            string batchFilePath;


            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            progressStruct.Clear();
            progressStruct.filename = pPath;
            ReportProgress(progress, progressStruct);

            try
            {                
                string extractedSq = XsfUtil.ExtractPsxSequences(pPath);

                if (!String.IsNullOrEmpty(extractedSq))
                {
                    PsxSequence psxSeq = null;
                    
                    using (FileStream fs = File.OpenRead(extractedSq))
                    {
                        PsxSequence.PsxSqInitStruct initStruct = new PsxSequence.PsxSqInitStruct();
                        initStruct.force2Loops = pPsxSeqExtractStruct.force2Loops;
                        initStruct.forceOppositeFormatType = false;
                        initStruct.forceSepType = pPsxSeqExtractStruct.forceSepType;
                        initStruct.forceSeqType = pPsxSeqExtractStruct.forceSeqType;

                        try
                        {
                            psxSeq = new PsxSequence(fs, initStruct);
                        }
                        catch (PsxSeqFormatException seqEx)
                        {
                            // sometimes the version number is wrong, (Devil Summoner 1-50b.minipsf)
                            initStruct.forceOppositeFormatType = true;
                            psxSeq = new PsxSequence(fs, initStruct);
                        }
                    }

                    if (psxSeq != null)
                    {
                        // Add line to batch file.
                        minutes = (int)(psxSeq.TimingInfo.TimeInSeconds / 60d);
                        seconds = psxSeq.TimingInfo.TimeInSeconds - (minutes * 60);
                        seconds = Math.Ceiling(seconds);

                        if (seconds > 59)
                        {
                            minutes++;
                            seconds -= 60;
                        }

                        batchFile.AppendFormat("psfpoint.exe -length=\"{0}:{1}\" -fade=\"{2}\" \"{3}\"",
                            minutes.ToString(), seconds.ToString().PadLeft(2, '0'),
                            psxSeq.TimingInfo.FadeInSeconds.ToString(), Path.GetFileName(pPath));
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
                            progressStruct.genericMessage = String.Format("{0}{1}  WARNINGS{2}    {3}", pPath, Environment.NewLine, Environment.NewLine, psxSeq.TimingInfo.Warnings);
                            ReportProgress(progress, progressStruct);
                        }
                    }

                    File.Delete(extractedSq);
                }
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message + Environment.NewLine;
                ReportProgress(progress, this.progressStruct);
            }
        }

        private void extractSeqsFromDirectory(string pPath, PsxSeqExtractStruct pPsxSeqExtractStruct,
            DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractSeqsFromDirectory(d, pPsxSeqExtractStruct, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
            foreach (string f in Directory.GetFiles(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractSeqsFromFile(f, pPsxSeqExtractStruct, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            PsxSeqExtractStruct psxSeqExtractStruct = (PsxSeqExtractStruct)e.Argument;

            this.extractSeqs(psxSeqExtractStruct, e);
        }        
    }
}
