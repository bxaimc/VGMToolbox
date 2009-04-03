using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class MkPsf2Worker : BackgroundWorker, IVgmtBackgroundWorker
    {
        private const int NO_SEQ_NUM_FOUND = -1;

        private readonly string WORKING_FOLDER =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "working_psf2"));
        private readonly string MODULES_FOLDER =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"), "psf2"), "modules"));
        private readonly string PROGRAMS_FOLDER =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"), "psf2"));
        private readonly string OUTPUT_FOLDER =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "rips"), "psf2s"));

        private const long PSF2_CSL_SEQ_COUNT_OFFSET = 0x3C;
        private const long PSF2_CSL_SEQ_OFFSET_BEGIN = 0x40;

        private int fileCount = 0;
        private int maxFiles = 0;

        private bool[] validSequenceArray;

        public struct MkPsf2Struct
        {
            public string sourcePath;
            public string modulePath;
            public string outputFolder;

            public string tickInterval;
            public string reverb;
            public string depth;
            public string tempo;
            public string volume;
        }

        public MkPsf2Worker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }


        private void makePsf2s(MkPsf2Struct pMkPsf2Struct, DoWorkEventArgs e)
        {
            string[] uniqueSqFiles;

            if (!CancellationPending)
            {
                // get list of unique files
                uniqueSqFiles = this.getUniqueFileNames(pMkPsf2Struct.sourcePath);
                if (uniqueSqFiles != null)
                {
                    this.maxFiles = uniqueSqFiles.Length;
                    this.buildPsf2s(uniqueSqFiles, pMkPsf2Struct, e);
                }
            }
            else
            {
                e.Cancel = true;
                return;
            }
                             
            return;
        }

        private string[] getUniqueFileNames(string pSourceDirectory)
        {
            int fileCount = 0;
            int i = 0;
            string[] ret = null;

            VGMToolbox.util.ProgressStruct vProgressStruct = new VGMToolbox.util.ProgressStruct();

            if (!Directory.Exists(pSourceDirectory))
            {
                vProgressStruct = new VGMToolbox.util.ProgressStruct();
                vProgressStruct.NewNode = null;
                vProgressStruct.ErrorMessage = String.Format("ERROR: Directory {0} not found.", pSourceDirectory);
                ReportProgress(Constants.PROGRESS_MSG_ONLY, vProgressStruct);
            }
            else
            {
                fileCount = Directory.GetFiles(pSourceDirectory, "*.SQ").Length;

                if (fileCount > 0)
                {
                    ret = new string[fileCount];
                }

                foreach (string f in Directory.GetFiles(pSourceDirectory, "*.SQ"))
                {
                    ret[i] = f;
                    i++;
                }
            }

            return ret;
        }

        private void buildPsf2s(string[] pUniqueSqFiles, MkPsf2Struct pMkPsf2Struct, 
            DoWorkEventArgs e)
        {
            Process makePsf2Process;
            VGMToolbox.util.ProgressStruct vProgressStruct = new VGMToolbox.util.ProgressStruct();

            string makePsf2SourcePath = Path.Combine(PROGRAMS_FOLDER, "mkpsf2.exe");
            string makePsf2DestinationPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "mkpsf2.exe");

            try
            {
                Directory.CreateDirectory(WORKING_FOLDER);

                // copy generic modules to working directory
                File.Copy(Path.Combine(MODULES_FOLDER, "psf2.irx"), Path.Combine(WORKING_FOLDER, "psf2.irx"), true);
                File.Copy(Path.Combine(MODULES_FOLDER, "sq.irx"), Path.Combine(WORKING_FOLDER, "sq.irx"), true);

                // copy source modules to working directory
                File.Copy(Path.Combine(pMkPsf2Struct.modulePath, "LIBSD.IRX"), Path.Combine(WORKING_FOLDER, "LIBSD.IRX"), true);
                File.Copy(Path.Combine(pMkPsf2Struct.modulePath, "MODHSYN.IRX"), Path.Combine(WORKING_FOLDER, "MODHSYN.IRX"), true);
                File.Copy(Path.Combine(pMkPsf2Struct.modulePath, "MODMIDI.IRX"), Path.Combine(WORKING_FOLDER, "MODMIDI.IRX"), true);

                // copy program
                File.Copy(makePsf2SourcePath, makePsf2DestinationPath, true);
            }
            catch (Exception ex)
            {
                vProgressStruct = new VGMToolbox.util.ProgressStruct();
                vProgressStruct.NewNode = null;
                vProgressStruct.Filename = null;
                vProgressStruct.ErrorMessage = ex.Message;
                ReportProgress(0, vProgressStruct);

                return;
            }
            
            foreach (string f in pUniqueSqFiles)
            {
                if (!CancellationPending)
                {
                    StringBuilder sqArguments = new StringBuilder();
                    int totalSequences;

                    // report progress
                    int progress = (++this.fileCount * 100) / maxFiles;
                    vProgressStruct = new VGMToolbox.util.ProgressStruct();
                    vProgressStruct.NewNode = null;
                    vProgressStruct.Filename = f;
                    ReportProgress(progress, vProgressStruct);

                    try
                    {
                        // copy data files to working directory                    
                        string filePrefix = Path.GetFileNameWithoutExtension(f);
                        string sourceDirectory = Path.GetDirectoryName(f);
                        string outputFilePrefix;

                        string bdFileName = filePrefix + ".bd";
                        string hdFileName = filePrefix + ".hd";
                        string sqFileName = filePrefix + ".sq";

                        string sourceBdFile = Path.Combine(sourceDirectory, bdFileName);
                        string sourceHdFile = Path.Combine(sourceDirectory, hdFileName);
                        string sourceSqFile = Path.Combine(sourceDirectory, sqFileName);

                        string destinationBdFile = Path.Combine(WORKING_FOLDER, bdFileName);
                        string destinationHdFile = Path.Combine(WORKING_FOLDER, hdFileName);
                        string destinationSqFile = Path.Combine(WORKING_FOLDER, sqFileName);

                        FileInfo fi = new FileInfo(sourceSqFile);

                        if (fi.Length > 0) // only make for nonempty SQ files
                        {
                            File.Copy(sourceBdFile, destinationBdFile);
                            File.Copy(sourceHdFile, destinationHdFile);
                            File.Copy(sourceSqFile, destinationSqFile);

                            totalSequences = getTotalSequenceCount(destinationSqFile);

                            for (int i = 0; i < totalSequences; i++)
                            {
                                if (validSequenceArray[i])
                                {
                                    sqArguments = new StringBuilder();

                                    if (totalSequences > 1)
                                    {
                                        outputFilePrefix = filePrefix + "_" + i.ToString("X2");
                                        sqArguments.Append(String.Format(" -n={0}", i));
                                    }
                                    else
                                    {
                                        outputFilePrefix = filePrefix;
                                    }

                                    // write ini file
                                    string iniPath = Path.Combine(WORKING_FOLDER, "psf2.ini");
                                    StreamWriter sw = File.CreateText(iniPath);
                                    sw.WriteLine("libsd.irx");
                                    sw.WriteLine("modhsyn.irx");
                                    sw.WriteLine("modmidi.irx");

                                    // build sq.irx arguments                    
                                    sqArguments.Append(String.IsNullOrEmpty(pMkPsf2Struct.reverb.Trim()) ?
                                        " -r=5" : String.Format(" -r={0}", pMkPsf2Struct.reverb.Trim()));
                                    sqArguments.Append(String.IsNullOrEmpty(pMkPsf2Struct.depth.Trim()) ?
                                        " -d=16383" : String.Format(" -d={0}", pMkPsf2Struct.depth.Trim()));

                                    sqArguments.Append(String.IsNullOrEmpty(pMkPsf2Struct.tickInterval.Trim()) ?
                                        String.Empty : String.Format(" -u={0}", pMkPsf2Struct.tickInterval.Trim()));
                                    sqArguments.Append(String.IsNullOrEmpty(pMkPsf2Struct.tempo.Trim()) ?
                                        String.Empty : String.Format(" -t={0}", pMkPsf2Struct.tempo.Trim()));
                                    sqArguments.Append(String.IsNullOrEmpty(pMkPsf2Struct.volume.Trim()) ?
                                        String.Empty : String.Format(" -v={0}", pMkPsf2Struct.volume.Trim()));

                                    sqArguments.Append(String.Format(" -s={0} -h={1} -b={2}",
                                        sqFileName, hdFileName, bdFileName));

                                    sw.WriteLine(String.Format("sq.irx {0}", sqArguments.ToString()));
                                    sw.Close();
                                    sw.Dispose();

                                    // run makepsf2                
                                    string arguments = String.Format(" {0}.psf2 {1}", outputFilePrefix, WORKING_FOLDER);
                                    makePsf2Process = new Process();
                                    makePsf2Process.StartInfo = new ProcessStartInfo(makePsf2DestinationPath, arguments);
                                    makePsf2Process.StartInfo.UseShellExecute = false;
                                    makePsf2Process.StartInfo.CreateNoWindow = true;
                                    bool isSuccess = makePsf2Process.Start();
                                    makePsf2Process.WaitForExit();

                                    if (isSuccess)
                                    {
                                        vProgressStruct = new VGMToolbox.util.ProgressStruct();
                                        vProgressStruct.NewNode = null;
                                        vProgressStruct.GenericMessage = String.Format("{0}.psf2 created.", outputFilePrefix) +
                                            Environment.NewLine;
                                        ReportProgress(Constants.PROGRESS_MSG_ONLY, vProgressStruct);

                                        if (!Directory.Exists(Path.Combine(OUTPUT_FOLDER, pMkPsf2Struct.outputFolder)))
                                        {
                                            Directory.CreateDirectory(Path.Combine(OUTPUT_FOLDER, pMkPsf2Struct.outputFolder));
                                        }

                                        File.Move(outputFilePrefix + ".psf2", Path.Combine(Path.Combine(OUTPUT_FOLDER, pMkPsf2Struct.outputFolder), outputFilePrefix + ".psf2"));
                                    }

                                    File.Delete(iniPath);
                                } // if (validSequenceArray[i])                                
                                else if (totalSequences == 1)
                                {
                                    vProgressStruct = new VGMToolbox.util.ProgressStruct();
                                    vProgressStruct.NewNode = null;
                                    vProgressStruct.GenericMessage = String.Format("WARNING: {0}.SQ has only ONE sequence and it is INVALID.  Skipping...", filePrefix) +
                                        Environment.NewLine;
                                    ReportProgress(Constants.PROGRESS_MSG_ONLY, vProgressStruct);                                                        
                                }
                            } // for (int i = 0; i < totalSequences; i++)

                            File.Delete(destinationBdFile);
                            File.Delete(destinationHdFile);
                            File.Delete(destinationSqFile);

                        } // if (fi.Length > 0)
                        else
                        {
                            vProgressStruct = new VGMToolbox.util.ProgressStruct();
                            vProgressStruct.NewNode = null;
                            vProgressStruct.GenericMessage = String.Format("WARNING: {0}.SQ has ZERO length.  Skipping...", filePrefix) +
                                Environment.NewLine;
                            ReportProgress(Constants.PROGRESS_MSG_ONLY, vProgressStruct);                        
                        }                        
                    }
                    catch (Exception ex2)
                    {
                        vProgressStruct = new VGMToolbox.util.ProgressStruct();
                        vProgressStruct.NewNode = null;
                        vProgressStruct.Filename = f;
                        vProgressStruct.ErrorMessage = ex2.Message;
                        ReportProgress(progress, vProgressStruct);
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                } // if (!CancellationPending)

            } // foreach (string f in pUniqueSqFiles)

            try
            {
                Directory.Delete(WORKING_FOLDER, true);
                File.Delete(makePsf2DestinationPath);
            }
            catch (Exception ex3)
            {
                vProgressStruct = new VGMToolbox.util.ProgressStruct();
                vProgressStruct.NewNode = null;
                vProgressStruct.Filename = null;
                vProgressStruct.ErrorMessage = ex3.Message;
                ReportProgress(100, vProgressStruct);
            }
        }

        private int getTotalSequenceCount(string pFileName)
        {
            int ret = NO_SEQ_NUM_FOUND;
            long seqOffset = PSF2_CSL_SEQ_COUNT_OFFSET;

            using (FileStream fs = File.OpenRead(Path.GetFullPath(pFileName)))
            {
                // byte[] seqNumberBytes = ParseFile.parseSimpleOffset(fs, seqOffset, 1);
                byte[] seqNumberBytes = ParseFile.parseSimpleOffset(fs, seqOffset, 4);
                ret = System.BitConverter.ToInt32(seqNumberBytes, 0);

                ret = ret > 0 ? ret : 1;

                this.buildValidSequenceArray(fs, ret);

                fs.Close();
            }

            return ret;
        }

        private void buildValidSequenceArray(FileStream pFileStream, int pSequenceCount)
        {
            int tempValue;
            this.validSequenceArray = new bool[pSequenceCount];

            tempValue = BitConverter.ToInt32(ParseFile.parseSimpleOffset(pFileStream,
                PSF2_CSL_SEQ_OFFSET_BEGIN, 4), 0);
            if (tempValue > 0)
            {
                this.validSequenceArray[0] = true;
            }

            for (int i = 1; i < pSequenceCount; i++)
            {
                tempValue = BitConverter.ToInt32(ParseFile.parseSimpleOffset(pFileStream,
                    PSF2_CSL_SEQ_OFFSET_BEGIN + (i * 4), 4), 0);
                
                if (tempValue > 0)
                {
                    this.validSequenceArray[i] = true;
                }            
            }           
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            MkPsf2Struct mkPsf2StructStruct = (MkPsf2Struct)e.Argument;

            this.makePsf2s(mkPsf2StructStruct, e);
        }    
    }

}
