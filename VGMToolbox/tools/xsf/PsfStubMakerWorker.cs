using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{        
    public struct PsfStubMakerStruct : IVgmtWorkerStruct
    {
        private string[] sourcePaths;
        public string[] SourcePaths
        {
            get { return sourcePaths; }
            set { sourcePaths = value; }
        }
    }
    
    class PsfStubMakerWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public static readonly string PsfToolFolderPath =
            Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "external"), "psf"));

        public static readonly string SigFindFilePath = Path.Combine(PsfToolFolderPath, "sigfind.exe");
        public static readonly string SigFind2FilePath = Path.Combine(PsfToolFolderPath, "sigfind2.exe");

        public static readonly string PsfOCycleSourceCodeFilePath = Path.Combine(PsfToolFolderPath, "psfdrv.c");
        public static readonly string PsfOCycleMakeFilePath = Path.Combine(PsfToolFolderPath, "mk.bat");

        public static readonly string WorkingFolderPath =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "working_psf"));         

        public PsfStubMakerWorker() : base() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPsfStubMakerStruct, DoWorkEventArgs e)
        {
            string sigFindDestination;
            string driverDestination;

            string arguments;
            string standardOutput;
            string standardError;
            bool isProcessSuccessful;
                        
            PsfPsyQAddresses sigFindAddresses;

            // create working folder
            if (!Directory.Exists(PsfStubMakerWorker.WorkingFolderPath))
            {
                Directory.CreateDirectory(PsfStubMakerWorker.WorkingFolderPath);
            }

            ////////////////////
            // call sigfind.exe
            ////////////////////
            sigFindDestination = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, Path.GetFileName(PsfStubMakerWorker.SigFindFilePath));
            driverDestination = Path.Combine(PsfStubMakerWorker.WorkingFolderPath, Path.GetFileName(pPath).Replace(" ", String.Empty));
            
            File.Copy(PsfStubMakerWorker.SigFindFilePath, sigFindDestination, true);
            File.Copy(pPath, driverDestination, true);
            arguments = String.Format(" {0}", Path.GetFileName(driverDestination));
            
            isProcessSuccessful = FileUtil.ExecuteExternalProgram(sigFindDestination, arguments,
                PsfStubMakerWorker.WorkingFolderPath, out standardOutput, out standardError);

            /////////////////////
            // get psyQAddresses
            /////////////////////
            if ((isProcessSuccessful) && (!String.IsNullOrEmpty(standardError)) && 
                (!standardOutput.Contains("ERROR")))
            {
                sigFindAddresses = 
                    XsfUtil.GetSigFindItems(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(standardOutput)));
            
            }


            // call sigfind2
            

            // compile stub
        }
    }
}
