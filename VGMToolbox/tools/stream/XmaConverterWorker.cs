using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using VGMToolbox.plugin;
using VGMToolbox.util;


namespace VGMToolbox.tools.stream
{
    public class XmaConverterWorker: AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        // xma parse
        public const string XMAPARSE_PATH = "external\\xma\\xma_test.exe";
        public const string XMAPARSE_CRC32 = "7437EE24"; // v0.8
        public static readonly string XMAPARSE_FULL_PATH =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            XMAPARSE_PATH);

        // to wav
        public const string TOWAV_PATH = "external\\xma\\ToWav.exe";
        public static readonly string TOWAV_FULL_PATH =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            TOWAV_PATH);

        // paths
        private const string WORKING_FOLDER_NAME = "_vgmt_xma_convert";

        private const string XMAPARSE_OUTPUT_EXTENSION = ".xmpo";
        private const string RIFF_COPY_OUTPUT_EXTENSION = ".xmariff";
        private const string TOWAV_OUTPUT_EXTENSION = ".wav";

        // combo items
        public const string RIFF_CHANNELS_1 = "Mono (1)";
        public const string RIFF_CHANNELS_2 = "Stereo (2+)";

        // RIFF header
        public static readonly byte[] RIFF_HEADER_XMA = new byte[] { 
            0x52, 0x49, 0x46, 0x46, 0xB8, 0x59, 0xA7, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20,
            0x20, 0x00, 0x00, 0x00, 0x65, 0x01, 0x10, 0x00, 0xD6, 0x10, 0x00, 0x00, 0x01, 0x00, 0x00, 0x03, 
            0xE3, 0x9A, 0x00, 0x00, 0x80, 0xBB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x02, 0x02, 0x00, 0x64, 0x61, 0x74, 0x61, 0x00, 0x58, 0xA7, 0x00};

        public struct XmaConverterStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
            public string OutputFolder { set; get; }

            public bool ShowExeOutput { set; get; }

            public string XmaParseXmaType { set; get; }
            public string XmaParseStartOffset { set; get; }
            public string XmaParseBlockSize { set; get; }

            public string RiffFrequency { set; get; }
            public string RiffChannelCount { set; get; }
        }

        public XmaConverterWorker() : 
            base() 
        {
            this.progressCounterIncrementer = 10;
        }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pXmaConverterStruct,
            DoWorkEventArgs e)
        {
            XmaConverterStruct taskStruct = (XmaConverterStruct)pXmaConverterStruct;
            string workingFolder;
            string workingFile;

            string consoleOutput;
            string consoleError;

            string xmaParseOutputFilePath;
            string riffHeaderedFile;
            string toWavOutputFilePath;

            uint riffFrequency;
            uint riffChannelCount;
            uint riffFileSize;

            byte[] riffHeaderBytes;

            //------------------
            // output file name
            //------------------
            this.progressStruct.Clear();
            this.progressStruct.FileName = pPath;
            this.progressStruct.GenericMessage = String.Format("[{0}]{1}", Path.GetFileName(pPath), Environment.NewLine);
            ReportProgress(Constants.ProgressMessageOnly, progressStruct);

            //----------------------
            // build working folder
            //----------------------
            workingFolder = this.createWorkingFolder(pPath, taskStruct);

            //-----------------------------
            // copy file to working folder
            //-----------------------------
            workingFile = Path.Combine(workingFolder, Path.GetFileName(pPath));
            File.Copy(pPath, workingFile, true);

            //---------------------------
            // xma_parse.exe
            //---------------------------            
            this.progressStruct.Clear();
            this.progressStruct.FileName = pPath;
            this.progressStruct.GenericMessage = String.Format("---- calling xma_parse.exe{0}", Environment.NewLine);
            ReportProgress(Constants.ProgressMessageOnly, progressStruct);
            
            xmaParseOutputFilePath = this.callXmaParse(workingFolder, workingFile, taskStruct, out consoleOutput, out consoleError);

            // show output
            if (taskStruct.ShowExeOutput && !String.IsNullOrEmpty(consoleOutput))
            {
                showOutput(pPath, consoleOutput, false);
            }

            if (String.IsNullOrEmpty(consoleError))
            {
                //-----------------
                // get RIFF header
                //-----------------
                this.progressStruct.Clear();
                this.progressStruct.FileName = pPath;
                this.progressStruct.GenericMessage = String.Format("---- adding RIFF header.{0}", Environment.NewLine);
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                riffFrequency = (uint)ByteConversion.GetLongValueFromString(taskStruct.RiffFrequency);
                riffChannelCount = (uint)ByteConversion.GetLongValueFromString(taskStruct.RiffChannelCount);
                riffFileSize = (uint)new FileInfo(xmaParseOutputFilePath).Length;

                riffHeaderBytes = this.getRiffHeader(riffFrequency, riffChannelCount, riffFileSize);

                //-------------------------
                // add RIFF header to file
                //-------------------------
                riffHeaderedFile = Path.ChangeExtension(xmaParseOutputFilePath, RIFF_COPY_OUTPUT_EXTENSION);
                FileUtil.AddHeaderToFile(riffHeaderBytes, xmaParseOutputFilePath, riffHeaderedFile);

                //-----------
                // ToWav.exe
                //-----------
                this.progressStruct.Clear();
                this.progressStruct.FileName = pPath;
                this.progressStruct.GenericMessage = String.Format("---- calling ToWav.exe{0}", Environment.NewLine);
                ReportProgress(Constants.ProgressMessageOnly, progressStruct);

                toWavOutputFilePath = this.callToWav(workingFolder, riffHeaderedFile, taskStruct, out consoleOutput, out consoleError);

                // show output
                if (taskStruct.ShowExeOutput && !String.IsNullOrEmpty(consoleOutput))
                {
                    showOutput(pPath, consoleOutput, false);
                }

                // dispay ToWav.exe error
                if (!String.IsNullOrEmpty(consoleError))
                {                    
                    showOutput(pPath, consoleError, true);
                }
            }
            else
            { 
                // dispay xma_parse.exe error
                showOutput(pPath, consoleError, true);
            }

            //----------------------
            // clean working folder
            //----------------------
            this.cleanWorkingFolder(pPath, workingFile, taskStruct);
           
        }

        //--------------------------
        // Working Folder functions
        //--------------------------
        private string getWorkingFolderPath(string processingFilePath, XmaConverterStruct taskStruct)
        {
            string workingFolder;

            if (String.IsNullOrEmpty(taskStruct.OutputFolder))
            {
                workingFolder = Path.Combine(Path.GetDirectoryName(processingFilePath), WORKING_FOLDER_NAME);
            }
            else
            {
                workingFolder = taskStruct.OutputFolder;
            }

            return workingFolder;
        }
        
        private string createWorkingFolder(string processingFilePath, XmaConverterStruct taskStruct)
        {
            string workingFolder = getWorkingFolderPath(processingFilePath, taskStruct);
            
            if (!Directory.Exists(workingFolder))
            {
                Directory.CreateDirectory(workingFolder);
            }

            return workingFolder;
        }

        private void cleanWorkingFolder(string processingFilePath, string workingFile, XmaConverterStruct taskStruct)
        {
            string workingFolder = getWorkingFolderPath(processingFilePath, taskStruct);

            // delete working copy of source file
            if (File.Exists(workingFile))
            {
                File.Delete(workingFile);
            }

            // delete xma_parse output file(s)
            foreach (string xmpo in Directory.GetFiles(workingFolder, String.Format("*{0}", XMAPARSE_OUTPUT_EXTENSION)))
            {
                File.Delete(xmpo);
            }

            // delete riff output file(s)
            foreach (string riff in Directory.GetFiles(workingFolder, String.Format("*{0}", RIFF_COPY_OUTPUT_EXTENSION)))
            {
                File.Delete(riff);
            }

            // delete xma_test.exe
            string xmaParseWorkingPath = Path.Combine(workingFolder, Path.GetFileName(XMAPARSE_FULL_PATH));

            if (File.Exists(xmaParseWorkingPath))
            {
                File.Delete(xmaParseWorkingPath);
            }

            // delete towav.exe
            string toWavWorkingPath = Path.Combine(workingFolder, Path.GetFileName(TOWAV_FULL_PATH));

            if (File.Exists(toWavWorkingPath))
            {
                File.Delete(toWavWorkingPath);
            }
        }        

        //---------------
        // External Apps
        //---------------
        private string callXmaParse(
            string workingFolder, 
            string workingFile, 
            XmaConverterStruct taskStruct,
            out string consoleOutput, 
            out string consoleError)
        {
            string xmaParseWorkingPath;
            string xmaParseOutputFilePath;
            Process xmaParseProcess;
            StringBuilder parameters = new StringBuilder();

            // copy to working folder
            xmaParseWorkingPath = Path.Combine(workingFolder, Path.GetFileName(XMAPARSE_FULL_PATH));
            File.Copy(XMAPARSE_FULL_PATH, xmaParseWorkingPath, true);

            // build parameters
            parameters.AppendFormat(" \"{0}\"", Path.GetFileName(workingFile)); // Filename
            parameters.AppendFormat(" -{0}", taskStruct.XmaParseXmaType); // Input File Type

            // offset
            if (!String.IsNullOrEmpty(taskStruct.XmaParseStartOffset))
            {
                // allow decimal or hex input, convert to hex for xma_parse.exe
                parameters.AppendFormat(" -o {0}", ByteConversion.GetLongValueFromString(taskStruct.XmaParseStartOffset).ToString("X"));
            }

            // block size
            if (!String.IsNullOrEmpty(taskStruct.XmaParseBlockSize))
            {
                // allow decimal or hex input, convert to hex for xma_parse.exe
                parameters.AppendFormat(" -b {0}", ByteConversion.GetLongValueFromString(taskStruct.XmaParseBlockSize).ToString("X"));
            }

            // output name
            xmaParseOutputFilePath = String.Format("{0}{1}", Path.GetFileNameWithoutExtension(workingFile), XMAPARSE_OUTPUT_EXTENSION);
            parameters.AppendFormat(" -x \"{0}\"", xmaParseOutputFilePath);

            // call function
            xmaParseProcess = new Process();
            xmaParseProcess.StartInfo = new ProcessStartInfo(xmaParseWorkingPath);
            xmaParseProcess.StartInfo.WorkingDirectory = workingFolder;
            xmaParseProcess.StartInfo.Arguments = parameters.ToString();
            xmaParseProcess.StartInfo.UseShellExecute = false;
            xmaParseProcess.StartInfo.CreateNoWindow = true;

            xmaParseProcess.StartInfo.RedirectStandardError = true;
            xmaParseProcess.StartInfo.RedirectStandardOutput = true;

            bool isSuccess = xmaParseProcess.Start();
            consoleOutput = xmaParseProcess.StandardOutput.ReadToEnd();
            consoleError = xmaParseProcess.StandardError.ReadToEnd();

            xmaParseProcess.WaitForExit();
            xmaParseProcess.Close();
            xmaParseProcess.Dispose();

            xmaParseOutputFilePath = xmaParseWorkingPath = Path.Combine(workingFolder, xmaParseOutputFilePath);

            return xmaParseOutputFilePath;
        }

        private string callToWav(
            string workingFolder,
            string riffHeaderedFile,
            XmaConverterStruct taskStruct,
            out string consoleOutput,
            out string consoleError)
        {
            string toWavWorkingPath;
            string toWavOutputFilePath;
            Process toWavProcess;
            StringBuilder parameters = new StringBuilder();

            // copy to working folder
            toWavWorkingPath = Path.Combine(workingFolder, Path.GetFileName(TOWAV_FULL_PATH));
            File.Copy(TOWAV_FULL_PATH, toWavWorkingPath, true);

            // build parameters
            parameters.AppendFormat(" \"{0}\"", Path.GetFileName(riffHeaderedFile)); // Filename

            // call function
            toWavProcess = new Process();
            toWavProcess.StartInfo = new ProcessStartInfo(toWavWorkingPath);
            toWavProcess.StartInfo.WorkingDirectory = workingFolder;
            toWavProcess.StartInfo.Arguments = parameters.ToString();
            toWavProcess.StartInfo.UseShellExecute = false;
            toWavProcess.StartInfo.CreateNoWindow = true;

            toWavProcess.StartInfo.RedirectStandardError = true;
            toWavProcess.StartInfo.RedirectStandardOutput = true;

            bool isSuccess = toWavProcess.Start();
            consoleOutput = toWavProcess.StandardOutput.ReadToEnd();
            consoleError = toWavProcess.StandardError.ReadToEnd();

            toWavProcess.WaitForExit();
            toWavProcess.Close();
            toWavProcess.Dispose();

            // build output path
            toWavOutputFilePath = Path.ChangeExtension(riffHeaderedFile, TOWAV_OUTPUT_EXTENSION);

            return toWavOutputFilePath;
        }

        //----------------
        // RIFF functions
        //----------------
        private byte[] getRiffHeader(uint frequency, uint channelCount, uint fileSize)
        {
            byte[] riffHeader = new byte[RIFF_HEADER_XMA.Length];
            Array.Copy(RIFF_HEADER_XMA, riffHeader, RIFF_HEADER_XMA.Length);

            // file size
            Array.Copy(BitConverter.GetBytes(fileSize - 8), 0, riffHeader, 4, 4);

            // frequency
            Array.Copy(BitConverter.GetBytes(frequency), 0, riffHeader, 0x24, 4);

            // channels
            riffHeader[0x31] = (byte)channelCount;

            return riffHeader;
        }
        
        //----------------
        // output/display
        //----------------
        private void showOutput(string path, string message, bool isError)
        {
            this.progressStruct.Clear();
            this.progressStruct.FileName = path;

            if (isError)
            {
                this.progressStruct.ErrorMessage = String.Format("ERROR: {0}{1}", message, Environment.NewLine);
            }
            else
            {
                this.progressStruct.GenericMessage = String.Format("{0}{1}", message, Environment.NewLine);
            }
            
            ReportProgress(this.progress, progressStruct);        
        }
    }
}
