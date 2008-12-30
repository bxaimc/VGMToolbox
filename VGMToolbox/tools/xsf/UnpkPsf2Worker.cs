using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

using VGMToolbox.format;
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
                using (FileStream fs = File.OpenRead(pPath))
                {
                    Type dataType = FormatUtil.getObjectType(fs);

                    if (dataType != null && dataType.Name.Equals("Xsf"))
                    {
                        Xsf psf2File = new Xsf(fs);

                        if (psf2File.getFormat().Equals(Xsf.FORMAT_NAME_PSF2))
                        {

                            string fileDir = Path.GetDirectoryName(Path.GetFullPath(pPath));
                            string fileName = Path.GetFileNameWithoutExtension(pPath);
                            
                            try
                            {
                                // copy unpkpsf2.exe to file's Dir
                                
                                // call unpkpsf2.exe

                                // delete unpkpsf2.exe
                            }
                            catch (Exception ex)
                            {
                                vProgressStruct = new Constants.ProgressStruct();
                                vProgressStruct.newNode = null;
                                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                                ReportProgress(progress, vProgressStruct);
                            }
                        }
                    }
                }
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
