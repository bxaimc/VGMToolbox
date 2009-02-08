using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class Bin2PsfWorker : BackgroundWorker
    {
        private const uint MIN_TEXT_SECTION_OFFSET = 0x80010000;
        private const uint PC_OFFSET_CORRECTION = 0x800;

        private readonly string WORKING_FOLDER =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "working_psf"));
        private readonly string PROGRAMS_FOLDER =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"));
        private readonly string OUTPUT_FOLDER =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "rips"), "psfs"));


        private int fileCount = 0;
        private int maxFiles = 0;
        Constants.ProgressStruct progressStruct;

        public struct Bin2PsfStruct
        {
            public string sourcePath;
            public string exePath;
            public string seqOffset;
            public string vhOffset;
            public string vbOffset;

            public string outputFolder;
            public bool makeMiniPsfs;
            public string psflibName;
        }

        public Bin2PsfWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void makePsfs(Bin2PsfStruct pBin2PsfStruct, DoWorkEventArgs e)
        {
            string[] uniqueSqFiles;

            if (!CancellationPending)
            {
                // get list of unique files
                uniqueSqFiles = this.getUniqueFileNames(pBin2PsfStruct.sourcePath);
                if (uniqueSqFiles != null)
                {
                    this.maxFiles = uniqueSqFiles.Length;
                    this.buildPsfs(uniqueSqFiles, pBin2PsfStruct, e);
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
            
            if (!Directory.Exists(pSourceDirectory))
            {
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = String.Format("ERROR: Directory {0} not found.", pSourceDirectory);
                ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
            }
            else
            {
                fileCount = Directory.GetFiles(pSourceDirectory, "*.SEQ").Length;

                if (fileCount > 0)
                {
                    ret = new string[fileCount];
                }

                foreach (string f in Directory.GetFiles(pSourceDirectory, "*.SEQ"))
                {
                    ret[i] = f;
                    i++;
                }
            }

            return ret;
        }

        private void buildPsfs(string[] pUniqueSqFiles, Bin2PsfStruct pBin2PsfStruct,
            DoWorkEventArgs e)
        {
            Process bin2PsfProcess;
            int progress;
            StringBuilder bin2PsfArguments = new StringBuilder();
            bool isSuccess;
            string builtFilePath;
            string outputExtension;
            string ripOutputFolder = Path.Combine(OUTPUT_FOLDER, pBin2PsfStruct.outputFolder);
            
            byte[] textSectionOffset;
            long textSectionOffsetValue;
            long pcOffsetSeq;
            long pcOffsetVb;
            long pcOffsetVh;

            FileInfo fi;

            string bin2PsfSourcePath = Path.Combine(PROGRAMS_FOLDER, "bin2psf.exe");
            string bin2PsfDestinationPath = Path.Combine(WORKING_FOLDER, "bin2psf.exe");

            try
            {
                Directory.CreateDirectory(WORKING_FOLDER);

                // copy program
                File.Copy(bin2PsfSourcePath, bin2PsfDestinationPath, true);
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = ex.Message;
                ReportProgress(0, this.progressStruct);

                return;
            }

            // setup output extension
            if (pBin2PsfStruct.makeMiniPsfs)
            {
                outputExtension = "minipsf";
            }
            else
            {
                outputExtension = "psf";
            }

            foreach (string f in pUniqueSqFiles)
            {
                if (!CancellationPending)
                {
                    bin2PsfArguments.Length = 0;

                    // report progress
                    progress = (++this.fileCount * 100) / maxFiles;
                    this.progressStruct.Clear();
                    this.progressStruct.filename = f;
                    ReportProgress(progress, this.progressStruct);

                    try
                    {
                        // copy data files to working directory                    
                        string filePrefix = Path.GetFileNameWithoutExtension(f);
                        string sourceDirectory = Path.GetDirectoryName(f);

                        string seqFileName = filePrefix + ".seq";
                        string vbFileName = filePrefix + ".vb";
                        string vhFileName = filePrefix + ".vh";

                        string sourceSeqFile = Path.Combine(sourceDirectory, seqFileName);
                        string sourceVbFile = Path.Combine(sourceDirectory, vbFileName);
                        string sourceVhFile = Path.Combine(sourceDirectory, vhFileName);

                        string destinationExeFile = Path.Combine(WORKING_FOLDER, filePrefix + ".BIN");
                        string destinationSeqFile = Path.Combine(WORKING_FOLDER, seqFileName);
                        string destinationVbFile = Path.Combine(WORKING_FOLDER, vbFileName);
                        string destinationVhFile = Path.Combine(WORKING_FOLDER, vhFileName);
                        builtFilePath = Path.Combine(WORKING_FOLDER, filePrefix + "." + outputExtension);

                        fi = new FileInfo(sourceSeqFile);

                        if (fi.Length > 0) // only make for nonempty SEQ files
                        {
                            File.Copy(pBin2PsfStruct.exePath, destinationExeFile);
                            File.Copy(sourceSeqFile, destinationSeqFile);
                            File.Copy(sourceVbFile, destinationVbFile);
                            File.Copy(sourceVhFile, destinationVhFile);
                                                        
                            // determine offsets
                            using (FileStream fs = File.OpenRead(destinationExeFile))
                            {
                                // get offset of text section
                                textSectionOffset = ParseFile.parseSimpleOffset(fs, 0x18, 4);
                                textSectionOffsetValue = BitConverter.ToUInt32(textSectionOffset, 0);

                                // calculate pc offsets
                                pcOffsetSeq = VGMToolbox.util.Encoding.GetIntFromString(pBin2PsfStruct.seqOffset) -
                                    textSectionOffsetValue + PC_OFFSET_CORRECTION;
                                pcOffsetVb = VGMToolbox.util.Encoding.GetIntFromString(pBin2PsfStruct.vbOffset) -
                                    textSectionOffsetValue + PC_OFFSET_CORRECTION;
                                pcOffsetVh = VGMToolbox.util.Encoding.GetIntFromString(pBin2PsfStruct.vhOffset) -
                                    textSectionOffsetValue + PC_OFFSET_CORRECTION;
                            }

                            // insert the data
                            fi = new FileInfo(destinationSeqFile);
                            ParseFile.ReplaceFileChunk(destinationSeqFile, 0, fi.Length, 
                                destinationExeFile, pcOffsetSeq);
                            fi = new FileInfo(destinationVbFile);
                            ParseFile.ReplaceFileChunk(destinationVbFile, 0, fi.Length,
                                destinationExeFile, pcOffsetVb);
                            fi = new FileInfo(destinationVhFile);
                            ParseFile.ReplaceFileChunk(destinationVhFile, 0, fi.Length,
                                destinationExeFile, pcOffsetVh);

                            // build bin2psf arguments                    
                            bin2PsfArguments.Append(String.Format(" {0} 1 {1}.bin", outputExtension, filePrefix));                                                        

                            // run bin2psf                
                            bin2PsfProcess = new Process();
                            bin2PsfProcess.StartInfo = new ProcessStartInfo(bin2PsfDestinationPath, bin2PsfArguments.ToString());
                            bin2PsfProcess.StartInfo.WorkingDirectory = WORKING_FOLDER;
                            bin2PsfProcess.StartInfo.UseShellExecute = false;
                            bin2PsfProcess.StartInfo.CreateNoWindow = true;
                            isSuccess = bin2PsfProcess.Start();
                            bin2PsfProcess.WaitForExit();

                            if (isSuccess)
                            {
                                // add lib tag
                                if (pBin2PsfStruct.makeMiniPsfs)
                                {
                                    using (FileStream ofs = File.Open(builtFilePath, FileMode.Open, FileAccess.Write))
                                    {
                                        ofs.Seek(0, SeekOrigin.End);
                                        using (BinaryWriter bw = new BinaryWriter(ofs))
                                        {
                                            System.Text.Encoding enc = System.Text.Encoding.ASCII;
                                            
                                            bw.Write(enc.GetBytes(Xsf.ASCII_TAG)); // [TAG]
                                            bw.Write(enc.GetBytes(String.Format("_lib={0}", pBin2PsfStruct.psflibName)));
                                            bw.Write(new byte[] { 0x0A });
                                        }
                                    }
                                }
                                this.progressStruct.Clear();
                                this.progressStruct.genericMessage = String.Format("{0}.{1} created.", filePrefix, outputExtension) +
                                    Environment.NewLine;
                                ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);

                                if (!Directory.Exists(ripOutputFolder))
                                {
                                    Directory.CreateDirectory(Path.Combine(OUTPUT_FOLDER, pBin2PsfStruct.outputFolder));
                                }
                                File.Move(builtFilePath, Path.Combine(ripOutputFolder, Path.GetFileName(builtFilePath)));
                            }

                            File.Delete(destinationExeFile);
                            File.Delete(destinationSeqFile);
                            File.Delete(destinationVbFile);
                            File.Delete(destinationVhFile);

                        } // if (fi.Length > 0)
                        else
                        {
                            this.progressStruct.Clear();
                            this.progressStruct.genericMessage = String.Format("WARNING: {0}.SEQ has ZERO length.  Skipping...", filePrefix) +
                                Environment.NewLine;
                            ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
                        }
                    }
                    catch (Exception ex2)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.filename = f;
                        this.progressStruct.errorMessage = ex2.Message;
                        ReportProgress(progress, this.progressStruct);
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
            }
            catch (Exception ex3)
            {
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = ex3.Message;
                ReportProgress(100, this.progressStruct);
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Bin2PsfStruct bin2PsfStruct = (Bin2PsfStruct)e.Argument;
            this.makePsfs(bin2PsfStruct, e);
        }            
    }
}
