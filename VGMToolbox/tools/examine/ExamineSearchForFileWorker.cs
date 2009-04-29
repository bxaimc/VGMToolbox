using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.examine
{
    class ExamineSearchForFileWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {

        public struct ExamineSearchForFileStruct : IVgmtWorkerStruct
        {
            public bool ExtractFile;
            public bool CaseSensitive;
            public string SearchString;
            public string OutputFolder;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public ExamineSearchForFileWorker() : base() { }

        protected override void DoTaskForFile(string pPath,
            IVgmtWorkerStruct pExamineSearchForFileStruct, DoWorkEventArgs e)
        {
            ExamineSearchForFileStruct examineSearchForFileStruct =
                (ExamineSearchForFileStruct)pExamineSearchForFileStruct;

            StringBuilder filePaths = new StringBuilder();
            string[] compressedFilePaths;
            string searchString;

            if (examineSearchForFileStruct.CaseSensitive)
            {
                searchString = examineSearchForFileStruct.SearchString;
            }
            else
            {
                searchString = examineSearchForFileStruct.SearchString.ToUpper();
            }

            if ((examineSearchForFileStruct.CaseSensitive && 
                Path.GetFileName(pPath).Contains(searchString)) ||
                (!examineSearchForFileStruct.CaseSensitive &&
                Path.GetFileName(pPath).ToUpper().Contains(searchString)))
            {
                filePaths.Append(pPath);
                filePaths.Append(Environment.NewLine);
            }

            compressedFilePaths = CompressionUtil.GetFileList(pPath);
            if (compressedFilePaths != null)
            {
                foreach (string f in compressedFilePaths)
                {
                    if ((examineSearchForFileStruct.CaseSensitive && 
                        f.Contains(searchString)) ||
                        (!examineSearchForFileStruct.CaseSensitive &&
                        f.ToUpper().Contains(searchString)))
                    {
                        filePaths.AppendFormat("{0} ({1})", pPath, f);
                        filePaths.Append(Environment.NewLine);

                        if (examineSearchForFileStruct.ExtractFile)
                        {
                            CompressionUtil.ExtractFileFromArchive(pPath, f, examineSearchForFileStruct.OutputFolder);
                        }
                    }
                }
            }

            this.progressStruct.Clear();
            progressStruct.GenericMessage = filePaths.ToString();
            ReportProgress(this.Progress, progressStruct);
        }
    }        
}
