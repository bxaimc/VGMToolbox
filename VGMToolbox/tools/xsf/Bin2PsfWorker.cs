using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    public enum PsfMakerTask
    { 
        SeqPsf,
        SeqMiniPsf,
        SeqPsfLib,
        SepPsf,
        SepMiniPsf,
        SepPsfLib
    }
    
    class Bin2PsfWorker : BackgroundWorker, IVgmtBackgroundWorker
    {                
        private const uint MIN_TEXT_SECTION_OFFSET = 0x80010000;
        private const uint PC_OFFSET_CORRECTION = 0x800;
        private const uint TEXT_SIZE_OFFSET = 0x1C;

        private const int MINIPSF_INITIAL_PC_OFFSET = 0x10;
        private const int MINIPSF_TEXT_SECTION_OFFSET = 0x18;
        private const int MINIPSF_TEXT_SECTION_SIZE_OFFSET = 0x1C;

        private static readonly string WORKING_FOLDER =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "working_psf"));
        private static readonly string PROGRAMS_FOLDER =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "external"));
        private static readonly string PSF_PROGRAMS_FOLDER = Path.Combine(PROGRAMS_FOLDER, "psf");
        private static readonly string OUTPUT_FOLDER =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "rips"), "psfs"));

        public const string GENERIC_DRIVER_MGRASS = "Mark Grass Generic Driver v2.5";
        public const string GENERIC_DRIVER_DAVIRONICA = "Davironica's Easy PSF Driver v0.1.4";

        public static readonly string MGRASS_EXE_PATH = Path.Combine(PSF_PROGRAMS_FOLDER, "MG_DRIVER_V25.EXE");
        // public static readonly string DAVIRONICA_EXE_PATH = Path.Combine(PSF_PROGRAMS_FOLDER, "DV_DRIVER_014.psflib");
        public static readonly string DAVIRONICA_EXE_PATH = Path.Combine(PSF_PROGRAMS_FOLDER, "DV_DRIVER_014.data.bin");
        public static readonly string DAVIRONICA_MINIPSF_PATH = Path.Combine(PSF_PROGRAMS_FOLDER, "DV_DRIVER_014.null.minipsf");
        public static readonly string GENERIC_MINIPSF_EXE_PATH = Path.Combine(PSF_PROGRAMS_FOLDER, "minipsf.exe");

        public const string PSFLIB_FILE_EXTENSION = ".psflib";

        public const int PARAM_SEQNUM_TICKMODE = 0x0C;
        public const int PARAM_MAXSEQ_LOOPOFF = 0x10;
        public const int PARAM_SEQNUM_OFFSET = 0x14;
        public const int PARAM_MAXSEQ_OFFSET = 0x18;

        private int progress = 0;
        private int fileCount = 0;
        private int maxFiles = 0;
        VGMToolbox.util.ProgressStruct progressStruct;

        public struct Bin2PsfStruct
        {
            public string sourcePath;
            public string exePath;
            public string seqOffset;
            public string SeqSize { set; get; }
            public string vhOffset;
            public string vbOffset;

            public string ParamOffset { set; get; }

            public string outputFolder;
            public bool makeMiniPsfs;
            public string psflibName;

            public bool TryCombinations;
            public bool DoSeqStyle { set; get; }
            public string DriverName;
        }

        public Bin2PsfWorker()
        {
            progressStruct = new VGMToolbox.util.ProgressStruct();
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void makePsfs(Bin2PsfStruct pBin2PsfStruct, DoWorkEventArgs e)
        {
            string[] uniqueSqFiles;
            string[] uniqueVhFiles;
            string[] uniqueSepFiles;

            if (!CancellationPending)
            {
                // get list of unique files
                uniqueSqFiles = this.getUniqueFileNames(pBin2PsfStruct.sourcePath, "*.SEQ");
                uniqueVhFiles = this.getUniqueFileNames(pBin2PsfStruct.sourcePath, "*.VH");
                uniqueSepFiles = this.getUniqueFileNames(pBin2PsfStruct.sourcePath, "*.SEP");

                if (uniqueSqFiles != null)
                {                    
                    if (pBin2PsfStruct.TryCombinations)
                    {
                        this.maxFiles = uniqueSqFiles.Length * uniqueVhFiles.Length;
                    }
                    else
                    {
                        this.maxFiles = uniqueSqFiles.Length;

                        if (pBin2PsfStruct.makeMiniPsfs)
                        {
                            this.maxFiles++;
                            
                            if (uniqueVhFiles.Length > 1)
                            {
                                this.progressStruct.Clear();
                                this.progressStruct.ErrorMessage = String.Format("ERROR: More than 1 VH/VB pair detected, please only include 1 VH/VB pair when making .minipsf files{0}", Environment.NewLine);
                                ReportProgress(this.progress, this.progressStruct);
                                return;
                            }
                        }                    
                    }

                    // check for SEP
                    if (uniqueSepFiles != null)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.ErrorMessage = String.Format("ERROR: Both SEQ and SEP found, cannot make both types of PSF with the same driver.{0}", Environment.NewLine);
                        ReportProgress(this.progress, this.progressStruct);
                        return;
                    }
                }
                else if (uniqueSepFiles != null)
                {
                    this.maxFiles = uniqueSepFiles.Length;
                }
                
                // build PSFs
                this.buildPsfs(uniqueSqFiles, uniqueVhFiles, uniqueSepFiles, pBin2PsfStruct, e);
            }
            else
            {
                e.Cancel = true;
                return;
            }

            return;
        }

        private string[] getUniqueFileNames(string pSourceDirectory, string mask)
        {
            int fileCount = 0;
            int i = 0;
            string[] ret = null;
            
            if (!Directory.Exists(pSourceDirectory))
            {
                this.progressStruct.Clear();
                this.progressStruct.ErrorMessage = String.Format("ERROR: Directory {0} not found.", pSourceDirectory);
                ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
            }
            else
            {
                fileCount = Directory.GetFiles(pSourceDirectory, mask, SearchOption.TopDirectoryOnly).Length;

                if (fileCount > 0)
                {
                    ret = new string[fileCount];
                }

                foreach (string f in Directory.GetFiles(pSourceDirectory, mask))
                {
                    ret[i] = f;
                    i++;
                }
            }

            return ret;
        }

        private void buildPsfs(string[] pUniqueSqFiles, string[] pUniqueVhFiles, string[] pUniqueSepFiles, Bin2PsfStruct pBin2PsfStruct,
            DoWorkEventArgs e)
        {
            string outputExtension;
            string ripOutputFolder = Path.Combine(OUTPUT_FOLDER, pBin2PsfStruct.outputFolder);
            
            string bin2PsfSourcePath = Path.Combine(PROGRAMS_FOLDER, "bin2psf.exe");
            string bin2PsfDestinationPath = Path.Combine(WORKING_FOLDER, "bin2psf.exe");

            string vhName;
            string vbName;

            string modifiedMiniPsfPath = null;
            string psfLibName;
            string filePrefix;

            uint seqCount = 0;

            // create working directory
            try
            {
                Directory.CreateDirectory(WORKING_FOLDER);

                // copy program
                File.Copy(bin2PsfSourcePath, bin2PsfDestinationPath, true);
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                this.progressStruct.ErrorMessage = ex.Message;
                ReportProgress(0, this.progressStruct);

                return;
            }

            // rebuild minipsf.exe as needed
            if (pBin2PsfStruct.makeMiniPsfs)
            {
                if (pUniqueSqFiles != null)
                {
                    modifiedMiniPsfPath = setMiniPsfValues(GENERIC_MINIPSF_EXE_PATH, pBin2PsfStruct, false);
                }
                else if (pUniqueSepFiles != null)
                {
                    modifiedMiniPsfPath = setMiniPsfValues(GENERIC_MINIPSF_EXE_PATH, pBin2PsfStruct, true);
                }
            }

            // Create psflib for SEQ style
            if ((pBin2PsfStruct.makeMiniPsfs) && (pUniqueSqFiles != null))
            {
                this.makePsfFile(
                    PsfMakerTask.SeqPsfLib, 
                    pBin2PsfStruct,
                    pBin2PsfStruct.exePath,
                    null, 
                    pUniqueVhFiles[0],
                    Path.ChangeExtension(pUniqueVhFiles[0], ".vb"), 
                    bin2PsfDestinationPath,
                    ripOutputFolder, 
                    null, 
                    Path.GetFileNameWithoutExtension(pBin2PsfStruct.psflibName), 
                    -1, 
                    -1);               
            }

            #region SEQ LOOP
            if (pUniqueSqFiles != null)
            {
                foreach (string seqFile in pUniqueSqFiles)
                {
                    if (!CancellationPending)
                    {
                        // All Combinations
                        if (pBin2PsfStruct.TryCombinations)
                        {
                            foreach (string vhFile in pUniqueVhFiles)
                            {
                                try
                                {
                                    filePrefix = String.Format(
                                        "S[{0}]_V[{1}]", 
                                        Path.GetFileNameWithoutExtension(seqFile), 
                                        Path.GetFileNameWithoutExtension(vhFile));
                                    
                                    this.makePsfFile(
                                        PsfMakerTask.SeqPsf, 
                                        pBin2PsfStruct,
                                        pBin2PsfStruct.exePath,
                                        seqFile, 
                                        vhFile,
                                        Path.ChangeExtension(vhFile, ".vb"), 
                                        bin2PsfDestinationPath,
                                        ripOutputFolder, 
                                        null, 
                                        filePrefix, 
                                        -1, 
                                        -1);

                                }
                                catch (Exception ex)
                                {
                                    this.progressStruct.Clear();
                                    this.progressStruct.FileName = seqFile;
                                    this.progressStruct.ErrorMessage = ex.Message;
                                    ReportProgress(this.progress, this.progressStruct);
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                filePrefix = Path.GetFileNameWithoutExtension(seqFile);
                                
                                if (pBin2PsfStruct.makeMiniPsfs)
                                {
                                    this.makePsfFile(
                                        PsfMakerTask.SeqMiniPsf,
                                        pBin2PsfStruct,
                                        modifiedMiniPsfPath,
                                        seqFile,
                                        null,
                                        null,
                                        bin2PsfDestinationPath,
                                        ripOutputFolder,
                                        pBin2PsfStruct.psflibName,
                                        filePrefix,
                                        -1,
                                        -1);
                                }
                                else
                                {
                                    vhName = Path.ChangeExtension(seqFile, ".vh");
                                    vbName = Path.ChangeExtension(seqFile, ".vb");

                                    this.makePsfFile(
                                        PsfMakerTask.SeqPsf,
                                        pBin2PsfStruct,
                                        modifiedMiniPsfPath,
                                        seqFile,
                                        vhName,
                                        vbName,
                                        bin2PsfDestinationPath,
                                        ripOutputFolder,
                                        null,
                                        filePrefix,
                                        -1,
                                        -1);
                                }
                            }
                            catch (Exception ex2)
                            {
                                this.progressStruct.Clear();
                                this.progressStruct.FileName = seqFile;
                                this.progressStruct.ErrorMessage = ex2.Message;
                                ReportProgress(this.progress, this.progressStruct);
                            }
                        }

                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    } // if (!CancellationPending)

                } // foreach (string seqFile in pUniqueSqFiles)            
            } // if (pUniqueSqFiles != null)
            #endregion
            #region SEP LOOP
            else if (pUniqueSepFiles != null)
            {
                string originalExe = pBin2PsfStruct.exePath;
                
                foreach (string sepFile in pUniqueSepFiles)
                {
                    // loop through SEPs
                    seqCount = PsxSequence.GetSeqCountForSep(sepFile);
                    this.maxFiles += (int)(seqCount + 1);

                    if (seqCount == 1)
                    {
                        this.makePsfFile(
                            PsfMakerTask.SepPsf,
                            pBin2PsfStruct,
                            pBin2PsfStruct.exePath,
                            sepFile,
                            Path.ChangeExtension(sepFile, ".vh"),
                            Path.ChangeExtension(sepFile, ".vb"),
                            bin2PsfDestinationPath,
                            ripOutputFolder,
                            null,
                            Path.GetFileNameWithoutExtension(sepFile),
                            0,
                            0);
                    }
                    else
                    {
                        psfLibName = Path.GetFileName(Path.ChangeExtension(sepFile, PSFLIB_FILE_EXTENSION));
                        
                        // create lib file
                        this.makePsfFile(
                            PsfMakerTask.SepPsfLib,
                            pBin2PsfStruct,
                            pBin2PsfStruct.exePath,
                            sepFile,
                            Path.ChangeExtension(sepFile, ".vh"),
                            Path.ChangeExtension(sepFile, ".vb"),
                            bin2PsfDestinationPath,
                            ripOutputFolder,
                            null,
                            Path.GetFileNameWithoutExtension(psfLibName),
                            -1,
                            -1);
                        
                        // create minipsfs
                        for (int i = 0; i < seqCount; i++)
                        {
                            if (!CancellationPending)
                            {
                                filePrefix = String.Format(
                                    "{0}_{1}", 
                                    Path.GetFileNameWithoutExtension(sepFile),
                                    i.ToString("X2"));
                                
                                this.makePsfFile(
                                    PsfMakerTask.SepMiniPsf,
                                    pBin2PsfStruct,
                                    modifiedMiniPsfPath,
                                    null,
                                    null,
                                    null,
                                    bin2PsfDestinationPath,
                                    ripOutputFolder,
                                    psfLibName,
                                    filePrefix,
                                    i,
                                    (int)(seqCount - 1));                                                                                               
                            }
                            else
                            {
                                e.Cancel = true;
                                return;
                            } // if (!CancellationPending)
                        } // for (int i = 1; i <= seqCount; i++)
                    }
                } // foreach (string sepFile in pUniqueSepFiles)
            }
            #endregion

            // delete working folder
            try
            {
                Directory.Delete(WORKING_FOLDER, true);
            }
            catch (Exception ex2)
            {
                this.progressStruct.Clear();
                this.progressStruct.ErrorMessage = ex2.Message;
                ReportProgress(100, this.progressStruct);
            }
        }

        /// <summary>
        /// Make a single PSF file.
        /// </summary>
        /// <param name="pBin2PsfStruct"></param>
        /// <param name="seqFile"></param>
        /// <param name="vhFile"></param>
        /// <param name="vbFile"></param>
        /// <param name="outputExtension"></param>
        /// <param name="bin2PsfDestinationPath"></param>
        /// <param name="ripOutputFolder"></param>
        private void makePsfFile(
            Bin2PsfStruct pBin2PsfStruct,
            string seqFile,
            string vhFile, 
            string vbFile, 
            string outputExtension,
            string bin2PsfDestinationPath,
            string ripOutputFolder,
            int sepCount,
            int sepTotalSeqs, 
            string sepMiniPsfPrefix)
        {

            long pcOffsetSeq;
            long pcOffsetVh;
            long pcOffsetVb;
            long pcOffsetSepParams = 0;
            long textSectionOffsetValue;

            bool isSeqPresent = !String.IsNullOrEmpty(seqFile);
            bool isVbPresent = !String.IsNullOrEmpty(vbFile);
            bool isVhPresent = !String.IsNullOrEmpty(vhFile);

            bool isMakingPsfLib = (isVhPresent && isVbPresent && pBin2PsfStruct.makeMiniPsfs);
            bool isMakingMiniPsf = (!isMakingPsfLib && (pBin2PsfStruct.makeMiniPsfs));
            bool isMakingSepMiniPsf = (!isSeqPresent && !isVhPresent && !isVbPresent && (sepTotalSeqs > 0) && (sepCount > -1));

            string destinationFile;

            FileInfo fi = null;

            // report progress
            this.progress = (++this.fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.FileName = seqFile;
            ReportProgress(this.progress, this.progressStruct);
                        
            // copy data files to working directory                    
            string sourceDirectory = pBin2PsfStruct.sourcePath;

            string filePrefix;

            if (pBin2PsfStruct.TryCombinations)
            {
                filePrefix = String.Format("S[{0}]_V[{1}]", Path.GetFileNameWithoutExtension(seqFile), Path.GetFileNameWithoutExtension(vhFile));
            }
            else if (isMakingSepMiniPsf)
            {
                filePrefix = String.Format("{0}_{1}", sepMiniPsfPrefix, sepCount.ToString("X2"));
            }
            else if (isMakingPsfLib) // making .psflib
            {
                filePrefix = Path.GetFileNameWithoutExtension(pBin2PsfStruct.psflibName);
            }
            else if (isSeqPresent)
            {
                filePrefix = Path.GetFileNameWithoutExtension(seqFile);
            }
            else
            {
                filePrefix = Path.GetFileNameWithoutExtension(vhFile);
            }

            string destinationExeFile = Path.Combine(WORKING_FOLDER, filePrefix + ".BIN");                                    
            string builtFilePath;

            if (isMakingPsfLib)
            {
                builtFilePath = Path.Combine(WORKING_FOLDER, pBin2PsfStruct.psflibName);
            }
            else
            {
                builtFilePath = Path.Combine(WORKING_FOLDER, filePrefix + outputExtension);
            }
            
            
            if (isSeqPresent)
            {
                fi = new FileInfo(seqFile);
            }
            
            // check for empty SEQ files
            if (((isSeqPresent) && (fi.Length > 0)) || pBin2PsfStruct.makeMiniPsfs)
            {                               
                // copy exe to destination folder
                File.Copy(pBin2PsfStruct.exePath, destinationExeFile, true);

                // determine offsets
                using (FileStream fs = File.OpenRead(destinationExeFile))
                {
                    // get offset of text section
                    byte[] textSectionOffset = ParseFile.ParseSimpleOffset(fs, 0x18, 4);
                    textSectionOffsetValue = BitConverter.ToUInt32(textSectionOffset, 0);

                    switch (pBin2PsfStruct.DriverName)
                    {
                        default:
                            // calculate pc offsets
                            pcOffsetSeq = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.seqOffset) -
                                textSectionOffsetValue + PC_OFFSET_CORRECTION;
                            pcOffsetVb = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.vbOffset) -
                                textSectionOffsetValue + PC_OFFSET_CORRECTION;
                            pcOffsetVh = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.vhOffset) -
                                textSectionOffsetValue + PC_OFFSET_CORRECTION;
                            pcOffsetSepParams = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.ParamOffset) -
                                textSectionOffsetValue + PC_OFFSET_CORRECTION;                            
                            break;
                    }
                }

                // copy and insert the data
                if (isSeqPresent)
                {
                    destinationFile = Path.Combine(WORKING_FOLDER, Path.GetFileName(seqFile));
                    File.Copy(seqFile, destinationFile, true);
                    fi = new FileInfo(destinationFile);
                    FileUtil.ReplaceFileChunk(destinationFile, 0, fi.Length,
                        destinationExeFile, pcOffsetSeq);
                    File.Delete(destinationFile);
                }

                if (isVbPresent)
                {
                    destinationFile = Path.Combine(WORKING_FOLDER, Path.GetFileName(vbFile));
                    File.Copy(vbFile, destinationFile, true);
                    fi = new FileInfo(destinationFile);
                    FileUtil.ReplaceFileChunk(destinationFile, 0, fi.Length,
                        destinationExeFile, pcOffsetVb);
                    File.Delete(destinationFile);
                }

                if (isVhPresent)
                {
                    destinationFile = Path.Combine(WORKING_FOLDER, Path.GetFileName(vhFile));
                    File.Copy(vhFile, destinationFile, true);
                    fi = new FileInfo(destinationFile);
                    FileUtil.ReplaceFileChunk(destinationFile, 0, fi.Length,
                        destinationExeFile, pcOffsetVh);
                    File.Delete(destinationFile);
                }

                if (isMakingSepMiniPsf)
                {

                    byte[] tickModeBytes = BitConverter.GetBytes((uint)1);
                    byte[] loopOffBytes = BitConverter.GetBytes((uint)1);
                    byte[] sepCountBytes = BitConverter.GetBytes((uint)sepCount);
                    byte[] sepTotalSeqsBytes = BitConverter.GetBytes((uint)sepTotalSeqs);
                    
                    uint totalFileSize = (uint)(pcOffsetSepParams + PARAM_MAXSEQ_OFFSET + 4);
                    byte[] totalFileSizeBytes = BitConverter.GetBytes(totalFileSize);

                    FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_SEQNUM_TICKMODE), tickModeBytes);
                    FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_MAXSEQ_LOOPOFF), loopOffBytes);
                    FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_SEQNUM_OFFSET), sepCountBytes);
                    FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_MAXSEQ_OFFSET), sepTotalSeqsBytes);

                    FileUtil.TrimFileToLength(destinationExeFile, (int)totalFileSize);
                }
                
                // build bin2psf arguments                    
                StringBuilder bin2PsfArguments = new StringBuilder();
                bin2PsfArguments.Append(String.Format(" {0} 1 {1}.bin", Path.GetExtension(builtFilePath).Substring(1), filePrefix));

                // run bin2psf                
                Process bin2PsfProcess = new Process();
                bin2PsfProcess.StartInfo = new ProcessStartInfo(bin2PsfDestinationPath, bin2PsfArguments.ToString());
                bin2PsfProcess.StartInfo.WorkingDirectory = WORKING_FOLDER;
                bin2PsfProcess.StartInfo.UseShellExecute = false;
                bin2PsfProcess.StartInfo.CreateNoWindow = true;
                bool isSuccess = bin2PsfProcess.Start();
                bin2PsfProcess.WaitForExit();

                if (isSuccess)
                {
                    // add lib tag
                    if (isMakingMiniPsf) // only add lib tags to the minipsfs
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
                    this.progressStruct.GenericMessage = String.Format("{0}{1} created.", filePrefix, outputExtension) +
                        Environment.NewLine;
                    ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                    if (!Directory.Exists(ripOutputFolder))
                    {
                        Directory.CreateDirectory(Path.Combine(OUTPUT_FOLDER, pBin2PsfStruct.outputFolder));
                    }
                    File.Move(builtFilePath, Path.Combine(ripOutputFolder, Path.GetFileName(builtFilePath)));
                }

            } // if (fi.Length > 0)
            else
            {
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("WARNING: {0}.SEQ has ZERO length.  Skipping...", filePrefix) +
                    Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
            }        
        }

        private void makeSepMiniPsfFile(
            Bin2PsfStruct pBin2PsfStruct,
            string bin2PsfDestinationPath,
            string ripOutputFolder,
            string sourceExePath,
            int sepCount,
            int sepTotalSeqs,
            string sepMiniPsfPrefix,
            string psfLibName)
        {
            long pcOffsetSepParams = 0;
            long textSectionOffsetValue;

            // report progress
            this.progress = (++this.fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.FileName = sepMiniPsfPrefix;
            ReportProgress(this.progress, this.progressStruct);

            // copy data files to working directory                    
            string sourceDirectory = pBin2PsfStruct.sourcePath;

            // build paths
            string filePrefix = String.Format("{0}_{1}", sepMiniPsfPrefix, sepCount.ToString("X2"));
            string destinationExeFile = Path.Combine(WORKING_FOLDER, filePrefix + ".BIN");
            string builtFilePath = Path.Combine(WORKING_FOLDER, filePrefix + ".minipsf");

            // copy exe to destination folder
            File.Copy(sourceExePath, destinationExeFile, true);

            // determine offsetof parameter area
            using (FileStream fs = File.OpenRead(destinationExeFile))
            {
                // get offset of text section
                byte[] textSectionOffset = ParseFile.ParseSimpleOffset(fs, 0x18, 4);
                textSectionOffsetValue = BitConverter.ToUInt32(textSectionOffset, 0);

                // get offset to place our parameters
                pcOffsetSepParams = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.ParamOffset) -
                    textSectionOffsetValue + PC_OFFSET_CORRECTION;
            }

            // insert parameters into driver
            byte[] tickModeBytes = BitConverter.GetBytes((uint)1);
            byte[] loopOffBytes = BitConverter.GetBytes((uint)1);
            byte[] sepCountBytes = BitConverter.GetBytes((uint)sepCount);
            byte[] sepTotalSeqsBytes = BitConverter.GetBytes((uint)sepTotalSeqs);

            uint totalFileSize = (uint)(pcOffsetSepParams + PARAM_MAXSEQ_OFFSET + 4);
            byte[] totalFileSizeBytes = BitConverter.GetBytes(totalFileSize);

            FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_SEQNUM_TICKMODE), tickModeBytes);
            FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_MAXSEQ_LOOPOFF), loopOffBytes);
            FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_SEQNUM_OFFSET), sepCountBytes);
            FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_MAXSEQ_OFFSET), sepTotalSeqsBytes);

            // trim excess bytes
            FileUtil.TrimFileToLength(destinationExeFile, (int)totalFileSize);

            // build bin2psf arguments                    
            StringBuilder bin2PsfArguments = new StringBuilder();
            bin2PsfArguments.Append(String.Format(" {0} 1 {1}.bin", Path.GetExtension(builtFilePath).Substring(1), filePrefix));

            // run bin2psf                
            Process bin2PsfProcess = new Process();
            bin2PsfProcess.StartInfo = new ProcessStartInfo(bin2PsfDestinationPath, bin2PsfArguments.ToString());
            bin2PsfProcess.StartInfo.WorkingDirectory = WORKING_FOLDER;
            bin2PsfProcess.StartInfo.UseShellExecute = false;
            bin2PsfProcess.StartInfo.CreateNoWindow = true;
            bool isSuccess = bin2PsfProcess.Start();
            bin2PsfProcess.WaitForExit();

            if (isSuccess)
            {
                // add lib tag
                using (FileStream ofs = File.Open(builtFilePath, FileMode.Open, FileAccess.Write))
                {
                    ofs.Seek(0, SeekOrigin.End);
                    using (BinaryWriter bw = new BinaryWriter(ofs))
                    {
                        System.Text.Encoding enc = System.Text.Encoding.ASCII;

                        bw.Write(enc.GetBytes(Xsf.ASCII_TAG)); // [TAG]
                        bw.Write(enc.GetBytes(String.Format("_lib={0}", psfLibName)));
                        bw.Write(new byte[] { 0x0A });
                    }
                }

                // move file to output folder
                if (!Directory.Exists(ripOutputFolder))
                {
                    Directory.CreateDirectory(Path.Combine(OUTPUT_FOLDER, pBin2PsfStruct.outputFolder));
                }
                
                File.Move(builtFilePath, Path.Combine(ripOutputFolder, Path.GetFileName(builtFilePath)));

                // notify user of file creations
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("{0}{1} created.", filePrefix, ".minipsf") +
                    Environment.NewLine;
                ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
            }
        }








        private void makePsfFile(
            PsfMakerTask task,
            Bin2PsfStruct pBin2PsfStruct,
            string sourceExePath,
            string seqFile,
            string vhFile,
            string vbFile,
            string bin2PsfDestinationPath,
            string ripOutputFolder,
            string psfLibFileName,
            string filePrefix,
            int sepCount,
            int sepTotalSeqs)
        {

            long pcOffsetSeq;
            long pcOffsetVh;
            long pcOffsetVb;
            long pcOffsetSepParams = 0;
            long textSectionOffsetValue;

            bool isSeqPresent = !String.IsNullOrEmpty(seqFile);
            bool isVbPresent = !String.IsNullOrEmpty(vbFile);
            bool isVhPresent = !String.IsNullOrEmpty(vhFile);

            string destinationFile;
            FileInfo fi = null;

            // report progress
            this.progress = (++this.fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.FileName = seqFile;
            ReportProgress(this.progress, this.progressStruct);

            // copy data files to working directory                    
            string sourceDirectory = pBin2PsfStruct.sourcePath;
            string destinationExeFile = Path.Combine(WORKING_FOLDER, filePrefix + ".BIN");
            string builtFilePath;

            // build output file name
            switch (task)
            {
                case PsfMakerTask.SepMiniPsf:
                case PsfMakerTask.SeqMiniPsf:
                    builtFilePath = String.Format("{0}.{1}", filePrefix, "minipsf");
                    break;                
                case PsfMakerTask.SepPsf:
                case PsfMakerTask.SeqPsf:
                    builtFilePath = String.Format("{0}.{1}", filePrefix, "psf");
                    break;
                case PsfMakerTask.SepPsfLib:
                case PsfMakerTask.SeqPsfLib:
                    builtFilePath = String.Format("{0}.{1}", filePrefix, "psflib");
                    break;
                default:
                    builtFilePath = String.Format("{0}.{1}", filePrefix, "xsf");
                    break;
            }

            builtFilePath = Path.Combine(WORKING_FOLDER, builtFilePath);

            // copy exe to destination folder
            File.Copy(sourceExePath, destinationExeFile, true);

            // determine offsets
            using (FileStream fs = File.OpenRead(destinationExeFile))
            {
                // get offset of text section
                byte[] textSectionOffset = ParseFile.ParseSimpleOffset(fs, 0x18, 4);
                textSectionOffsetValue = BitConverter.ToUInt32(textSectionOffset, 0);

                // calculate pc offsets
                pcOffsetSeq = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.seqOffset) -
                    textSectionOffsetValue + PC_OFFSET_CORRECTION;
                pcOffsetVb = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.vbOffset) -
                    textSectionOffsetValue + PC_OFFSET_CORRECTION;
                pcOffsetVh = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.vhOffset) -
                    textSectionOffsetValue + PC_OFFSET_CORRECTION;
                pcOffsetSepParams = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.ParamOffset) -
                    textSectionOffsetValue + PC_OFFSET_CORRECTION;
            }

            // copy and insert the data
            if ((task == PsfMakerTask.SepPsf) ||
                (task == PsfMakerTask.SepPsfLib) ||
                (task == PsfMakerTask.SeqMiniPsf) ||
                (task == PsfMakerTask.SeqPsf))
            {                                                       
                if (isSeqPresent)
                {
                    destinationFile = Path.Combine(WORKING_FOLDER, Path.GetFileName(seqFile));
                    File.Copy(seqFile, destinationFile, true);
                    fi = new FileInfo(destinationFile);
                    FileUtil.ReplaceFileChunk(destinationFile, 0, fi.Length,
                        destinationExeFile, pcOffsetSeq);
                    File.Delete(destinationFile);
                }
            }

            if ((task == PsfMakerTask.SepPsf) ||
                (task == PsfMakerTask.SepPsfLib) ||
                (task == PsfMakerTask.SeqPsf) || 
                (task == PsfMakerTask.SeqPsfLib))
            {

                if (isVbPresent)
                {
                    destinationFile = Path.Combine(WORKING_FOLDER, Path.GetFileName(vbFile));
                    File.Copy(vbFile, destinationFile, true);
                    fi = new FileInfo(destinationFile);
                    FileUtil.ReplaceFileChunk(destinationFile, 0, fi.Length,
                        destinationExeFile, pcOffsetVb);
                    File.Delete(destinationFile);
                }

                if (isVhPresent)
                {
                    destinationFile = Path.Combine(WORKING_FOLDER, Path.GetFileName(vhFile));
                    File.Copy(vhFile, destinationFile, true);
                    fi = new FileInfo(destinationFile);
                    FileUtil.ReplaceFileChunk(destinationFile, 0, fi.Length,
                        destinationExeFile, pcOffsetVh);
                    File.Delete(destinationFile);
                }
            }

            if ((task == PsfMakerTask.SepMiniPsf) || 
                (task == PsfMakerTask.SepPsf))
            {
                    byte[] tickModeBytes = BitConverter.GetBytes((uint)1);
                    byte[] loopOffBytes = BitConverter.GetBytes((uint)1);
                    byte[] sepCountBytes = BitConverter.GetBytes((uint)sepCount);
                    byte[] sepTotalSeqsBytes = BitConverter.GetBytes((uint)sepTotalSeqs);

                    FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_SEQNUM_TICKMODE), tickModeBytes);
                    FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_MAXSEQ_LOOPOFF), loopOffBytes);
                    FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_SEQNUM_OFFSET), sepCountBytes);
                    FileUtil.UpdateChunk(destinationExeFile, (int)(pcOffsetSepParams + PARAM_MAXSEQ_OFFSET), sepTotalSeqsBytes);

                    if (task == PsfMakerTask.SepMiniPsf)
                    {
                        uint totalFileSize = (uint)(pcOffsetSepParams + PARAM_MAXSEQ_OFFSET + 4);
                        byte[] totalFileSizeBytes = BitConverter.GetBytes(totalFileSize);
                        
                        FileUtil.TrimFileToLength(destinationExeFile, (int)totalFileSize);
                    }
            }
                
            // build bin2psf arguments                    
            StringBuilder bin2PsfArguments = new StringBuilder();
            bin2PsfArguments.Append(String.Format(" {0} 1 {1}.bin", Path.GetExtension(builtFilePath).Substring(1), filePrefix));

            // run bin2psf                
            Process bin2PsfProcess = new Process();
            bin2PsfProcess.StartInfo = new ProcessStartInfo(bin2PsfDestinationPath, bin2PsfArguments.ToString());
            bin2PsfProcess.StartInfo.WorkingDirectory = WORKING_FOLDER;
            bin2PsfProcess.StartInfo.UseShellExecute = false;
            bin2PsfProcess.StartInfo.CreateNoWindow = true;
            bool isSuccess = bin2PsfProcess.Start();
            bin2PsfProcess.WaitForExit();

            if (isSuccess)
            {
                // add lib tag
                if ((task == PsfMakerTask.SepMiniPsf) || 
                    (task == PsfMakerTask.SeqMiniPsf))
                {
                    using (FileStream ofs = File.Open(builtFilePath, FileMode.Open, FileAccess.Write))
                    {
                        ofs.Seek(0, SeekOrigin.End);
                        using (BinaryWriter bw = new BinaryWriter(ofs))
                        {
                            System.Text.Encoding enc = System.Text.Encoding.ASCII;

                            bw.Write(enc.GetBytes(Xsf.ASCII_TAG)); // [TAG]
                            bw.Write(enc.GetBytes(String.Format("_lib={0}", psfLibFileName)));
                            bw.Write(new byte[] { 0x0A });
                        }
                    }
                }

                // copy file to output folder
                if (!Directory.Exists(ripOutputFolder))
                {
                    Directory.CreateDirectory(Path.Combine(OUTPUT_FOLDER, pBin2PsfStruct.outputFolder));
                }
                
                File.Move(builtFilePath, Path.Combine(ripOutputFolder, Path.GetFileName(builtFilePath)));

                // report to user
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("{0} created.{1}", Path.GetFileName(builtFilePath), Environment.NewLine);                       
                ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
            }
        }













        private string setMiniPsfValues(string templateMiniPsfPath, Bin2PsfStruct pBin2PsfStruct, bool useSepParameters)
        {
            string modifiedMiniPsfPath = Path.Combine(WORKING_FOLDER, Path.GetFileName(templateMiniPsfPath));
            long seqSize = VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.SeqSize);
            int totalFileSize;

            // copy file
            File.Copy(templateMiniPsfPath, modifiedMiniPsfPath, true);

            // edit values            
            if (!useSepParameters)
            {
                byte[] seqOffsetBytes = BitConverter.GetBytes((uint)VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.seqOffset));
                byte[] seqSizeBytes = BitConverter.GetBytes((uint)seqSize);

                FileUtil.UpdateChunk(modifiedMiniPsfPath, MINIPSF_INITIAL_PC_OFFSET, seqOffsetBytes);
                FileUtil.UpdateChunk(modifiedMiniPsfPath, MINIPSF_TEXT_SECTION_OFFSET, seqOffsetBytes);
                FileUtil.UpdateChunk(modifiedMiniPsfPath, MINIPSF_TEXT_SECTION_SIZE_OFFSET, seqSizeBytes);

                // trim end of file
                totalFileSize = (int)(seqSize + PC_OFFSET_CORRECTION);
            }
            else
            {
                uint paramOffet = (uint)VGMToolbox.util.Encoding.GetLongValueFromString(pBin2PsfStruct.ParamOffset);
                byte[] paramOffsetBytes = BitConverter.GetBytes(paramOffet);

                totalFileSize = (int)(PC_OFFSET_CORRECTION + PARAM_MAXSEQ_OFFSET + 4);
                byte[] textSectionSizeBytes = BitConverter.GetBytes((uint)(totalFileSize - PC_OFFSET_CORRECTION));

                FileUtil.UpdateChunk(modifiedMiniPsfPath, MINIPSF_INITIAL_PC_OFFSET, paramOffsetBytes);
                FileUtil.UpdateChunk(modifiedMiniPsfPath, MINIPSF_TEXT_SECTION_OFFSET, paramOffsetBytes);
                FileUtil.UpdateChunk(modifiedMiniPsfPath, MINIPSF_TEXT_SECTION_SIZE_OFFSET, textSectionSizeBytes);
            }
            
            FileUtil.TrimFileToLength(modifiedMiniPsfPath, totalFileSize);

            return modifiedMiniPsfPath;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Bin2PsfStruct bin2PsfStruct = (Bin2PsfStruct)e.Argument;            
            this.makePsfs(bin2PsfStruct, e);
        }            
    }
}
