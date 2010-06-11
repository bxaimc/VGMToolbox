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
        public const string XMAPARSE_PATH = "external\\xma\\xma_test.exe";
        public const string XMAPARSE_CRC32 = "7437EE24"; // v0.8
        public static readonly string XMAPARSE_FULL_PATH =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            XMAPARSE_PATH);

        public const string TOWAV_PATH = "external\\xma\\ToWav.exe";
        public static readonly string TOWAV_FULL_PATH =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            TOWAV_PATH);


        private const string WORKING_FOLDER_NAME = "_vgmt_xma_convert";

        private const string XMAPARSE_OUTPUT_EXTENSION = ".xmpo";
        private const string RIFF_COPY_OUTPUT_EXTENSION = ".xmariff";


        public struct XmaConverterStruct : IVgmtWorkerStruct
        {
            public string[] SourcePaths { set; get; }
            public string OutputFolder { set; get; }

            public string XmaParseXmaType { set; get; }
            public string XmaParseStartOffset { set; get; }
            public string XmaParseBlockSize { set; get; }
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
            xmaParseOutputFilePath = this.callXmaParse(workingFolder, workingFile, taskStruct, out consoleOutput, out consoleError);

            //-----------------
            // get RIFF header
            //-----------------
            riffHeaderBytes = this.getRiffHeader();

            //-------------------------
            // add RIFF header to file
            //-------------------------
            FileUtil.AddHeaderToFile(riffHeaderBytes, xmaParseOutputFilePath, Path.ChangeExtension(xmaParseOutputFilePath, RIFF_COPY_OUTPUT_EXTENSION)); 

            //-----------
            // ToWav.exe
            //-----------

            //----------------------
            // clean working folder
            //----------------------
            this.cleanWorkingFolder(pPath, taskStruct);
           
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

        private void cleanWorkingFolder(string processingFilePath, XmaConverterStruct taskStruct)
        {
            string workingFolder = getWorkingFolderPath(processingFilePath, taskStruct);

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

            return xmaParseOutputFilePath;
        }

        //----------------
        // RIFF functions
        //----------------
        private byte[] getRiffHeader()
        {
            byte[] riffHeader = new byte[0];

            return riffHeader;
        }
    }
}
