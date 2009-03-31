using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.plugin
{
    public abstract class AVgmtWorker : BackgroundWorker
    {
        private int fileCount;
        private int maxFiles;
        private int progress;
        protected Constants.ProgressStruct progressStruct;

        protected int Progress
        {
            get { return progress; }
            set { progress = value; }
        }

        protected AVgmtWorker()
        {
            this.progressStruct = new Constants.ProgressStruct();
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
        }

        protected void DoTask(IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e)
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
                            this.DoTaskForFile(path, pTaskStruct, e);
                        }
                        catch (Exception ex)
                        {
                            this.progressStruct.Clear();
                            this.progressStruct.errorMessage = 
                                String.Format(CultureInfo.CurrentCulture,"Error processing <{0}>.  Error received: ", path) + ex.Message + Environment.NewLine;
                            ReportProgress(this.progress, this.progressStruct);
                        }
                        finally
                        {
                            this.DoFinally();
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
                    this.DoTaskForDirectory(path, pTaskStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        protected void DoTaskForDirectory(string pPath, IVgmtWorkerStruct pTaskStruct,
            DoWorkEventArgs e)
        {            
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.DoTaskForDirectory(d, pTaskStruct, e);
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
                        this.DoTaskForFile(f, pTaskStruct, e);
                    }
                    catch (Exception ex)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.errorMessage = 
                            String.Format(CultureInfo.CurrentCulture, "Error processing <{0}>.  Error received: ", f) + ex.Message + Environment.NewLine;
                        ReportProgress(progress, this.progressStruct);
                    }
                    finally
                    {
                        this.DoFinally();
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
            this.DoTask((IVgmtWorkerStruct)e.Argument, e);
        }
        protected virtual void DoFinally() { }

        // abstract methods
        protected abstract void DoTaskForFile(string pPath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e);
    }
}
