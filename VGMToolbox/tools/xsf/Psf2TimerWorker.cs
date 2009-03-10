using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class Psf2TimerWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        Constants.ProgressStruct progressStruct;

        public struct Psf2TimerStruct
        {
            public string[] sourcePaths;
        }

        public Psf2TimerWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void timePsf2s(Psf2TimerStruct pPsf2TimerStruct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pPsf2TimerStruct.sourcePaths);

            foreach (string path in pPsf2TimerStruct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.timePsf2File(path, pPsf2TimerStruct, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.timePsf2sFromDirectory(path, pPsf2TimerStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void timePsf2File(string pPath, Psf2TimerStruct pPsf2TimerStruct, DoWorkEventArgs e)
        {
            UInt32 tempo;
            UInt32 endOfTrack;
            Ps2SequenceData.Ps2SqTimingStruct time;

            int minutes;
            double seconds;

            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            progressStruct.Clear();
            progressStruct.filename = pPath;
            ReportProgress(progress, progressStruct);

            try
            {
                using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read))
                {
                    this.progressStruct.Clear();
                    this.progressStruct.genericMessage = String.Format("[{0}]", pPath) + Environment.NewLine;
                    ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
                    
                    Ps2SequenceData ps2SequenceData = new Ps2SequenceData(fs);

                    for (int i = 0; i <= ps2SequenceData.GetMaxSequenceCount(); i++)
                    {
                        /*
                        tempo = ps2SequenceData.getTempoForSequenceNumber(fs, i);                        
                        this.progressStruct.Clear();
                        this.progressStruct.genericMessage = String.Format("  Tempo for track {0}: {1} microseconds/quarter note", i.ToString(), tempo.ToString()) + Environment.NewLine;
                        ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);

                        endOfTrack = ps2SequenceData.getEndOfTrackForSequenceNumber(fs, i);
                        this.progressStruct.Clear();
                        this.progressStruct.genericMessage = String.Format("  End of Track Offset for track {0}: 0x{1}", i.ToString(), endOfTrack.ToString("X8")) + Environment.NewLine;
                        ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
                        */
                        time = ps2SequenceData.getTimeInSecondsForSequenceNumber(fs, i);

                        minutes = (int)(time.TimeInSeconds / 60d);
                        seconds = (time.TimeInSeconds - (minutes * 60));
                        seconds = Math.Ceiling(seconds);

                        this.progressStruct.Clear();
                        this.progressStruct.genericMessage = String.Format("  Time, Fade for track {0}: {1}:{2}, {3}", i.ToString(), minutes.ToString(), seconds.ToString().PadLeft(2, '0'), time.FadeInSeconds.ToString().PadLeft(2, '0')) + Environment.NewLine;
                        ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
                    }
                }
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, progressStruct);
            }
            finally
            {

            }
        }

        private void timePsf2sFromDirectory(string pPath, Psf2TimerStruct pPsf2TimerStruct, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.timePsf2sFromDirectory(d, pPsf2TimerStruct, e);
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
                    this.timePsf2File(f, pPsf2TimerStruct, e);
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
            Psf2TimerStruct psf2TimerStruct = (Psf2TimerStruct)e.Argument;
            this.timePsf2s(psf2TimerStruct, e);
        }  
    
    }
}
