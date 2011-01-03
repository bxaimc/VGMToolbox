using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;

namespace VGMToolbox.tools.xsf
{
    class PspSeqDataFinderWorker : AVgmtDragAndDropWorker, IVgmtBackgroundWorker
    {
        public static readonly byte[] PPHD_SIGNATURE = new byte[] { 0x50, 0x50, 0x48, 0x44 };

        #region STRUCTURES
        public struct PspSeqDataFinderStruct : IVgmtWorkerStruct
        {
            public bool ReorderMidFiles;
            public bool UseZeroOffsetForPbd;

            public string[] SourcePaths { set; get; }
        }

        public struct PphdStruct
        {
            public long progSectionOffset;
            public long toneSectionOffset;            
            public long vagSectionOffset;
            
            public long maxVagInfoNumber;
            public long vagInfoSize;
            public long[] vagOffsets;
            public long[] vagLengths;
            public bool IsSmallSamplePresent { set; get; }

            public string FileName;
            public long startingOffset;
            public long length;

            public long expectedBdLength;
            public long bdStartingOffset;
            public long bdLength;
        }
        
        #endregion

        public PspSeqDataFinderWorker() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPspSeqDataFinderStruct, DoWorkEventArgs e)
        {
            PspSeqDataFinderStruct pspStruct = (PspSeqDataFinderStruct)pPspSeqDataFinderStruct;

            long offset;

            int pphdNumber = 0;
            PphdStruct pphdObject;
            ArrayList pphdArrayList = new ArrayList();

            // display file name
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = String.Format("[{0}]{1}", pPath, Environment.NewLine);
            this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

            // open file
            using (FileStream fs = File.OpenRead(pPath))
            {
                string destinationFolder = Path.Combine(Path.GetDirectoryName(pPath), Path.GetFileNameWithoutExtension(pPath));

                // get PHD files
                #region PHD EXTRACT
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting PHD{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                offset = 0;

                while ((offset = ParseFile.GetNextOffset(fs, offset, PPHD_SIGNATURE)) > -1)
                {
                    try
                    {
                        // get PPHD data
                        pphdObject = GetPphdData(fs, offset);
                        pphdObject.FileName = String.Format("{0}_{1}.PPHD", Path.GetFileNameWithoutExtension(pPath), pphdNumber++.ToString("X4"));

                        // extract the file
                        ParseFile.ExtractChunkToFile(fs, offset, (int)pphdObject.length,
                            Path.Combine(destinationFolder, pphdObject.FileName), true, true);

                        // add to array
                        pphdArrayList.Add(pphdObject);
                    }
                    catch (Exception pphdEx)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.ErrorMessage = String.Format("      Error extracting PPHD at offset 0x{0}: {1}{2}", offset.ToString("X8"), pphdEx.Message, Environment.NewLine);
                        ReportProgress(progress, this.progressStruct);
                    }

                    // increment offset
                    offset += 1;
                }      

                #endregion
            }
        }

        public static PphdStruct GetPphdData(FileStream fs, long offset)
        {
            PphdStruct pphdObject = new PphdStruct();

            // save off info
            pphdObject.startingOffset = offset;

            pphdObject.progSectionOffset = offset + BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0x10, 4), 0);
            pphdObject.toneSectionOffset = offset + BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0x14, 4), 0);
            pphdObject.vagSectionOffset = offset + BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0x18, 4), 0);

            pphdObject.maxVagInfoNumber = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, pphdObject.vagSectionOffset + 0x14, 4), 0);
            pphdObject.vagInfoSize = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, pphdObject.vagSectionOffset + 8, 4), 0);

            pphdObject.vagOffsets = new long[pphdObject.maxVagInfoNumber + 1];
            pphdObject.vagLengths = new long[pphdObject.maxVagInfoNumber + 1];
            pphdObject.expectedBdLength = 0;
            pphdObject.IsSmallSamplePresent = false;

            for (int i = 0; i <= pphdObject.maxVagInfoNumber; i++)
            {
                pphdObject.vagOffsets[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, pphdObject.vagSectionOffset + (i * pphdObject.vagInfoSize) + 4, 4), 0);
                pphdObject.vagLengths[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, pphdObject.vagSectionOffset + (i * pphdObject.vagInfoSize) + 8, 4), 0);
                pphdObject.expectedBdLength += pphdObject.vagLengths[i];

                if (pphdObject.vagLengths[i] < Psf.MIN_ADPCM_ROW_SIZE)
                {
                    pphdObject.IsSmallSamplePresent = true;
                }
            }

            pphdObject.length = pphdObject.vagSectionOffset + 0x20 + ((pphdObject.maxVagInfoNumber + 1) * pphdObject.vagInfoSize);

            return pphdObject;
        }
    }
}
