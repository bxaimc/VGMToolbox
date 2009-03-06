using System;
using System.ComponentModel;
using System.IO;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class XsfCompressedProgramExtractorWorker : BackgroundWorker
    {
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct XsfCompressedProgramExtractorStruct
        {
            public string[] pPaths;
            public int totalFiles;
            public bool includeExtension;
            public bool stripGsfHeader;
        }

        public XsfCompressedProgramExtractorWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }


        private void extractCompressedPrograms(XsfCompressedProgramExtractorStruct pXsfCompressedProgramExtractorStruct, DoWorkEventArgs e)
        {
            foreach (string path in pXsfCompressedProgramExtractorStruct.pPaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.extractCompressedProgramFromFile(path, 
                            pXsfCompressedProgramExtractorStruct, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.extractCompressedProgramsFromDirectory(path, 
                        pXsfCompressedProgramExtractorStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }


        private void extractCompressedProgramsFromDirectory(string pPath, 
            XsfCompressedProgramExtractorStruct pXsfCompressedProgramExtractorStruct, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractCompressedProgramsFromDirectory(d, pXsfCompressedProgramExtractorStruct, e);
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
                    this.extractCompressedProgramFromFile(f, pXsfCompressedProgramExtractorStruct, e);
                    // fileCount++;
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void extractCompressedProgramFromFile(string pPath, 
            XsfCompressedProgramExtractorStruct pXsfCompressedProgramExtractorStruct, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);
         
            try
            {
                FileStream fs = File.OpenRead(pPath);
                Type dataType = FormatUtil.getObjectType(fs);

                if (dataType != null && dataType.Name.Equals("Xsf"))
                {
                    XsfUtil.Xsf2ExeStruct xsf2ExeStruct = new XsfUtil.Xsf2ExeStruct();
                    xsf2ExeStruct.IncludeExtension = pXsfCompressedProgramExtractorStruct.includeExtension;
                    xsf2ExeStruct.StripGsfHeader = pXsfCompressedProgramExtractorStruct.stripGsfHeader;
                    XsfUtil.Xsf2Exe(fs, pPath, xsf2ExeStruct);
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

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            XsfCompressedProgramExtractorStruct xsfCompressedProgramExtractorStruct = (XsfCompressedProgramExtractorStruct)e.Argument;
            maxFiles = xsfCompressedProgramExtractorStruct.totalFiles;

            this.extractCompressedPrograms(xsfCompressedProgramExtractorStruct, e);
        }    
    }
}
