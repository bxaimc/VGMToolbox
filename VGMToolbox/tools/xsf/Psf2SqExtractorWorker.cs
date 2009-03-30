using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;

namespace VGMToolbox.tools.xsf
{
    class Psf2SqExtractorWorker : AVgmtWorker
    {
        Dictionary<string, string> extractedLibHash;

        public struct Psf2SqExtractorStruct : IVgmtWorkerStruct
        {
            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public Psf2SqExtractorWorker() : 
            base()
        {
            extractedLibHash = new Dictionary<string, string>();
        }

        protected override void doTaskForFile(string pPath, IVgmtWorkerStruct pPsf2SqExtractorStruct, 
            DoWorkEventArgs e)
        {
            string outputSqFileName;
            string[] libPaths;            
            string[] sqFiles;
            string[] iniFiles;
            Psf2.Psf2IniSqIrxStruct psf2IniStruct;
            
            string filePath;
            string fileDir;
            string fileName;
            string outputDir;
            string libOutputDir;
            
            string unpkOutput = null;
            string unpkError = null;

            string formatString = XsfUtil.GetXsfFormatString(pPath);

            if (!String.IsNullOrEmpty(formatString) && (formatString.Equals(Xsf.FORMAT_NAME_PSF2)))
            {                                
                filePath = Path.GetFullPath(pPath);
                fileDir = Path.GetDirectoryName(filePath);
                fileName = Path.GetFileNameWithoutExtension(filePath);
                
                outputDir = XsfUtil.UnpackPsf2(filePath, ref unpkOutput, ref unpkError);
                
                // parse ini
                iniFiles = Directory.GetFiles(outputDir, "PSF2.INI", SearchOption.AllDirectories);
                using (FileStream iniFs = File.Open(iniFiles[0], FileMode.Open, FileAccess.Read))
                {
                    // parse ini file to get SQ info
                    psf2IniStruct = Psf2.ParseClsIniFile(iniFs);
                }

                using (FileStream fs = File.OpenRead(pPath))
                {
                    Psf2 psf2File = new Psf2();
                    psf2File.Initialize(fs, pPath);
                    
                    // check for libs
                    libPaths = psf2File.GetLibPathArray();
                }
                if ((libPaths == null) || (libPaths.Length == 0)) // PSF2
                {
                    // copy the SQ file out (should only be one)
                    sqFiles = Directory.GetFiles(outputDir, psf2IniStruct.SqFileName, SearchOption.AllDirectories);

                    if (sqFiles.Length > 0)
                    {
                        if (!String.IsNullOrEmpty(psf2IniStruct.SequenceNumber))
                        {
                            outputSqFileName = String.Format("{0}_n={1}.SQ", outputDir, psf2IniStruct.SequenceNumber);
                        }
                        else
                        {
                            outputSqFileName = String.Format("{0}.SQ", outputDir);                                        
                        }
                        File.Copy(sqFiles[0], outputSqFileName, true);
                    }
                }
                else // miniPSF2
                {                                    
                    // unpack each lib, looking for the needed file
                    foreach (string libPath in libPaths)
                    {
                        fileDir = Path.GetDirectoryName(libPath);
                        fileName = Path.GetFileNameWithoutExtension(libPath);
                        libOutputDir = Path.Combine(fileDir, fileName);

                        if (!extractedLibHash.ContainsKey(libPath))
                        {
                            libOutputDir = XsfUtil.UnpackPsf2(libPath, ref unpkOutput, ref unpkError);                                           
                            extractedLibHash.Add(libPath, libOutputDir);
                        }

                        // look for the file in this lib
                        sqFiles = Directory.GetFiles(libOutputDir, psf2IniStruct.SqFileName, SearchOption.AllDirectories);

                        if (sqFiles.Length > 0)
                        {
                            if (!String.IsNullOrEmpty(psf2IniStruct.SequenceNumber))
                            {
                                outputSqFileName = String.Format("{0}_n={1}.SQ", outputDir, psf2IniStruct.SequenceNumber);
                            }
                            else
                            {
                                outputSqFileName = String.Format("{0}.SQ", outputDir);
                            }
                            
                            File.Copy(sqFiles[0], outputSqFileName, true);                                                                                       
                            break;
                        }

                        // delete the unpkpsf2 output folder
                        Directory.Delete(libOutputDir, true);
                    
                    } // foreach (string libPath in libPaths)
                }
                // delete the unpkpsf2 output folder
                Directory.Delete(outputDir, true);
            } // if (!String.IsNullOrEmpty(formatString) && (formatString.Equals(Xsf.FORMAT_NAME_PSF2)))
        }    

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            base.OnDoWork(e);

            // delete lib folders
            foreach (string k in extractedLibHash.Keys)
            {
                Directory.Delete(extractedLibHash[k], true);
            }
        }        
    }
}
