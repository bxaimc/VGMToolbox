using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms
{
    public partial class Extract_SnakebiteGuiForm : AVgmtForm
    {
        public Extract_SnakebiteGuiForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            InitializeComponent();

            this.lblTitle.Text = "Simple Cutter (SnakeBite(TM) GUI)";
            this.btnDoTask.Text = "Cut File";

            this.rbEndAddress.Checked = true;
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            this.tbSourceFiles.Text = base.browseForFile(sender, e);
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            this.tbOutputFile.Text = base.browseForFileToSave(sender, e);
        }

        private void setRadioButtons()
        {
            if (rbEndAddress.Checked)
            {
                tbEndAddress.Enabled = true;
                tbEndAddress.ReadOnly = false;

                tbLength.Enabled = false;
                tbLength.ReadOnly = true;
            }
            else if (rbLength.Checked)
            {
                tbEndAddress.Enabled = false;
                tbEndAddress.ReadOnly = true;

                tbLength.Enabled = true;
                tbLength.ReadOnly = false;
            }
            else if (rbEndOfFile.Checked)
            {
                tbEndAddress.Enabled = false;
                tbEndAddress.ReadOnly = true;

                tbLength.Enabled = false;
                tbLength.ReadOnly = true;
            }
        }

        private void rbEndAddress_CheckedChanged(object sender, EventArgs e)
        {
            this.setRadioButtons();
        }

        private void rbLength_CheckedChanged(object sender, EventArgs e)
        {
            this.setRadioButtons();
        }

        private void rbEndOfFile_CheckedChanged(object sender, EventArgs e)
        {
            this.setRadioButtons();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        } 

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new SimpleCutterSnakebiteWorker();
        }
        protected override string getCancelMessage()
        {
            return "Cutting File...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Cutting File...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Cutting File...Begin";
        }

        private void tbSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            bool cutFiles = false;
            string warningMessage = "Please only drop a single file.";
            
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if ((s.Length > 1) ||
                ((s.Length == 1) && (Directory.Exists(s[0]))))
            {
                MessageBox.Show(warningMessage, "Error");
            }
            else
            {
                cutFiles = true;
            }

            if (cutFiles)
            { 
                this.cutTheFile(s);
            }
        }

        private void cutTheFile(string[] pPaths)
        {
            if (this.validateInputs())
            {
                SimpleCutterSnakebiteWorker.SimpleCutterSnakebiteStruct snbStruct =
                    new SimpleCutterSnakebiteWorker.SimpleCutterSnakebiteStruct();

                snbStruct.EndAddress = this.tbEndAddress.Text;
                snbStruct.Length = this.tbLength.Text;
                snbStruct.OutputFile = this.tbOutputFile.Text;
                snbStruct.SourcePaths = pPaths;
                snbStruct.StartOffset = this.tbStartAddress.Text;
                snbStruct.UseEndAddress = this.rbEndAddress.Checked;
                snbStruct.UseFileEnd = this.rbEndOfFile.Checked;
                snbStruct.UseLength = this.rbLength.Checked;

                base.backgroundWorker_Execute(snbStruct);
            }
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            string[] s = new string[] { this.tbSourceFiles.Text };
            this.cutTheFile(s);
        }

        private bool validateInputs()
        {
            bool ret = true;

            ret &= base.checkFileExists(this.tbSourceFiles.Text, this.lblSourceFiles.Text);
            ret &= base.checkTextBox(this.tbStartAddress.Text, this.lblStartAddress.Text);

            if (rbEndAddress.Checked)
            {
                ret &= base.checkTextBox(this.tbEndAddress.Text, this.rbEndAddress.Text);
            }

            if (rbLength.Checked)
            {
                ret &= base.checkTextBox(this.tbLength.Text, this.rbLength.Text);
            }

            if (this.tbSourceFiles.Text.Equals(this.tbOutputFile.Text))
            {
                MessageBox.Show("Input and Output files cannot be the same.", "Error");
                ret = false;
            }

            return ret;
        }
    }
}
