using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.format.sdat;
using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class SdatOptimizerWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;
        Constants.ProgressStruct progressStruct;

        public struct SdatOptimizerStruct
        {
            public string[] sourcePaths;
            public string startSequence;
            public string endSequence;
        }

        public SdatOptimizerWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void optimizeSdat(SdatOptimizerStruct pSdatOptimizerStruct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pSdatOptimizerStruct.sourcePaths);

            foreach (string path in pSdatOptimizerStruct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.optimizeSdatFile(path, pSdatOptimizerStruct, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.optimizeSdatForDirectory(path, pSdatOptimizerStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void optimizeSdatFile(string pPath, SdatOptimizerStruct pSdatOptimizerStruct, DoWorkEventArgs e)
        {
            string sdatDirectory;
            string sdatOptimizingFileName;
            string sdatOptimizingPath;

            string sdatCompletedFileName;
            string sdatCompletedPath;

            Sdat sdat = null;
            int startSequence = Sdat.NO_SEQUENCE_RESTRICTION;
            int endSequence = Sdat.NO_SEQUENCE_RESTRICTION;

            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            progressStruct.Clear();
            progressStruct.filename = pPath;
            ReportProgress(progress, progressStruct);

            try
            {
                sdatDirectory = Path.GetDirectoryName(pPath);
                sdatOptimizingFileName = String.Format("{0}_OPTIMIZING.sdat", 
                    Path.GetFileNameWithoutExtension(pPath));
                sdatOptimizingPath = Path.Combine(sdatDirectory, sdatOptimizingFileName);

                sdatCompletedFileName = String.Format("{0}_OPTIMIZED.sdat",
                    Path.GetFileNameWithoutExtension(pPath));
                sdatCompletedPath = Path.Combine(sdatDirectory, sdatCompletedFileName);

                File.Copy(pPath, sdatOptimizingPath, true);

                using (FileStream fs = File.Open(sdatOptimizingPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    Type dataType = FormatUtil.getObjectType(fs);
                    
                    if (dataType != null && dataType.Name.Equals("Sdat"))
                    {
                        sdat = new Sdat();
                        sdat.Initialize(fs, sdatOptimizingPath);                        
                    }
                }

                if (sdat != null)
                {
                    if (!String.IsNullOrEmpty(pSdatOptimizerStruct.startSequence))
                    {
                        startSequence = (int)VGMToolbox.util.Encoding.GetIntFromString(pSdatOptimizerStruct.startSequence.Trim());
                    }

                    if (!String.IsNullOrEmpty(pSdatOptimizerStruct.endSequence))
                    {
                        endSequence = (int)VGMToolbox.util.Encoding.GetIntFromString(pSdatOptimizerStruct.endSequence);
                    }

                    sdat.OptimizeForZlib(startSequence, endSequence);                
                }

                File.Copy(sdatOptimizingPath, sdatCompletedPath, true);
                File.Delete(sdatOptimizingPath);
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, progressStruct);
            }
        }

        private void optimizeSdatForDirectory(string pPath, SdatOptimizerStruct pSdatOptimizerStruct, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.optimizeSdatForDirectory(d, pSdatOptimizerStruct, e);
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
                    this.optimizeSdatFile(f, pSdatOptimizerStruct, e);
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
            SdatOptimizerStruct sdatOptimizerStruct = (SdatOptimizerStruct)e.Argument;
            this.optimizeSdat(sdatOptimizerStruct, e);
        }         
    
    }        
}
