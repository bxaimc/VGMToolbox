using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using VGMToolbox.format.sdat;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class Time2sfWorker : BackgroundWorker
    {
        private static readonly string SSEQ2MID_SOURCE_PATH = 
            Path.Combine(Path.Combine(Path.Combine(".", "external"), "2sf"), "sseq2mid.exe");
        private static readonly string PSFPOINT_SOURCE_PATH =
            Path.GetFullPath(Path.Combine(Path.Combine(".", "external"), "psfpoint.exe"));

        private const string EMPTY_FILE_DIRECTORY = "Empty_Files";

        public const string SSEQ2MID_TXT = "sseq2mid_output.txt";
        public const string PSFPOINT_BATCH_TXT = "psfpoint_batch.bat";

        public const string SSEQ2MID_TXT_MARKER = ".sseq:";
        public const string SSEQ2MID_TXT_END_OF_TRACK = "End of Track";


        [DllImport("WinMM.dll")]
        private static extern long mciSendString(string strCommand,
            StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        private int fileCount = 0;
        private int maxFiles = 0;
        private Constants.ProgressStruct progressStruct;

        public struct Time2sfStruct
        {
            public string pathTo2sf;
            public string pathToSdat;
            public string filePrefix;
        }

        public Time2sfWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void time2sfs(Time2sfStruct pTime2sfStruct, DoWorkEventArgs e)
        {
            bool haveDirectoryError = false;
            string extractedSdatPath;
            string extractedSseqPath;

            if (!CancellationPending)
            {

                // Check incoming values
                if (!Directory.Exists(pTime2sfStruct.pathTo2sf))
                {
                    haveDirectoryError = true;

                    this.progressStruct = new Constants.ProgressStruct();
                    this.progressStruct.newNode = null;
                    this.progressStruct.errorMessage = String.Format("ERROR: Path <{0}> not found.",
                        pTime2sfStruct.pathTo2sf) + Environment.NewLine;
                    ReportProgress(0, this.progressStruct);
                }

                if (!File.Exists(pTime2sfStruct.pathToSdat))
                {
                    haveDirectoryError = true;

                    this.progressStruct = new Constants.ProgressStruct();
                    this.progressStruct.newNode = null;
                    this.progressStruct.errorMessage = String.Format("ERROR: Path <{0}> not found.",
                        pTime2sfStruct.pathToSdat) + Environment.NewLine;
                    ReportProgress(0, this.progressStruct);
                }
            }
            else
            {
                e.Cancel = true;
                return;
            }

            if (!haveDirectoryError)
            {                
                if (!CancellationPending)
                {
                    Smap smap;
                    bool processSuccess = false;

                    // extract SDAT
                    extractedSdatPath =
                        Path.Combine(Path.GetDirectoryName(pTime2sfStruct.pathToSdat), Path.GetFileNameWithoutExtension(pTime2sfStruct.pathToSdat));
                    extractedSseqPath = Path.Combine(extractedSdatPath, "Seq");

                    using (FileStream fs = File.Open(pTime2sfStruct.pathToSdat, FileMode.Open, FileAccess.Read))
                    {
                        Sdat sdat = new Sdat();
                        sdat.Initialize(fs, pTime2sfStruct.pathToSdat);
                        sdat.ExtractSseqs(fs, extractedSdatPath);

                        // make SMAP
                        smap = new Smap(sdat);
                    }

                    // create dir for empty files
                    string emptyFileDir = Path.Combine(pTime2sfStruct.pathTo2sf, EMPTY_FILE_DIRECTORY);

                    // setup vals for progress bar
                    this.maxFiles = smap.SseqSection.Length;
                    this.fileCount = 0;

                    // loop through SMAP SSEQs
                    foreach (Smap.SmapSeqStruct s in smap.SseqSection)
                    {                                                
                        if (!CancellationPending)
                        {
                            string rippedFileName = String.Format("{0}-{1}.mini2sf", pTime2sfStruct.filePrefix, 
                                s.number.ToString("X4"));
                            string rippedFilePath = Path.Combine(pTime2sfStruct.pathTo2sf, rippedFileName);

                            // report progress
                            int progress = (++this.fileCount * 100) / this.maxFiles;
                            this.progressStruct = new Constants.ProgressStruct();
                            this.progressStruct.newNode = null;
                            this.progressStruct.filename = rippedFilePath;
                            ReportProgress(progress, this.progressStruct);

                            if (s.fileID == Smap.EMPTY_FILE_ID)
                            {
                                // move to empty dir
                                if (!Directory.Exists(emptyFileDir))
                                {
                                    Directory.CreateDirectory(emptyFileDir);
                                }

                                if (File.Exists(rippedFilePath))
                                {
                                    File.Move(rippedFilePath, Path.Combine(emptyFileDir, rippedFileName));
                                }
                            }
                            else
                            {
                                string sseqFilePath = Path.Combine(extractedSseqPath, s.name);

                                // convert sseq file to midi
                                processSuccess = this.convertSseqFile(SSEQ2MID_SOURCE_PATH, 
                                    pTime2sfStruct.pathTo2sf,  sseqFilePath);

                                // time file
                                if (processSuccess)
                                {
                                    processSuccess = this.buildFileTimingBatch(pTime2sfStruct.pathTo2sf,
                                        rippedFilePath, sseqFilePath);
                                }
                            }
                            
                            this.fileCount++;
                        }
                        else
                        {
                            e.Cancel = true;
                            break;
                        }
                    } // foreach (Smap.SmapSeqStruct s in smap.SseqSection)

                    this.executeBatchScript(PSFPOINT_SOURCE_PATH, pTime2sfStruct.pathTo2sf);

                    this.doCleanup(extractedSdatPath, pTime2sfStruct.pathTo2sf, PSFPOINT_SOURCE_PATH);

                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }        
        }

        private bool convertSseqFile(string pSseq2MidToolPath, string pMini2sfPath,
            string pSseqFilePath)
        {
            Process ndsProcess;
            bool isSuccess;

            // convert existing sseq to mid            
            string sseqPath = Path.GetDirectoryName(pSseqFilePath);

            string sseq2MidDestinationPath = Path.Combine(sseqPath, Path.GetFileName(pSseq2MidToolPath));

            try
            {
                File.Copy(pSseq2MidToolPath, sseq2MidDestinationPath, true);

                string arguments = String.Format(" -2 -l {0}", Path.GetFileName(pSseqFilePath));

                ndsProcess = new Process();

                ndsProcess.StartInfo = new ProcessStartInfo(sseq2MidDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = sseqPath;
                ndsProcess.StartInfo.UseShellExecute = false;
                ndsProcess.StartInfo.CreateNoWindow = true;
                ndsProcess.StartInfo.RedirectStandardOutput = true;
                isSuccess = ndsProcess.Start();
                string sseqOutputFile = ndsProcess.StandardOutput.ReadToEnd();
                ndsProcess.WaitForExit();

                // output redirected standard output
                string textOutputPath = Path.Combine(pMini2sfPath, "text");
                string sseq2MidOutputPath = Path.Combine(textOutputPath, SSEQ2MID_TXT);

                if (!Directory.Exists(textOutputPath)) { Directory.CreateDirectory(textOutputPath); }

                TextWriter tw = File.CreateText(sseq2MidOutputPath);
                tw.Write(sseqOutputFile);
                tw.Close();
                tw.Dispose();
            }
            catch (Exception _e)
            {
                isSuccess = false;
                Console.WriteLine(_e.Message);
            }

            return isSuccess;
        }

        private bool buildFileTimingBatch(string pMini2sfPath,
            string p2sfFilePath, string pSseqFilePath)
        {
            bool isSuccess = false;
            string arguments;

            StreamWriter sw;

            StringBuilder strReturn = new StringBuilder(128);
            int tempTime;
            int minutes;
            int seconds;
            string command;
            long err;

            string _2sfFileName = Path.GetFileName(p2sfFilePath);

            string psfpointBatchFilePath = Path.Combine(Path.Combine(pMini2sfPath, "text"), PSFPOINT_BATCH_TXT);

            if (!File.Exists(psfpointBatchFilePath))
            {
                sw = File.CreateText(psfpointBatchFilePath);
            }
            else
            {
                sw = new StreamWriter(File.Open(psfpointBatchFilePath, FileMode.Append, FileAccess.Write));
            }

            try
            {
                string midiFilePath = pSseqFilePath + ".mid";

                if (File.Exists(midiFilePath))
                {
                    command = string.Format("open \"{0}\" type sequencer alias MidiFile", midiFilePath);
                    err = mciSendString(command, strReturn, 128, IntPtr.Zero);

                    command = string.Format("set MidiFile time format milliseconds");
                    err = mciSendString(command, strReturn, 128, IntPtr.Zero);

                    command = string.Format("status MidiFile length");
                    err = mciSendString(command, strReturn, 128, IntPtr.Zero);

                    tempTime = Int32.Parse(strReturn.ToString());
                    minutes = tempTime / 60000;
                    seconds = ((tempTime - (minutes * 60000)) % 60000) / 1000;

                    command = string.Format("close MidiFile");
                    err = mciSendString(command, strReturn, 128, IntPtr.Zero);

                    // Do Fade
                    if (isLoopingTrack(pMini2sfPath, Path.GetFileName(pSseqFilePath)))
                    {
                        arguments = " -fade=\"10\" " + _2sfFileName;

                        if (minutes == 0 && seconds == 0)
                        {
                            seconds = 1;
                        }
                    }
                    else
                    {
                        arguments = " -fade=\"1\" " + _2sfFileName;
                        seconds++;
                        if (seconds > 60)
                        {
                            minutes++;
                            seconds -= 60;
                        }

                    }

                    // Add fade info to batch file
                    sw.WriteLine("psfpoint.exe" + arguments);

                    // Add length info to batch file
                    arguments = " -length=\"" + minutes + ":" + seconds.ToString().PadLeft(2, '0') + "\" " + _2sfFileName;
                    sw.WriteLine("psfpoint.exe" + arguments);
                }

                isSuccess = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error timing {0}: {1}", p2sfFilePath, e.Message));
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

            return isSuccess;
        }

        private bool isLoopingTrack(string pMini2sfPath, string pSequenceName)
        {
            string sseq2MidOutputPath = Path.Combine(Path.Combine(pMini2sfPath, "text"), SSEQ2MID_TXT);
            string oneLineBack = String.Empty;
            string twoLinesBack = String.Empty;

            bool _ret = true;

            // Check Path
            if (File.Exists(sseq2MidOutputPath))
            {
                string inputLine = String.Empty;

                TextReader textReader = new StreamReader(sseq2MidOutputPath);
                while ((inputLine = textReader.ReadLine()) != null)
                {
                    // Check for the incoming sequence name
                    string sseqFileName = Path.GetFileName(pSequenceName);
                    if (inputLine.Trim().Contains(sseqFileName))
                    {
                        // Skip columns headers
                        textReader.ReadLine();

                        // Read until EOF or End of SEQ section (blank line)
                        while (((inputLine = textReader.ReadLine()) != null) &&
                               !inputLine.Trim().Contains(SSEQ2MID_TXT_MARKER))
                        {
                            twoLinesBack = oneLineBack;
                            oneLineBack = inputLine;
                        }

                        if (twoLinesBack.Contains(SSEQ2MID_TXT_END_OF_TRACK))
                        {
                            _ret = false;
                        }
                    }


                }

                textReader.Close();
                textReader.Dispose();
            }

            return _ret;
        }

        private void executeBatchScript(string pPsfPointPath, string pMini2sfPath)
        {
            Process ndsProcess;

            // copy psfpoint.exe
            string psfpointDestinationPath = Path.Combine(pMini2sfPath, Path.GetFileName(pPsfPointPath));
            File.Copy(pPsfPointPath, psfpointDestinationPath, true);

            // copy script
            string psfpointBatchFilePath = Path.Combine(Path.Combine(pMini2sfPath, "text"), PSFPOINT_BATCH_TXT);
            string psfpointBatchFileDestinationPath = Path.Combine(pMini2sfPath, PSFPOINT_BATCH_TXT);
            File.Copy(psfpointBatchFilePath, psfpointBatchFileDestinationPath, true);

            // execute script
            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(PSFPOINT_BATCH_TXT);
            ndsProcess.StartInfo.WorkingDirectory = pMini2sfPath;
            ndsProcess.StartInfo.CreateNoWindow = true;
            ndsProcess.Start();
            ndsProcess.WaitForExit();

            // delete files
            File.Delete(psfpointDestinationPath);
            File.Delete(psfpointBatchFileDestinationPath);
        }

        private void doCleanup(string pExtractedSdatPath, string pMini2sfPath, string pPsfPointPath)
        {
            Directory.Delete(pExtractedSdatPath, true);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Time2sfStruct time2sfStruct = (Time2sfStruct) e.Argument;
            time2sfStruct.pathTo2sf = Path.GetFullPath(time2sfStruct.pathTo2sf);
            time2sfStruct.pathToSdat = Path.GetFullPath(time2sfStruct.pathToSdat);
            
            // add code to sanitize file prefix here.

            this.time2sfs(time2sfStruct, e);
        }    
    }
}
