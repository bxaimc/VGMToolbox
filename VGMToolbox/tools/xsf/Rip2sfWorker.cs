using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using VGMToolbox.auditing;

namespace VGMToolbox.tools.xsf
{
    class Rip2sfWorker : BackgroundWorker
    {
        [DllImport("WinMM.dll")]
        private static extern long mciSendString(string strCommand,
            StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);
        
        private int fileCount = 0;
        private int maxFiles = 0;

        public struct Rip2sfStruct
        {
            public string[] pPaths;
            public string containerRomPath;
            public string filePrefix;
            public int totalFiles;
        }

        public Rip2sfWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }


        private void rip2sfFiles(Rip2sfStruct pRip2sfStruct, DoWorkEventArgs e)
        {
            foreach (string path in pRip2sfStruct.pPaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.rip2sfFromFile(path, pRip2sfStruct.containerRomPath, pRip2sfStruct.filePrefix, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    //this.extractCompressedProgramsFromDirectory(path, 
                    //    pXsfCompressedProgramExtractorStruct.includeExtension, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        /*
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
        */

        private void rip2sfFromFile(string pPath, string pContainerRomPath, 
            string pFilePrefix, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            AuditingUtil.ProgressStruct vProgressStruct = new AuditingUtil.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);
         
            try
            {
                bool processSuccess = false;
                // string containerPath = @"H:\ROMs\Music\[2sf]\[test]\yoshi.nds";

                string ndsToolFileName = ".\\external\\2sf\\ndstool.exe";
                string sdatToolFileName = ".\\external\\2sf\\sdattool.exe";
                string desmumeSaveFileName = ".\\external\\2sf\\DeSmuME_save.exe";
                string desmumeTraceFileName = ".\\external\\2sf\\DeSmuME_trace.exe";
                string dst2BinFileName = ".\\external\\2sf\\dst2bin.exe";
                string _2sfToolFileName = ".\\external\\2sf\\2sftool.exe";
                string psfPointPath = ".\\external\\psfpoint.exe";
                string sseq2MidFileName = ".\\external\\2sf\\sseq2mid.exe";

                string sourceRomDestinationPath = Path.Combine(Path.GetDirectoryName(pContainerRomPath), Path.GetFileNameWithoutExtension(pPath));
                string containerRomDestinationPath = Path.Combine(Path.GetDirectoryName(pContainerRomPath), Path.GetFileNameWithoutExtension(pContainerRomPath));
                string rebuiltRomName;
                string rebuiltRomPath;
                string make2sfPath = Path.Combine(Path.GetDirectoryName(pContainerRomPath), "2sfmake");

                string ripFolder = Path.Combine(".\\rips\\2sf\\", Path.GetFileNameWithoutExtension(pPath)); 

                int sdatIndex = 0;
                string filePrefix;

                if (String.IsNullOrEmpty(pFilePrefix))
                {
                    filePrefix = "test";
                }
                else
                {
                    filePrefix = pFilePrefix;
                }

                //////////////////////////////////
                //Step 1 - Extract the Source ROM
                //////////////////////////////////
                processSuccess = this.rip2sf_Step1(ndsToolFileName, pPath, sourceRomDestinationPath);

                //////////////////////////////////////
                // Step 2 - Extract the Container ROM
                //////////////////////////////////////
                if (processSuccess)
                {
                    processSuccess = this.rip2sf_Step2(ndsToolFileName, pContainerRomPath, containerRomDestinationPath);                
                }

                /////////////////////////////////
                // Loop on source rom sdat files
                /////////////////////////////////
                if (processSuccess)
                {                     
                    string[] sourceSdats = Directory.GetFiles(sourceRomDestinationPath, "*.sdat", SearchOption.AllDirectories);

                    foreach (string sourceSdat in sourceSdats)
                    {
                        // for now assume yoshi rom (sound_data.sdat)
                        string[] destinationSdats = Directory.GetFiles(containerRomDestinationPath, "sound_data.sdat", SearchOption.AllDirectories);

                        /////////////////////////////////////////
                        // Step 3 - Replace SDAT and Rebuild ROM
                        /////////////////////////////////////////
                        rebuiltRomName = "modcrap_" + Path.GetFileNameWithoutExtension(sourceSdat) + ".nds";
                        rebuiltRomPath = Path.Combine(Path.GetDirectoryName(pContainerRomPath), rebuiltRomName);
                        processSuccess = rip2sf_Step3(ndsToolFileName, sourceSdat, destinationSdats[0], pContainerRomPath, containerRomDestinationPath, rebuiltRomName);

                        if (processSuccess)
                        {

                            ///////////////////////////////
                            // Step 4 - Unpack Source SDAT
                            ///////////////////////////////
                            processSuccess = rip2sf_Step4(sdatToolFileName, sourceSdat, pContainerRomPath);

                            if (processSuccess)
                            {
                                string extractedSdatFolder = Path.Combine(Path.GetDirectoryName(pContainerRomPath), Path.GetFileNameWithoutExtension(sourceSdat));
                                string[] smapFiles = Directory.GetFiles(extractedSdatFolder, Path.GetFileNameWithoutExtension(sourceSdat) + ".smap", SearchOption.AllDirectories);
                                Smap smapFile = new Smap(smapFiles[0]);

                                //////////////////////////////
                                // copy streams to rip folder
                                //////////////////////////////
                                string streamsSourceFolder = Path.Combine(extractedSdatFolder, "Strm");
                                
                                // get the list of files to move
                                string[] streamFileList = Directory.GetFiles(streamsSourceFolder, "*.*", SearchOption.AllDirectories);

                                if (streamFileList.Length > 0)
                                {
                                    string streamsDestinationFolder = Path.Combine(ripFolder, "streams");

                                    // create destination folder if it doesn't exist (it shouldn't)
                                    if (!Directory.Exists(streamsDestinationFolder))
                                    {
                                        Directory.CreateDirectory(streamsDestinationFolder);
                                    }

                                    // move the files
                                    foreach (string s in streamFileList)
                                    {
                                        File.Move(s, Path.Combine(streamsDestinationFolder, Path.GetFileName(s)));
                                    }
                                }

                                if (smapFile.SequenceArray.Length > 0)
                                { 
                                    //////////////////////////////////
                                    // Step 5 - Create the Save State
                                    //////////////////////////////////                                    
                                    processSuccess = rip2sf_Step5(desmumeSaveFileName, rebuiltRomPath);

                                    if (processSuccess)
                                    { 
                                        ///////////////////////
                                        // Step 6 - Run Tracer
                                        ///////////////////////
                                        
                                        // rename prefix as needed if has multiple sdats
                                        if (sourceSdats.Length > 1)
                                        {
                                            filePrefix = pFilePrefix + sdatIndex.ToString("X2");
                                        }
                                        else
                                        {
                                            filePrefix = pFilePrefix;
                                        }
                                        
                                        processSuccess = rip2sf_Step6(desmumeTraceFileName, rebuiltRomPath, 
                                            smapFile.SequenceArray.GetLowerBound(0), smapFile.SequenceArray.GetUpperBound(0),
                                            make2sfPath, filePrefix);

                                        if (processSuccess)
                                        {
                                            ///////////////////////
                                            // Step 7 - Build 2SFs
                                            ///////////////////////
                                            processSuccess = rip2sf_Step7(dst2BinFileName, _2sfToolFileName,
                                                psfPointPath, make2sfPath, filePrefix, smapFile.SequenceArray.GetLowerBound(0),
                                                smapFile.SequenceArray.GetUpperBound(0));


                                            /////////////////////////////
                                            // Step 8 - Time the Tracks
                                            ////////////////////////////
                                            processSuccess = rip2sf_Step8(sseq2MidFileName, psfPointPath,
                                                make2sfPath, extractedSdatFolder, filePrefix, smapFile);

                                            ///////////////////////////////////////////
                                            // Step 9 - Move the tracks to rips folder
                                            ///////////////////////////////////////////                                            
                                            moveRippedTracks(make2sfPath, ripFolder);


                                            // Do Cleanup
                                            Directory.Delete(make2sfPath, true);
                                        }
                                    }
                                }
                                // Do cleanup
                                Directory.Delete(extractedSdatFolder, true);
                            }
                        }
                        
                        // Do cleanup
                        File.Delete(rebuiltRomPath);

                        // Increment index for dynamic naming
                        sdatIndex++;
                    }                
                }
                // Do cleamup
                Directory.Delete(sourceRomDestinationPath, true);
                Directory.Delete(containerRomDestinationPath, true);
                Directory.Delete(Path.Combine(Path.GetDirectoryName(pContainerRomPath), "overlay"), true);
                File.Delete(Path.Combine(Path.GetDirectoryName(pContainerRomPath), "arm7.bin"));
                File.Delete(Path.Combine(Path.GetDirectoryName(pContainerRomPath), "arm9.bin"));
                File.Delete(Path.Combine(Path.GetDirectoryName(pContainerRomPath), "banner.bin"));
                File.Delete(Path.Combine(Path.GetDirectoryName(pContainerRomPath), "header.bin"));
                File.Delete(Path.Combine(Path.GetDirectoryName(pContainerRomPath), "y7.bin"));
                File.Delete(Path.Combine(Path.GetDirectoryName(pContainerRomPath), "y9.bin"));
            }
            catch (Exception ex)
            {
                vProgressStruct = new AuditingUtil.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, vProgressStruct);
            }            
        }

        private bool rip2sf_Step1(string pNdsToolFileName, string pSourceRomPath, string pSourceRomDestinationPath)
        {
            // string arguments = @"-x H:\ROMs\Music\[2sf]\RipKit\target.nds -d H:\ROMs\Music\[2sf]\RipKit\targetDir";                
            
            bool isSuccess = false;
            Process ndsProcess;

            string arguments = "-x \"" + pSourceRomPath + "\" ";
            arguments += "-d \"" + pSourceRomDestinationPath + "\"";

            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(pNdsToolFileName, arguments);
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            return isSuccess;
        }

        private bool rip2sf_Step2(string pNdsToolFileName, string pContainerRomPath, string pContainerRomDestinationPath)
        {
            bool isSuccess = false;
            Process ndsProcess;

            string arguments = "-x \"" + pContainerRomPath + "\"";
            arguments += " -9 \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "arm9.bin") + "\"";
            arguments += " -7 \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "arm7.bin") + "\"";
            arguments += " -y9 \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "y9.bin") + "\"";
            arguments += " -y7 \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "y7.bin") + "\"";
            arguments += " -d \"" + pContainerRomDestinationPath + "\"";
            arguments += " -y \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "overlay") + "\"";
            arguments += " -t \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "banner.bin") + "\"";
            arguments += " -h \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "header.bin") + "\"";
            
            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(pNdsToolFileName, arguments);            
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();
             
            return isSuccess;
        }

        private bool rip2sf_Step3(string pNdsToolFileName, string pSourceSdat, string pContainerRomSdat, string pContainerRomPath, string pContainerRomDestinationPath, string pCombinedRomName)
        {
            bool isSuccess = false;
            Process ndsProcess;

            // Replace Container SDAT with Source SDAT
            File.Copy(pSourceSdat, pContainerRomSdat, true);

            string arguments = " -c \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), pCombinedRomName) + "\"";
            arguments += " -9 \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "arm9.bin") + "\"";
            arguments += " -7 \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "arm7.bin") + "\"";
            arguments += " -y9 \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "y9.bin") + "\"";
            arguments += " -y7 \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "y7.bin") + "\"";
            arguments += " -d \"" + pContainerRomDestinationPath + "\"";
            arguments += " -y \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "overlay") + "\"";
            arguments += " -t \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "banner.bin") + "\"";
            arguments += " -h \"" + Path.Combine(Path.GetDirectoryName(pContainerRomPath), "header.bin") + "\"";

            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(pNdsToolFileName, arguments);
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            return isSuccess;
        }

        private bool rip2sf_Step4(string pSdatToolFileName, string pSourceSdat, string pContainerRomPath)
        {
            bool isSuccess = false;
            Process ndsProcess;

            // Copy Source SDAT to Container Rom Directory
            string tempSdatPath = Path.Combine(Path.GetDirectoryName(pContainerRomPath), Path.GetFileName(pSourceSdat));
            File.Copy(pSourceSdat, tempSdatPath, true);

            // Extract SDAT
            string arguments = " -x \"" + tempSdatPath + "\"";

            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(pSdatToolFileName, arguments);
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            // Delete Temporary SDAT
            File.Delete(tempSdatPath);

            return isSuccess;
        }

        private bool rip2sf_Step5(string pDesmumeSaveFileName, string pCombinedRomPath)
        {
            bool isSuccess = false;
            Process ndsProcess;

            // Build Save State
            string arguments = " \"" + pCombinedRomPath + "\"";

            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(pDesmumeSaveFileName, arguments);
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            // Move Save State Files
            // File.Copy("BASE.DST", Path.Combine(Path.GetDirectoryName(pCombinedRomPath), "BASE.DST"));
            // File.Copy("main_mem.arm9", Path.Combine(Path.GetDirectoryName(pCombinedRomPath), "main_mem.arm9"));            
            // File.Copy("Register_Trace.txt", Path.Combine(Path.GetDirectoryName(pCombinedRomPath), "Register_Trace.txt"));

            // File.Copy("BASE.DST", Path.Combine(@".\external\2sf\", "BASE.DST"), true);
            // File.Copy("main_mem.arm9", Path.Combine(@".\external\2sf", "main_mem.arm9"), true);
            // File.Copy("Register_Trace.txt", Path.Combine(@".\external\2sf", "Register_Trace.txt"), true);

            // File.Delete("BASE.DST");
            // File.Delete("main_mem.arm9");
            // File.Delete("Register_Trace.txt");

            return isSuccess;
        }

        private bool rip2sf_Step6(string pDesmumeTraceFileName, string pCombinedRomPath,
            int pStartIndex, int pEndIndex, string pMake2sfPath, string pFilePrefix)
        {
            bool isSuccess = false;
            Process ndsProcess;

            // Run Tracer
            string arguments = " \"" + pCombinedRomPath + "\"";
            arguments += " --trace_bottom=" + pStartIndex.ToString() + " --trace_top=" + pEndIndex.ToString();

            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(pDesmumeTraceFileName, arguments);
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            // Move Cleaned Files
            if (!Directory.Exists(pMake2sfPath))
            {
                Directory.CreateDirectory(pMake2sfPath);
            }
            
            File.Move("BASE.DST", Path.Combine(pMake2sfPath, "BASE.DST"));
            File.Move("clean.nds", Path.Combine(pMake2sfPath, pFilePrefix + ".nds"));

            // Delete Uneeded Files
            File.Delete("data_mask.bin");
            File.Delete("main_mem.arm9");
            File.Delete("Register_Trace.txt");

            return isSuccess;
        }

        private bool rip2sf_Step7(string pDst2BinFileName, string p2sfToolFileName, 
            string pPsfPointFileName, string pMake2sfPath, string pFilePrefix, int pStartIndex, int pEndIndex)
        {
            bool isSuccess = false;
            string arguments;
            Process ndsProcess;

            // Run Dst2Bin
            arguments = " -z \"" + Path.Combine(pMake2sfPath, "BASE.DST") + "\"";
            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(pDst2BinFileName, arguments);
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            // Run 2sfTool
            arguments = " \"" + Path.Combine(pMake2sfPath, pFilePrefix + ".nds") + "\"";
            arguments += " \"" + Path.Combine(pMake2sfPath, "base.dst_zlib.bin") + "\" \"" + Path.Combine(pMake2sfPath, pFilePrefix + ".2sflib") + "\"";
            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(p2sfToolFileName, arguments);
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            // Run Dst2Bin
            arguments = " -mini 236 " + pStartIndex.ToString() + " " + 
                pEndIndex.ToString() + " \"" + Path.Combine(pMake2sfPath, pFilePrefix + ".2sflib") + "\"";
            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(pDst2BinFileName, arguments);
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            // Reset Lib tag
            //psfPointPath
            string psfPointDestination = Path.Combine(pMake2sfPath, Path.GetFileName(pPsfPointFileName));
            File.Copy(pPsfPointFileName, psfPointDestination);
            
            arguments = " -_lib=\"" + pFilePrefix + ".2sflib\" *.mini2sf";
            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
            ndsProcess.StartInfo.WorkingDirectory = pMake2sfPath;
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            arguments = " -fade=10 *.mini2sf";
            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
            ndsProcess.StartInfo.WorkingDirectory = pMake2sfPath;
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();

            return isSuccess;
        }

        private bool rip2sf_Step8(string pSseq2MidFileName, string pPsfPointFileName, 
            string pMake2sfPath, string pExtractedSdatPath, string pFilePrefix, Smap pSmap)
        {
            bool isSuccess = false;
            string arguments;
            Process ndsProcess;

            StringBuilder strReturn = new StringBuilder(128);
            int tempTime;
            int minutes;
            int seconds;
            string command;
            long err;

            // copy psfpoint
            string psfPointDestination = Path.Combine(pMake2sfPath, Path.GetFileName(pPsfPointFileName));
            File.Copy(pPsfPointFileName, psfPointDestination, true);

            // Run Sseq2Mid
            string sseqPath = Path.Combine(pExtractedSdatPath, "Seq");
            string ssqe2midDestination = Path.Combine(sseqPath, Path.GetFileName(pSseq2MidFileName));
            File.Copy(pSseq2MidFileName, ssqe2midDestination, true);
            
            arguments = " -2 -l >>dp *.sseq";
            ndsProcess = new Process();
            ndsProcess.StartInfo = new ProcessStartInfo(ssqe2midDestination, arguments);
            ndsProcess.StartInfo.WorkingDirectory = sseqPath;
            isSuccess = ndsProcess.Start();
            ndsProcess.WaitForExit();



            foreach (Smap.SmapSeqStruct s in pSmap.SequenceArray)
            {
                string _2sfFileName = pFilePrefix + "-" + s.number.ToString("X4") + ".mini2sf";

                if (!String.IsNullOrEmpty(s.name))
                {
                    string midiFilePath = Path.Combine(pExtractedSdatPath, s.name + ".mid");

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

                        // Update length on mini2sf
                        arguments = " -length=\"" + minutes + ":" + seconds.ToString().PadLeft(2, '0') + "\" " + _2sfFileName;
                        ndsProcess = new Process();
                        ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
                        ndsProcess.StartInfo.WorkingDirectory = pMake2sfPath;
                        isSuccess = ndsProcess.Start();
                        ndsProcess.WaitForExit();
                    }
                    else
                    {
                        // Delete empty sequence 2sf file
                        File.Delete(Path.Combine(pMake2sfPath, _2sfFileName));
                    }
                }
                else
                { 
                    // Delete empty sequence 2sf file
                    File.Delete(Path.Combine(pMake2sfPath, _2sfFileName));
                }
            }
            
            return isSuccess;
        }

        private void moveRippedTracks(string pSourcePath, string pDestinationPath)
        {            
            if (!Directory.Exists(pDestinationPath))
            {
                Directory.CreateDirectory(pDestinationPath);
            }
            
            foreach (string s in Directory.GetFiles(pSourcePath, "*.mini2sf", SearchOption.TopDirectoryOnly))
            {
                File.Move(s, Path.Combine(pDestinationPath, Path.GetFileName(s)));
            }

            foreach (string s in Directory.GetFiles(pSourcePath, "*.2sflib", SearchOption.TopDirectoryOnly))
            {
                File.Move(s, Path.Combine(pDestinationPath, Path.GetFileName(s)));
            }
        }
        
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Rip2sfStruct rip2sfStruct = (Rip2sfStruct)e.Argument;
            maxFiles = rip2sfStruct.totalFiles;

            this.rip2sfFiles(rip2sfStruct, e);
        }    
    }
}
