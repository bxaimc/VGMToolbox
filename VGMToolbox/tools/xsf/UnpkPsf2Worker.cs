using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class UnpkPsf2Worker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct UnpkPsf2Struct
        {
            public string[] sourcePaths;
        }

        public UnpkPsf2Worker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void unpackPsf2s(UnpkPsf2Struct pUnpkPsf2Struct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pUnpkPsf2Struct.sourcePaths);

            foreach (string path in pUnpkPsf2Struct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.unpackPsf2FromFile(path, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.unpackPsf2FromDirectory(path, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void unpackPsf2FromFile(string pPath, DoWorkEventArgs e)
        {            
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);

            try
            {
                string stdOutput = null;
                string stdError = null;

                string unpackedDir = XsfUtil.UnpackPsf2(pPath, ref stdOutput, ref stdError);                
            }
            catch (Exception ex)
            {
                vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, vProgressStruct);
            }
        }    

        private void unpackPsf2FromDirectory(string pPath, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.unpackPsf2FromDirectory(d, e);
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
                    this.unpackPsf2FromFile(f, e);
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
            UnpkPsf2Struct unpkPsf2Struct = (UnpkPsf2Struct)e.Argument;

            this.unpackPsf2s(unpkPsf2Struct, e);
        }
    }
}
