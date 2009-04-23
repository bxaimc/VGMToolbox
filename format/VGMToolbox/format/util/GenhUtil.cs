using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format.util
{
    public struct GenhCreationStruct
    {
        public string Format;
        public string HeaderSkip;
        public string Interleave;
        public string Channels;
        public string Frequency;

        public string LoopStart;
        public string LoopEnd;
        public bool NoLoops;
        public bool UseFileEnd;
        public bool FindLoop;

        public string CoefRightChannel;
        public string CoefLeftChannel;
        public bool CapcomHack;

        public bool OutputHeaderOnly;
        public string[] SourcePaths;
    }
    
    public class GenhUtil
    {
        public static string CreateGenhFile(string pSourcePath, GenhCreationStruct pGenhCreationStruct)
        {
            string ret = String.Empty;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            
            int dspInterleaveType =
                GetDspInterleave(pGenhCreationStruct.Interleave, pGenhCreationStruct.Channels);

            if (pGenhCreationStruct.NoLoops)
            {
                pGenhCreationStruct.LoopStart = Genh.EMPTY_SAMPLE_COUNT;
            }

            if (pGenhCreationStruct.UseFileEnd)
            {
                pGenhCreationStruct.LoopEnd = GetFileEndLoopEnd(pSourcePath, pGenhCreationStruct);
            }

            if (pGenhCreationStruct.FindLoop)
            {
                string loopStartFound;
                string loopEndFound;

                if (GetPsAdpcmLoop(pSourcePath, pGenhCreationStruct, out loopStartFound,
                    out loopEndFound))
                {
                    pGenhCreationStruct.LoopStart = loopStartFound;
                    pGenhCreationStruct.LoopEnd = loopEndFound;
                }
                else
                {
                    pGenhCreationStruct.LoopStart = Genh.EMPTY_SAMPLE_COUNT;
                    pGenhCreationStruct.LoopEnd = String.Empty;
                }
            }

            FileInfo fi = new FileInfo(Path.GetFullPath(pSourcePath));
            UInt32 fileLength = (UInt32)fi.Length;

            string outputFilePath = Path.ChangeExtension(pSourcePath, Genh.FILE_EXTENSION);

            // write the file
            using (FileStream outputFs = File.Open(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(outputFs))
                {
                    bw.Write(Genh.ASCII_SIGNATURE);
                    bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Channels));
                    bw.Write((Int32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Interleave));
                    bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Frequency));
                    bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.LoopStart));

                    if (!String.IsNullOrEmpty(pGenhCreationStruct.LoopEnd))
                    {
                        bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.LoopEnd));
                    }
                    else
                    {
                        bw.Write(new byte[] {0x00, 0x00, 0x00, 0x00});
                    }
                    
                    bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Format));
                    bw.Write((UInt32)(VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.HeaderSkip) + Genh.GENH_HEADER_SIZE));
                    bw.Write((UInt32)Genh.GENH_HEADER_SIZE);

                    if (VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Format) == 12)
                    {
                        bw.Write((UInt32)(VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.CoefLeftChannel) + Genh.GENH_HEADER_SIZE));
                        bw.Write((UInt32)(VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.CoefRightChannel) + Genh.GENH_HEADER_SIZE));
                        bw.Write((UInt32)dspInterleaveType);

                        if (pGenhCreationStruct.CapcomHack)
                        {
                            bw.Write(new byte[] { 0x01 });
                            bw.Write((UInt32)(VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.CoefLeftChannel) + Genh.GENH_HEADER_SIZE + 0x10));
                            bw.Write((UInt32)(VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.CoefRightChannel) + Genh.GENH_HEADER_SIZE + 0x10));
                        }
                        else
                        {
                            bw.Write(new byte[] { 0x00 });
                        }
                    }
                    
                    
                    bw.BaseStream.Position = 0x200;
                    bw.Write(enc.GetBytes(Path.GetFileName(pSourcePath).Trim()));

                    bw.BaseStream.Position = 0x300;
                    bw.Write(fileLength);
                    bw.Write(Genh.CURRENT_VERSION);

                    bw.BaseStream.Position = 0xFFC;
                    bw.Write(0x0);

                    // add input file now
                    if (!pGenhCreationStruct.OutputHeaderOnly)
                    {
                        using (FileStream inputFs = File.Open(pSourcePath, FileMode.Open, FileAccess.Read))
                        {
                            using (BinaryReader br = new BinaryReader(inputFs))
                            {
                                byte[] data = new byte[Constants.FILE_READ_CHUNK_SIZE];
                                int bytesRead = 0;

                                while ((bytesRead = br.Read(data, 0, data.Length)) > 0)
                                {
                                    bw.Write(data, 0, bytesRead);
                                }
                            }
                        }                        
                    }
                }

                ret = outputFilePath;
            }

            return ret;
        }

        public static int GetDspInterleave(string pGenhInterleave, string GenhChannels)
        {
            int dspInterleave = 0;
            int genhInterleave = (int)VGMToolbox.util.Encoding.GetIntFromString(pGenhInterleave);
            int genhChannels = (int)VGMToolbox.util.Encoding.GetIntFromString(GenhChannels);

            // Calculating the Interleave Type for DSP
            if (genhInterleave >= 8)
            {
                dspInterleave = 0; // Normal Interleave Layout
            }

            if (genhInterleave <= 7)
            {
                dspInterleave = 1; // Sub-FrameInterleave
            }

            if (genhChannels == 1)
            {
                dspInterleave = 2; // No layout (mono fies or whatever
            }

            return dspInterleave;
        }

        public static string GetFileEndLoopEnd(string pSourcePath, GenhCreationStruct pGenhCreationStruct)
        {
            int formatId = (int) VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Format);
            int headerSkip = (int) VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.HeaderSkip);
            int channels = (int) VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Channels);
            int interleave = (int) VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Interleave);
            int loopEnd = -1;

            int frames;
            int lastFrame;
            int dataSize;

            FileInfo fi = new FileInfo(Path.GetFullPath(pSourcePath));
            int fileLength = (int)fi.Length;

            switch (formatId)
            {
                case 0x00: // "0x00 - PlayStation 4-bit ADPCM"
                case 0x0E: // "0x0E - PlayStation 4-bit ADPCM (with bad flags)
                    loopEnd = ((fileLength - headerSkip) / 16 / channels * 28);
                    break;
                
                case 0x01: // "0x01 - XBOX 4-bit IMA ADPCM"
                    loopEnd = ((fileLength - headerSkip) / 36 / channels * 64);
                    break;
                
                case 0x02: // "0x02 - GameCube ADP/DTK 4-bit ADPCM"
                    loopEnd = ((fileLength - headerSkip) / 32 * 28);
                    break;

                case 0x03: // "0x03 - PCM RAW (Big Endian)"
                case 0x04: // "0x04 - PCM RAW (Little Endian)
                    loopEnd = ((fileLength - headerSkip) / 2 / channels);
                    break;

                case 0x05: // "0x05 - PCM RAW (8-Bit)"
                case 0x0D: // "0x05 - PCM RAW (8-Bit unsigned)"
                    loopEnd = ((fileLength - headerSkip) / channels);
                    break;

                case 0x06: // "0x06 - Squareroot-delta-exact 8-bit DPCM"
                    loopEnd = ((fileLength - headerSkip) / 1 / channels);
                    break;

                case 0x08: // "0x08 - MPEG Layer Audio File (MP1/2/3)"
                    // Not Implemented
                    break;

                case 0x07: // "0x07 - Interleaved DVI 4-Bit IMA ADPCM"
                case 0x09: // "0x09 - 4-bit IMA ADPCM"
                case 0x0A: // "0x0A - Yamaha AICA 4-bit ADPCM"    
                    loopEnd = ((fileLength - headerSkip) / channels * 2);
                    break;

                case 0x0B:
                    frames = (fileLength - headerSkip) / interleave;
                    lastFrame = (fileLength - headerSkip) - (frames * interleave);
                    loopEnd = (frames * (0x800 - (14 - 2))) + (lastFrame - (14 - 2));                    
                    break;

                case 0x0C: // "0x0C - Nintendo GameCube DSP 4-bit ADPCM"
                    loopEnd = ((fileLength - headerSkip) / channels / 8 * 14);
                    break;

                case 0x0F: // "0x0F - Microsoft 4-bit IMA ADPCM"
                    dataSize = (fileLength - headerSkip);
                    loopEnd = (dataSize / 0x800) * (0x800 - 4 * channels) * 2 / channels + ((dataSize % 0x800) != 0 ? (dataSize % 0x800 - 4 * channels) * 2 / channels : 0);
                    break;

                default:
                    break;
            }

            return loopEnd.ToString();
        }

        public static bool GetPsAdpcmLoop(string pSourcePath, 
            GenhCreationStruct pGenhCreationStruct, out string pLoopStart, out string pLoopEnd)
        {
            bool ret = false;
            pLoopStart = Genh.EMPTY_SAMPLE_COUNT;
            pLoopEnd = Genh.EMPTY_SAMPLE_COUNT;

            long loopStart = -1;
            long loopEnd = -1;

            long headerSkip = VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.HeaderSkip);
            int channels = (int)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Channels);
            string fullIncomingPath = Path.GetFullPath(pSourcePath);

            byte[] checkByte = new byte[1];

            FileInfo fi = new FileInfo(Path.GetFullPath(fullIncomingPath));
            if ((fi.Length - headerSkip) % 0x10 != 0)
            { 
                throw new Exception(String.Format("Error processing <{0}> Length of file minus the header skip value is not divisible by 0x10.  This is not a proper PS ADPCM rip.", fullIncomingPath));
            }
            
            using (BinaryReader br = new BinaryReader(File.OpenRead(fullIncomingPath)))            
            {   
                // Loop Start
                br.BaseStream.Position = headerSkip + 0x01;
                
                while (br.BaseStream.Position < fi.Length)
                {
                    br.Read(checkByte, 0, checkByte.Length);

                    if (checkByte[0] == 0x06)
                    {
                        loopStart = br.BaseStream.Position - 2 - headerSkip;
                        break;
                    }
                    else
                    {
                        br.BaseStream.Position += 0x10 - 0x01;
                    }
                }

                // Loop End
                br.BaseStream.Position = fi.Length - 0x0F;
                
                while (br.BaseStream.Position > headerSkip)
                {
                    br.Read(checkByte, 0, checkByte.Length);

                    if (checkByte[0] == 0x03)
                    {
                        loopEnd = br.BaseStream.Position + 0x0E - headerSkip;
                        break;
                    }
                    else
                    {
                        br.BaseStream.Position -= 0x10 - 0x01;
                    }
                }
            }
            
            if (loopStart >= 0)
            {
                pLoopStart = (loopStart / 16 / channels * 28).ToString();
                ret = true;
            }
            
            if (loopEnd >= 0)
            {
                pLoopEnd = (loopEnd / 16 / channels * 28).ToString();
                ret = true;
            }

            return ret;
        }        
    }
}
