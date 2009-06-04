using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using VGMToolbox.format.sdat;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class Mk2sfWorker : BackgroundWorker, IVgmtBackgroundWorker
    {
        public const string TESTPACK_PATH = "external\\2sf\\testpack.nds";
        public const string TESTPACK_CRC32 = "FB16DF0E";
        public readonly string TWOSFTOOL_PATH =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "external\\2sf\\2sftool.exe");
        public readonly string TESTPACK_FULL_PATH =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            TESTPACK_PATH);

        private int fileCount;
        private int maxFiles;
        VGMToolbox.util.ProgressStruct progressStruct = new VGMToolbox.util.ProgressStruct();        
        
        public struct Mk2sfStruct
        {
            public ArrayList AllowedSequences;
            public ArrayList UnAllowedSequences;

            public string DestinationFolder;
            public string SourcePath;
            public string GameSerial;

            public string TagArtist;
            public string TagCopyright;
            public string TagYear;
            public string TagGame;
        }

        public Mk2sfWorker() 
        {
            fileCount = 0;
            maxFiles = 0;
            this.progressStruct = new VGMToolbox.util.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;        
        }

        private void Make2sfFiles(Mk2sfStruct pMk2sfStruct)
        {
            string sdatDestinationPath;
            string TwoSFDestinationPath;
            string TwoSFToolDestinationPath;
            string sdatPrefix;
            string testpackDestinationPath;
            string unallowedDestinationPath;
            string strmDestinationPath;
            Sdat sdat;

            // Build Paths
            if (String.IsNullOrEmpty(pMk2sfStruct.GameSerial))
            {
                sdatDestinationPath =
                    Path.Combine(pMk2sfStruct.DestinationFolder, Path.GetFileName(pMk2sfStruct.SourcePath));
            }
            else
            {
                sdatDestinationPath =
                    Path.Combine(pMk2sfStruct.DestinationFolder, pMk2sfStruct.GameSerial + Path.GetExtension(pMk2sfStruct.SourcePath));            
            }
            TwoSFDestinationPath =
                Path.Combine(pMk2sfStruct.DestinationFolder, Path.GetFileNameWithoutExtension(sdatDestinationPath));
            TwoSFToolDestinationPath =
                Path.Combine(pMk2sfStruct.DestinationFolder, Path.GetFileName(TWOSFTOOL_PATH));
            sdatPrefix = Path.GetFileNameWithoutExtension(sdatDestinationPath);
            testpackDestinationPath = Path.Combine(pMk2sfStruct.DestinationFolder, 
                Path.GetFileName(TESTPACK_FULL_PATH));
            unallowedDestinationPath = Path.Combine(TwoSFDestinationPath, "UnAllowed Sequences");
            strmDestinationPath = TwoSFDestinationPath;

            // Copy SDAT to destination folder
            try
            {
                File.Copy(pMk2sfStruct.SourcePath, sdatDestinationPath, false);
            }
            catch (Exception sdatException)
            {
                throw new IOException(String.Format("Error: Cannot copy SDAT <{0}> to destination directory: {1}.", sdatDestinationPath, sdatException.Message));
            }
            
            // Copy STRMs
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = "Copying STRM files" + Environment.NewLine;
            ReportProgress(Constants.PROGRESS_MSG_ONLY, progressStruct);
            
            using (FileStream sdatStream = File.OpenRead(sdatDestinationPath))
            {
                sdat = new Sdat();
                sdat.Initialize(sdatStream, sdatDestinationPath);
                sdat.ExtractStrms(sdatStream, strmDestinationPath);
            }

            // Optimize SDAT
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = "Optimizing SDAT" + Environment.NewLine;
            ReportProgress(Constants.PROGRESS_MSG_ONLY, progressStruct);

            using (FileStream sdatStream = File.OpenRead(sdatDestinationPath))
            {
                sdat = new Sdat();
                sdat.Initialize(sdatStream, sdatDestinationPath);                
            }
            sdat.OptimizeForZlib(pMk2sfStruct.AllowedSequences);

            // Copy 2sfTool.exe
            File.Copy(TWOSFTOOL_PATH, TwoSFToolDestinationPath, true);

            // Copy testpack.nds
            File.Copy(TESTPACK_FULL_PATH, testpackDestinationPath, true);

            // Create 2SF output path
            if (!Directory.Exists(TwoSFDestinationPath))
            {
                Directory.CreateDirectory(TwoSFDestinationPath);
            }

            // Execute 2SF Tool
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = "Execute 2sfTool" + Environment.NewLine;
            ReportProgress(Constants.PROGRESS_MSG_ONLY, progressStruct);
            
            using (Process toolProcess = new Process())
            {
                string arguments = String.Format(" \"{0}{1}{2}\" \"{3}\" {4} {5}",
                    Path.GetFileName(TwoSFDestinationPath),
                    Path.DirectorySeparatorChar,
                    sdatPrefix,
                    Path.GetFileName(sdatDestinationPath),
                    GetMinAllowedSseq(pMk2sfStruct.AllowedSequences).ToString(),
                    GetMaxAllowedSseq(pMk2sfStruct.AllowedSequences).ToString());
                toolProcess.StartInfo = new ProcessStartInfo(TwoSFToolDestinationPath, arguments);
                toolProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(TwoSFToolDestinationPath);
                toolProcess.StartInfo.UseShellExecute = false;
                toolProcess.StartInfo.CreateNoWindow = true;
                toolProcess.Start();
                toolProcess.WaitForExit();
                toolProcess.Close();
            }

            // Check XsfUtil
            //XsfUtil.Make2sfSet(testpackDestinationPath, sdatDestinationPath,
            //    GetMinAllowedSseq(pMk2sfStruct.AllowedSequences),
            //    GetMaxAllowedSseq(pMk2sfStruct.AllowedSequences), TwoSFDestinationPath);

            // Move unallowed Sequences
            string unallowedFileName;
            string unallowedFilePath;
            foreach (int unallowedSequenceNumber in pMk2sfStruct.UnAllowedSequences)
            {
                unallowedFileName = String.Format("{0}-{1}.mini2sf", sdatPrefix, unallowedSequenceNumber.ToString("X4"));
                unallowedFilePath = Path.Combine(TwoSFDestinationPath, unallowedFileName);

                if (!Directory.Exists(unallowedDestinationPath))
                {
                    Directory.CreateDirectory(unallowedDestinationPath);
                }

                if (File.Exists(unallowedFilePath))
                {
                    File.Copy(unallowedFilePath, Path.Combine(unallowedDestinationPath, unallowedFileName), true);
                    File.Delete(unallowedFilePath);
                }                
            }

            // Add Tags            
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = "Tagging Output" + Environment.NewLine;
            ReportProgress(Constants.PROGRESS_MSG_ONLY, progressStruct);
            
            XsfUtil.XsfBasicTaggingStruct tagStruct = new XsfUtil.XsfBasicTaggingStruct();
            tagStruct.TagArtist = pMk2sfStruct.TagArtist;
            tagStruct.TagCopyright = pMk2sfStruct.TagCopyright;
            tagStruct.TagYear = pMk2sfStruct.TagYear;
            tagStruct.TagGame = pMk2sfStruct.TagGame;
            tagStruct.TagComment = "uses Legacy of Ys: Book II driver hacked by Caitsith2";            
            tagStruct.TagXsfByTagName = "-2sfby";
            tagStruct.TagXsfByTagValue = "VGMToolbox";

            string taggingBatchPath = XsfUtil.BuildBasicTaggingBatch(TwoSFDestinationPath, tagStruct, "*.mini2sf");
            XsfUtil.ExecutePsfPointBatchScript(taggingBatchPath, true);

            // Time 2SFs
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = "Timing Output" + Environment.NewLine;
            ReportProgress(Constants.PROGRESS_MSG_ONLY, progressStruct);
            
            string outputTimerMessages;
            
            XsfUtil.Time2sfStruct timerStruct = new XsfUtil.Time2sfStruct();
            timerStruct.DoSingleLoop = false;
            timerStruct.Mini2sfDirectory = TwoSFDestinationPath;
            timerStruct.SdatPath = pMk2sfStruct.SourcePath;

            XsfUtil.Time2sfFolder(timerStruct, out outputTimerMessages);

            // Delete Files
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = "Cleaning Up" + Environment.NewLine;
            ReportProgress(Constants.PROGRESS_MSG_ONLY, progressStruct);
            
            if (File.Exists(sdatDestinationPath))
            {
                File.Delete(sdatDestinationPath);
            }
            //if (File.Exists(TwoSFToolDestinationPath))
            //{
            //    File.Delete(TwoSFToolDestinationPath);
            //}
            if (File.Exists(testpackDestinationPath))
            {
                File.Delete(testpackDestinationPath);
            }
        }

        private int GetMinAllowedSseq(ArrayList pAllowedSequences)
        {
            int ret = int.MaxValue;
            int checkVal;

            foreach (object o in pAllowedSequences)
            { 
                checkVal = (int) o;

                if (checkVal < ret)
                {
                    ret = checkVal;
                }
            }
            return ret;
        }
        private int GetMaxAllowedSseq(ArrayList pAllowedSequences)
        {
            int ret = int.MinValue;
            int checkVal;

            foreach (object o in pAllowedSequences)
            {
                checkVal = (int)o;

                if (checkVal > ret)
                {
                    ret = checkVal;
                }
            }
            return ret;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Mk2sfStruct mk2sfStruct = (Mk2sfStruct)e.Argument;
            Make2sfFiles(mk2sfStruct);
        }
    }
}
