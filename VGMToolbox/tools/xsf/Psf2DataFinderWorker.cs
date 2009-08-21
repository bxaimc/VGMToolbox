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

        private static int ADPCM_ROW_COUNT = 20;

        public struct Psf2DataFinderStruct : IVgmtWorkerStruct
        {
            public bool UseSeqMinimumSize;
            public int MinimumSize;
            public bool ReorderSqFiles;
            public bool UseZeroOffsetForBd;

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
            public long[] vagLengths;

            public string FileName;
            public long startingOffset;
            public long length;

            public long expectedBdLength;
            public long bdStartingOffset;
            public long bdLength;
        }

        public struct ProbableBdStruct
        {
            public long offset;
            public uint length;
        }

        public struct ProbableSqStruct
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
            ProbableSqStruct sqEntry;
            ArrayList sqFiles = new ArrayList();
            bool sqNamingMessageDisplayed = false;

            uint hdLength;
            string hdName;
            int hdNumber = 0;

            HdStruct hdObject;
            ArrayList hdArrayList = new ArrayList();

            ArrayList emptyRowList = new ArrayList();
            ProbableBdStruct potentialBd;
            ProbableBdStruct[] potentialBdList;
            byte[] bdRow = new byte[0x10];

            // display file name
            this.progressStruct.Clear();
            this.progressStruct.GenericMessage = String.Format("[{0}]{1}", pPath, Environment.NewLine);
            this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

            using (FileStream fs = File.OpenRead(pPath))
            {
                string destinationFolder = Path.Combine(Path.GetDirectoryName(pPath), Path.GetFileNameWithoutExtension(pPath));
                
                // get HD Files
                #region HD EXTRACT
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting HD{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                offset = 0;

                while ((offset = ParseFile.GetNextOffset(fs, offset, Psf2.HD_SIGNATURE)) > -1)
                {                    
                    hdLength = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0xC, 4), 0);

                    hdName = String.Format("{0}_{1}.HD", Path.GetFileNameWithoutExtension(pPath), hdNumber++.ToString("X4"));
                    ParseFile.ExtractChunkToFile(fs, offset - 0x10, (int)hdLength,
                        Path.Combine(destinationFolder, hdName), true);

                    // get info
                    hdObject = new HdStruct();
                    hdObject.FileName = hdName;
                    hdObject.startingOffset = offset - 0x10;
                    hdObject.length = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0xC, 4), 0);
                    hdObject.expectedBdLength = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0x10, 4), 0);
                    hdObject.vagSectionOffset = hdObject.startingOffset + BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0x20, 4), 0);
                    hdObject.maxVagInfoNumber = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, hdObject.vagSectionOffset + 0xC, 4), 0);

                    hdObject.vagInfoOffsetAddr = new long[hdObject.maxVagInfoNumber + 1];
                    hdObject.vagOffset = new long[hdObject.maxVagInfoNumber + 1];
                    hdObject.vagLengths = new long[hdObject.maxVagInfoNumber];

                    for (int i = 0; i <= hdObject.maxVagInfoNumber; i++)
                    {
                        hdObject.vagInfoOffsetAddr[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, hdObject.vagSectionOffset + 0x10 + (i * 4), 4), 0);
                        hdObject.vagOffset[i] = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, hdObject.vagSectionOffset + hdObject.vagInfoOffsetAddr[i], 4), 0);

                        if (i > 0)
                        {
                            hdObject.vagLengths[i - 1] = hdObject.vagOffset[i] - hdObject.vagOffset[i - 1];
                        }
                    }

                    // add to array
                    hdArrayList.Add(hdObject);
                   
                    // increment offset
                    offset += 1;
                }
                #endregion

                // get SQ Files
                #region SQ EXTRACT
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting SQ{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                sqEntry = new ProbableSqStruct();
                offset = 0;

                // build file list
                while ((offset = ParseFile.GetNextOffset(fs, offset, Ps2SequenceData.SIGNATURE_BYTES)) > -1)
                {
                    sqLength = BitConverter.ToUInt32(ParseFile.ParseSimpleOffset(fs, offset + 0xC, 4), 0);

                    if ((!psf2Struct.UseSeqMinimumSize) || ((psf2Struct.UseSeqMinimumSize) &&
                        (sqLength >= psf2Struct.MinimumSize)))
                    {
                        sqEntry.offset = offset - 0x10;
                        sqEntry.length = sqLength;
                        sqFiles.Add(sqEntry);
                    }                                                                           

                    offset += 1;
                }

                foreach (ProbableSqStruct sq in sqFiles)
                {
                    if (psf2Struct.ReorderSqFiles)
                    {
                        if (hdArrayList.Count < sqFiles.Count)
                        {
                            if (!sqNamingMessageDisplayed)
                            {
                                this.progressStruct.Clear();
                                this.progressStruct.ErrorMessage = String.Format(
                                    "Warning, cannot reorder SQ files, there are less HD files than SQ files.{0}", Environment.NewLine);
                                this.ReportProgress(this.progress, this.progressStruct);
                                sqNamingMessageDisplayed = true;
                            }

                            sqName = String.Format("{0}_{1}.SQ", Path.GetFileNameWithoutExtension(pPath), sqNumber++.ToString("X4"));
                        }
                        else
                        {
                            hdObject = (HdStruct)hdArrayList[hdArrayList.Count - sqFiles.Count + sqNumber++];
                            sqName = Path.ChangeExtension(hdObject.FileName, ".SQ");
                        }
                    }
                    else
                    {
                        sqName = String.Format("{0}_{1}.SQ", Path.GetFileNameWithoutExtension(pPath), sqNumber++.ToString("X4"));
                    }

                    ParseFile.ExtractChunkToFile(fs, sq.offset, (int)sq.length,
                        Path.Combine(destinationFolder, sqName), true);


                }
                
                
                #endregion

                // get BD files
                #region BD EXTRACT
                this.progressStruct.Clear();
                this.progressStruct.GenericMessage = String.Format("  Extracting BD...WARNING, THIS CAN TAKE A LONG TIME...{0}", Environment.NewLine);
                this.ReportProgress(Constants.ProgressMessageOnly, this.progressStruct);

                offset = 0;

                // build list of potential adpcm start indexes
                potentialBd = new ProbableBdStruct();

                while ((offset = ParseFile.GetNextOffset(fs, offset, VB_START_BYTES, false)) > -1)
                {
                    if ((psf2Struct.UseZeroOffsetForBd) && (offset % 0x10 == 0) ||
                        (!psf2Struct.UseZeroOffsetForBd))
                    {
                        try
                        {
                            bdRow = ParseFile.ParseSimpleOffset(fs, offset, bdRow.Length);

                            if (IsPotentialAdpcm(fs, offset + 0x10))
                            {
                                potentialBd.offset = offset;
                                emptyRowList.Add(potentialBd);
                            }

                        }
                        catch (Exception bdEx)
                        {
                            this.progressStruct.Clear();
                            this.progressStruct.ErrorMessage = String.Format(" ERROR finding BD for <{0}> at Offset 0x{1}: {2}{3}", pPath, offset.ToString("X8"), bdEx.Message, Environment.NewLine);
                            this.ReportProgress(this.progress, this.progressStruct);
                        }
                    }
                    
                    offset += 1;
                }

                potentialBdList = (ProbableBdStruct[])emptyRowList.ToArray(typeof(ProbableBdStruct));

                // set probable lengths
                for (int i = 0; i < potentialBdList.Length; i++)
                {
                    if (i > 0)
                    {
                        potentialBdList[i - 1].length = (uint)(potentialBdList[i].offset - potentialBdList[i - 1].offset);
                    }
                }

                // compare HD sample sizes to potential adpcm sizes/indexes
                hdObject.startingOffset = 0;
                hdObject.length = 0;
                hdObject.bdStartingOffset = 0;
                hdObject.bdLength = 0;

                string bdName;
                string newFileName;
                string[] dupeFileNames;

                for (int i = 0; i < hdArrayList.Count; i++)
                {
                    hdObject = (HdStruct)hdArrayList[i];
                    if (hdObject.vagLengths.Length < 1)
                    {
                        this.progressStruct.Clear();
                        this.progressStruct.ErrorMessage = String.Format(" ERROR building BD for <{0}>: {1} refers to a single VAG, cannot determine proper BD.  Skipping...{2}", pPath, hdObject.FileName, Environment.NewLine);
                        this.ReportProgress(this.progress, this.progressStruct);
                    }
                    else
                    {
                        for (int j = 0; j < potentialBdList.Length; j++)
                        {
                            // we have a potential match
                            if (hdObject.vagLengths[0] == potentialBdList[j].length)
                            {
                                try
                                {
                                    hdObject = PopulateBdOffsetLength(fs, potentialBdList, j, hdObject);

                                    if (hdObject.bdLength > 0)
                                    {
                                        
                                        // check for other BD files that matched and rename accordingly
                                        dupeFileNames = Directory.GetFiles(destinationFolder, Path.GetFileNameWithoutExtension(hdObject.FileName) + "*.BD");

                                        if (dupeFileNames.Length >= 1)
                                        {
                                            bdName = String.Format("{0}_{1}.BD", Path.GetFileNameWithoutExtension(hdObject.FileName), (dupeFileNames.Length).ToString("X4"));

                                            if (dupeFileNames.Length == 1)
                                            {
                                                // rename existing
                                                newFileName = String.Format("{0}_{1}.BD", Path.GetFileNameWithoutExtension(hdObject.FileName), (dupeFileNames.Length - 1).ToString("X4"));
                                                File.Move(dupeFileNames[0], Path.Combine(Path.GetDirectoryName(dupeFileNames[0]), newFileName));
                                            }
                                        }
                                        else
                                        {
                                            bdName = Path.ChangeExtension(hdObject.FileName, ".BD");
                                        }


                                        ParseFile.ExtractChunkToFile(fs, hdObject.bdStartingOffset, (int)hdObject.bdLength,
                                            Path.Combine(destinationFolder, bdName), true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.progressStruct.Clear();
                                    this.progressStruct.ErrorMessage = String.Format(" ERROR building BD for <{0}>: {1}{2}", pPath, ex.Message, Environment.NewLine);
                                    this.ReportProgress(this.progress, this.progressStruct);
                                }
                            }
                        }
                    }
                }
                #endregion
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

        private HdStruct PopulateBdOffsetLength(Stream searchStream,
            ProbableBdStruct[] potentialBdList, int potentialBdStartIndex,
            HdStruct hdObject)
        {
            HdStruct ret = hdObject;
            long totalLength = 0;
            byte[] lastLine = new byte[0x10];
            string errorMessage;

            for (int i = 0; i < hdObject.vagLengths.Length; i++)
            {
                if ((potentialBdStartIndex + i) >= potentialBdList.Length)
                {
                    errorMessage = String.Format("  Warning, a potential BD match for {0} found at 0x{1}, " + 
                        "but index would exceed array bounds.  It is suggested that you check manually if a match " +
                        "is not found at completion.{2}", hdObject.FileName, 
                        potentialBdList[potentialBdStartIndex].offset.ToString("X8"), 
                        Environment.NewLine);
                    throw new IndexOutOfRangeException(errorMessage);
                }
                else
                {
                    totalLength += hdObject.vagLengths[i];

                    if (i == (hdObject.vagLengths.Length - 1))
                    {                        
                        // check last value
                        searchStream.Position =
                            potentialBdList[potentialBdStartIndex + i].offset + hdObject.vagLengths[i] + 
                            (hdObject.expectedBdLength - totalLength) - lastLine.Length;
                        searchStream.Read(lastLine, 0, lastLine.Length);

                        if (lastLine[1] == 3 ||
                            ParseFile.CompareSegment(lastLine, 0, VB_END_BYTES_1) ||
                            ParseFile.CompareSegment(lastLine, 0, VB_END_BYTES_2))
                        {
                            ret.bdStartingOffset = potentialBdList[potentialBdStartIndex].offset;                            
                            ret.bdLength = hdObject.expectedBdLength;
                        }
                    }
                    else if (hdObject.vagLengths[i] != potentialBdList[potentialBdStartIndex + i].length)
                    {
                        ret.bdStartingOffset = -1;
                        ret.bdLength = -1;
                        break;
                    }
                }
            }

            return ret;
        }
    }
}
