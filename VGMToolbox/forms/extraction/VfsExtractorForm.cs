using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms.extraction
{
    public partial class VfsExtractorForm : AVgmtForm
    {
        public VfsExtractorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.btnDoTask.Hide();
            this.lblTitle.Text = "Virtual File System (VFS) Extractor";
            this.tbOutput.Text = "Extract files from simple VFS archives.";

            // file count
            this.createFileCountOffsetSizeList();
            this.createFileCountOffsetEndianList();
            this.rbFileCountEndOffset.Checked = true;

            // file record
            this.rbUseVfsFileOffset.Checked = true;
            this.createFileRecordSizeList();
            this.createFileRecordOffsetSizeList();
            this.createFileRecordOffsetEndianList();

            this.doFileRecordOffsetRadioButtons();
            this.doCbOffsetMultiplier();

            this.rbUseVfsFileLength.Checked = true;
            this.createFileRecordLengthSizeList();
            this.createFileRecordLengthEndianList();
            this.doFileRecordLengthRadioButtons();

            this.doFileRecordNameCheckbox();
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new VfsExtractorWorker();
        }
        protected override string getCancelMessage()
        {
            return "Extracting files using VFS...Cancelled.";
        }
        protected override string getCompleteMessage()
        {
            return "Extracting files using VFS...Complete.";
        }
        protected override string getBeginMessage()
        {
            return "Extracting files using VFS...Begin.";
        }

        private void VfsExtractorForm_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void VfsExtractorForm_DragDrop(object sender, DragEventArgs e)
        {
            if (this.validateInputs())
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                
                VfsExtractorWorker.VfsExtractorStruct bgStruct = new VfsExtractorWorker.VfsExtractorStruct();
                bgStruct.SourcePaths = s;

                // File Count            
                bgStruct.UseFileCountOffset = this.rbOffsetBasedFileCount.Checked;
                bgStruct.FileCountValue = this.tbFileCountValue.Text;
                bgStruct.FileCountValueOffset = this.tbFileCountOffset.Text;
                bgStruct.FileCountValueLength = this.comboFileCountOffsetSize.Text;
                bgStruct.FileCountValueIsLittleEndian = this.comboFileCountByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN);
                bgStruct.FileCountEndOffset = this.tbFileCountEndOffset.Text;

                // File Records
                bgStruct.FileRecordsStartOffset = this.tbFileRecordsBeginOffset.Text;
                bgStruct.FileRecordSize = this.comboFileRecordSize.Text;
                bgStruct.UsePreviousFilesSizeToDetermineOffset = this.rbUseFileSizeToDetermineOffset.Checked;
                bgStruct.BeginCuttingFilesAtOffset = this.tbUseFileLengthBeginOffset.Text;

                bgStruct.FileRecordOffsetOffset = this.tbFileRecordOffsetOffset.Text;
                bgStruct.FileRecordOffsetLength = this.comboFileRecordOffsetSize.Text;
                bgStruct.FileRecordOffsetIsLittleEndian = this.comboFileRecordOffsetByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN);
                bgStruct.UseFileRecordOffsetMultiplier = this.cbUseOffsetMultiplier.Checked;
                bgStruct.FileRecordOffsetMultiplier = this.tbFileRecordOffsetMultiplier.Text;

                bgStruct.FileRecordLengthOffset = this.tbFileRecordLengthOffset.Text;
                bgStruct.FileRecordLengthLength = this.comboFileRecordLengthSize.Text;
                bgStruct.FileRecordLengthIsLittleEndian = this.comboFileRecordLengthByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN);
                bgStruct.UseLocationOfNextFileToDetermineLength = this.rbUseOffsetsToDetermineLength.Checked;

                bgStruct.FileRecordNameIsPresent = this.cbFileNameIsPresent.Checked;
                bgStruct.FileRecordNameOffset = this.tbFileRecordNameOffset.Text;
                bgStruct.FileRecordNameLength = this.tbFileRecordNameSize.Text;

                base.backgroundWorker_Execute(bgStruct);
            }
        }

        // Validate Inputs
        private bool validateInputs()
        {
            bool isValid = true;

            isValid &= this.validateFileCountSection();

            return isValid;
        }

        //////////////
        // File Count
        //////////////
        private void createFileCountOffsetSizeList()
        {
            this.comboFileCountOffsetSize.Items.Add("1");
            this.comboFileCountOffsetSize.Items.Add("2");
            this.comboFileCountOffsetSize.Items.Add("4");
        }
        private void createFileCountOffsetEndianList()
        {
            this.comboFileCountByteOrder.Items.Add(VfsExtractorWorker.BIG_ENDIAN);
            this.comboFileCountByteOrder.Items.Add(VfsExtractorWorker.LITTLE_ENDIAN);
        }
        
        private void doFileCountRadioButtons()
        {
            if (this.rbUserEnteredFileCount.Checked)
            {
                this.tbFileCountValue.Enabled = true;
                this.tbFileCountValue.ReadOnly = false;

                this.tbFileCountOffset.Clear();
                this.tbFileCountOffset.Enabled = false;
                this.tbFileCountOffset.ReadOnly = true;
                this.comboFileCountOffsetSize.Enabled = false;
                this.comboFileCountByteOrder.Enabled = false;

                this.tbFileCountEndOffset.Clear();
                this.tbFileCountEndOffset.Enabled = false;
                this.tbFileCountEndOffset.ReadOnly = true;
            }
            else if (this.rbOffsetBasedFileCount.Checked)
            {
                this.tbFileCountValue.Clear();
                this.tbFileCountValue.Enabled = false;
                this.tbFileCountValue.ReadOnly = true;

                this.tbFileCountOffset.Enabled = true;
                this.tbFileCountOffset.ReadOnly = false;
                this.comboFileCountOffsetSize.Enabled = true;
                this.comboFileCountByteOrder.Enabled = true;

                this.tbFileCountEndOffset.Clear();
                this.tbFileCountEndOffset.Enabled = false;
                this.tbFileCountEndOffset.ReadOnly = true;
            }
            else if (this.rbFileCountEndOffset.Checked)
            {
                this.tbFileCountValue.Clear();
                this.tbFileCountValue.Enabled = false;
                this.tbFileCountValue.ReadOnly = true;

                this.tbFileCountOffset.Clear();
                this.tbFileCountOffset.Enabled = false;
                this.tbFileCountOffset.ReadOnly = true;
                this.comboFileCountOffsetSize.Enabled = false;
                this.comboFileCountByteOrder.Enabled = false;

                this.tbFileCountEndOffset.Enabled = true;
                this.tbFileCountEndOffset.ReadOnly = false;            
            }
        }
        private void rbUserEnteredFileCount_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileCountRadioButtons();
        }
        private void rbOffsetBasedFileCount_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileCountRadioButtons();
        }
        private void rbFileCountEndOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileCountRadioButtons();
        }

        private bool validateFileCountSection()
        {
            bool isValid = true;
            
            if (this.rbUserEnteredFileCount.Checked)
            {
                isValid &= base.checkTextBox(this.tbFileCountValue.Text, this.rbUserEnteredFileCount.Text);               
            }
            else if (this.rbOffsetBasedFileCount.Checked)
            {
                isValid &= base.checkTextBox(this.tbFileCountOffset.Text, this.rbOffsetBasedFileCount.Text);
                isValid &= base.checkTextBox(this.comboFileCountOffsetSize.Text, "File Count Offset Size");
                isValid &= base.checkTextBox(this.comboFileCountByteOrder.Text, "File Count Offset Byte Order");
            }
            else if (this.rbFileCountEndOffset.Checked)
            {
                isValid &= base.checkTextBox(this.tbFileCountEndOffset.Text, this.rbFileCountEndOffset.Text);
            }

            return isValid;
        }        

        ///////////////
        // File Record
        ///////////////
        private void createFileRecordSizeList()
        {
            this.comboFileRecordSize.Items.Add("0x02");
            this.comboFileRecordSize.Items.Add("0x04");
            this.comboFileRecordSize.Items.Add("0x08");
            this.comboFileRecordSize.Items.Add("0x0C");
            this.comboFileRecordSize.Items.Add("0x10");
            this.comboFileRecordSize.Items.Add("0x14");
            this.comboFileRecordSize.Items.Add("0x18");
            this.comboFileRecordSize.Items.Add("0x1C");
            this.comboFileRecordSize.Items.Add("0x20");
        }
        private void createFileRecordOffsetSizeList()
        {
            this.comboFileRecordOffsetSize.Items.Add("1");
            this.comboFileRecordOffsetSize.Items.Add("2");
            this.comboFileRecordOffsetSize.Items.Add("4");
        }
        private void createFileRecordOffsetEndianList()
        {
            this.comboFileRecordOffsetByteOrder.Items.Add(VfsExtractorWorker.BIG_ENDIAN);
            this.comboFileRecordOffsetByteOrder.Items.Add(VfsExtractorWorker.LITTLE_ENDIAN);
        }

        private void doFileRecordOffsetRadioButtons()
        {
            if (this.rbUseVfsFileOffset.Checked)
            {
                this.tbFileRecordOffsetOffset.Enabled = true;
                this.tbFileRecordOffsetOffset.ReadOnly = false;
                this.comboFileRecordOffsetSize.Enabled = true;
                this.comboFileRecordOffsetByteOrder.Enabled = true;
                
                this.cbUseOffsetMultiplier.Enabled = true;                
                this.tbFileRecordOffsetMultiplier.Enabled = true;
                this.tbFileRecordOffsetMultiplier.ReadOnly = false;

                this.tbUseFileLengthBeginOffset.Clear();
                this.tbUseFileLengthBeginOffset.Enabled = false;
                this.tbUseFileLengthBeginOffset.ReadOnly = true;
            }
            else if (this.rbUseFileSizeToDetermineOffset.Checked)
            {
                this.tbFileRecordOffsetOffset.Clear();
                this.tbFileRecordOffsetOffset.Enabled = false;
                this.tbFileRecordOffsetOffset.ReadOnly = true;
                this.comboFileRecordOffsetSize.Enabled = false;
                this.comboFileRecordOffsetByteOrder.Enabled = false;

                this.cbUseOffsetMultiplier.Checked = false;
                this.cbUseOffsetMultiplier.Enabled = false;
                this.tbFileRecordOffsetMultiplier.Clear();
                this.tbFileRecordOffsetMultiplier.Enabled = false;
                this.tbFileRecordOffsetMultiplier.ReadOnly = true;
                
                this.tbUseFileLengthBeginOffset.Enabled = true;
                this.tbUseFileLengthBeginOffset.ReadOnly = false;            
            }

            this.doCbOffsetMultiplier();
        }
        private void rbUseVfsFileOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileRecordOffsetRadioButtons();
        }
        private void rbUseFileSizeToDetermineOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileRecordOffsetRadioButtons();
        }
        
        private void doCbOffsetMultiplier()
        {
            if (this.cbUseOffsetMultiplier.Checked)
            {
                this.tbFileRecordOffsetMultiplier.Enabled = true;
                this.tbFileRecordOffsetMultiplier.ReadOnly = false;
            }
            else
            {
                this.tbFileRecordOffsetMultiplier.Clear();
                this.tbFileRecordOffsetMultiplier.Enabled = false;
                this.tbFileRecordOffsetMultiplier.ReadOnly = true;
            }
        }
        private void cbUseOffsetMultiplier_CheckedChanged(object sender, EventArgs e)
        {
            this.doCbOffsetMultiplier();
        }

        private void doFileRecordLengthRadioButtons()
        {
            if (this.rbUseVfsFileLength.Checked)
            {
                this.tbFileRecordLengthOffset.Enabled = true;
                this.tbFileRecordLengthOffset.ReadOnly = false;
                this.comboFileRecordLengthSize.Enabled = true;
                this.comboFileRecordLengthByteOrder.Enabled = true;
            }
            else if (this.rbUseOffsetsToDetermineLength.Checked)
            {
                this.tbFileRecordLengthOffset.Enabled = false;
                this.tbFileRecordLengthOffset.ReadOnly = true;
                this.comboFileRecordLengthSize.Enabled = false;
                this.comboFileRecordLengthByteOrder.Enabled = false;            
            }
        }
        private void createFileRecordLengthSizeList()
        {
            this.comboFileRecordLengthSize.Items.Add("1");
            this.comboFileRecordLengthSize.Items.Add("2");
            this.comboFileRecordLengthSize.Items.Add("4");
        }
        private void createFileRecordLengthEndianList()
        {
            this.comboFileRecordLengthByteOrder.Items.Add(VfsExtractorWorker.BIG_ENDIAN);
            this.comboFileRecordLengthByteOrder.Items.Add(VfsExtractorWorker.LITTLE_ENDIAN);
        }
        private void rbUseVfsFileLength_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileRecordLengthRadioButtons();
        }
        private void rbUseOffsetsToDetermineLength_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileRecordLengthRadioButtons();
        }

        private void doFileRecordNameCheckbox()
        {
            if (this.cbFileNameIsPresent.Checked)
            {
                this.tbFileRecordNameOffset.Enabled = true;
                this.tbFileRecordNameOffset.ReadOnly = false;
                this.tbFileRecordNameSize.Enabled = true;
                this.tbFileRecordNameSize.ReadOnly = false;
            }
            else
            {
                this.tbFileRecordNameOffset.Enabled = false;
                this.tbFileRecordNameOffset.ReadOnly = true;
                this.tbFileRecordNameSize.Enabled = false;
                this.tbFileRecordNameSize.ReadOnly = true;            
            }
        }
        private void cbFileNameIsPresent_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileRecordNameCheckbox();
        }
    }
}
