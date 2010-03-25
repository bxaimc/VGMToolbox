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

namespace VGMToolbox.forms.extraction
{
    public partial class VfsExtractorForm : AVgmtForm
    {
        private static readonly string PLUGIN_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "plugins"), "VFSExtractor");                
        
        public VfsExtractorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.btnDoTask.Text = "Extract Files";
            this.lblTitle.Text = "Virtual File System (VFS) Extractor";
            this.tbOutput.Text =  "- Extract files from simple VFS archives." + System.Environment.NewLine;
            this.tbOutput.Text += "1) Enter settings." + System.Environment.NewLine;
            this.tbOutput.Text += "2) Select data file and header file and click 'Extract Files'" + System.Environment.NewLine;
            this.tbOutput.Text += "          OR" + System.Environment.NewLine;
            this.tbOutput.Text += "Drag and Drop data files onto the application (Header file not supported for Drag and Drop)." + System.Environment.NewLine;

            // header file
            this.doUserHeaderFileCheckbox();

            // file count
            this.createHeaderSizeOffsetSizeList();
            this.createHeaderSizeOffsetEndianList();
            
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
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);            
            this.extractFiles(s, false);            
        }

        private void extractFiles(string[] inputPaths, bool includeHeaderPath)
        {
            if (this.validateInputs())
            {
                VfsExtractorWorker.VfsExtractorStruct bgStruct = new VfsExtractorWorker.VfsExtractorStruct();
                bgStruct.SourcePaths = inputPaths;

                if (includeHeaderPath)
                {
                    bgStruct.HeaderSourcePath = this.tbHeaderFilePath.Text;
                }

                // File Count            
                bgStruct.UseFileCountOffset = this.rbOffsetBasedFileCount.Checked;
                bgStruct.FileCountValue = this.tbFileCountValue.Text;
                bgStruct.FileCountValueOffset = this.tbFileCountOffset.Text;
                bgStruct.FileCountValueLength = this.comboFileCountOffsetSize.Text;
                bgStruct.FileCountValueIsLittleEndian = this.comboFileCountByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN);

                // header size
                bgStruct.UseHeaderSizeOffset = this.rbHeaderSizeOffset.Checked;
                bgStruct.HeaderSizeValueOffset = this.tbHeaderSizeOffset.Text;
                bgStruct.HeaderSizeValueLength = this.comboHeaderSizeOffsetSize.Text;
                bgStruct.HeaderSizeValueIsLittleEndian = this.comboHeaderSizeByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN);

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
        private void createHeaderSizeOffsetSizeList()
        {
            this.comboHeaderSizeOffsetSize.Items.Add("1");
            this.comboHeaderSizeOffsetSize.Items.Add("2");
            this.comboHeaderSizeOffsetSize.Items.Add("4");
        }
        private void createHeaderSizeOffsetEndianList()
        {
            this.comboHeaderSizeByteOrder.Items.Add(VfsExtractorWorker.BIG_ENDIAN);
            this.comboHeaderSizeByteOrder.Items.Add(VfsExtractorWorker.LITTLE_ENDIAN);
        }
        
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
            if (this.rbHeaderSizeOffset.Checked)
            {
                this.tbHeaderSizeOffset.Enabled = true;
                this.tbHeaderSizeOffset.ReadOnly = false;
                this.comboHeaderSizeOffsetSize.Enabled = true;
                this.comboHeaderSizeByteOrder.Enabled = true;

                this.tbFileCountValue.Clear();
                this.tbFileCountValue.Enabled = false;
                this.tbFileCountValue.ReadOnly = true;

                this.tbFileCountOffset.Clear();
                this.tbFileCountOffset.Enabled = false;
                this.tbFileCountOffset.ReadOnly = true;
                this.comboFileCountOffsetSize.Enabled = false;
                this.comboFileCountByteOrder.Enabled = false;

                this.tbFileCountEndOffset.Clear();
                this.tbFileCountEndOffset.Enabled = false;
                this.tbFileCountEndOffset.ReadOnly = true;
            }            
            else if (this.rbUserEnteredFileCount.Checked)
            {
                this.tbHeaderSizeOffset.Clear();
                this.tbHeaderSizeOffset.Enabled = false;
                this.tbHeaderSizeOffset.ReadOnly = true;
                this.comboHeaderSizeOffsetSize.Enabled = false;
                this.comboHeaderSizeByteOrder.Enabled = false;
                
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
                this.tbHeaderSizeOffset.Clear();
                this.tbHeaderSizeOffset.Enabled = false;
                this.tbHeaderSizeOffset.ReadOnly = true;
                this.comboHeaderSizeOffsetSize.Enabled = false;
                this.comboHeaderSizeByteOrder.Enabled = false;
                
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
                this.tbHeaderSizeOffset.Clear();
                this.tbHeaderSizeOffset.Enabled = false;
                this.tbHeaderSizeOffset.ReadOnly = true;
                this.comboHeaderSizeOffsetSize.Enabled = false;
                this.comboHeaderSizeByteOrder.Enabled = false;
                
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

            if (this.rbHeaderSizeOffset.Checked)
            {
                isValid &= base.checkTextBox(this.tbHeaderSizeOffset.Text, this.rbHeaderSizeOffset.Text);
                isValid &= base.checkTextBox(this.comboHeaderSizeOffsetSize.Text, "Header Size Offset Size");
                isValid &= base.checkTextBox(this.comboHeaderSizeByteOrder.Text, "Header Size Offset Byte Order");            
            }
            else if (this.rbUserEnteredFileCount.Checked)
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

        private void doUserHeaderFileCheckbox()
        {
            if (cbUseHeaderFile.Checked)
            {
                this.tbHeaderFilePath.Enabled = true;
                this.tbHeaderFilePath.ReadOnly = false;
                this.btnBrowseHeaderFile.Enabled = true;
            }
            else
            {
                this.tbHeaderFilePath.Clear();
                this.tbHeaderFilePath.Enabled = false;
                this.tbHeaderFilePath.ReadOnly = true;
                this.btnBrowseHeaderFile.Enabled = false;
            }        
        }
        private void cbUseHeaderFile_CheckedChanged(object sender, EventArgs e)
        {
            this.doUserHeaderFileCheckbox();
        }

        private void btnBrowseHeaderFile_Click(object sender, EventArgs e)
        {
            this.tbHeaderFilePath.Text = base.browseForFile(sender, e);
        }
        private void tbHeaderFilePath_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void tbHeaderFilePath_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if ((s.Length == 1) && (File.Exists(s[0])))
            {
                this.tbHeaderFilePath.Text = s[0];
            }
        }

        private void btnBrowseDataFile_Click(object sender, EventArgs e)
        {
            this.tbDataFilePath.Text = base.browseForFile(sender, e);
        }
        private void tbDataFilePath_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void tbDataFilePath_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if ((s.Length == 1) && (File.Exists(s[0])))
            {
                this.tbDataFilePath.Text = s[0];
            }
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            string[] dataFiles = new string[] {Path.GetFullPath(this.tbDataFilePath.Text)};
            this.extractFiles(dataFiles, true);
        }

        private void comboHeaderSizeByteOrder_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboHeaderSizeByteOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileCountByteOrder_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileCountByteOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileRecordOffsetByteOrder_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordOffsetByteOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileRecordLengthByteOrder_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordLengthByteOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void loadVfsPresets()
        {
            comboPresets.Items.Clear();

            foreach (string f in Directory.GetFiles(PLUGIN_PATH, "*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    //OffsetFinderTemplate preset = getPresetFromFile(f);

                    //if ((preset != null) && (!String.IsNullOrEmpty(preset.Header.FormatName)))
                    //{
                    //    comboPresets.Items.Add(preset);
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error loading preset file <{0}>", Path.GetFileName(f)), "Error");
                }
            }

            comboPresets.Sorted = true;
        }
    }
}
