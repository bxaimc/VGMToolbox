using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format.sdat;
using VGMToolbox.util;


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

        private void rip2sfFromFile(string pPath, string pContainerRomPath, 
            string pFilePrefix, DoWorkEventArgs e)
        {
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
            vProgressStruct.newNode = null;
            vProgressStruct.filename = pPath;
            ReportProgress(progress, vProgressStruct);

            // Used to output progress messages
            Constants.ProgressStruct vMessageProgressStruct = new Constants.ProgressStruct();            

            try
            {
                bool processSuccess = false;

                string ndsToolFileName = Application.StartupPath  + "\\external\\2sf\\ndstool.exe";
                string desmumeSaveFileName = Application.StartupPath + "\\external\\2sf\\DeSmuME_save.exe";
                string desmumeTraceFileName = Application.StartupPath + "\\external\\2sf\\DeSmuME_trace.exe";
                string zlibFileName = Application.StartupPath + "\\external\\2sf\\zlib1.dll";
                string dst2BinFileName = Application.StartupPath + "\\external\\2sf\\dst2bin.exe";
                string _2sfToolFileName = Application.StartupPath + "\\external\\2sf\\2sftool.exe";
                string psfPointPath = Application.StartupPath + "\\external\\psfpoint.exe";

                string sourceRomDestinationPath = Path.Combine(Path.GetDirectoryName(pContainerRomPath), Path.GetFileNameWithoutExtension(pPath));
                string containerRomDestinationPath = Path.Combine(Path.GetDirectoryName(pContainerRomPath), Path.GetFileNameWithoutExtension(pContainerRomPath));
                string rebuiltRomName;
                string rebuiltRomPath;
                string make2sfPath = Path.Combine(Path.GetDirectoryName(pContainerRomPath), "2sfmake");                

                string workingDirectory = Path.GetDirectoryName(pContainerRomPath);

                string ripFolder = Path.Combine(Application.StartupPath + "\\rips\\2sf\\", Path.GetFileNameWithoutExtension(pPath)); 
                string copySdatDir = Path.Combine(ripFolder, "sdats");

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

                #region Step 1 - Extract the Source ROM
                //////////////////////////////////
                //Step 1 - Extract the Source ROM
                //////////////////////////////////                
                // Report Stage of Process
                vMessageProgressStruct.genericMessage = String.Format("1) Extracting source ROM: {0}", pPath) +
                    Environment.NewLine;
                ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct); 
                
                // processSuccess = this.rip2sf_Step1(ndsToolFileName, pPath, sourceRomDestinationPath);
                processSuccess = extractRom(ndsToolFileName, pPath, workingDirectory, false);
                #endregion

                #region Step 2 - Extract the Container ROM
                //////////////////////////////////////
                // Step 2 - Extract the Container ROM
                //////////////////////////////////////
                if (processSuccess)
                {
                    // Report Stage of Process
                    vMessageProgressStruct.genericMessage = String.Format("2) Extracting container ROM: {0}", pContainerRomPath) +
                        Environment.NewLine;
                    ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct);
                    
                    // processSuccess = this.rip2sf_Step2(ndsToolFileName, pContainerRomPath, containerRomDestinationPath);                
                    processSuccess = extractRom(ndsToolFileName, pContainerRomPath, workingDirectory, true);
                }
                #endregion

                /////////////////////////////////
                // Loop on source rom sdat files
                /////////////////////////////////
                if (processSuccess)
                {
                    #region Check for SDATs in Source ROM
                    // Report Stage of Process
                    vMessageProgressStruct.genericMessage = "3.1) Searching for SDATs" + Environment.NewLine;
                    ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct);
                    
                    string[] sourceSdats = Directory.GetFiles(sourceRomDestinationPath, "*.sdat", SearchOption.AllDirectories);

                    // Report Stage of Process
                    vMessageProgressStruct.genericMessage = String.Format("3.2) Found {0} SDATs", sourceSdats.Length) + 
                        Environment.NewLine;
                    ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct);
                    #endregion

                    foreach (string sourceSdat in sourceSdats)
                    {
                        #region Copy SDAT to Destination
                        // copy sdat to destination
                        string sdatDestination;

                        // build destination path
                        if (sourceSdats.Length > 1)
                        {
                            sdatDestination = Path.Combine(copySdatDir, sdatIndex + "_" + Path.GetFileName(sourceSdat));
                        }
                        else
                        {
                            sdatDestination = Path.Combine(copySdatDir, Path.GetFileName(sourceSdat)); ;
                        }
                        
                        // copy file
                        if (!File.Exists(sdatDestination))
                        {
                            if (!Directory.Exists(copySdatDir))
                            {
                                Directory.CreateDirectory(copySdatDir);
                            }
                            File.Copy(sourceSdat, sdatDestination);
                        }
                        #endregion
                        
                        // Report Stage of Process
                        vMessageProgressStruct.genericMessage = String.Format("3.3.{0} Processing SDAT {1}: <{2}>", sdatIndex, sdatIndex, sourceSdat) +
                            Environment.NewLine;
                        ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct);                        
                        
                        // for now assume yoshi rom (sound_data.sdat)
                        string[] destinationSdats = Directory.GetFiles(containerRomDestinationPath, "sound_data.sdat", SearchOption.AllDirectories);

                        #region Step 3 - Replace SDAT and Rebuild ROM
                        /////////////////////////////////////////
                        // Step 3 - Replace SDAT and Rebuild ROM
                        /////////////////////////////////////////
                        // Report Stage of Process
                        vMessageProgressStruct.genericMessage = "3.4) Replacing SDAT" +  Environment.NewLine;
                        ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct);                        
                                                
                        rebuiltRomName = "modcrap_" + Path.GetFileNameWithoutExtension(sourceSdat) + ".nds";                        
                        
                        // processSuccess = rip2sf_Step3(ndsToolFileName, sourceSdat, destinationSdats[0], pContainerRomPath, containerRomDestinationPath, rebuiltRomName);
                        processSuccess = rebuildRom(ndsToolFileName, sourceSdat, destinationSdats[0],
                                            workingDirectory, rebuiltRomName, pContainerRomPath);
                        #endregion

                        rebuiltRomPath = Path.Combine(workingDirectory, rebuiltRomName);

                        if (processSuccess)
                        {
                            #region Step 4 - Create and Unpack Source SDAT
                            ///////////////////////////////
                            // Step 4 - Create and Unpack Source SDAT
                            ///////////////////////////////
                            // Report Stage of Process
                            vMessageProgressStruct.genericMessage = "4.1) Unpacking SDAT" + Environment.NewLine;
                            ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct);

                            Sdat currentSdat = new Sdat();
                            processSuccess = extractSdat(sourceSdat, workingDirectory, ref currentSdat);
                            #endregion

                            if (processSuccess)
                            {
                                #region Parse SMAP
                                string extractedSdatFolder = 
                                    Path.Combine(workingDirectory, Path.GetFileNameWithoutExtension(sourceSdat));
                                //string[] smapFiles = 
                                //    Directory.GetFiles(extractedSdatFolder, Path.GetFileNameWithoutExtension(sourceSdat) + ".smap", SearchOption.AllDirectories);
                                //Smap smapFile = new Smap(smapFiles[0]);
                                Smap smap = new Smap(currentSdat);
                                #endregion

                                #region Copy Strm folder data to rip folder
                                //////////////////////////////
                                // copy streams to rip folder
                                //////////////////////////////
                                // Report Stage of Process
                                vMessageProgressStruct.genericMessage = "4.2) Copying Streams" + Environment.NewLine;
                                ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct);                        
                                
                                string streamsSourceFolder = Path.Combine(extractedSdatFolder, "Strm");
                                
                                // get the list of files to move
                                if (Directory.Exists(streamsSourceFolder))
                                {
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
                                }
                                #endregion

                                if (smap.SseqSection.Length > 0)
                                //if (smapFile.SequenceArray.Length > 0)
                                {
                                    #region Step 5 - Create the Save State
                                    //////////////////////////////////
                                    // Step 5 - Create the Save State
                                    //////////////////////////////////                                    

                                    // Report Stage of Process
                                    vMessageProgressStruct.genericMessage = "5) Creating Savestate" + Environment.NewLine;
                                    ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct); 
                                    
                                    processSuccess = createSaveState(desmumeSaveFileName, zlibFileName, 
                                        workingDirectory, rebuiltRomName);
                                    #endregion

                                    if (processSuccess)
                                    {
                                        #region Step 6 - Run Tracer
                                        ///////////////////////
                                        // Step 6 - Run Tracer
                                        ///////////////////////
                                        // Report Stage of Process
                                        vMessageProgressStruct.genericMessage = "6) Run Tracer" + Environment.NewLine;
                                        ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct); 
                                        

                                        // rename prefix as needed if has multiple sdats
                                        if (sourceSdats.Length > 1)
                                        {
                                            filePrefix = pFilePrefix + "_" + sdatIndex.ToString("X2");
                                        }
                                        else
                                        {
                                            filePrefix = pFilePrefix;
                                        }
                                        
                                        processSuccess = cleanCombinedRom(desmumeTraceFileName, zlibFileName,
                                            workingDirectory, rebuiltRomName, smap.MinSseq,
                                            smap.MaxSseq, make2sfPath, filePrefix);
                                        #endregion

                                        if (processSuccess)
                                        {
                                            ///////////////////////
                                            // Step 7 - Build 2SFs
                                            ///////////////////////
                                            // Report Stage of Process
                                            vMessageProgressStruct.genericMessage = "7) Building 2SFs" + Environment.NewLine;
                                            ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct); 
                                            
                                            processSuccess = build2sfs(dst2BinFileName, _2sfToolFileName,
                                                psfPointPath, zlibFileName, workingDirectory,
                                                filePrefix, smap.MinSseq, smap.MaxSseq);

                                            ///////////////////////////////////////////
                                            // Step 9 - Move the tracks to rips folder
                                            ///////////////////////////////////////////                                            
                                            // Report Stage of Process
                                            vMessageProgressStruct.genericMessage = "9) Moving 2sfs to Rip folder" + Environment.NewLine;
                                            ReportProgress(Constants.PROGRESS_MSG_ONLY, vMessageProgressStruct); 
                                            
                                            moveRippedTracks(workingDirectory, ripFolder);
                                        }
                                    }
                                }                                                               
                                Directory.Delete(extractedSdatFolder, true);
                            }
                        }                        
                        // Increment index for dynamic naming
                        sdatIndex++;
                    }                
                }

                Directory.Delete(Path.Combine(workingDirectory, "overlay"), true);
                File.Delete(Path.Combine(workingDirectory, "arm7.bin"));
                File.Delete(Path.Combine(workingDirectory, "arm9.bin"));
                File.Delete(Path.Combine(workingDirectory, "banner.bin"));
                File.Delete(Path.Combine(workingDirectory, "header.bin"));
                File.Delete(Path.Combine(workingDirectory, "y7.bin"));
                File.Delete(Path.Combine(workingDirectory, "y9.bin"));

                Directory.Delete(sourceRomDestinationPath, true);
                Directory.Delete(containerRomDestinationPath, true);
            }
            catch (Exception ex)
            {
                vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, vProgressStruct);
            }
        }

        private bool extractRom(string pNdsToolFileName, string pSourceRomPath, string pDestinationPath, 
            bool pIsContainerRom)
        {
            bool isSuccess = false;
            Process ndsProcess;

            string sourceRomDestinationPath = Path.Combine(pDestinationPath, Path.GetFileName(pSourceRomPath));
            string sourceRomExtractionDir = Path.Combine(pDestinationPath, Path.GetFileNameWithoutExtension(sourceRomDestinationPath));
            string ndsToolDestinationPath = Path.Combine(pDestinationPath, Path.GetFileName(pNdsToolFileName));

            // copy source rom to container rom path
            if (!File.Exists(sourceRomDestinationPath))
            {
                File.Copy(pSourceRomPath, sourceRomDestinationPath);
            }

            // copy tool to destination path
            File.Copy(pNdsToolFileName, ndsToolDestinationPath, true);

            // Delete existing extraction dir
            if (Directory.Exists(sourceRomExtractionDir))
            {
                Directory.Delete(sourceRomExtractionDir);
            }

            // setup arguments
            string arguments = "-x \"" + Path.GetFileName(pSourceRomPath) + "\" ";  // rom to extract
            arguments += "-d \"" + Path.GetFileNameWithoutExtension(sourceRomDestinationPath) + "\"";  // destination directory

            if (pIsContainerRom)
            {
                arguments += " -9 \"" + Path.Combine(pDestinationPath, "arm9.bin") + "\"";
                arguments += " -7 \"" + Path.Combine(pDestinationPath, "arm7.bin") + "\"";
                arguments += " -y9 \"" + Path.Combine(pDestinationPath, "y9.bin") + "\"";
                arguments += " -y7 \"" + Path.Combine(pDestinationPath, "y7.bin") + "\"";
                arguments += " -y \"" + Path.Combine(pDestinationPath, "overlay") + "\"";
                arguments += " -t \"" + Path.Combine(pDestinationPath, "banner.bin") + "\"";
                arguments += " -h \"" + Path.Combine(pDestinationPath, "header.bin") + "\"";            
            }

            try
            {
                // execute NDSTool and extract the rom contents
                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(ndsToolDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = pDestinationPath;
                ndsProcess.StartInfo.UseShellExecute = false;
                
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                // Check for destination dir to verify succss
                if (!Directory.Exists(sourceRomExtractionDir))
                {
                    isSuccess = false;

                    // delete ndstool.exe
                    File.Delete(ndsToolDestinationPath);

                    Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                    vProgressStruct.newNode = null;
                    vProgressStruct.errorMessage = String.Format("Error extracting <{0}>.", pSourceRomPath) + Environment.NewLine;
                    ReportProgress(0, vProgressStruct);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;

                Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error extracting <{0}>.  Error received: ", pSourceRomPath) + ex.Message;
                ReportProgress(0, vProgressStruct);
            }

            // delete ndstool.exe
            File.Delete(ndsToolDestinationPath);
            
            return isSuccess;
        }
        
        private bool rebuildRom(string pNdsToolFileName, string pSourceSdatPath, string pContainerRomSdatPath,
            string pDestinationPath, string pRebuiltRomName, string pContainerRomPath)
        {
            bool isSuccess = false;
            Process ndsProcess;                        
            
            string ndsToolDestinationPath = Path.Combine(pDestinationPath, Path.GetFileName(pNdsToolFileName));
            string combinedRomOutputPath = Path.Combine(pDestinationPath, pRebuiltRomName);

            try
            {
                // copy tool to destination path                
                File.Copy(pNdsToolFileName, ndsToolDestinationPath, true);
                
                // replace Container SDAT with Source SDAT
                File.Copy(pSourceSdatPath, pContainerRomSdatPath, true);

                string arguments = " -c \"" + pRebuiltRomName + "\"";
                arguments += " -9 arm9.bin";
                arguments += " -7 arm7.bin";
                arguments += " -y9 y9.bin";
                arguments += " -y7 y7.bin";
                arguments += " -d \"" + Path.GetFileNameWithoutExtension(pContainerRomPath) + "\"";
                arguments += " -y overlay";
                arguments += " -t banner.bin";
                arguments += " -h header.bin";

                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(ndsToolDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = pDestinationPath;
                ndsProcess.StartInfo.UseShellExecute = false;                
                
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                // Verify combined ROM was created
                if (!File.Exists(combinedRomOutputPath))
                {
                    isSuccess = false;

                    // delete ndstool.exe
                    File.Delete(ndsToolDestinationPath);
                    
                    /*
                    Directory.Delete(Path.Combine(pDestinationPath, "overlay"), true);
                    File.Delete(Path.Combine(pDestinationPath, "arm7.bin"));
                    File.Delete(Path.Combine(pDestinationPath, "arm9.bin"));
                    File.Delete(Path.Combine(pDestinationPath, "banner.bin"));
                    File.Delete(Path.Combine(pDestinationPath, "header.bin"));
                    File.Delete(Path.Combine(pDestinationPath, "y7.bin"));
                    File.Delete(Path.Combine(pDestinationPath, "y9.bin"));
                    */

                    Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                    vProgressStruct.newNode = null;
                    vProgressStruct.errorMessage = String.Format("Error creating combined ROM <{0}>.", pRebuiltRomName) + Environment.NewLine;
                    ReportProgress(0, vProgressStruct);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;

                Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error creating combined ROM <{0}>.  Error received: ", pRebuiltRomName) + ex.Message;
                ReportProgress(0, vProgressStruct);
            }

            // delete ndstool.exe
            File.Delete(ndsToolDestinationPath);
            
            /*
            Directory.Delete(Path.Combine(pDestinationPath, "overlay"), true);
            File.Delete(Path.Combine(pDestinationPath, "arm7.bin"));
            File.Delete(Path.Combine(pDestinationPath, "arm9.bin"));
            File.Delete(Path.Combine(pDestinationPath, "banner.bin"));
            File.Delete(Path.Combine(pDestinationPath, "header.bin"));
            File.Delete(Path.Combine(pDestinationPath, "y7.bin"));
            File.Delete(Path.Combine(pDestinationPath, "y9.bin"));
            */

            return isSuccess;
        }

        private bool extractSdat(string pSdatToolFileName, string pSourceSdat, string pDestinationPath)
        {
            bool isSuccess = false;
            Process ndsProcess;

            string sdatToolDestinationPath =
                Path.Combine(pDestinationPath, Path.GetFileName(pSdatToolFileName));

            try
            {
                // copy tool to destination path                
                File.Copy(pSdatToolFileName, sdatToolDestinationPath, true);
                
                // Copy Source SDAT to Container Rom Directory
                string tempSdatPath = Path.Combine(pDestinationPath, Path.GetFileName(pSourceSdat));
                File.Copy(pSourceSdat, tempSdatPath, true);

                // Extract SDAT
                string arguments = " -x \"" + Path.GetFileName(pSourceSdat) + "\"";

                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(sdatToolDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = pDestinationPath;
                ndsProcess.StartInfo.UseShellExecute = false;
                ndsProcess.StartInfo.RedirectStandardOutput = true;

                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                // Delete Temporary SDAT
                File.Delete(tempSdatPath);

                // Verify SDAT dir was created
                string sdatOutputPath = 
                    Path.Combine(pDestinationPath, Path.GetFileNameWithoutExtension(pSourceSdat));
                if (!Directory.Exists(sdatOutputPath))
                {
                    isSuccess = false;

                    // delete tool
                    File.Delete(sdatToolDestinationPath);

                    Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                    vProgressStruct.newNode = null;
                    vProgressStruct.errorMessage = String.Format("Error extracting SDAT <{0}>.", pSourceSdat) + Environment.NewLine;
                    ReportProgress(0, vProgressStruct);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;

                Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error extracting SDAT <{0}>.  Error received: ", pSourceSdat) + ex.Message;
                ReportProgress(0, vProgressStruct);            
            }

            // delete tool
            File.Delete(sdatToolDestinationPath);

            return isSuccess;
        }

        private bool extractSdat(string pSourceSdat, string pDestinationPath, ref Sdat pSdat)
        {
            bool isSuccess = false;
            FileStream fs = null;

            string tempSdatPath = null;

            try
            {
                // Copy Source SDAT to Working Directory
                tempSdatPath = Path.Combine(pDestinationPath, Path.GetFileName(pSourceSdat));
                File.Copy(pSourceSdat, tempSdatPath, true);

                // Extract SDAT
                fs = File.OpenRead(tempSdatPath);
                pSdat = new Sdat();

                pSdat.Initialize(fs, tempSdatPath);
                pSdat.ExtractSseqs(fs,
                    Path.Combine(pDestinationPath, Path.GetFileNameWithoutExtension(pSourceSdat)));
                pSdat.ExtractStrms(fs,
                    Path.Combine(pDestinationPath, Path.GetFileNameWithoutExtension(pSourceSdat)));
                
                // Verify SDAT dir was created
                string sdatOutputPath =
                    Path.Combine(pDestinationPath, Path.GetFileNameWithoutExtension(pSourceSdat));
                if (!Directory.Exists(sdatOutputPath))
                {
                    isSuccess = false;

                    Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                    vProgressStruct.newNode = null;
                    vProgressStruct.errorMessage = String.Format("Error extracting SDAT <{0}>.", pSourceSdat) + Environment.NewLine;
                    ReportProgress(0, vProgressStruct);
                }
                else
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;

                Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = String.Format("Error extracting SDAT <{0}>.  Error received: ", pSourceSdat) + ex.Message;
                ReportProgress(0, vProgressStruct);
            }
            finally
            {                
                // close file stream
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                // Delete Temporary SDAT
                if (!String.IsNullOrEmpty(tempSdatPath))
                {
                    File.Delete(tempSdatPath);
                }
            }

            return isSuccess;
        }

        private bool createSaveState(string pDesmumeSaveFileName, string pZlibFileName, 
            string pDestinationPath, string pRebuiltRomName)
        {
            bool isSuccess = false;
            Process ndsProcess;

            string desSaveDestinationPath =
                Path.Combine(pDestinationPath, Path.GetFileName(pDesmumeSaveFileName));
            string zlibDestinationPath =
                Path.Combine(pDestinationPath, Path.GetFileName(pZlibFileName));

            try
            {
                // copy tool to destination path                
                File.Copy(pDesmumeSaveFileName, desSaveDestinationPath, true);
                File.Copy(pZlibFileName, zlibDestinationPath, true);

                // Build Save State
                string arguments = " \"" + pRebuiltRomName + "\"";

                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(desSaveDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = pDestinationPath;
                // ndsProcess.StartInfo.UseShellExecute = false;
                
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                // check for output artifacts to verify completion
                if (!File.Exists(Path.Combine(pDestinationPath, "BASE.DST")) ||
                    !File.Exists(Path.Combine(pDestinationPath, "main_mem.arm9")) ||
                    !File.Exists(Path.Combine(pDestinationPath, "Register_Trace.txt")))
                {
                    isSuccess = false;

                    // delete tool
                    File.Delete(desSaveDestinationPath);
                    File.Delete(zlibDestinationPath);

                    Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                    vProgressStruct.newNode = null;
                    vProgressStruct.errorMessage = "Error creating save state." + Environment.NewLine;
                    ReportProgress(0, vProgressStruct);                 
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;

                Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = "Error creating save state.  Error received: " + ex.Message;
                ReportProgress(0, vProgressStruct);             
            }

            // delete tool
            File.Delete(desSaveDestinationPath);
            File.Delete(zlibDestinationPath);

            return isSuccess;
        }

        private bool cleanCombinedRom(string pDesmumeTraceFileName, string pZlibFileName,
            string pDestinationPath, string pRebuiltRomName, int pStartIndex, int pEndIndex, 
            string pMake2sfPath, string pFilePrefix)
        {
            bool isSuccess = false;
            Process ndsProcess;

            string desTraceDestinationPath =
                Path.Combine(pDestinationPath, Path.GetFileName(pDesmumeTraceFileName));
            string zlibDestinationPath =
                Path.Combine(pDestinationPath, Path.GetFileName(pZlibFileName));

            try
            {
                // copy tool to destination path                
                File.Copy(pDesmumeTraceFileName, desTraceDestinationPath, true);
                File.Copy(pZlibFileName, zlibDestinationPath, true);
                
                // Run Tracer
                string arguments = " \"" + pRebuiltRomName + "\"";
                arguments += " --trace_bottom=" + pStartIndex.ToString() + " --trace_top=" + pEndIndex.ToString();

                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(desTraceDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = pDestinationPath;
                
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                // check for needed files
                if (!File.Exists(Path.Combine(pDestinationPath, "clean.nds")) || 
                    !File.Exists(Path.Combine(pDestinationPath, "BASE.DST")))
                {
                    isSuccess = false;

                    // delete tool
                    File.Delete(desTraceDestinationPath);
                    File.Delete(zlibDestinationPath);
                    File.Delete(Path.Combine(pDestinationPath, pRebuiltRomName));

                    Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                    vProgressStruct.newNode = null;
                    vProgressStruct.errorMessage = "Error while running tracer.  Output files do not exist." + Environment.NewLine;
                    ReportProgress(0, vProgressStruct);   
                }
                else
                {
                    // Move Cleaned Files
                    //if (!Directory.Exists(pMake2sfPath))
                    //{
                    //    Directory.CreateDirectory(pMake2sfPath);
                    //}

                    //File.Move(Path.Combine(pDestinationPath, "BASE.DST"), Path.Combine(pMake2sfPath, "BASE.DST"));
                    //File.Move(Path.Combine(pDestinationPath, "clean.nds"), Path.Combine(pMake2sfPath, pFilePrefix + ".nds"));
                }
                
                // Delete Uneeded Files
                File.Delete(Path.Combine(pDestinationPath, "data_mask.bin"));
                File.Delete(Path.Combine(pDestinationPath, "main_mem.arm9"));
                File.Delete(Path.Combine(pDestinationPath, "Register_Trace.txt"));

            }
            catch (Exception ex)
            {
                isSuccess = false;

                Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = "Error while running tracer.  Error received: " + ex.Message;
                ReportProgress(0, vProgressStruct);            
            }

            // delete tool
            File.Delete(desTraceDestinationPath);
            File.Delete(zlibDestinationPath);
            File.Delete(Path.Combine(pDestinationPath, pRebuiltRomName));

            return isSuccess;
        }

        private bool build2sfs(string pDst2BinFileName, string p2sfToolFileName, 
            string pPsfPointFileName, string pZlibFileName, string pWorkingDirectory, 
            string pFilePrefix, int pStartIndex, int pEndIndex)
        {
            bool isSuccess = false;
            string arguments;
            Process ndsProcess;

            string dst2BinDestinationPath =
                Path.Combine(pWorkingDirectory, Path.GetFileName(pDst2BinFileName));
            string _2sfToolDestinationPath =
                Path.Combine(pWorkingDirectory, Path.GetFileName(p2sfToolFileName));
            string psfPointDestinationPath =
                Path.Combine(pWorkingDirectory, Path.GetFileName(pPsfPointFileName));
            string zlibDestinationPath =
                Path.Combine(pWorkingDirectory, Path.GetFileName(pZlibFileName));
            string ndsRomName = pFilePrefix + ".nds";

            try
            {
                // copy tools to working directory
                File.Copy(pDst2BinFileName, dst2BinDestinationPath, true);
                File.Copy(p2sfToolFileName, _2sfToolDestinationPath, true);
                File.Copy(pPsfPointFileName, psfPointDestinationPath, true);
                File.Copy(pZlibFileName, zlibDestinationPath, true);
                
                // Run Dst2Bin
                arguments = " -z BASE.DST";
                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(dst2BinDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = pWorkingDirectory;
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                // Run 2sfTool
                File.Move(Path.Combine(pWorkingDirectory, "clean.nds"),
                    Path.Combine(pWorkingDirectory, ndsRomName));

                arguments = " \"" + ndsRomName + "\"";
                arguments += " \"" + "base.dst_zlib.bin" + "\" \"" + pFilePrefix + ".2sflib" + "\"";
                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(_2sfToolDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = pWorkingDirectory;
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                // Run Dst2Bin
                arguments = " -mini 236 " + pStartIndex.ToString() + " " +
                    pEndIndex.ToString() + " \"" + pFilePrefix + ".2sflib" + "\"";
                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(dst2BinDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = pWorkingDirectory;
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                // Reset Lib tag
                arguments = " -_lib=\"" + pFilePrefix + ".2sflib\" *.mini2sf";
                ndsProcess = new Process();
                ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestinationPath), arguments);
                ndsProcess.StartInfo.WorkingDirectory = pWorkingDirectory;
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();

                //arguments = " -fade=10 *.mini2sf";
                //ndsProcess = new Process();
                //ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestinationPath), arguments);
                //ndsProcess.StartInfo.WorkingDirectory = pWorkingDirectory;
                //isSuccess = ndsProcess.Start();
                //ndsProcess.WaitForExit();

            }
            catch (Exception ex)
            {
                isSuccess = false;

                // delete tools
                File.Delete(dst2BinDestinationPath);
                File.Delete(_2sfToolDestinationPath);
                File.Delete(psfPointDestinationPath);
                File.Delete(zlibDestinationPath);
                File.Delete(Path.Combine(pWorkingDirectory, "base.dst_zlib.bin"));
                File.Delete(Path.Combine(pWorkingDirectory, "BASE.DST"));
                File.Delete(Path.Combine(pWorkingDirectory, ndsRomName));

                Constants.ProgressStruct vProgressStruct = new Constants.ProgressStruct();
                vProgressStruct.newNode = null;
                vProgressStruct.errorMessage = "Error building 2SFs.  Error received: " + ex.Message;
                ReportProgress(0, vProgressStruct);             
            }

            // delete tools
            File.Delete(dst2BinDestinationPath);
            File.Delete(_2sfToolDestinationPath);
            File.Delete(psfPointDestinationPath);
            File.Delete(zlibDestinationPath);
            File.Delete(Path.Combine(pWorkingDirectory, "base.dst_zlib.bin"));
            File.Delete(Path.Combine(pWorkingDirectory, "BASE.DST"));
            File.Delete(Path.Combine(pWorkingDirectory, ndsRomName));

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
