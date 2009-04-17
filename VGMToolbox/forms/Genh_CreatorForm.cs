using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;

namespace VGMToolbox.forms
{
    public partial class Genh_CreatorForm : VgmtForm
    {
        public Genh_CreatorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.lblTitle.Text = "Create GENHs";
            this.tbSourceDirectory.Text = "<Source Directory>";
            this.btnDoTask.Text = "Create GENHs";

            this.loadFormats();
        }



        private void loadFormats()
        {
            this.comboFormat.Items.Add("0x00 - PlayStation 4-bit ADPCM");
            this.comboFormat.Items.Add("0x01 - XBOX 4-bit IMA ADPCM");
            this.comboFormat.Items.Add("0x02 - GameCube ADP/DTK 4-bit ADPCM");
            this.comboFormat.Items.Add("0x03 - PCM RAW (Big Endian)");
            this.comboFormat.Items.Add("0x04 - PCM RAW (Little Endian)");
            this.comboFormat.Items.Add("0x05 - PCM RAW (8-Bit)");
            this.comboFormat.Items.Add("0x06 - Squareroot-delta-exact 8-bit DPCM");
            this.comboFormat.Items.Add("0x07 - Interleaved DVI 4-Bit IMA ADPCM");
            this.comboFormat.Items.Add("0x08 - MPEG Layer Audio File (MP1/2/3)");
            this.comboFormat.Items.Add("0x09 - 4-bit IMA ADPCM");
            this.comboFormat.Items.Add("0x0A - Yamaha AICA 4-bit ADPCM");
            this.comboFormat.Items.Add("0x0B - Microsoft 4-bit ADPCM");
            this.comboFormat.Items.Add("0x0C - Nintendo GameCube DSP 4-bit ADPCM");
            this.comboFormat.Items.Add("0x0D - PCM RAW (8-Bit Unsigned)");
            this.comboFormat.Items.Add("0x0E - PlayStation 4-bit ADPCM (with bad flags)");
            this.comboFormat.Items.Add("0x0F - Microsoft 4-bit IMA ADPCM");
        }

        private void comboFormat_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFormat_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnBrowseDirectory_Click(object sender, EventArgs e)
        {
            this.tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }
        private void tbSourceDirectory_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(this.tbSourceDirectory.Text))
            {
                foreach (string f in Directory.GetFiles(this.tbSourceDirectory.Text))
                {
                    this.lbFiles.Items.Add(Path.GetFileName(f));
                }
            }
            else
            {
                this.lbFiles.Items.Clear();
            }
        }
    }
}
