using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class SsfMakeWorker : BackgroundWorker
    {
        private static readonly string SSFMAKE_FOLDER_PATH =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.Combine(".", "external"), "ssf"), "ssfmake"));
        private static readonly string SSFMAKE_SOURCE_PATH = Path.Combine(SSFMAKE_FOLDER_PATH, "ssfmake.py");
        
        private static readonly string WORKING_FOLDER =
            Path.GetFullPath(Path.Combine(".", "working_ssf"));
        private static readonly string SSFMAKE_DESTINATION_PATH =
            Path.Combine(WORKING_FOLDER, "ssfmake.py");
        private static readonly string BIN2PSF_SOURCE_PATH =
            Path.GetFullPath(Path.Combine(Path.Combine(".", "external"), "bin2psf.exe"));
        private static readonly string BIN2PSF_DESTINATION_PATH =
            Path.GetFullPath(Path.Combine(WORKING_FOLDER, "bin2psf.exe"));        
        private static readonly string PSFPOINT_SOURCE_PATH =
            Path.GetFullPath(Path.Combine(Path.Combine(".", "external"), "psfpoint.exe"));
        private static readonly string PSFPOINT_DESTINATION_PATH =
            Path.GetFullPath(Path.Combine(WORKING_FOLDER, "psfpoint.exe"));

        private const int LINE_NUM_SEQ_BANK = 130;
        private const int LINE_NUM_SEQ_TRACK = 131;
        private const int LINE_NUM_VOLUME = 132;
        private const int LINE_NUM_MIXER_BANK = 133;
        private const int LINE_NUM_MIXER_NUMBER = 134;
        private const int LINE_NUM_EFFECT = 135;
        private const int LINE_NUM_USE_DSP = 136;
        
        private const int LINE_NUM_DRIVER = 139;
        private const int LINE_NUM_MAP = 140;
        private const int LINE_NUM_TONE_DATA = 141;
        private const int LINE_NUM_SEQ_DATA = 142;
        private const int LINE_NUM_DSP_PROGRAM = 143;

        private const int LINE_NUM_OUT_FILE = 145;

        Constants.ProgressStruct progressStruct = new Constants.ProgressStruct();

        public struct SsfMakeStruct
        {
            public string sequenceBank;
            public string sequenceTrack;
            public string volume;
            public string mixerBank;
            public string mixerNumber;
            public string effectNumber;
            public string useDsp;
            
            public string driver;
            public string map;
            public string toneData;
            public string sequenceData;
            public string dspProgram;
        }

        public SsfMakeWorker()
        {            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void makeSsfs(SsfMakeStruct pSsfMakeStruct, DoWorkEventArgs e)
        {
            this.progressStruct = new Constants.ProgressStruct();
            
            // first run (only run if no DSP)
            pSsfMakeStruct.map = this.getMapFile(pSsfMakeStruct);
            
            // check if a suitable default map was found
            if (!String.IsNullOrEmpty(pSsfMakeStruct.map))
            {
                // prepare working dir
                if (prepareWorkingDir(pSsfMakeStruct))
                {
                    // report progress
                    this.progressStruct = new Constants.ProgressStruct();
                    this.progressStruct.newNode = null;
                    this.progressStruct.genericMessage = 
                        "Working Directory Prepared" + Environment.NewLine;
                    ReportProgress(20, this.progressStruct);

                    // modify script
                    if (this.customizeScript(pSsfMakeStruct))
                    {
                        // report progress
                        this.progressStruct = new Constants.ProgressStruct();
                        this.progressStruct.newNode = null;
                        this.progressStruct.genericMessage =
                            "Script Modified" + Environment.NewLine;
                        ReportProgress(40, this.progressStruct);                    
                    
                    }
                }




                // cleanup working folder
                Directory.Delete(WORKING_FOLDER, true);
            }
            else
            { 
                // no suitable map found
                progressStruct = new Constants.ProgressStruct();
                progressStruct.newNode = null;
                progressStruct.filename = null;
                progressStruct.errorMessage = "ERROR: No suitable map file was found.";
                ReportProgress(0, progressStruct);

                // for now, return (dynamic map creation in the future?)
            }

            return;
        }

        private string getMapFile(SsfMakeStruct pSsfMakeStruct)
        {
            FileInfo fi;
            string ret = null;

            string seqPath = Path.GetFullPath(pSsfMakeStruct.sequenceData);
            string tonePath = Path.GetFullPath(pSsfMakeStruct.toneData);

            fi = new FileInfo(seqPath);
            long seqSize = fi.Length;

            fi = new FileInfo(tonePath);
            long toneSize = fi.Length;

            if (pSsfMakeStruct.useDsp.Equals("0"))
            {
                if ((toneSize <= 0x65000) && (seqSize <= 0x10000))
                {
                    ret = "GEN_SEQ10000.MAP";
                }
                else if ((toneSize <= 0x55000) && (seqSize <= 0x20000))
                {
                    ret = "GEN_SEQ20000.MAP";
                }
            }
            else // will require multi run, but try to use tone size as a guess
            {
                if (toneSize <= 0x44FC0)
                {
                    ret = "GEN_DSP20040.MAP";
                }
                else if (toneSize <= 0x54FC0)
                {
                    ret = "GEN_DSP10040.MAP";
                }
                else if (toneSize <= 0x5CFC0)
                {
                    ret = "GEN_DSP8040.MAP";
                }
                else if (toneSize <= 0x60FC0)
                {
                    ret = "GEN_DSP4040.MAP";
                }
            }

            return ret;
        }

        private bool prepareWorkingDir(SsfMakeStruct pSsfMakeStruct)
        {
            bool ret = false;
            
            try
            {
                // create working dir
                if (!Directory.Exists(WORKING_FOLDER))
                {
                    Directory.CreateDirectory(WORKING_FOLDER);
                }

                // copy needed files
                string mapFileSourcePath = Path.Combine(SSFMAKE_FOLDER_PATH, pSsfMakeStruct.map);
                string mapFileDestinationPath = Path.Combine(WORKING_FOLDER, pSsfMakeStruct.map);

                File.Copy(mapFileSourcePath, mapFileDestinationPath, true);
                File.Copy(SSFMAKE_SOURCE_PATH, SSFMAKE_DESTINATION_PATH, true);
                File.Copy(BIN2PSF_SOURCE_PATH, BIN2PSF_DESTINATION_PATH, true);
                File.Copy(PSFPOINT_SOURCE_PATH, PSFPOINT_DESTINATION_PATH, true);

                ret = true;
            }
            catch (Exception _ex)
            {
                Directory.Delete(WORKING_FOLDER, true);

                // no suitable map found
                progressStruct = new Constants.ProgressStruct();
                progressStruct.newNode = null;
                progressStruct.filename = null;
                progressStruct.errorMessage = String.Format("ERROR: {0}", _ex.Message);
                ReportProgress(0, progressStruct);
            }

            return ret;
        }

        private bool customizeScript(SsfMakeStruct pSsfMakeStruct)
        {            
            bool ret = true;
            int lineNumber;
            string inputLine;

            string workingFile = SSFMAKE_DESTINATION_PATH + ".tmp";

            // open reader
            StreamReader reader = 
                new StreamReader(File.Open(SSFMAKE_DESTINATION_PATH, FileMode.Open, FileAccess.Read));
            // open writer for temp file
            StreamWriter writer = 
                new StreamWriter(File.Open(workingFile, FileMode.Create, FileAccess.Write));

            lineNumber = 1;
            while ((inputLine = reader.ReadLine()) != null)
            {
                switch (lineNumber)
                {
                    case LINE_NUM_SEQ_BANK:
                        writer.WriteLine(String.Format("bank      = {0}    # sequence bank number", pSsfMakeStruct.sequenceBank));
                        break;
                    case LINE_NUM_SEQ_TRACK:
                        writer.WriteLine(String.Format("track     = {0}    # sequence track number", pSsfMakeStruct.sequenceTrack));
                        break;
                    case LINE_NUM_VOLUME:
                        writer.WriteLine(String.Format("volume    = {0}    # volume (reduce if clipping)", pSsfMakeStruct.volume));
                        break;
                    case LINE_NUM_MIXER_BANK:
                        writer.WriteLine(String.Format("mixerbank = {0}    # mixer bank number (usually same as sequence bank number)", pSsfMakeStruct.mixerBank));
                        break;
                    case LINE_NUM_MIXER_NUMBER:
                        writer.WriteLine(String.Format("mixern    = {0}    # mixer number (usually 0)", pSsfMakeStruct.mixerNumber));
                        break;
                    case LINE_NUM_EFFECT:
                        writer.WriteLine(String.Format("effect    = {0}    # effect number (usually 0)", pSsfMakeStruct.effectNumber));
                        break;
                    case LINE_NUM_USE_DSP:
                        writer.WriteLine(String.Format("use_dsp   = {0}       # 1: use DSP, 0: do not use DSP", pSsfMakeStruct.useDsp));
                        break;
        
                    case LINE_NUM_DRIVER:
                        writer.WriteLine(String.Format("ndrv = '{0}'    # sound driver", Path.GetFileName(pSsfMakeStruct.driver)));
                        break;
                    case LINE_NUM_MAP:
                        writer.WriteLine(String.Format("nmap = '{0}'    # sound area map", pSsfMakeStruct.map));
                        break;
                    case LINE_NUM_TONE_DATA:
                        writer.WriteLine(String.Format("nbin = '{0}'    # tone data", Path.GetFileName(pSsfMakeStruct.toneData)));
                        break;
                    case LINE_NUM_SEQ_DATA:
                        writer.WriteLine(String.Format("nseq = '{0}'    # sequence data", Path.GetFileName(pSsfMakeStruct.sequenceData)));
                        break;
                    case LINE_NUM_DSP_PROGRAM:
                        writer.WriteLine(String.Format("nexb = '{0}'    # DSP program", Path.GetFileName(pSsfMakeStruct.dspProgram)));
                        break;

                    case LINE_NUM_OUT_FILE:
                        writer.WriteLine(String.Format("nout = 'ssfdata.ssf'    # output file name (if .ssflib, create ssflib and minissfs for each track in the bank)", "ssfdata.ssf"));
                        break;
                
                    default:
                        writer.WriteLine(inputLine);
                        break;
                }
                
                lineNumber++;
            }
            
            // close reader
            reader.Close();
            reader.Dispose();
            
            // close writer
            writer.Close();
            writer.Dispose();

            // delete original
            File.Delete(SSFMAKE_DESTINATION_PATH);
            // rename edited temp file
            File.Move(workingFile, SSFMAKE_DESTINATION_PATH);

            return ret;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            SsfMakeStruct ssfMakeStruct = (SsfMakeStruct)e.Argument;

            this.makeSsfs(ssfMakeStruct, e);
        }
    }
}
