using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.plugin
{
    public abstract class AVgmtWorker : BackgroundWorker
    {
        protected int fileCount = 0;
        protected int maxFiles = 0;
        protected int progress = 0;
        protected Constants.ProgressStruct progressStruct;

        public AVgmtWorker()
        {
            this.fileCount = 0;
            this.maxFiles = 0;
            this.progressStruct = new Constants.ProgressStruct();

            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
        }

        protected void doTask(IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e)
        {
            this.maxFiles = FileUtil.GetFileCount(pTaskStruct.SourcePaths);

            foreach (string path in pTaskStruct.SourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        // Report Progress
                        this.progress = (++fileCount * 100) / maxFiles;
                        this.progressStruct.Clear();
                        this.progressStruct.filename = path;
                        ReportProgress(progress, progressStruct);
                        
                        
                        // perform task
                        try
                        {
                            this.doTaskForFile(path, pTaskStruct, e);
                        }
                        catch (Exception ex)
                        {
                            this.progressStruct.Clear();
                            this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", path) + ex.Message + Environment.NewLine;
                            ReportProgress(this.progress, this.progressStruct);                        
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.doTaskForDirectory(path, pTaskStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        protected void doTaskForDirectory(string pPath, IVgmtWorkerStruct pTaskStruct,
            DoWorkEventArgs e)
        {            
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.doTaskForDirectory(d, pTaskStruct, e);
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
                    // Report Progress
                    this.progress = (++fileCount * 100) / maxFiles;
                    this.progressStruct.Clear();
                    this.progressStruct.filename = f;
                    ReportProgress(this.progress, progressStruct);
                    
                    // perform task
                    try
                    {
                        this.doTaskForFile(f, pTaskStruct, e);
                    }
                    catch (Exception ex)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", f) + ex.Message + Environment.NewLine;
                        ReportProgress(progress, this.progressStruct);
                    }
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        // this can be overridden in implementing classes that have more to do
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            this.doTask((IVgmtWorkerStruct)e.Argument, e);
        }

        // abstract methods
        protected abstract void doTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e);
    }
}
