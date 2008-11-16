using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using VGMToolbox.format.sdat;

namespace _2sftimer
{
    class Program
    {
        public const string SSEQ2MID_TXT = "sseq2mid_output.txt";
        public const string PSFPOINT_BATCH_TXT = "psfpoint_batch.bat";

        public const string SSEQ2MID_TXT_MARKER = ".sseq:";
        public const string SSEQ2MID_TXT_END_OF_TRACK = "End of Track";

        [DllImport("WinMM.dll")]
        private static extern long mciSendString(string strCommand,
            StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        static void Main(string[] args)
        {
            string pathTo2sf;
            string pathToSdat;
            string filePrefix;
            string parseMethod;
            string smapPath = String.Empty;
            bool processSuccess = false;

            if (args.Length < 4)
            {
                usage();
                return;
            }
            else
            {
                parseMethod = args[0].Replace("-", String.Empty).Trim().ToLower();
                pathTo2sf = args[1];
                pathToSdat = args[2];
                filePrefix = args[3];

                if (parseMethod.Equals("m"))
                {
                    smapPath = args[4];
                }
            }

            if (!Directory.Exists(pathTo2sf))
            {
                Console.WriteLine(String.Format("Cannot find directory <{0}>", pathTo2sf));
            }

            if (!File.Exists(pathToSdat))
            {
                Console.WriteLine(String.Format("Cannot find file <{0}>", pathToSdat));
            }

            if (!String.IsNullOrEmpty(smapPath) && !File.Exists(smapPath))
            {
                Console.WriteLine(String.Format("Cannot find file <{0}>", smapPath));
            }

            string sdatToolPath = Path.Combine(Path.Combine(".", "helper"), "sdattool.exe");
            string sseq2MidPath = Path.Combine(Path.Combine(".", "helper"), "sseq2mid.exe");
            string psfpointPath = Path.Combine(Path.Combine(".", "helper"), "psfpoint.exe");
            string ndssndextPath = Path.Combine(Path.Combine(".", "helper"), "ndssndext.exe");

            Console.WriteLine();

            // Extract SDAT
            Console.WriteLine("Extracting SDAT");

            string extractedSdatPath = String.Empty;
            string extractedSseqPath = String.Empty;

            processSuccess = extractSdat(sdatToolPath, ndssndextPath, pathToSdat, parseMethod);
            switch (parseMethod)
            {
                case "m":
                    string[] tempSdatPaths = Directory.GetDirectories(Path.GetDirectoryName(pathToSdat), "SDAT*");
                    extractedSdatPath = Path.Combine(Path.GetDirectoryName(pathToSdat), tempSdatPaths[0]);
                    extractedSseqPath = Path.Combine(extractedSdatPath, "sequence");
                    break;
                case "s":
                    extractedSdatPath = Path.Combine(Path.GetDirectoryName(pathToSdat), Path.GetFileNameWithoutExtension(pathToSdat));
                    extractedSseqPath = Path.Combine(extractedSdatPath, "Seq");
                    break;
            }

            if (processSuccess)
            {
                Directory.CreateDirectory(Path.Combine(pathTo2sf, "text"));

                // Parse SMAP if using SdatTool method
                Smap smapFile = new Smap();

                if (parseMethod.Equals("s"))
                {
                    Console.WriteLine("Parsing SMAP");
                    string[] smapFiles = Directory.GetFiles(extractedSdatPath, Path.GetFileNameWithoutExtension(pathToSdat) + ".smap", SearchOption.TopDirectoryOnly);

                    // parse .smap file
                    try
                    {
                        smapFile = new Smap(smapFiles[0]);

                        // copy smap to output dir
                        File.Copy(smapFiles[0], Path.Combine(Path.Combine(pathTo2sf, "text"),
                                                             Path.GetFileName(smapFiles[0])),
                                                             true);
                    }
                    catch (Exception _e)
                    {
                        Console.WriteLine("-- ERROR PARSING SMAP --");
                        Console.WriteLine(_e.Message);
                        doCleanup(extractedSdatPath, pathTo2sf, psfpointPath);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        smapFile = new Smap(smapPath);
                    }
                    catch (Exception _e)
                    {
                        Console.WriteLine("-- ERROR PARSING SMAP --");
                        Console.WriteLine(_e.Message);
                        doCleanup(extractedSdatPath, pathTo2sf, psfpointPath);
                        return;
                    }
                }

                if (smapFile.SequenceArray.Length > 0)
                {
                    // convert sseq to midi
                    Console.WriteLine("Converting SSEQ to MIDI");
                    processSuccess = convertSseq(sseq2MidPath, extractedSdatPath, pathTo2sf, parseMethod);

                    if (processSuccess)
                    {
                        // time files
                        Console.WriteLine();
                        Console.WriteLine("Timing Files");

                        switch (parseMethod)
                        {
                            case "m":
                                // timeFilesByFileName(psfpointPath, extractedSseqPath, pathTo2sf, filePrefix);
                                timeFilesBySmapFileId(psfpointPath, extractedSseqPath, pathTo2sf,
                                    smapFile, filePrefix);
                                break;
                            case "s":
                                processSuccess = timeFilesBySmap(psfpointPath, extractedSdatPath, pathTo2sf, smapFile, filePrefix);
                                break;
                        }

                        if (!processSuccess)
                        {
                            Console.WriteLine("-- ERROR TIMING FILES --");
                        }
                    }
                    else
                    {
                        Console.WriteLine("-- ERROR CONVERTING SSEQ TO MIDI --");
                    }
                }
            }
            else
            {
                Console.WriteLine("-- ERROR EXTRACTING SDAT --");
            }

            // Cleanup
            Console.WriteLine("Cleaning Up");
            doCleanup(extractedSdatPath, pathTo2sf, psfpointPath);

        }

        private static void usage()
        {
            Console.WriteLine("Usage: 2sftimer.exe -m/s <mini2sf directory> <sdat path> <filename prefix> [smap_path]");
            Console.WriteLine("  -m: use ndssndext.exe method, must include smap path");
            Console.WriteLine("  -s: use sdattool.exe method");
        }

        private static bool extractSdat(string pSdatToolPath, string pNdsSndExtPath, string pSdatPath, string pParseMethod)
        {
            Process ndsProcess;
            bool isSuccess;

            // Extract SDAT
            string arguments = " -x \"" + pSdatPath + "\"";

            try
            {
                ndsProcess = new Process();

                switch (pParseMethod)
                {
                    case "m":
                        ndsProcess.StartInfo = new ProcessStartInfo(pNdsSndExtPath, arguments);
                        break;
                    case "s":
                        ndsProcess.StartInfo = new ProcessStartInfo(pSdatToolPath, arguments);
                        break;
                }

                ndsProcess.StartInfo.CreateNoWindow = true;
                isSuccess = ndsProcess.Start();
                ndsProcess.WaitForExit();
            }
            catch (Exception _e)
            {
                isSuccess = false;
                Console.WriteLine(_e.Message);
            }
            return isSuccess;
        }

        private static bool convertSseq(string pSseq2MidToolPath, string pExtractedSdatPath,
            string pMini2sfPath, string pParseMethod)
        {
            Process ndsProcess;
            bool isSuccess;

            // convert existing sseq to mid            
            string sseqPath = String.Empty;

            switch (pParseMethod)
            {
                case "m":
                    sseqPath = Path.Combine(pExtractedSdatPath, "sequence");
                    break;
                case "s":
                    sseqPath = Path.Combine(pExtractedSdatPath, "Seq");
                    break;
            }

            string sseq2MidDestinationPath = Path.Combine(sseqPath, Path.GetFileName(pSseq2MidToolPath));

            try
            {
                File.Copy(pSseq2MidToolPath, sseq2MidDestinationPath, true);

                string arguments = " -2 -l *.sseq";

                ndsProcess = new Process();

                ndsProcess.StartInfo = new ProcessStartInfo(sseq2MidDestinationPath, arguments);
                ndsProcess.StartInfo.WorkingDirectory = sseqPath;
                ndsProcess.StartInfo.UseShellExecute = false;
                ndsProcess.StartInfo.RedirectStandardOutput = true;
                isSuccess = ndsProcess.Start();
                string sseqOutputFile = ndsProcess.StandardOutput.ReadToEnd();
                ndsProcess.WaitForExit();

                // output redirected standard output
                string sseq2MidOutputPath = Path.Combine(Path.Combine(pMini2sfPath, "text"), SSEQ2MID_TXT);
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

        private static bool timeFilesBySmap(string pPsfPointPath, string pExtractedSdatPath, string pMini2sfPath,
            Smap pSmap, string pFilePrefix)
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

            string unmatchedOrEmptyFolder = Path.Combine(pMini2sfPath, "Unmatched_or_Empty");
            string mini2sfSourcePath;
            string mini2sfDestinationPath;

            // copy psfpoint
            string psfPointDestination = Path.Combine(pMini2sfPath, Path.GetFileName(pPsfPointPath));
            File.Copy(pPsfPointPath, psfPointDestination, true);

            string psfpointBatchFilePath = Path.Combine(Path.Combine(pMini2sfPath, "text"), PSFPOINT_BATCH_TXT);
            TextWriter tw = File.CreateText(psfpointBatchFilePath);

            foreach (Smap.SmapSeqStruct s in pSmap.SequenceArray)
            {

                try
                {
                    string _2sfFileName = pFilePrefix + "-" + s.number.ToString("X4") + ".mini2sf";
                    mini2sfSourcePath = Path.Combine(pMini2sfPath, _2sfFileName);

                    if (!String.IsNullOrEmpty(s.name))
                    {
                        string midiFilePath = Path.Combine(pExtractedSdatPath, s.name + ".mid");

                        if (File.Exists(midiFilePath) && File.Exists(mini2sfSourcePath))
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
                            if (isLoopingTrack(pMini2sfPath, s.name))
                            {
                                arguments = " -fade=\"10\" " + _2sfFileName;
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

                            // Call process                            
                            ndsProcess = new Process();
                            ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
                            ndsProcess.StartInfo.WorkingDirectory = pMini2sfPath;
                            isSuccess = ndsProcess.Start();
                            ndsProcess.WaitForExit();

                            // Add info to batch file
                            tw.WriteLine(Path.GetFileName(psfPointDestination) + arguments);

                            // Update length on mini2sf
                            arguments = " -length=\"" + minutes + ":" + seconds.ToString().PadLeft(2, '0') + "\" " + _2sfFileName;

                            // Call process                            
                            ndsProcess = new Process();
                            ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
                            ndsProcess.StartInfo.WorkingDirectory = pMini2sfPath;
                            isSuccess = ndsProcess.Start();
                            ndsProcess.WaitForExit();

                            // Add info to batch file
                            tw.WriteLine(Path.GetFileName(psfPointDestination) + arguments);

                        }
                        else
                        {
                            // move unmatched mini2sf file
                            mini2sfDestinationPath = Path.Combine(unmatchedOrEmptyFolder, _2sfFileName);

                            if (!Directory.Exists(unmatchedOrEmptyFolder))
                            {
                                Directory.CreateDirectory(unmatchedOrEmptyFolder);
                            }

                            File.Move(mini2sfSourcePath, mini2sfDestinationPath);
                        }
                    }
                    else
                    {
                        // move empty mini2sf file
                        mini2sfDestinationPath = Path.Combine(unmatchedOrEmptyFolder, _2sfFileName);

                        if (!Directory.Exists(unmatchedOrEmptyFolder))
                        {
                            Directory.CreateDirectory(unmatchedOrEmptyFolder);
                        }

                        File.Move(mini2sfSourcePath, mini2sfDestinationPath);
                    }
                }
                catch (Exception _e)
                {
                    isSuccess = false;
                    Console.WriteLine(_e.Message);
                }
            }

            tw.Close();
            tw.Dispose();

            return isSuccess;
        }

        private static bool isLoopingTrack(string pMini2sfPath, string pSequenceName)
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

        private static void doCleanup(string pExtractedSdatPath, string pMini2sfPath, string pPsfPointPath)
        {
            Directory.Delete(pExtractedSdatPath, true);
            File.Delete(Path.Combine(pMini2sfPath, Path.GetFileName(pPsfPointPath)));
        }

        private static bool timeFilesByFileName(string pPsfPointPath, string pExtractedSseqPath,
            string pMini2sfPath, string pFilePrefix)
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

            string unmatchedOrEmptyFolder = Path.Combine(pMini2sfPath, "Unmatched_or_Empty");
            string mini2sfSourcePath;
            string mini2sfDestinationPath;

            // copy psfpoint
            string psfPointDestination = Path.Combine(pMini2sfPath, Path.GetFileName(pPsfPointPath));
            File.Copy(pPsfPointPath, psfPointDestination, true);

            string psfpointBatchFilePath = Path.Combine(Path.Combine(pMini2sfPath, "text"), PSFPOINT_BATCH_TXT);
            TextWriter tw = File.CreateText(psfpointBatchFilePath);

            foreach (string f in Directory.GetFiles(pExtractedSseqPath, "*.mid", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string midiFileName = Path.GetFileNameWithoutExtension(f).Replace(".sseq", String.Empty);
                    string _2sfFileName = pFilePrefix + "-" + midiFileName + ".mini2sf";
                    mini2sfSourcePath = Path.Combine(pMini2sfPath, _2sfFileName);

                    string midiFilePath = f;

                    if (File.Exists(midiFilePath) && File.Exists(mini2sfSourcePath))
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
                        if (isLoopingTrack(pMini2sfPath, Path.GetFileNameWithoutExtension(f)))
                        {
                            arguments = " -fade=\"10\" " + _2sfFileName;
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

                        // Call process                            
                        ndsProcess = new Process();
                        ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
                        ndsProcess.StartInfo.WorkingDirectory = pMini2sfPath;
                        isSuccess = ndsProcess.Start();
                        ndsProcess.WaitForExit();

                        // Add info to batch file
                        tw.WriteLine(Path.GetFileName(psfPointDestination) + arguments);

                        // Update length on mini2sf
                        arguments = " -length=\"" + minutes + ":" + seconds.ToString().PadLeft(2, '0') + "\" " + _2sfFileName;

                        // Call process                            
                        ndsProcess = new Process();
                        ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
                        ndsProcess.StartInfo.WorkingDirectory = pMini2sfPath;
                        isSuccess = ndsProcess.Start();
                        ndsProcess.WaitForExit();

                        // Add info to batch file
                        tw.WriteLine(Path.GetFileName(psfPointDestination) + arguments);

                    }
                    else
                    {
                        // move unmatched mini2sf file
                        mini2sfDestinationPath = Path.Combine(unmatchedOrEmptyFolder, _2sfFileName);

                        if (!Directory.Exists(unmatchedOrEmptyFolder))
                        {
                            Directory.CreateDirectory(unmatchedOrEmptyFolder);
                        }

                        File.Move(mini2sfSourcePath, mini2sfDestinationPath);
                    }

                }
                catch (Exception _e)
                {
                    tw.Close();
                    tw.Dispose();

                    isSuccess = false;
                    Console.WriteLine(_e.Message);
                }
            }

            tw.Close();
            tw.Dispose();

            return isSuccess;
        }

        private static bool timeFilesBySmapFileId(string pPsfPointPath, string pExtractedSdatPath, string pMini2sfPath,
            Smap pSmap, string pFilePrefix)
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

            string unmatchedOrEmptyFolder = Path.Combine(pMini2sfPath, "Unmatched_or_Empty");
            string mini2sfSourcePath;
            string mini2sfDestinationPath;

            // copy psfpoint
            string psfPointDestination = Path.Combine(pMini2sfPath, Path.GetFileName(pPsfPointPath));
            File.Copy(pPsfPointPath, psfPointDestination, true);

            string psfpointBatchFilePath = Path.Combine(Path.Combine(pMini2sfPath, "text"), PSFPOINT_BATCH_TXT);
            TextWriter tw = File.CreateText(psfpointBatchFilePath);

            foreach (Smap.SmapSeqStruct s in pSmap.SequenceArray)
            {
                string sseqName = s.fileID.ToString("X4") + ".sseq";

                try
                {
                    string _2sfFileName = pFilePrefix + "-" + s.number.ToString("X4") + ".mini2sf";
                    mini2sfSourcePath = Path.Combine(pMini2sfPath, _2sfFileName);

                    if (!String.IsNullOrEmpty(s.name))
                    {
                        string midiFilePath = Path.Combine(pExtractedSdatPath, sseqName + ".mid");

                        if (File.Exists(midiFilePath) && File.Exists(mini2sfSourcePath))
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
                            if (isLoopingTrack(pMini2sfPath, sseqName))
                            {
                                arguments = " -fade=\"10\" " + _2sfFileName;
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

                            // Call process                            
                            ndsProcess = new Process();
                            ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
                            ndsProcess.StartInfo.WorkingDirectory = pMini2sfPath;
                            isSuccess = ndsProcess.Start();
                            ndsProcess.WaitForExit();

                            // Add info to batch file
                            tw.WriteLine(Path.GetFileName(psfPointDestination) + arguments);

                            // Update length on mini2sf
                            arguments = " -length=\"" + minutes + ":" + seconds.ToString().PadLeft(2, '0') + "\" " + _2sfFileName;

                            // Call process                            
                            ndsProcess = new Process();
                            ndsProcess.StartInfo = new ProcessStartInfo(Path.GetFileName(psfPointDestination), arguments);
                            ndsProcess.StartInfo.WorkingDirectory = pMini2sfPath;
                            isSuccess = ndsProcess.Start();
                            ndsProcess.WaitForExit();

                            // Add info to batch file
                            tw.WriteLine(Path.GetFileName(psfPointDestination) + arguments);

                        }
                        else
                        {
                            // move unmatched mini2sf file
                            mini2sfDestinationPath = Path.Combine(unmatchedOrEmptyFolder, _2sfFileName);

                            if (!Directory.Exists(unmatchedOrEmptyFolder))
                            {
                                Directory.CreateDirectory(unmatchedOrEmptyFolder);
                            }

                            File.Move(mini2sfSourcePath, mini2sfDestinationPath);
                        }
                    }
                    else
                    {
                        // move empty mini2sf file
                        mini2sfDestinationPath = Path.Combine(unmatchedOrEmptyFolder, _2sfFileName);

                        if (!Directory.Exists(unmatchedOrEmptyFolder))
                        {
                            Directory.CreateDirectory(unmatchedOrEmptyFolder);
                        }

                        File.Move(mini2sfSourcePath, mini2sfDestinationPath);
                    }
                }
                catch (Exception _e)
                {
                    isSuccess = false;
                    Console.WriteLine(_e.Message);
                }
            }

            tw.Close();
            tw.Dispose();

            return isSuccess;
        }
    }
}
