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
                    bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Interleave));
                    bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.Frequency));
                    bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.LoopStart));
                    bw.Write((UInt32)VGMToolbox.util.Encoding.GetIntFromString(pGenhCreationStruct.LoopEnd));                        
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
                    bw.Write(Path.GetFileName(pSourcePath));

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
            if (genhInterleave > 7)
            {
                dspInterleave = 0; // Normal Interleave Layout
            }

            if (genhInterleave < 8)
            {
                dspInterleave = 1; // Sub-FrameInterleave
            }

            if (genhChannels == 1)
            {
                dspInterleave = 2; // No layout (mono fies or whatever
            }

            return dspInterleave;
        }
        
        /*
        private void cmdFindLoopPS2_Click(object sender, EventArgs e)
        {

            FileStream strInputFileEditor = new FileStream(Path.GetFullPath(this.listBox1.Text), FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(strInputFileEditor);
            FileInfo fi = new FileInfo(Path.GetFullPath(this.listBox1.Text));



            long fileLength = fi.Length;
            long readOffsetLoopStart = 0;
            long readOffsetLoopEnd = 0;
            int LoopStartFound = 0;
            int LoopEndFound = 0;
            int GENHChannels = int.Parse(this.txtChannelsCreator.Text);

            // Loop Start
            while (readOffsetLoopStart < fileLength)
            {
                br.BaseStream.Position = readOffsetLoopStart + 0x01;
                byte LoopFlag = br.ReadByte();

                // Loop Start
                if (LoopFlag == 0x06)
                {
                    LoopStartFound = 1;
                    //this.txtLoopStartCreator.Text = (readOffset / 16 / GENHChannels * 28).ToString();
                    break;
                }
                readOffsetLoopStart += 0x10;
            }
            // Loop Start End

            // Loop End
            readOffsetLoopEnd = fileLength;
            while (readOffsetLoopEnd > 0)
            {
                br.BaseStream.Position = readOffsetLoopEnd - 0xF;
                byte LoopFlag = br.ReadByte();

                // Loop Start 
                if (LoopFlag == 0x03)
                {
                    LoopEndFound = 1;
                    //this.txtLoopEndCreator.Text = (readOffset / 16 / GENHChannels * 28).ToString();
                    break;
                }
                readOffsetLoopEnd -= 0x10;
            }

            if (LoopStartFound == 1)
            {
                this.txtLoopStartCreator.Text = (readOffsetLoopStart / 16 / GENHChannels * 28).ToString();
            }
            else if (LoopStartFound == 0)
            {
                this.toolStripStatus.Text = "Loop Start not found";
            }

            if (LoopEndFound == 1)
            {
                this.txtLoopEndCreator.Text = (readOffsetLoopEnd / 16 / GENHChannels * 28).ToString();
            }
            else if (LoopEndFound == 0)
            {
                this.toolStripStatus.Text = "Loop End not found";
            }


            if (LoopStartFound == 0 && LoopEndFound == 0)
            {
                this.toolStripStatus.Text = "Loop Values not found";
            }

        }
        */    
    }
}
