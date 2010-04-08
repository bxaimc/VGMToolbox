using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.format.util
{
    public struct ExtractXaStruct
    {
        private string path;
        private bool addRiffHeader;
        private bool patchByte0x11;
        private uint silentFramesCount;

        public string Path
        {
            set { path = value; }
            get { return path; }
        }
        public bool AddRiffHeader
        {
            set { addRiffHeader = value; }
            get { return addRiffHeader; }
        }
        public bool PatchByte0x11
        {
            set { patchByte0x11 = value; }
            get { return patchByte0x11; }
        }
        public uint SilentFramesCount
        {
            set { silentFramesCount = value; }
            get { return silentFramesCount; }
        }

        public bool FilterAgainstBlockId
        {
            set;
            get;
        }
    }

    public struct CdxaWriterStruct
    {
        public BinaryWriter FileWriter { set; get; }
        public long CurrentChunkOffset { set; get; }
    }

    public class CdxaUtil
    {
        const long EMPTY_BLOCK_OFFSET_FLAG = -1;
        
        public static void ExtractXaFiles(ExtractXaStruct pExtractXaStruct, bool useTwoPasses)
        {
            Dictionary<UInt32, CdxaWriterStruct> bwDictionary = new Dictionary<UInt32, CdxaWriterStruct>();
            List<UInt32> bwKeys;

            long offset;
            byte[] trackId;
            byte[] buffer = new byte[Cdxa.XA_BLOCK_SIZE];
            UInt32 trackKey;

            long previousOffset;
            long distanceBetweenBlocks;
            long distanceStatisticalMode = EMPTY_BLOCK_OFFSET_FLAG;
            Dictionary<long, int> distanceFrequency = new Dictionary<long, int>();
            
            CdxaWriterStruct workingStruct = new CdxaWriterStruct();

            string outputFileName;
            string outputDirectory = Path.Combine(Path.GetDirectoryName(pExtractXaStruct.Path),
                Path.GetFileNameWithoutExtension(pExtractXaStruct.Path));

            int totalPasses = 1;
            bool doFileWrite = false;

            if (useTwoPasses)
            {
                totalPasses = 2; 
            }

            using (FileStream fs = File.OpenRead(pExtractXaStruct.Path))
            {
                for (int currentPass = 1; currentPass <= totalPasses; currentPass++)
                {
                    // turn on write flag
                    if (currentPass == totalPasses)
                    {
                        doFileWrite = true;
                    }
                    
                    // get first offset to start the party
                    offset = ParseFile.GetNextOffset(fs, 0, Cdxa.XA_SIG);

                    if (offset != -1) // we have found an XA sig
                    {
                        if (!Directory.Exists(outputDirectory))
                        {
                            Directory.CreateDirectory(outputDirectory);
                        }

                        while ((offset != -1) && ((offset + Cdxa.XA_BLOCK_SIZE) <= fs.Length))
                        {
                            trackId = ParseFile.ParseSimpleOffset(fs, offset + Cdxa.XA_TRACK_OFFSET, Cdxa.XA_TRACK_SIZE);
                            trackKey = BitConverter.ToUInt32(trackId, 0);

                            if ((pExtractXaStruct.FilterAgainstBlockId) && (trackId[2] != Cdxa.XA_CHUNK_ID_DIGITS))
                            {
                                offset = ParseFile.GetNextOffset(fs, offset + 1, Cdxa.XA_SIG);
                            }
                            else
                            {
                                // check distance between blocks for possible split
                                if ((doFileWrite) &&
                                    (bwDictionary.ContainsKey(trackKey)) &&
                                    (distanceStatisticalMode != EMPTY_BLOCK_OFFSET_FLAG) &&
                                    (bwDictionary[trackKey].CurrentChunkOffset != EMPTY_BLOCK_OFFSET_FLAG))
                                {
                                    previousOffset = bwDictionary[trackKey].CurrentChunkOffset;
                                    distanceBetweenBlocks = offset - previousOffset;

                                    if (distanceBetweenBlocks > distanceStatisticalMode)
                                    {
                                        // close up this file, we're done.
                                        FixHeaderAndCloseWriter(bwDictionary[trackKey].FileWriter, pExtractXaStruct);
                                        bwDictionary.Remove(trackKey);
                                    }
                                }                                
                                
                                // First Block only
                                if (!bwDictionary.ContainsKey(trackKey))
                                {
                                    outputFileName = GetOutputFileName(pExtractXaStruct, trackId);
                                    workingStruct = new CdxaWriterStruct();
                                    workingStruct.CurrentChunkOffset = EMPTY_BLOCK_OFFSET_FLAG;

                                    if (doFileWrite)
                                    {
                                        workingStruct.FileWriter = new BinaryWriter(File.Open(outputFileName, FileMode.Create, FileAccess.Write));
                                    }
                                    
                                    bwDictionary.Add(trackKey, workingStruct);

                                    if (doFileWrite && pExtractXaStruct.AddRiffHeader)
                                    {
                                        bwDictionary[trackKey].FileWriter.Write(Cdxa.XA_RIFF_HEADER);
                                    }
                                }

                                // calculate distance between blocks for possible split
                                if ((!doFileWrite) &&
                                    (bwDictionary[trackKey].CurrentChunkOffset != EMPTY_BLOCK_OFFSET_FLAG))
                                {
                                    previousOffset = bwDictionary[trackKey].CurrentChunkOffset;
                                    distanceBetweenBlocks = offset - previousOffset;

                                    if (!distanceFrequency.ContainsKey(distanceBetweenBlocks))
                                    {
                                        distanceFrequency.Add(distanceBetweenBlocks, 1);
                                    }
                                    else
                                    {
                                        distanceFrequency[distanceBetweenBlocks]++;
                                    }
                                }

                                // set offset, doing this way because of boxing issues
                                workingStruct = bwDictionary[trackKey];
                                workingStruct.CurrentChunkOffset = offset;
                                bwDictionary[trackKey] = workingStruct;

                                // get the next block
                                buffer = ParseFile.ParseSimpleOffset(fs, offset, Cdxa.XA_BLOCK_SIZE);
                                
                                // check if this is a "silent" block, ignore leading silence (first block)
                                if (IsSilentBlock(buffer, pExtractXaStruct))
                                {
                                    if (doFileWrite && (bwDictionary[trackKey].FileWriter.BaseStream.Length > Cdxa.XA_BLOCK_SIZE))
                                    {
                                        // write the block
                                        bwDictionary[trackKey].FileWriter.Write(buffer);

                                        // close up this file, we're done.
                                        FixHeaderAndCloseWriter(bwDictionary[trackKey].FileWriter, pExtractXaStruct);
                                    }
                                    
                                    bwDictionary.Remove(trackKey);
                                }
                                else if (doFileWrite)
                                {
                                    // patch if needed
                                    if (pExtractXaStruct.PatchByte0x11)
                                    {
                                        buffer[0x11] = 0x00;
                                    }

                                    // write the block
                                    bwDictionary[trackKey].FileWriter.Write(buffer);
                                }

                                offset += Cdxa.XA_BLOCK_SIZE;
                            }
                        }

                        // fix header and close writers
                        bwKeys = new List<UInt32>(bwDictionary.Keys);
                        foreach (UInt32 keyname in bwKeys)
                        {
                            if (doFileWrite)
                            {
                                FixHeaderAndCloseWriter(bwDictionary[keyname].FileWriter, pExtractXaStruct);
                            }
                            
                            bwDictionary.Remove(keyname);
                        }

                        // get statistical mode of distance between blocks
                        if (!doFileWrite)
                        {
                            distanceStatisticalMode = GetDistanceMode(distanceFrequency);
                        }
                    }
                    else
                    {
                        // no CD-XA found
                        break;
                    }
                }
            }                
        }

        private static void FixHeaderAndCloseWriter(BinaryWriter pBw, ExtractXaStruct pExtractXaStruct)
        {
            if (pExtractXaStruct.AddRiffHeader)
            {
                UInt32 xaFileSize = (UInt32)pBw.BaseStream.Length;

                // add file size
                pBw.BaseStream.Position = Cdxa.FILESIZE_OFFSET;
                pBw.Write(BitConverter.GetBytes(xaFileSize - 8));

                // add data size
                pBw.BaseStream.Position = Cdxa.DATA_LENGTH_OFFSET;
                pBw.Write(BitConverter.GetBytes((uint)(xaFileSize - Cdxa.XA_RIFF_HEADER.Length)));
            }

            pBw.Close();
        }

        private static bool IsSilentBlock(byte[] pCdxaBlock, ExtractXaStruct pExtractXaStruct)
        {
            bool ret = false;
            int silentFrameCount = 0;
            long bufferOffset = 0;

            while ((bufferOffset = ParseFile.GetNextOffset(pCdxaBlock, bufferOffset, Cdxa.XA_SILENT_FRAME)) > -1)
            {
                silentFrameCount++;
                bufferOffset += 1;
            }

            // did we find enough silent frames?
            if (silentFrameCount >= pExtractXaStruct.SilentFramesCount)
            {
                ret = true;
            }

            return ret;
        }

        private static string GetOutputFileName(ExtractXaStruct pExtractXaStruct, byte[] pTrackId)
        {
            string outputDirectory = Path.Combine(Path.GetDirectoryName(pExtractXaStruct.Path),
                Path.GetFileNameWithoutExtension(pExtractXaStruct.Path));
            string outputFileName = Path.GetFileNameWithoutExtension(pExtractXaStruct.Path) + "_" + ParseFile.ByteArrayToString(pTrackId) + Cdxa.XA_FILE_EXTENSION;

            int fileCount = Directory.GetFiles(outputDirectory, String.Format("{0}*", Path.GetFileNameWithoutExtension(outputFileName)), SearchOption.TopDirectoryOnly).Length;
            outputFileName = String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(outputFileName), fileCount.ToString("X4"), Cdxa.XA_FILE_EXTENSION);

            string ret = Path.Combine(outputDirectory, outputFileName);
            
            return ret;
        }

        private static long GetDistanceMode(Dictionary<long, int> distanceFrequencyList)
        {
            long distanceMode = EMPTY_BLOCK_OFFSET_FLAG;

            foreach (long key in distanceFrequencyList.Keys)
            {
                if (distanceFrequencyList[key] > distanceMode)
                {
                    distanceMode = key;
                }
            }

            return distanceMode;
        }
    }
}
