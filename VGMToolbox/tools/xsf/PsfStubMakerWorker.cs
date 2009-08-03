using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{        
    public struct PsfStubMakerStruct : IVgmtWorkerStruct
    {
        private string[] sourcePaths;
        private string driverText;
        
        public string[] SourcePaths
        {
            get { return sourcePaths; }
            set { sourcePaths = value; }
        }
        public string DriverText
        {
            set { driverText = value; }
            get { return driverText; }
        }
    }
    
    class PsfStubMakerWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public static readonly string PsfToolFolderPath =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "external"), "psf"));

        public static readonly string SigFindFilePath = Path.Combine(PsfToolFolderPath, "sigfind.exe");
        public static readonly string SigFind2FilePath = Path.Combine(PsfToolFolderPath, "sigfind2.exe");
        public static readonly string SigDatFilePath = Path.Combine(PsfToolFolderPath, "sig.dat");

        public static readonly string PsfOCycleSourceCodeFilePath = Path.Combine(PsfToolFolderPath, "psfdrv.c");
        public static readonly string PsfOCycleMakeFilePath = Path.Combine(PsfToolFolderPath, "mk.bat");

        public static readonly string WorkingFolderPath =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "working_psf"));         

        public PsfStubMakerWorker() : base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPsfStubMakerStruct, DoWorkEventArgs e)
        {
            if (XsfUtil.IsPsxExe(pPath))
            {
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("[{0}]", pPath) + Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                PsfStubMakerStruct stubMakerParameters = (PsfStubMakerStruct)pPsfStubMakerStruct;

                string sigFindDestination;
                string sigFind2Destination;
                string sigDatDestination;
                string driverDestination;
                string psfOCycleSourceDestination;
                string psfOCycleMakeFileDestination;

                string arguments;
                string standardOutput;
                string standardError;
                bool isProcessSuccessful;

                PsfPsyQAddresses sigFindAddresses = null;

                string psfdrvObjFileName;
                string psfdrvBinFileName;
                string psfdrvDestinationFileName;

                //////////////////////////
                // prepare working folder
                //////////////////////////
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = "    - Preparing working folder." + Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                if (!Directory.Exists(PsfStubMakerWorker.WorkingFolderPath))
                {
                    Directory.CreateDirectory(PsfStubMakerWorker.WorkingFolderPath);
                }

                sigFindDestination = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, Path.GetFileName(PsfStubMakerWorker.SigFindFilePath));
                sigFind2Destination = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, Path.GetFileName(PsfStubMakerWorker.SigFind2FilePath));
                sigDatDestination = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, Path.GetFileName(PsfStubMakerWorker.SigDatFilePath));
                driverDestination = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, Path.GetFileName(pPath).Replace(" ", String.Empty));
                psfOCycleSourceDestination = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, Path.GetFileName(PsfStubMakerWorker.PsfOCycleSourceCodeFilePath));
                psfOCycleMakeFileDestination = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, Path.GetFileName(PsfStubMakerWorker.PsfOCycleMakeFilePath));

                File.Copy(PsfStubMakerWorker.SigFindFilePath, sigFindDestination, true);
                File.Copy(PsfStubMakerWorker.SigFind2FilePath, sigFind2Destination, true);
                File.Copy(PsfStubMakerWorker.SigDatFilePath, sigDatDestination, true);
                File.Copy(pPath, driverDestination, true);
                File.Copy(PsfStubMakerWorker.PsfOCycleSourceCodeFilePath, psfOCycleSourceDestination, true);
                File.Copy(PsfStubMakerWorker.PsfOCycleMakeFilePath, psfOCycleMakeFileDestination, true);

                ////////////////////
                // call sigfind.exe
                ////////////////////            
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = "    - Execute sigfind/sigfind2." + Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                arguments = String.Format("{0}", Path.GetFileName(driverDestination));
                isProcessSuccessful = FileUtil.ExecuteExternalProgram(sigFindDestination, arguments,
                    PsfStubMakerWorker.WorkingFolderPath, out standardOutput, out standardError);

                /////////////////////
                // get psyQAddresses
                /////////////////////
                if ((isProcessSuccessful) && (String.IsNullOrEmpty(standardError)) &&
                    (!standardOutput.Contains("ERROR")))
                {
                    sigFindAddresses = XsfUtil.GetSigFindItems(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(standardOutput)));
                }

                /////////////////
                // call sigfind2
                /////////////////
                arguments = String.Format("{0}", Path.GetFileName(driverDestination));
                isProcessSuccessful = FileUtil.ExecuteExternalProgram(sigFind2Destination, arguments,
                    PsfStubMakerWorker.WorkingFolderPath, out standardOutput, out standardError);

                /////////////////////
                // get psyQAddresses
                /////////////////////
                if ((isProcessSuccessful) && (String.IsNullOrEmpty(standardError)) &&
                    (!standardOutput.Contains("ERROR")))
                {
                    sigFindAddresses = XsfUtil.GetSigFindItems(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(standardOutput)), sigFindAddresses);
                }

                /////////////////////////
                // get additional values
                /////////////////////////
                sigFindAddresses = this.getAdditionalDriverInfo(sigFindAddresses, stubMakerParameters, driverDestination);

                //////////////////////////////
                // rewrite driver source code
                //////////////////////////////
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = "    - Rewriting driver source and batch file." + Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                this.rewritePsfOCycleDriverSource(psfOCycleSourceDestination, sigFindAddresses);

                /////////////////////
                // rewrite make file
                /////////////////////
                this.rewritePsfOCycleMakeFile(psfOCycleMakeFileDestination, sigFindAddresses);

                //////////////////////////////////
                // delete old files if they exist
                //////////////////////////////////
                psfdrvObjFileName = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, "psfdrv.obj");
                psfdrvBinFileName = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, "psfdrv.bin");

                if (File.Exists(psfdrvObjFileName)) { File.Delete(psfdrvObjFileName); }
                if (File.Exists(psfdrvBinFileName)) { File.Delete(psfdrvBinFileName); }

                ////////////////
                // compile stub
                ////////////////
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = "    - Compiling stub." + Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                arguments = String.Empty;
                isProcessSuccessful = FileUtil.ExecuteExternalProgram(psfOCycleMakeFileDestination, arguments,
                    PsfStubMakerWorker.WorkingFolderPath, out standardOutput, out standardError);

                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("        STDOUT: {0}{1}", standardOutput, Environment.NewLine);
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("        STDERR: {0}{1}", standardError, Environment.NewLine);
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                ////////////////////////////////////////
                // copy driver and source to source dir
                ////////////////////////////////////////
                psfdrvDestinationFileName = Path.Combine(Path.GetDirectoryName(pPath), Path.GetFileName(pPath) + ".stub.bin");

                if (!File.Exists(psfdrvBinFileName))
                {
                    this.progressStruct.Clear();
                    this.progressStruct.ErrorMessage = "[ERROR from VGMToolbox] Compiled output file not found.  Compilation has failed.  Stub file source code will be copied to your source directory for examination." + Environment.NewLine;
                    ReportProgress(this.Progress, progressStruct);
                }
                else
                {
                    File.Copy(psfdrvBinFileName, psfdrvDestinationFileName, true);
                }                
                                
                File.Copy(psfOCycleSourceDestination, Path.Combine(Path.GetDirectoryName(pPath), Path.GetFileName(pPath) + ".stub.c"), true);

                //////////////////////
                // remove working dir
                //////////////////////
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = "    - Delete working folder." + Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                if (Directory.Exists(PsfStubMakerWorker.WorkingFolderPath)) { Directory.Delete(PsfStubMakerWorker.WorkingFolderPath, true); }

                // output warnings
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = getWarnings(sigFindAddresses);
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);
            }
            else
            {
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("[{0}] Skipped.  Does not have a valid PS-X EXE signature.", pPath) + Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);
            }
        }

        private PsfPsyQAddresses getAdditionalDriverInfo(PsfPsyQAddresses existingValues, PsfStubMakerStruct stubMakerParameters, 
            string driverPath)
        {
            string checksum;
            byte[] jumpAddress;
            PsfPsyQAddresses ret = existingValues;
            
            using (FileStream checksumStream = File.OpenRead(driverPath))
            {
                checksum = ChecksumUtil.GetCrc32OfFullFile(checksumStream);
                jumpAddress = ParseFile.ParseSimpleOffset(checksumStream, 0x10, 0x04);
            }
           
            ret.DriverTextString = stubMakerParameters.DriverText;
            ret.ExeFileNameCrc = String.Format("  (int)\"{0}\", 0x{1},", Path.GetFileName(driverPath), checksum);
            ret.JumpPatchAddress = String.Format("0x{0}", BitConverter.ToUInt32(jumpAddress, 0).ToString("X8"));

            return ret;
        }

        private void rewritePsfOCycleDriverSource(string driverSourceCodePath, PsfPsyQAddresses addresses)
        {
            int lineNumber;
            string inputLine;
            string tempFilePath = Path.GetTempFileName();

            string lineItem;
            string lineFormat;
            string lineValue;
            PropertyInfo psyQValue;

            Dictionary<int, string> psyQSourceCodeLineNumber = XsfUtil.getPsyQSourceCodeLineNumberList();
            Dictionary<string, string> psyQSourceCode = XsfUtil.getPsyQSourceCodeList();

            // open reader
            using (StreamReader reader =
                new StreamReader(File.Open(driverSourceCodePath, FileMode.Open, FileAccess.Read)))
            {
                // open writer for temp file
                using (StreamWriter writer =
                    new StreamWriter(File.Open(tempFilePath, FileMode.Create, FileAccess.Write)))
                {
                    lineNumber = 1;

                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        if (psyQSourceCodeLineNumber.ContainsKey(lineNumber))
                        {
                            lineItem = psyQSourceCodeLineNumber[lineNumber];
                            lineFormat = psyQSourceCode[lineItem];
                            psyQValue = addresses.GetType().GetProperty(lineItem);
                            lineValue = (string)psyQValue.GetValue(addresses, null);

                            if (!String.IsNullOrEmpty(lineValue))
                            {
                                writer.WriteLine(String.Format(lineFormat, lineValue));
                            }
                            else
                            {
                                // comment out this line
                                writer.WriteLine(String.Format("//{0}", inputLine));
                            }
                        }
                        else
                        {
                            writer.WriteLine(inputLine);
                        }

                        lineNumber++;
                    }
                } // using (StreamWriter writer...
            } // using (StreamReader reader...

            // overwrite original
            File.Copy(tempFilePath, driverSourceCodePath, true);
        }

        private void rewritePsfOCycleMakeFile(string makeFilePath, PsfPsyQAddresses addresses)
        {

            int lineNumber;
            string inputLine;
            string tempFilePath = Path.GetTempFileName();

            // open reader
            using (StreamReader reader =
                new StreamReader(File.Open(makeFilePath, FileMode.Open, FileAccess.Read)))
            {
                // open writer for temp file
                using (StreamWriter writer =
                    new StreamWriter(File.Open(tempFilePath, FileMode.Create, FileAccess.Write)))
                {
                    lineNumber = 1;

                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        switch (lineNumber)
                        { 
                            case 20:
                                writer.WriteLine(String.Format("psylink /o {0} /p /z psfdrv.obj,psfdrv.bin", addresses.PsfDrvLoadAddress));
                                break;
                            default:
                                writer.WriteLine(inputLine);
                                break;
                        }                        

                        lineNumber++;
                    }
                } // using (StreamWriter writer...
            } // using (StreamReader reader...

            // overwrite original
            File.Copy(tempFilePath, makeFilePath, true);
        }

        private string getWarnings(PsfPsyQAddresses addresses)
        {
            PropertyInfo psyQValue;
            string lineValue;
            StringBuilder warnings = new StringBuilder();
            
            foreach (string label in XsfUtil.GetPsyQFunctionList())            
            {
                psyQValue = addresses.GetType().GetProperty(label);
                lineValue = (string)psyQValue.GetValue(addresses, null);

                if (String.IsNullOrEmpty(lineValue))
                {
                    warnings.AppendFormat("    Warning: {0} function not found.{1}", label, Environment.NewLine);
                }
            }

            return warnings.ToString();
        }
    }
}
