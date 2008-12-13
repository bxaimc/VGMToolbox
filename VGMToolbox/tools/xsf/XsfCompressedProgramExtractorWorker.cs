using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using VGMToolbox.auditing;
using VGMToolbox.format;

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
                            pXsfCompressedProgramExtractorStruct.includeExtension, e);
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
                        pXsfCompressedProgramExtractorStruct.includeExtension, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }


        private void extractCompressedProgramsFromDirectory(string pPath, bool pIncludeFileExtension, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.extractCompressedProgramsFromDirectory(d, pIncludeFileExtension, e);
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
                    this.extractCompressedProgramFromFile(f, pIncludeFileExtension, e);
                    // fileCount++;
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void extractCompressedProgramFromFile(string pPath, bool pIncludeFileExtension, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            AuditingUtil.ProgressStruct vProgressStruct = new AuditingUtil.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);
         
            try
            {
                FileStream fs = File.OpenRead(pPath);
                Type dataType = FormatUtil.getObjectType(fs);

                if (dataType != null && dataType.Name.Equals("Xsf"))
                {
                    Xsf vgmData = new Xsf();
                    vgmData.Initialize(fs);

                    if (vgmData.CompressedProgramLength > 0)
                    {
                        BinaryWriter bw;
                        string outputFile = Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar;
                        outputFile += (pIncludeFileExtension? Path.GetFileName(pPath) : Path.GetFileNameWithoutExtension(pPath)) + ".bin";
                        
                        
                        bw = new BinaryWriter(File.Create(outputFile));
                        
                        InflaterInputStream inflater;
                        int read;
                        byte[] data = new byte[4096];

                        fs.Seek((long)(Xsf.RESERVED_SECTION_OFFSET + vgmData.ReservedSectionLength), SeekOrigin.Begin);
                        inflater = new InflaterInputStream(fs);

                        while ((read = inflater.Read(data, 0, data.Length)) > 0)
                        {
                            bw.Write(data);
                        }
                        
                        bw.Close();
                        inflater.Close();
                        inflater.Dispose();                        
                    }
                }
                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                vProgressStruct = new AuditingUtil.ProgressStruct();
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
