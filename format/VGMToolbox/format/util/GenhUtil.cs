using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.util;

namespace VGMToolbox.format.util
{
    public struct GenhCreationStruct
    {
        public bool doCreation;
        public bool doEdit;
        public bool doExtract;
        
        public string Format;
        public string HeaderSkip;
        public string Interleave;
        public string Channels;
        
        public string Frequency;
        public bool UseFrequencyOffset { set; get; }
        public OffsetDescription FrequencyOffsetDescription { set; get; }

        public string LoopStart;
        public bool UseLoopStartOffset { set; get; }
        public OffsetDescription LoopStartOffsetDescription { set; get; }
        public bool DoLoopStartBytesToSamples { set; get; }

        public string LoopEnd;
        public bool UseLoopEndOffset { set; get; }
        public OffsetDescription LoopEndOffsetDescription { set; get; }
        public bool DoLoopEndBytesToSamples { set; get; }

        public bool NoLoops;
        public bool UseFileEnd;
        public bool FindLoop;

        public string CoefRightChannel;
        public string CoefLeftChannel;
        public bool CapcomHack;

        public bool OutputHeaderOnly;
        public string[] SourcePaths;
    }

    public sealed class GenhUtil
    {
        public const int UNKNOWN_SAMPLE_COUNT = -1;
        private const int SONY_ADPCM_LOOP_HACK_BYTE_COUNT = 0x30;
        private const string GENH_BATCH_FILE_NAME = "vgmt_genh_copy.bat";

        private GenhUtil() { }

        public static bool IsGenhFile(string path)
        {
            bool ret = false;

            using (FileStream typeFs = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                Type dataType = FormatUtil.getObjectType(typeFs);

                if (dataType != null && dataType.Name.Equals("Genh"))
                {
                    ret = true;
                }
            }

            return ret;
        }
        
        public static string CreateGenhFile(string pSourcePath, GenhCreationStruct pGenhCreationStruct)
        {
            string ret = String.Empty;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            
            int dspInterleaveType =
                GetDspInterleave(pGenhCreationStruct.Interleave, pGenhCreationStruct.Channels);

            //--------------
            // looping info
            //--------------
            if (pGenhCreationStruct.NoLoops)
            {
                pGenhCreationStruct.LoopStart = Genh.EMPTY_SAMPLE_COUNT;
                pGenhCreationStruct.LoopEnd = GetFileEndLoopEnd(pSourcePath, pGenhCreationStruct);
            }
            else if (pGenhCreationStruct.UseFileEnd)
            {
                pGenhCreationStruct.LoopEnd = GetFileEndLoopEnd(pSourcePath, pGenhCreationStruct);
            }
            else if (pGenhCreationStruct.FindLoop)
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
                    pGenhCreationStruct.LoopEnd = GetFileEndLoopEnd(pSourcePath, pGenhCreationStruct);
                }
            }

