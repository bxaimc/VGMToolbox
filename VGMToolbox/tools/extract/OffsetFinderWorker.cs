using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox.tools.extract
{
    class OffsetFinderWorker : BackgroundWorker
    {
        public const string LITTLE_ENDIAN = "Little Endian";
        public const string BIG_ENDIAN = "Big Endian";
        
        private int fileCount = 0;
        private int maxFiles = 0;
        private Constants.ProgressStruct progressStruct;

        public struct OffsetFinderStruct
        {
            public string[] sourcePaths;
            public string searchString;
            public bool treatSearchStringAsHex;

            public bool cutFile;
            public string searchStringOffset;            
            public string cutSize;
            public string cutSizeOffsetSize;
            public bool isCutSizeAnOffset;
            public string outputFileExtension;
            public bool isLittleEndian;
        }

        public OffsetFinderWorker()
        {
            fileCount = 0;
            maxFiles = 0;
            progressStruct = new Constants.ProgressStruct();
            
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }
        
        private void findOffsets(OffsetFinderStruct pOffsetFinderStruct, 
            DoWorkEventArgs e)
        {
            this.maxFiles = FileUtil.GetFileCount(pOffsetFinderStruct.sourcePaths);

            foreach (string path in pOffsetFinderStruct.sourcePaths)
            {                
                if (File.Exists(path))
                {
                    if (!CancellationPending)
                    {
                        this.findOffsetsInFile(path, pOffsetFinderStruct, e);
                    }
                    else 
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (Directory.Exists(path))
                {
                    this.findOffsetsInDirectory(path, pOffsetFinderStruct, e);

                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;                        
                    }
                }                               
            }

            return;
        }

        private void findOffsetsInDirectory(string pPath, OffsetFinderStruct pOffsetFinderStruct, 
            DoWorkEventArgs e)
        {
            foreach (string d in Directory.GetDirectories(pPath))
            {
                if (!CancellationPending)
                {
                    this.findOffsetsInDirectory(d, pOffsetFinderStruct, e);
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
                    this.findOffsetsInFile(f, pOffsetFinderStruct, e);
                }
                else
                {
                    e.Cancel = true;
                    break;
                }
            }                
        }

        private void findOffsetsInFile(string pPath, OffsetFinderStruct pOffsetFinderStruct,
            DoWorkEventArgs e)
        {
            int i;
            int j = 0;
            byte[] searchBytes;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            long cutStart;
            long cutSize;
            long cutSizeOffset;
            byte[] cutSizeBytes;
            
            long previousPosition;
            string outputFolder;
            string outputFile;
            int chunkCount = 0;

            long offset;
            long previousOffset;

            bool skipCut;

            // Report Progress
            int progress = (++fileCount * 100) / maxFiles;
            this.progressStruct.Clear();
            this.progressStruct.filename = pPath;
            ReportProgress(progress, this.progressStruct);

            try
            {
                if (pOffsetFinderStruct.treatSearchStringAsHex)
                {
                    searchBytes = new byte[pOffsetFinderStruct.searchString.Length / 2];

                    // convert the search string to bytes
                    for (i = 0; i < pOffsetFinderStruct.searchString.Length; i += 2)
                    {
                        searchBytes[j] = BitConverter.GetBytes(Int16.Parse(pOffsetFinderStruct.searchString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier))[0];
                        j++;
                    }
                }
                else
                {
                    searchBytes = enc.GetBytes(pOffsetFinderStruct.searchString);
                }

                FileInfo fi = new FileInfo(pPath);
                                
                using (FileStream fs = File.Open(Path.GetFullPath(pPath), FileMode.Open, FileAccess.Read))
                {
                    this.progressStruct.Clear();
                    this.progressStruct.genericMessage = String.Format("[{0}]", pPath) + Environment.NewLine;
                    ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
                    
                    previousOffset = 0;
                    outputFolder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(pPath),
                        Path.GetFileNameWithoutExtension(pPath) + "_CUT"));

                    while ((offset = ParseFile.GetNextOffset(fs, previousOffset, searchBytes)) != -1)
                    {
                        if (pOffsetFinderStruct.cutFile)
                        {
                            skipCut = false;
                            
                            cutStart = offset - VGMToolbox.util.Encoding.GetIntFromString(pOffsetFinderStruct.searchStringOffset);

                            if (pOffsetFinderStruct.isCutSizeAnOffset)
                            {
                                cutSizeOffset = cutStart + VGMToolbox.util.Encoding.GetIntFromString(pOffsetFinderStruct.cutSize);                                
                                previousPosition = fs.Position;
                                cutSizeBytes = ParseFile.parseSimpleOffset(fs, cutSizeOffset, 
                                    (int) VGMToolbox.util.Encoding.GetIntFromString(pOffsetFinderStruct.cutSizeOffsetSize));
                                fs.Position = previousPosition;

                                if (!pOffsetFinderStruct.isLittleEndian)
                                {
                                    Array.Reverse(cutSizeBytes);
                                }

                                switch (cutSizeBytes.Length)
                                { 
                                    case 1:
                                        cutSize = cutSizeBytes[0];
                                        break;
                                    case 2:
                                        cutSize = BitConverter.ToInt16(cutSizeBytes, 0);
                                        break;
                                    case 4:
                                        cutSize = BitConverter.ToInt32(cutSizeBytes, 0);
                                        break;
                                    default:
                                        cutSize = 0;
                                        break;
                                }                                
                            }
                            else
                            {
                                cutSize = VGMToolbox.util.Encoding.GetIntFromString(pOffsetFinderStruct.cutSize);
                            }

                            outputFile = String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(pPath), chunkCount.ToString("X8"), pOffsetFinderStruct.outputFileExtension);

                            if (cutStart < 0)
                            {
                                this.progressStruct.Clear();
                                this.progressStruct.errorMessage = String.Format("  Warning: For string found at: 0x{0}, cut begin is less than 0 ({1})...Skipping",
                                    offset.ToString("X8"), cutStart.ToString("X8")) + Environment.NewLine;
                                ReportProgress(progress, this.progressStruct);

                                skipCut = true;
                            }
                            else if (cutSize < 0)
                            {
                                this.progressStruct.Clear();
                                this.progressStruct.errorMessage = String.Format("  Warning: For string found at: 0x{0}, cut size is less than 0 ({1})...Skipping",
                                    offset.ToString("X8"), cutSize.ToString("X8")) + Environment.NewLine;
                                ReportProgress(progress, this.progressStruct);

                                skipCut = true;
                            }
                            else if ((cutStart + cutSize) > fi.Length)
                            {
                                this.progressStruct.Clear();
                                this.progressStruct.errorMessage = String.Format("  Warning: For string found at: 0x{0}, total file end will go past the end of the file ({1})",
                                    offset.ToString("X8"), (cutStart + cutSize).ToString("X8")) + Environment.NewLine;
                                ReportProgress(progress, this.progressStruct);

                            }

                            if (skipCut)
                            {
                                previousOffset = offset + 1;
                            }
                            else
                            {
                                ParseFile.ExtractChunkToFile(fs, cutStart, (int)cutSize, Path.Combine(outputFolder, outputFile));

                                this.progressStruct.Clear();
                                this.progressStruct.genericMessage = String.Format("  Extracted [{3}] begining at 0x{0}, for string found at: 0x{1}, with size 0x{2}",
                                    cutStart.ToString("X8"), offset.ToString("X8"), cutSize.ToString("X8"), outputFile) + Environment.NewLine;
                                ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);

                                previousOffset = offset + cutSize;
                                chunkCount++;
                            }
                        }
                        else
                        {
                            // just report the offset
                            this.progressStruct.Clear();
                            this.progressStruct.genericMessage = String.Format("  String found at: 0x{0}", offset.ToString("X8")) + Environment.NewLine;
                            ReportProgress(Constants.PROGRESS_MSG_ONLY, this.progressStruct);
                            
                            previousOffset = offset + searchBytes.Length;
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                this.progressStruct.Clear();
                this.progressStruct.errorMessage = String.Format("Error processing <{0}>.  Error received: ", pPath) + ex.Message;
                ReportProgress(progress, this.progressStruct);
            }            
        }    

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            OffsetFinderStruct offsetFinderStruct = (OffsetFinderStruct)e.Argument;

            if (!String.IsNullOrEmpty(offsetFinderStruct.outputFileExtension) &&
                !offsetFinderStruct.outputFileExtension.StartsWith("."))
            {
                offsetFinderStruct.outputFileExtension = "." + offsetFinderStruct.outputFileExtension;
            }

            this.findOffsets(offsetFinderStruct, e);
        }    
    }
}
