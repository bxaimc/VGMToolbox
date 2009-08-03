using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using VGMToolbox.util;

namespace VGMToolbox.plugin
{    
    /// <summary>
    /// Abstract class used as a skeleton for Drag and Drop forms.
    /// </summary>
    public abstract class AVgmtDragAndDropWorker : BackgroundWorker, IVgmtBackgroundWorker
    {        
        /// <summary>
        /// Field used to send progress information to the parent form.
        /// </summary>
        protected VGMToolbox.util.ProgressStruct progressStruct;

        /// <summary>
        /// Holds count of file currently being processed.
        /// </summary>
        private int fileCount;
        
        /// <summary>
        /// Holds count of total files droppped.
        /// </summary>
        private int maxFiles;
        
        /// <summary>
        /// Holds the percentage of files processed versus the total number of files to process.
        /// </summary>
        private int progress;

        /// <summary>
        /// Initializes a new instance of the AVgmtDragAndDropWorker class.
        /// </summary>
        protected AVgmtDragAndDropWorker()
        {
            this.progressStruct = new VGMToolbox.util.ProgressStruct();
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Gets or sets fileCount.
        /// </summary>
        protected int FileCount
        {
            get { return this.fileCount; }
            set { this.fileCount = value; }
        }
        
        /// <summary>
        /// Gets or sets maxFiles.
        /// </summary>
        protected int MaxFiles
        {
            get { return this.maxFiles; }
            set { this.maxFiles = value; }
        }
        
        /// <summary>
        /// Gets or sets progress.
        /// </summary>
        protected int Progress
        {
            get { return this.progress; }
            set { this.progress = value; }
        }
        
        protected void DoTask(IVgmtWorkerStruct taskParameters, DoWorkEventArgs e)
        {
            this.maxFiles = FileUtil.GetFileCount(taskParameters.SourcePaths);

            foreach (string path in taskParameters.SourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        // Report Progress
                        this.progress = (++this.fileCount * 100) / this.maxFiles;
                        this.progressStruct.Clear();
                        this.progressStruct.FileName = path;
                        ReportProgress(this.progress, this.progressStruct);
                                                
                        // perform task
                        try
                        {
                            this.DoTaskForFile(path, taskParameters, e);
                        }
                        catch (Exception ex)
                        {
                            this.progressStruct.Clear();
                            this.progressStruct.ErrorMessage = 
                                String.Format(CultureInfo.CurrentCulture, "Error processing <{0}>.  Error received: ", path) + ex.Message + Environment.NewLine;
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
                    this.DoTaskForDirectory(path, taskParameters, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            
            return;
        }

        protected virtual void DoTaskForDirectory(
            string directoryPath,
            IVgmtWorkerStruct taskParameters,
            DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(directoryPath))
            {
                if (!CancellationPending)
                {
                    this.DoTaskForDirectory(d, taskParameters, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }

            foreach (string f in Directory.GetFiles(directoryPath))
            {
                if (!CancellationPending)
                {
                    // Report Progress
                    this.progress = (++this.fileCount * 100) / this.maxFiles;
                    this.progressStruct.Clear();
                    this.progressStruct.FileName = f;
                    ReportProgress(this.progress, this.progressStruct);
                    
                    // perform task
                    try
                    {
                        this.DoTaskForFile(f, taskParameters, e);
                    }
                    catch (Exception ex)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.ErrorMessage = 
                            String.Format(CultureInfo.CurrentCulture, "Error processing <{0}>.  Error received: ", f) + ex.Message + Environment.NewLine;
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
                    break;
                }
            }
        }

        // this can be overridden in implementing classes that have more to do
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            this.DoTask((IVgmtWorkerStruct)e.Argument, e);
        }
        
        /// <summary>
        /// Method to call after completing processing of each file.
        /// </summary>
        protected virtual void DoFinally() 
        { 
        }

        // abstract methods
        protected abstract void DoTaskForFile(string filePath, IVgmtWorkerStruct pTaskStruct, DoWorkEventArgs e);
    }
}
