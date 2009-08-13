using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class Psf2DataFinderWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
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

        public struct Psf2DataFinderStruct : IVgmtWorkerStruct
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

        public struct HdStruct
        {
            public long vagSectionOffset;
            public long maxVagInfoNumber;
            public long[] vagInfoOffsetAddr;
            public long[] vagOffset;

            public long startingOffset;
            public long length;

            public long bdStartingOffset;
            public long bdLength;
        }

        public struct ProbableBdStruct
        {
            public long offset;
            public uint length;
        }

        public Psf2DataFinderWorker() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPsf2DataFinderStruct, DoWorkEventArgs e)
        {
            Psf2DataFinderStruct psf2Struct = (Psf2DataFinderStruct)pPsf2DataFinderStruct;

            long offset;
            
            uint sqLength;
            string sqName;
            int sqNumber = 0;

            uint hdLength;
            string hdName;
            int hdNumber = 0;

            HdStruct hdObject;
            ArrayList hdArrayList = new ArrayList();

            // display file name
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = String.Format("[{0}]{1}", pPath, Environment.NewLine);
            this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

            using (FileStream fs = File.OpenRead(pPath))
            {
                // get SQ Files
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting SQ{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                offset = 0;

                while ((offset = ParseFile.GetNextOffset(fs, offset, Ps2SequenceData.SIGNATURE_BYTES)) > -1)
                {
                    sqLength = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0xC, 4), 0);
                    
                    if ((!psf2Struct.UseSeqMinimumSize) || ((psf2Struct.UseSeqMinimumSize) &&
                        (sqLength >= psf2Struct.MinimumSize)))
                    {
                        sqName = String.Format("{0}_{1}.SQ", Path.GetFileNameWithoutExtension(pPath), sqNumber++.ToString("X4"));
                        ParseFile.ExtractChunkToFile(fs, offset - 0x10, (int)sqLength,
                            Path.Combine(Path.GetDirectoryName(pPath), sqName));
                    }

                    offset += 1;
                }

                // get HD Files
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting HD{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                offset = 0;

                while ((offset = ParseFile.GetNextOffset(fs, offset, Psf2.HD_SIGNATURE)) > -1)
                {
                    hdLength = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0xC, 4), 0);

                    hdName = String.Format("{0}_{1}.HD", Path.GetFileNameWithoutExtension(pPath), hdNumber++.ToString("X4"));
                    ParseFile.ExtractChunkToFile(fs, offset - 0x10, (int)hdLength,
                        Path.Combine(Path.GetDirectoryName(pPath), hdName));

                    // get info
                    hdObject = new HdStruct();
                    hdObject.startingOffset = offset - 0x10;
                    hdObject.length = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0xC, 4), 0);
                    hdObject.vagSectionOffset = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0x20, 4), 0);
                    hdObject.maxVagInfoNumber = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, hdObject.vagSectionOffset + 0xC, 4), 0);

                    hdObject.vagInfoOffsetAddr = new long[hdObject.maxVagInfoNumber + 1];
                    hdObject.vagOffset = new long[hdObject.maxVagInfoNumber + 1];

                    for (int i = 0; i < hdObject.maxVagInfoNumber; i++)
                    {
                        hdObject.vagInfoOffsetAddr[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, hdObject.vagSectionOffset + 0x10 + (i * 4), 4), 0);
                        hdObject.vagOffset[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, hdObject.vagSectionOffset + hdObject.vagInfoOffsetAddr[i], 4), 0);
                    }

                    // add to array
                    hdArrayList.Add(hdObject);
                   
                    // increment offset
                    offset += 1;
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
    }
}
