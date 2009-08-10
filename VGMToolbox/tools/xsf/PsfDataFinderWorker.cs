using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

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
        private static int ADPCM_ROW_COUNT = 10;


        public struct PsfDataFinderStruct : IVgmtWorkerStruct
        {
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
        }
        
        public PsfDataFinderWorker() { }

        protected override void DoTaskForFile(string pPath, IVgmtWorkerStruct pPsfDataFinderStruct, DoWorkEventArgs e)
        {
            long offset;
            
            VhStruct vhObject;
            ArrayList vhArrayList = new ArrayList();

            ArrayList emptyRowList = new ArrayList();
            byte[] vbRow = new byte[0x10];

            // improve algorithm later
            using (FileStream fs = File.OpenRead(pPath))
            { 
            
                // get VH files
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
                offset = 0;

                // build list of potential adpcm start indexes (VB_START_BYTES)
                while ((offset = ParseFile.GetNextOffset(fs, offset, VB_START_BYTES)) > -1)
                {
                    vbRow = ParseFile.ParseSimpleOffset(fs, offset, vbRow.Length);

                    if (IsPotentialAdpcm(fs, offset + 0x10))
                    {
                        emptyRowList.Add(offset);
                    }

                    offset += 1;
                }

                // compare VH sample sizes to potential adpcm sizes/indexes


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
