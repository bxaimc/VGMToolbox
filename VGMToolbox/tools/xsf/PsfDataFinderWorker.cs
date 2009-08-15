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

            public long startingOffset;
            public long length;

            public long vbStartingOffset;
            public long vbLength;
        }

        public struct ProbableVbStruct
        {
            public long offset;
            public uint length;
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

            VhStruct vhObject;
            ArrayList vhArrayList = new ArrayList();

            ArrayList emptyRowList = new ArrayList();
            ProbableVbStruct potentialVb;
            ProbableVbStruct[] potentialVbList;
            byte[] vbRow = new byte[0x10];

            // improve algorithm later
            using (FileStream fs = File.OpenRead(pPath))
            { 
                // get SEQ Files
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting SEQ{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                
                offset = 0;

                while ((offset = ParseFile.GetNextOffset(fs, offset, PsxSequence.ASCII_SIGNATURE)) > -1)
                {
                    seqEof = ParseFile.GetNextOffset(fs, offset, PsxSequence.END_SEQUENCE);

                    if ((!psfStruct.UseSeqMinimumSize) || ((psfStruct.UseSeqMinimumSize) &&
                        ((seqEof - offset + PsxSequence.END_SEQUENCE.Length) > psfStruct.MinimumSize)))
                    {
                        seqName = String.Format("{0}_{1}.SEQ", Path.GetFileNameWithoutExtension(pPath), seqNumber++.ToString("X4"));
                        ParseFile.ExtractChunkToFile(fs, offset, (int)(seqEof - offset + PsxSequence.END_SEQUENCE.Length),
                            Path.Combine(Path.GetDirectoryName(pPath), seqName), true);
                    }

                    offset += 1;
                }

                // get VH files
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting VH{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                
                offset = 0;

                while ((offset = ParseFile.GetNextOffset(fs, offset, XsfUtil.VAB_SIGNATURE)) > -1)
                {
                    vhObject = new VhStruct();
                    vhObject.startingOffset = offset; 
                    vhObject.vhProgramCount = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, (offset + 0x12), 2), 0);
                    vhObject.vbSampleCount = BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, (offset + 0x16), 2), 0);
                    vhObject.vbSampleSizes = new uint[vhObject.vbSampleCount];
                    vhObject.length = 2592 + (512 * vhObject.vhProgramCount);

                    vhObject.offsetTableOffset = offset + (512 * vhObject.vhProgramCount) + 2080;
                    vhObject.offsetTableOffset += 2; // not sure but seems to be needed

                    for (int i = 0; i < vhObject.vbSampleCount; i++)
                    {
                        vhObject.vbSampleSizes[i] = (uint)BitConverter.ToUInt16(ParseFile.ParseSimpleOffset(fs, vhObject.offsetTableOffset + (i * 2), 2), 0);
                        vhObject.vbSampleSizes[i] <<= 3;
                    }

                    vhArrayList.Add(vhObject);

                    offset += 1;
                }

                // get VB files
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting VB...WARNING, THIS WILL TAKE A LONG TIME...{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);
                
                offset = 0;

                // build list of potential adpcm start indexes (VB_START_BYTES)
                potentialVb = new ProbableVbStruct();
                
                while ((offset = ParseFile.GetNextOffset(fs, offset, VB_START_BYTES, false)) > -1)
                {
                    vbRow = ParseFile.ParseSimpleOffset(fs, offset, vbRow.Length);

                    if (IsPotentialAdpcm(fs, offset + 0x10))
                    {
                        potentialVb.offset = offset;
                        emptyRowList.Add(potentialVb);
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
                string vhName;

                for (int i = 0; i < vhArrayList.Count; i++)
                {
                    vhObject = (VhStruct)vhArrayList[i];
                    
                    for (int j = 0; j < potentialVbList.Length; j++)
                    {
                        // we have a potential match
                        if (vhObject.vbSampleSizes[0] == potentialVbList[j].length)
                        {
                            vhObject = PopulateVbOffsetLength(fs, potentialVbList, j, vhObject);                            

                            if (vhObject.vbLength > 0)
                            {
                                vbName = String.Format("{0}_{1}.VB", Path.GetFileNameWithoutExtension(pPath), i.ToString("X4"));
                                
                                ParseFile.ExtractChunkToFile(fs, vhObject.vbStartingOffset, (int)vhObject.vbLength,
                                    Path.Combine(Path.GetDirectoryName(pPath), vbName), true);
                            }
                        }
                    }

                    // extract if possible.
                    if (vhObject.length > 0)
                    {
                        vhName = String.Format("{0}_{1}.VH", Path.GetFileNameWithoutExtension(pPath), i.ToString("X4"));
                        
                        ParseFile.ExtractChunkToFile(fs, vhObject.startingOffset, (int)vhObject.length,
                            Path.Combine(Path.GetDirectoryName(pPath), vhName), true);
                    }                    
                }
            }
        }

        private bool IsPotentialAdpcm(Stream searchStream, long offset)
        {
            bool ret = true;
            byte[] checkBytes = new byte[0x10];
            int bytesRead;

            searchStream.Position = offset;

            // check for 10 rows meeting criteria
            for (int i = 0; i < ADPCM_ROW_COUNT; i++)
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
            VhStruct vhObject)
        {
            VhStruct ret = vhObject;
            long totalLength = 0;
            byte[] lastLine = new byte[0x10];

            for (int i = 0; i < vhObject.vbSampleSizes.Length; i++)
            {
                totalLength += vhObject.vbSampleSizes[i];
                
                if (i == (vhObject.vbSampleSizes.Length - 1))
                {
                    // check last value
                    searchStream.Position =
                        potentialVbList[potentialVbStartIndex + i].offset + vhObject.vbSampleSizes[i] - lastLine.Length;
                    searchStream.Read(lastLine, 0, lastLine.Length);

                    if (lastLine[1] == 3 ||
                        ParseFile.CompareSegment(lastLine, 0, VB_END_BYTES_1) ||
                        ParseFile.CompareSegment(lastLine, 0, VB_END_BYTES_2))
                    {
                        ret.vbStartingOffset = potentialVbList[potentialVbStartIndex].offset;
                        ret.vbLength = totalLength;
                    }
                }
                else if (vhObject.vbSampleSizes[i] != potentialVbList[potentialVbStartIndex + i].length)
                {
                    ret.vbStartingOffset = -1;
                    ret.vbLength = -1;
                    break;
                }
            }

            return ret;
        }
    }
}
