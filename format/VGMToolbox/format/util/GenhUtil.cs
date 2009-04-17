using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VGMToolbox.format.util
{
    public class GenhUtil
    {
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