            //---------------------
            // offset based values
            //---------------------
            if (pGenhCreationStruct.UseLoopStartOffset || 
                pGenhCreationStruct.UseLoopEndOffset ||
                pGenhCreationStruct.UseFrequencyOffset)
            {
                int formatId = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Format);
                int channels = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Channels);
                int interleave = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Interleave);
                
                using (FileStream inputFs = File.Open(pSourcePath, FileMode.Open, FileAccess.Read))
                {
                    if (pGenhCreationStruct.UseLoopStartOffset)
                    {
                        long loopStart = ParseFile.GetVaryingByteValueAtAbsoluteOffset(inputFs, pGenhCreationStruct.LoopStartOffsetDescription);

                        if (pGenhCreationStruct.DoLoopStartBytesToSamples)
                        {
                            loopStart = BytesToSamples(formatId, (int)loopStart, channels, interleave);
                        }

                        pGenhCreationStruct.LoopStart = ((int)loopStart).ToString();
                    }

                    if (pGenhCreationStruct.UseLoopEndOffset)
                    {
                        long loopEnd = ParseFile.GetVaryingByteValueAtAbsoluteOffset(inputFs, pGenhCreationStruct.LoopEndOffsetDescription);

                        if (pGenhCreationStruct.DoLoopEndBytesToSamples)
                        {
                            loopEnd = BytesToSamples(formatId, (int)loopEnd, channels, interleave);
                        }
                        
                        pGenhCreationStruct.LoopEnd = ((int)loopEnd).ToString();
                    }

                    if (pGenhCreationStruct.UseFrequencyOffset)
                    {
                        long frequency = ParseFile.GetVaryingByteValueAtAbsoluteOffset(inputFs, pGenhCreationStruct.FrequencyOffsetDescription);
                        pGenhCreationStruct.Frequency = ((int)frequency).ToString();                    
                    }
                }
            }

            FileInfo fi = new FileInfo(Path.GetFullPath(pSourcePath));
            UInt32 fileLength = (UInt32)fi.Length;
            string outputFilePath;

            if (pGenhCreationStruct.OutputHeaderOnly)
            {
                outputFilePath = Path.ChangeExtension(pSourcePath, Genh.FILE_EXTENSION_HEADER);
            }
            else
            {
                outputFilePath = Path.ChangeExtension(pSourcePath, Genh.FILE_EXTENSION);
            }
            
            // write the file
            using (FileStream outputFs = File.Open(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(outputFs))
                {
                    bw.Write(Genh.ASCII_SIGNATURE);
                    bw.Write((UInt32)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Channels));
                    bw.Write((Int32)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Interleave));
                    bw.Write((UInt32)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Frequency));
                    bw.Write((UInt32)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.LoopStart));

                    if (!String.IsNullOrEmpty(pGenhCreationStruct.LoopEnd))
                    {
                        bw.Write((UInt32)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.LoopEnd));
                    }
                    else
                    {
                        bw.Write(new byte[] {0x00, 0x00, 0x00, 0x00});
                    }

                    bw.Write((UInt32)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Format));
                    bw.Write((UInt32)(VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.HeaderSkip) + Genh.GENH_HEADER_SIZE));
                    bw.Write((UInt32)Genh.GENH_HEADER_SIZE);

                    if (VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Format) == 12)
                    {
                        bw.Write((UInt32)(VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.CoefLeftChannel) + Genh.GENH_HEADER_SIZE));
                        bw.Write((UInt32)(VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.CoefRightChannel) + Genh.GENH_HEADER_SIZE));
                        bw.Write((UInt32)dspInterleaveType);

                        if (pGenhCreationStruct.CapcomHack)
                        {
                            bw.Write((UInt32)1);
                            bw.Write((UInt32)(VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.CoefLeftChannel) + Genh.GENH_HEADER_SIZE + 0x10));
                            bw.Write((UInt32)(VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.CoefRightChannel) + Genh.GENH_HEADER_SIZE + 0x10));
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

                    // create batch script or add input file
                    if (pGenhCreationStruct.OutputHeaderOnly)
                    {
                        AddCopyItemToBatchFile(Path.GetDirectoryName(pSourcePath), pSourcePath, outputFilePath);
                    }
                    else
                    {
                        using (FileStream inputFs = File.Open(pSourcePath, FileMode.Open, FileAccess.Read))
                        {
                            using (BinaryReader br = new BinaryReader(inputFs))
                            {
                                byte[] data = new byte[Constants.FileReadChunkSize];
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
            int genhInterleave = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhInterleave);
            int genhChannels = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(GenhChannels);

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
            int formatId = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Format);
            int headerSkip = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.HeaderSkip);
            int channels = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Channels);
            int interleave = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Interleave);
            int loopEnd = -1;

            int frames;
            int lastFrame;
            int dataSize;

            FileInfo fi = new FileInfo(Path.GetFullPath(pSourcePath));
            int fileLength = (int)fi.Length;
            
            loopEnd = BytesToSamples(formatId, (fileLength - headerSkip), channels, interleave);
            
            /*
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

                case 0x0B: // 0x0B - Microsoft 4-bit ADPCM
                    frames = (fileLength - headerSkip) / interleave;
                    lastFrame = (fileLength - headerSkip) - (frames * interleave);
                    loopEnd = (frames * (interleave - (14 - 2))) + (lastFrame - (14 - 2));                    
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
            */
            return loopEnd.ToString();
        }

        public static int BytesToSamples(int formatId, int byteValue, int channels, int interleave)
        { 
            int sampleCount = UNKNOWN_SAMPLE_COUNT;
            int frames, lastFrame;

            switch (formatId)
            {
                case 0x00: // "0x00 - PlayStation 4-bit ADPCM"
                case 0x0E: // "0x0E - PlayStation 4-bit ADPCM (with bad flags)
                    sampleCount = (byteValue / 16 / channels * 28);
                    break;

                case 0x01: // "0x01 - XBOX 4-bit IMA ADPCM"
                    sampleCount = (byteValue / 36 / channels * 64);
                    break;

                case 0x02: // "0x02 - GameCube ADP/DTK 4-bit ADPCM"
                    sampleCount = (byteValue / 32 * 28);
                    break;

                case 0x03: // "0x03 - PCM RAW (Big Endian)"
                case 0x04: // "0x04 - PCM RAW (Little Endian)
                    sampleCount = (byteValue / 2 / channels);
                    break;

                case 0x05: // "0x05 - PCM RAW (8-Bit)"
                case 0x0D: // "0x0D - PCM RAW (8-Bit unsigned)"
                    sampleCount = (byteValue / channels);
                    break;

                case 0x06: // "0x06 - Squareroot-delta-exact 8-bit DPCM"
                    sampleCount = (byteValue / 1 / channels);
                    break;

                case 0x08: // "0x08 - MPEG Layer Audio File (MP1/2/3)"
                    // Not Implemented
                    break;

                case 0x07: // "0x07 - Interleaved DVI 4-Bit IMA ADPCM"
                case 0x09: // "0x09 - 4-bit IMA ADPCM"
                case 0x0A: // "0x0A - Yamaha AICA 4-bit ADPCM"    
                case 0x11: // "0x11 - Apple Quicktime 4-bit IMA ADPCM"    
                    sampleCount = (byteValue / channels * 2);
                    break;

                case 0x0B: // 0x0B - Microsoft 4-bit ADPCM
                    frames = byteValue / interleave;
                    lastFrame = byteValue - (frames * interleave);
                    sampleCount = (frames * (interleave - (14 - 2))) + (lastFrame - (14 - 2));
                    break;

                case 0x0C: // "0x0C - Nintendo GameCube DSP 4-bit ADPCM"
                    sampleCount = (byteValue / channels / 8 * 14);
                    break;

                case 0x0F: // "0x0F - Microsoft 4-bit IMA ADPCM"
                    sampleCount = (byteValue / 0x800) * (0x800 - 4 * channels) * 2 / channels + ((byteValue % 0x800) != 0 ? (byteValue % 0x800 - 4 * channels) * 2 / channels : 0);
                    break;

                default:
                    sampleCount = UNKNOWN_SAMPLE_COUNT;
                    break;

            }

            return sampleCount;
        }

        public static bool GetPsAdpcmLoop(string pSourcePath, 
            GenhCreationStruct pGenhCreationStruct, out string pLoopStart, out string pLoopEnd)
        {
            bool ret = false;
            pLoopStart = Genh.EMPTY_SAMPLE_COUNT;
            pLoopEnd = Genh.EMPTY_SAMPLE_COUNT;

            long loopStart = -1;
            long loopEnd = -1;
            
            long loopCheckBytesOffset;
            byte[] possibleLoopBytes;

            long headerSkip = VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.HeaderSkip);
            int channels = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Channels);
            int interleave = (int)VGMToolbox.util.ByteConversion.GetLongValueFromString(pGenhCreationStruct.Interleave);
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

                        //if (channels > 1)
                        //{
                        //    loopEnd -= interleave;
                        //}

                        break;
                    }
                    else if (br.BaseStream.Position >= 0x11)
                    {
                        br.BaseStream.Position -= 0x11;
                    }
                    else
                    {
                        br.BaseStream.Position = headerSkip;
                    }
                }

                // if loop end found but start not found, try alternate method
                if ((loopEnd >= 0) && (loopStart < 0))
                {
                    loopCheckBytesOffset = loopEnd + headerSkip - SONY_ADPCM_LOOP_HACK_BYTE_COUNT;
                    possibleLoopBytes = ParseFile.ParseSimpleOffset(
                                                br.BaseStream,
                                                loopCheckBytesOffset, 
                                                SONY_ADPCM_LOOP_HACK_BYTE_COUNT - 0x10);
                    loopStart = ParseFile.GetNextOffset(br.BaseStream, 0, possibleLoopBytes, true);

                    if ((loopStart > 0) && (loopStart < loopCheckBytesOffset))
                    {
                        loopStart += SONY_ADPCM_LOOP_HACK_BYTE_COUNT - headerSkip;
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

        public static string ExtractGenhFile(string pSourcePath, bool extractHeaderToFile, bool outputExtractionLog, bool outputExtractionFile)
        {
            string outputFileName = null;
            string headerOutputFileName = null;

            if (IsGenhFile(pSourcePath))
            {
                using (FileStream fs = File.Open(pSourcePath, FileMode.Open, FileAccess.Read))
                {
                    Genh genhFile = new Genh();
                    genhFile.Initialize(fs, pSourcePath);
                    Int32 headerLength = BitConverter.ToInt32(genhFile.HeaderLength, 0);
                    Int32 originalFileSize = BitConverter.ToInt32(genhFile.OriginalFileSize, 0);
                    string originalFileName = System.Text.Encoding.ASCII.GetString(genhFile.OriginalFileName);
                    
                    outputFileName = Path.Combine(Path.GetDirectoryName(pSourcePath), originalFileName).Trim();                    
                    ParseFile.ExtractChunkToFile(fs, headerLength, originalFileSize, outputFileName, outputExtractionLog, outputExtractionFile);                    

                    if (extractHeaderToFile)
                    {
                        headerOutputFileName = Path.Combine(Path.GetDirectoryName(pSourcePath), String.Format("{0}{1}", originalFileName.Trim(), Genh.FILE_EXTENSION_HEADER)).Trim();
                        ParseFile.ExtractChunkToFile(fs, 0, headerLength, headerOutputFileName, outputExtractionLog, outputExtractionFile);
                    }

                    FileInfo fi = new FileInfo(outputFileName);
                    if (fi.Length != (long)originalFileSize)
                    {
                        throw new IOException(String.Format("Extracted file <{0}> size did not match size in header of <{1}>: 0x{2}{3}", outputFileName, 
                            pSourcePath, originalFileSize.ToString("X8"), Environment.NewLine));
                    }
                }            
            }

            return outputFileName;
        }

        public static string AddCopyItemToBatchFile(string destinationFolder, string originalFileName,
            string headerFileName)
        {
            string batchOutputPath = Path.Combine(destinationFolder, GENH_BATCH_FILE_NAME);
            string copyStatementFormat = "copy /b \"{0}\" + \"{1}\" \"{2}\"";

            using (StreamWriter sw = new StreamWriter(File.Open(batchOutputPath, FileMode.Append, FileAccess.Write)))
            {
                sw.WriteLine(String.Format(copyStatementFormat, Path.GetFileName(headerFileName), Path.GetFileName(originalFileName), Path.GetFileName(Path.ChangeExtension(originalFileName, Genh.FILE_EXTENSION))));
            }

            return batchOutputPath;
        }

        public static GenhCreationStruct GetGenhCreationStruct(Genh genhItem)
        { 
            GenhCreationStruct gcStruct = new GenhCreationStruct();

            int formatValue = BitConverter.ToInt32(genhItem.Identifier, 0);
            gcStruct.Format = formatValue.ToString();
            gcStruct.HeaderSkip = "0x" + ((BitConverter.ToInt32(genhItem.AudioStart, 0) - BitConverter.ToInt32(genhItem.HeaderLength, 0))).ToString("X4");
            gcStruct.Interleave = "0x" + BitConverter.ToInt32(genhItem.Interleave, 0).ToString("X4");
            gcStruct.Channels = BitConverter.ToInt32(genhItem.Channels, 0).ToString();
        
            gcStruct.Frequency = BitConverter.ToInt32(genhItem.Frequency, 0).ToString();

            int loopStartValue = BitConverter.ToInt32(genhItem.LoopStart, 0);
            if (loopStartValue > -1)
            {
                gcStruct.LoopStart = loopStartValue.ToString();
            }
            else
            {
                gcStruct.LoopStart = String.Empty;          
            }

            int loopEndValue = BitConverter.ToInt32(genhItem.LoopEnd, 0);
            if (loopEndValue > 0)
            {
                gcStruct.LoopEnd = loopEndValue.ToString();
                gcStruct.UseLoopEndOffset = true;
            }
            else
            {
                gcStruct.LoopEnd = String.Empty;
                gcStruct.UseLoopEndOffset = false;            
            }

            if (formatValue == 12)
            {
                /*
                    public string CoefRightChannel;
                    public string CoefLeftChannel;
                    public bool CapcomHack;

                */
            }
            
            return gcStruct;
        }
    }
}
