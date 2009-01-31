using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class Bin2PsfWorker : BackgroundWorker
    {
        private const uint MIN_TEXT_SECTION_OFFSET = 0x80010000;
        private const uint PC_OFFSET_CORRECTION = 0x8000F800;
        
        private int fileCount = 0;
        private int maxFiles = 0;
        Constants.ProgressStruct progressStruct;

        public struct Bin2PsfStruct
        {
            public string[] sourcePaths;
            public string dataOffset;
        }

        public Bin2PsfWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void makePsfs(Bin2PsfStruct pBin2PsfStruct, DoWorkEventArgs e)
        {
            maxFiles = FileUtil.GetFileCount(pBin2PsfStruct.sourcePaths);

            foreach (string path in pBin2PsfStruct.sourcePaths)
            {
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.makePsfFromFile(path, pBin2PsfStruct, e);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.makePsfsFromDirectory(path, pBin2PsfStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }            
            return;
        }

        private void makePsfFromFile(string pPath, Bin2PsfStruct pBin2PsfStruct, DoWorkEventArgs e)
        {
            byte[] textSectionOffset;
            long textSectionOffsetValue;
            long pcOffset;
            
            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            progressStruct.Clear();
            progressStruct.filename = pPath;
            ReportProgress(progress, progressStruct);

            using (FileStream fs = File.OpenRead(pPath))
            {
                textSectionOffset = ParseFile.parseSimpleOffset(fs, 0x18, 4);
                textSectionOffsetValue = BitConverter.ToUInt32(textSectionOffset, 0);

                pcOffset = textSectionOffsetValue - MIN_TEXT_SECTION_OFFSET +
                    VGMToolbox.util.Encoding.GetIntFromString(pBin2PsfStruct.dataOffset) - 
                    PC_OFFSET_CORRECTION;

                progressStruct.Clear();
                progressStruct.genericMessage = String.Format("<{0}> [{1}]", pPath, pcOffset.ToString("X16"));
                ReportProgress(Constants.PROGRESS_MSG_ONLY, progressStruct);
            }
        }    

        private void makePsfsFromDirectory(string pPath, Bin2PsfStruct pBin2PsfStruct, DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.makePsfsFromDirectory(d, pBin2PsfStruct, e);
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
                    this.makePsfFromFile(f, pBin2PsfStruct, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Bin2PsfStruct bin2PsfStruct = (Bin2PsfStruct)e.Argument;
            this.makePsfs(bin2PsfStruct, e);
        }            
    }
}
