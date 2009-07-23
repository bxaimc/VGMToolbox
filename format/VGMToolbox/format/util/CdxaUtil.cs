using System;
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
    }
    
    public class CdxaUtil
    {
        public static void ExtractXaFiles(ExtractXaStruct pExtractXaStruct)
        { 
            Dictionary<UInt32, BinaryWriter> bwDictionary = new Dictionary<UInt32, BinaryWriter>();
            List<UInt32> bwKeys;

            long offset;
            byte[] trackId;
            byte[] buffer = new byte[Cdxa.XA_BLOCK_SIZE];
            UInt32 trackKey;

            string outputFileName;
            string outputDirectory = Path.Combine(Path.GetDirectoryName(pExtractXaStruct.Path),
                Path.GetFileNameWithoutExtension(pExtractXaStruct.Path));

            using (FileStream fs = File.OpenRead(pExtractXaStruct.Path))
            {                
                // get first offset to start the party
                offset = ParseFile.GetNextOffset(fs, 0, Cdxa.XA_SIG);

                if (offset != -1) // we have found an XA sig
                {
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    while ((offset != -1)  && ((offset + Cdxa.XA_BLOCK_SIZE) <= fs.Length))
                    {
                        trackId = ParseFile.ParseSimpleOffset(fs, offset + Cdxa.XA_TRACK_OFFSET, Cdxa.XA_TRACK_SIZE);
                        trackKey = BitConverter.ToUInt32(trackId, 0);

                        if (!ParseFile.ByteArrayToString(trackId).EndsWith(Cdxa.XA_ENDING_DIGITS))
                        {
                            offset = ParseFile.GetNextOffset(fs, offset + 1, Cdxa.XA_SIG);
                        }
                        else
                        {
                            if (!bwDictionary.ContainsKey(trackKey))
                            {
                                // outputFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(pExtractXaStruct.Path) + "_" + ParseFile.ByteArrayToString(trackId) + Cdxa.XA_FILE_EXTENSION);
                                outputFileName = GetOutputFileName(pExtractXaStruct, trackId);
                                bwDictionary.Add(trackKey, new BinaryWriter(File.Open(outputFileName, FileMode.Create, FileAccess.Write)));

                                if (pExtractXaStruct.AddRiffHeader)
                                {
                                    bwDictionary[trackKey].Write(Cdxa.XA_RIFF_HEADER);
                                }
                            }

                            // get the next block
                            buffer = ParseFile.ParseSimpleOffset(fs, offset, Cdxa.XA_BLOCK_SIZE);

                            // check if this is a "silent" block, ignore leading silence (first block)
                            if ((bwDictionary[trackKey].BaseStream.Length > Cdxa.XA_BLOCK_SIZE) && IsSilentBlock(buffer, pExtractXaStruct))
                            {
                                // write the block
                                bwDictionary[trackKey].Write(buffer);

                                // close up this file, we're done.
                                FixHeaderAndCloseWriter(bwDictionary[trackKey], pExtractXaStruct);
                                bwDictionary.Remove(trackKey);
                            }
                            else
                            {
                                // patch if needed
                                if (pExtractXaStruct.PatchByte0x11)
                                {
                                    buffer[0x11] = 0x00;
                                }

                                // write the block
                                bwDictionary[trackKey].Write(buffer);
                            }

                            offset += Cdxa.XA_BLOCK_SIZE;
                        }
                    }

                    // fix header and close writers
                    bwKeys = new List<UInt32>(bwDictionary.Keys);
                    foreach (UInt32 keyname in bwKeys)
                    {
                        FixHeaderAndCloseWriter(bwDictionary[keyname], pExtractXaStruct);
                        bwDictionary.Remove(keyname);
                    }
                }
                else
                {
                    // Console.WriteLine("XA Signature bytes not found");
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
                pBw.BaseStream.Position = Cdxa.DATA_OFFSET;
                pBw.Write(BitConverter.GetBytes(xaFileSize - Cdxa.XA_RIFF_HEADER.Length));
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


            //for (int i = 0; i < Cdxa.NUM_SILENT_FRAMES_FOR_SILENT_BLOCK; i++)
            //{
            //    ret = ret && ParseFile.CompareSegment(pCdxaBlock, 
            //        (i * Cdxa.XA_SILENT_FRAME.Length) + ((i + 1) * Cdxa.BLOCK_HEADER_SIZE), Cdxa.XA_SILENT_FRAME);

            //    if (!ret)
            //    {
            //        break;
            //    }
            //}

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
    }
}
