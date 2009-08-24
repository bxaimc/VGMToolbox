using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class PsfDataFinderWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public static readonly byte[] VB_START_BYTES = new byte[] { 0x00, 0x00, 0x00, 0x00,
                                                                    0x00, 0x00, 0x00, 0x00,
                                                                    0x00, 0x00, 0x00, 0x00,
                                                                    0x00, 0x00, 0x00, 0x00};

        public static readonly byte[] VB_END_BYTES_1 = new byte[] { 0x00, 0x07, 0x07, 0x07,
                                                                  0x07, 0x07, 0x07, 0x07, 
                                                                  0x07, 0x07, 0x07, 0x07, 
                                                                  0x07, 0x07, 0x07, 0x07};
        
        public static readonly byte[] VB_END_BYTES_2 = new byte[] { 0x00, 0x07, 0x77, 0x77,
                                                                  0x77, 0x77, 0x77, 0x77, 
                                                                  0x77, 0x77, 0x77, 0x77,
                                                                  0x77, 0x77, 0x77, 0x77};

        private static int ADPCM_ROW_COUNT = 10;


        public struct PsfDataFinderStruct : IVgmtWorkerStruct
        {
            public bool UseSeqMinimumSize;
            public int MinimumSize;
            public bool ReorderSeqFiles;
            public bool UseZeroOffsetForVb;
            public bool RelaxVbEofRestrictions;

            private string[] sourcePaths;
            public string[] SourcePaths
            {
                get { return sourcePaths; }
                set { sourcePaths = value; }
            }
        }

        public struct VhStruct
        {
            public UInt16 vhProgramCount;
            public UInt16 vbSampleCount;
            public uint[] vbSampleSizes;
            public long offsetTableOffset;

            public string FileName;
            public long startingOffset;
            public long length;
            public long vabSize;

            public long vbStartingOffset;
            public long vbLength;
            public long expectedVbLength;
        }

        public struct ProbableVbStruct
        {
            public long offset;
            public uint length;
        }
        public struct ProbableSeqStruct
        {
            public long offset;
            public int length;
        }

        public PsfDataFinderWorker() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPsfDataFinderStruct, DoWorkEventArgs e)
        {
            PsfDataFinderStruct psfStruct = (PsfDataFinderStruct)pPsfDataFinderStruct;

            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = String.Format("[{0}]{1}", pPath, Environment.NewLine);
            this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

            long offset;

            long seqEof;
            string seqName;
            int seqNumber = 0;
            int seqLength;
            ProbableSeqStruct seqEntry;
            ArrayList seqFiles = new ArrayList();
            bool seqNamingMessageDisplayed = false;

            int vhNumber = 0;
            int minSampleSize = -1;
            int minRowLength;

            VhStruct vhObject;
            ArrayList vhArrayList = new ArrayList();

            ArrayList emptyRowList = new ArrayList();
            ProbableVbStruct potentialVb;
            ProbableVbStruct[] potentialVbList;
            byte[] vbRow = new byte[0x10];

            // improve algorithm later
            using (FileStream fs = File.OpenRead(pPath))
            {
                string destinationFolder = Path.Combine(Path.GetDirectoryName(pPath), Path.GetFileNameWithoutExtension(pPath));
                
                // get VH files
                #region VH EXTRACT
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting VH{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                offset = 0;

                while ((offset = ParseFile.GetNextOffset(fs, offset, XsfUtil.VAB_SIGNATURE)) > -1)
                {
                    vhObject = new VhStruct();
                    vhObject.FileName = String.Format("{0}_{1}.VH", Path.GetFileNameWithoutExtension(pPath), vhNumber++.ToString("X4"));
                    vhObject.startingOffset = offset;
                    vhObject.vhProgramCount = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, (offset + 0x12), 2), 0);
                    vhObject.vbSampleCount = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, (offset + 0x16), 2), 0);
                    vhObject.vbSampleSizes = new uint[vhObject.vbSampleCount];
                    vhObject.length = 2592 + (512 * vhObject.vhProgramCount);

                    vhObject.vabSize = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0xC, 4), 0);
                    vhObject.expectedVbLength = vhObject.vabSize - vhObject.length;

                    vhObject.offsetTableOffset = offset + (512 * vhObject.vhProgramCount) + 2080;
                    vhObject.offsetTableOffset += 2; // not sure but seems to be needed

                    for (int i = 0; i < vhObject.vbSampleCount; i++)
                    {
                        vhObject.vbSampleSizes[i] = (uint)BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, vhObject.offsetTableOffset + (i * 2), 2), 0);
                        vhObject.vbSampleSizes[i] <<= 3;

                        if ((minSampleSize < 0) || (vhObject.vbSampleSizes[i] < minSampleSize))
                        {
                            minSampleSize = (int)vhObject.vbSampleSizes[i];
                        }
                    }
                    
                    vhArrayList.Add(vhObject);

                    // extract file
                    ParseFile.ExtractChunkToFile(fs, vhObject.startingOffset, (int)vhObject.length,
                        Path.Combine(destinationFolder, vhObject.FileName), true);

                    offset += 1;
                }

                #endregion


                // get SEQ Files
                #region SEQ EXTRACT
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting SEQ{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                seqEntry = new ProbableSeqStruct();
                offset = 0;

                // build file list
                while ((offset = ParseFile.GetNextOffset(fs, offset, PsxSequence.ASCII_SIGNATURE)) > -1)
                {
                    seqEof = ParseFile.GetNextOffset(fs, offset, PsxSequence.END_SEQUENCE);

                    if (seqEof > 0)
                    {
                        seqLength = (int)(seqEof - offset + PsxSequence.END_SEQUENCE.Length);

                        if ((!psfStruct.UseSeqMinimumSize) || ((psfStruct.UseSeqMinimumSize) &&
                            (seqLength >= psfStruct.MinimumSize)))
                        {
                            seqEntry.offset = offset;
                            seqEntry.length = (int)(seqEof - offset + PsxSequence.END_SEQUENCE.Length);
                            seqFiles.Add(seqEntry);
                        }
                    }
                    offset += 1;
                }

                foreach (ProbableSeqStruct seq in seqFiles)
                {
                    if (seq.length > 0)
                    {
                        if (psfStruct.ReorderSeqFiles)
                        {
                            if ((vhArrayList.Count < seqFiles.Count))
                            {
                                if (!seqNamingMessageDisplayed)
                                {
                                    this.progressStruct.Clear();
                                    this.progressStruct.ErrorMessage = String.Format(
                                        "Warning, cannot reorder SEQ files, there are less VH files than SEQ files.{0}", Environment.NewLine);
                                    this.ReportProgress(this.progress, this.progressStruct);
                                    seqNamingMessageDisplayed = true;
                                }
                                seqName = String.Format("{0}_{1}.SEQ", Path.GetFileNameWithoutExtension(pPath), seqNumber++.ToString("X4"));
                            }
                            else
                            {
                                vhObject = (VhStruct)vhArrayList[vhArrayList.Count - seqFiles.Count + seqNumber++];
                                seqName = Path.ChangeExtension(vhObject.FileName, ".SEQ");
                            }
                        }
                        else
                        {
                            seqName = String.Format("{0}_{1}.SEQ", Path.GetFileNameWithoutExtension(pPath), seqNumber++.ToString("X4"));
                        }

                        ParseFile.ExtractChunkToFile(fs, seq.offset, (int)seq.length,
                            Path.Combine(destinationFolder, seqName), true);
                    }
                    else
                    { 
                        this.progressStruct.Clear();
                        this.progressStruct.ErrorMessage = String.Format(" Warning SEQ found with length less than 0,  at Offset 0x{1}: {2}{3}", seq.offset.ToString("X8"), seq.length.ToString("X8"), Environment.NewLine);
                        this.ReportProgress(this.progress, this.progressStruct);
                    }
                }
                #endregion

                // get VB files
                #region VB EXTRACT
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting VB...WARNING, THIS WILL TAKE A LONG TIME...{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                
                offset = 0;

                // build list of potential adpcm start indexes (VB_START_BYTES)
                potentialVb = new ProbableVbStruct();

                // check for the smallest found size or use default
                minRowLength = (minSampleSize / 0x10) - 1; // divide into rows
                if ((minRowLength > 0) && (minRowLength > ADPCM_ROW_COUNT))
                {
                    minRowLength = ADPCM_ROW_COUNT;
                }

                while ((offset = ParseFile.GetNextOffset(fs, offset, VB_START_BYTES, false)) > -1)
                {
                    if ((psfStruct.UseZeroOffsetForVb) && (offset % 0x10 == 0) ||
                        (!psfStruct.UseZeroOffsetForVb))
                    {
                        try
                        {
                            vbRow = ParseFile.ParseSimpleOffset(fs, offset, vbRow.Length);

                            if (IsPotentialAdpcm(fs, offset + 0x10, minRowLength))
                            {
                                potentialVb.offset = offset;
                                emptyRowList.Add(potentialVb);
                            }

                        }
                        catch (Exception vbEx)
                        {
                            this.progressStruct.Clear();
                            this.progressStruct.ErrorMessage = String.Format(" ERROR finding VB for <{0}> at Offset 0x{1}: {2}{3}", pPath, offset.ToString("X8"), vbEx.Message, Environment.NewLine);
                            this.ReportProgress(this.progress, this.progressStruct);
                        }
                    }

                    offset += 1;
                }

                potentialVbList = (ProbableVbStruct[])emptyRowList.ToArray(typeof(ProbableVbStruct));

                // set probable lengths
                for (int i = 0; i < potentialVbList.Length; i++)
                {
                    if (i > 0)
                    {
                        potentialVbList[i - 1].length = (uint)(potentialVbList[i].offset - potentialVbList[i - 1].offset);
                    }
                }

                // compare VH sample sizes to potential adpcm sizes/indexes
                vhObject.startingOffset = 0;
                vhObject.length = 0;
                vhObject.vbStartingOffset = 0;
                vhObject.vbLength = 0;

                string vbName;
                string newFileName;
                string[] dupeFileNames;

                for (int i = 0; i < vhArrayList.Count; i++)
                {
                    vhObject = (VhStruct)vhArrayList[i];
                    
                    if (vhObject.vbSampleSizes.Length < 1)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.ErrorMessage = String.Format(" ERROR building VB for <{0}>: {1} refers to a single VAG, cannot determine proper VB.  Skipping...{2}", pPath, vhObject.FileName, Environment.NewLine);
                        this.ReportProgress(this.progress, this.progressStruct);
                    }
                    else
                    {
                        for (int j = 0; j < potentialVbList.Length; j++)
                        {
                            // we have a potential match
                            if (vhObject.vbSampleSizes[0] == potentialVbList[j].length)
                            {
                                try
                                {
                                    vhObject = PopulateVbOffsetLength(fs, potentialVbList, j, vhObject, psfStruct.RelaxVbEofRestrictions);

                                    if (vhObject.vbLength > 0)
                                    {

                                        // check for other BD files that matched and rename accordingly
                                        dupeFileNames = Directory.GetFiles(destinationFolder, Path.GetFileNameWithoutExtension(vhObject.FileName) + "*.VB");

                                        if (dupeFileNames.Length >= 1)
                                        {
                                            vbName = String.Format("{0}_{1}.VB", Path.GetFileNameWithoutExtension(vhObject.FileName), (dupeFileNames.Length).ToString("X4"));

                                            if (dupeFileNames.Length == 1)
                                            {
                                                // rename existing
                                                newFileName = String.Format("{0}_{1}.VB", Path.GetFileNameWithoutExtension(vhObject.FileName), (dupeFileNames.Length - 1).ToString("X4"));
                                                File.Move(dupeFileNames[0], Path.Combine(Path.GetDirectoryName(dupeFileNames[0]), newFileName));
                                            }
                                        }
                                        else
                                        {
                                            vbName = Path.ChangeExtension(vhObject.FileName, ".VB");
                                        }


                                        ParseFile.ExtractChunkToFile(fs, vhObject.vbStartingOffset, (int)vhObject.vbLength,
                                            Path.Combine(destinationFolder, vbName), true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.progressStruct.Clear();
                                    this.progressStruct.ErrorMessage = String.Format(" ERROR building VB for <{0}>: {1}{2}", pPath, ex.Message, Environment.NewLine);
                                    this.ReportProgress(this.progress, this.progressStruct);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }

        private bool IsPotentialAdpcm(Stream searchStream, long offset, int rowsToCheck)
        {
            bool ret = true;
            byte[] checkBytes = new byte[0x10];
            int bytesRead;

            searchStream.Position = offset;

            // check for rows meeting criteria
            for (int i = 0; i < rowsToCheck; i++)
            {
                bytesRead = searchStream.Read(checkBytes, 0, checkBytes.Length);

                if (!((bytesRead == checkBytes.Length) &&
                    (!ParseFile.CompareSegment(checkBytes, 0, VB_START_BYTES)) &&
                    (checkBytes[1] < 8)))
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        private VhStruct PopulateVbOffsetLength(Stream searchStream,
            ProbableVbStruct[] potentialVbList, int potentialVbStartIndex,
            VhStruct vhObject, bool RelaxVbEofRestrictions)
        {
            VhStruct ret = vhObject;
            long totalLength = 0;
            byte[] lastLine = new byte[0x10];
            string errorMessage;

            for (int i = 0; i < vhObject.vbSampleSizes.Length; i++)
            {
                if ((potentialVbStartIndex + i) >= potentialVbList.Length)
                {
                    errorMessage = String.Format("  Warning, a potential VB match for {0} found at 0x{1}, " +
                        "but index would exceed array bounds.  It is suggested that you check manually if a match " +
                        "is not found at completion.{2}", vhObject.FileName,
                        potentialVbList[potentialVbStartIndex].offset.ToString("X8"),
                        Environment.NewLine);
                    throw new IndexOutOfRangeException(errorMessage);
                }
                else
                {
                    totalLength += vhObject.vbSampleSizes[i];

                    if (i == (vhObject.vbSampleSizes.Length - 1))
                    {
                        // check if lengths match
                        if (totalLength == vhObject.expectedVbLength)
                        {
                            // check EOF conditions
                            if (!RelaxVbEofRestrictions)
                            {
                                // check last value
                                searchStream.Position =
                                    potentialVbList[potentialVbStartIndex + i].offset + vhObject.vbSampleSizes[i] - lastLine.Length;
                                searchStream.Read(lastLine, 0, lastLine.Length);

                                if (lastLine[1] == 3 ||
                                    ParseFile.CompareSegment(lastLine, 0, VB_END_BYTES_1) ||
                                    ParseFile.CompareSegment(lastLine, 0, VB_END_BYTES_2))
                                {

                                    {
                                        ret.vbStartingOffset = potentialVbList[potentialVbStartIndex].offset;
                                        ret.vbLength = vhObject.expectedVbLength;
                                    }
                                }
                            }
                            else
                            {
                                ret.vbStartingOffset = potentialVbList[potentialVbStartIndex].offset;
                                ret.vbLength = vhObject.expectedVbLength;                            
                            }                       
                        }                        
                    }
                    else if (vhObject.vbSampleSizes[i] != potentialVbList[potentialVbStartIndex + i].length)
                    {
                        ret.vbStartingOffset = -1;
                        ret.vbLength = -1;
                        break;
                    }
                }
            }

            return ret;
        }
    }
}
