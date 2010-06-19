using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using VGMToolbox.plugin;
using VGMToolbox.tools.extract;
using VGMToolbox.util;

namespace VGMToolbox.forms.extraction
{
    public partial class VfsExtractorForm : VgmtForm
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
            this.tbOutput.Text += "- Preset .xml files should be placed in <" + PLUGIN_PATH + ">" + System.Environment.NewLine;
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

            // offset
            this.rbUseVfsFileOffset.Checked = true;
            this.createFileRecordSizeList();
            this.createFileRecordOffsetSizeList();
            this.createFileRecordOffsetEndianList();

            this.doFileRecordOffsetRadioButtons();
            this.doCbOffsetMultiplier();

            // length
            this.rbUseVfsFileLength.Checked = true;
            this.createFileRecordLengthSizeList();
            this.createFileRecordLengthEndianList();
            this.doFileRecordLengthRadioButtons();
            this.doCbLengthMultiplier();

            // name
            this.doFileRecordNameCheckbox();

            this.createFileNameAbsoluteOffsetSizeList();
            this.createFileNameAbsoluteOffsetEndianList();
            this.createFileNameRelativeOffsetSizeList();
            this.createFileNameRelativeOffsetEndianList();
            this.createFileNameRelativeOffsetLocationList();
            // presets
            this.loadPresetList();
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

                bgStruct.OutputFolderPath = this.tbOutputFolder.Text;

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

                // offset
                bgStruct.FileRecordOffsetOffset = this.tbFileRecordOffsetOffset.Text;
                bgStruct.FileRecordOffsetLength = this.comboFileRecordOffsetSize.Text;
                bgStruct.FileRecordOffsetIsLittleEndian = this.comboFileRecordOffsetByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN);
                bgStruct.UseFileRecordOffsetMultiplier = this.cbUseOffsetMultiplier.Checked;
                bgStruct.FileRecordOffsetMultiplier = this.tbFileRecordOffsetMultiplier.Text;

                bgStruct.UsePreviousFilesSizeToDetermineOffset = this.rbUseFileSizeToDetermineOffset.Checked;
                bgStruct.BeginCuttingFilesAtOffset = this.tbUseFileLengthBeginOffset.Text;
                bgStruct.ByteAlignmentValue = this.tbByteAlignement.Text;

                // length
                bgStruct.FileRecordLengthOffset = this.tbFileRecordLengthOffset.Text;
                bgStruct.FileRecordLengthLength = this.comboFileRecordLengthSize.Text;
                bgStruct.FileRecordLengthIsLittleEndian = this.comboFileRecordLengthByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN);
                bgStruct.UseLocationOfNextFileToDetermineLength = this.rbUseOffsetsToDetermineLength.Checked;
                bgStruct.UseFileRecordLengthMultiplier = this.cbUseLengthMultiplier.Checked;
                bgStruct.FileRecordLengthMultiplier = tbFileRecordOffsetMultiplier.Text;

                // name location
                bgStruct.FileRecordNameIsPresent = this.cbFileNameIsPresent.Checked;                
                
                bgStruct.FileRecordNameIsIncludedInFileRecord = this.rbFileRecordFileNameInRecord.Checked;
                bgStruct.FileRecordNameOffset = this.tbFileRecordNameOffset.Text;
                
                bgStruct.FileRecordNameAbsoluteOffsetIsPresent = this.rbFileNameAbsoluteOffset.Checked;
                bgStruct.FileRecordNameAbsoluteOffsetOffset = this.tbFileRecordNameAbsoluteOffset.Text;
                bgStruct.FileRecordNameAbsoluteOffsetLength = this.comboFileRecordNameAbsoluteOffsetSize.Text;
                bgStruct.FileRecordNameAbsoluteOffsetIsLittleEndian = this.comboFileRecordNameAbsoluteOffsetBytesOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN);

                bgStruct.FileRecordNameRelativeOffsetIsPresent = this.rbFileNameRelativeOffset.Checked;
                bgStruct.FileRecordNameRelativeOffsetOffset = this.tbFileRecordNameRelativeOffset.Text;
                bgStruct.FileRecordNameRelativeOffsetLength = this.comboFileRecordNameRelativeOffsetSize.Text;
                bgStruct.FileRecordNameRelativeOffsetIsLittleEndian = this.comboFileRecordNameRelativeOffsetBytesOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN); ;

                switch (this.comboFileRecordNameRelativeLocation.Text)
                {
                    case VfsExtractorWorker.RELATIVE_TO_START_OF_FILE_RECORD:
                        bgStruct.FileRecordNameRelativeOffsetLocation = VfsFileRecordRelativeOffsetLocationType.FileRecordStart;
                        break;
                    case VfsExtractorWorker.RELATIVE_TO_END_OF_FILE_RECORD:
                        bgStruct.FileRecordNameRelativeOffsetLocation = VfsFileRecordRelativeOffsetLocationType.FileRecordEnd;
                        break;                    
                }

                // name size
                bgStruct.FileRecordNameStaticLength = this.tbFileRecordNameSize.Text;
                bgStruct.FileRecordNameTerminator = this.tbFileRecordNameTerminatorBytes.Text;

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

                this.cbDoOffsetByteAlginment.Checked = false;
                this.cbDoOffsetByteAlginment.Enabled = false;

                this.tbByteAlignement.Clear();
                this.tbByteAlignement.Enabled = false;
                this.tbByteAlignement.ReadOnly = true;
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

                this.cbDoOffsetByteAlginment.Enabled = true;

                this.tbByteAlignement.Enabled = true;
                this.tbByteAlignement.ReadOnly = false;
            }

            this.doCbOffsetMultiplier();
            this.doCbDoOffsetByteAlignment();
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

                this.cbUseLengthMultiplier.Enabled = true;
                this.tbFileRecordLengthMultiplier.Enabled = true;
                this.tbFileRecordLengthMultiplier.ReadOnly = false;
            }
            else if (this.rbUseOffsetsToDetermineLength.Checked)
            {
                this.tbFileRecordLengthOffset.Enabled = false;
                this.tbFileRecordLengthOffset.ReadOnly = true;
                this.comboFileRecordLengthSize.Enabled = false;
                this.comboFileRecordLengthByteOrder.Enabled = false;

                this.cbUseLengthMultiplier.Checked = false;
                this.cbUseLengthMultiplier.Enabled = false;
                this.tbFileRecordLengthMultiplier.Clear();
                this.tbFileRecordLengthMultiplier.Enabled = false;
                this.tbFileRecordLengthMultiplier.ReadOnly = true;
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

        private void doCbLengthMultiplier()
        {
            if (this.cbUseLengthMultiplier.Checked)
            {
                this.tbFileRecordLengthMultiplier.Enabled = true;
                this.tbFileRecordLengthMultiplier.ReadOnly = false;
            }
            else
            {
                this.tbFileRecordLengthMultiplier.Clear();
                this.tbFileRecordLengthMultiplier.Enabled = false;
                this.tbFileRecordLengthMultiplier.ReadOnly = true;
            }
        }
        private void cbUseLengthMultiplier_CheckedChanged(object sender, EventArgs e)
        {
            this.doCbLengthMultiplier();
        }

        private void createFileNameAbsoluteOffsetSizeList()
        {
            this.comboFileRecordNameAbsoluteOffsetSize.Items.Add("1");
            this.comboFileRecordNameAbsoluteOffsetSize.Items.Add("2");
            this.comboFileRecordNameAbsoluteOffsetSize.Items.Add("4");
        }
        private void createFileNameAbsoluteOffsetEndianList()
        {
            this.comboFileRecordNameAbsoluteOffsetBytesOrder.Items.Add(VfsExtractorWorker.BIG_ENDIAN);
            this.comboFileRecordNameAbsoluteOffsetBytesOrder.Items.Add(VfsExtractorWorker.LITTLE_ENDIAN);
        }

        private void createFileNameRelativeOffsetSizeList()
        {
            this.comboFileRecordNameRelativeOffsetSize.Items.Add("1");
            this.comboFileRecordNameRelativeOffsetSize.Items.Add("2");
            this.comboFileRecordNameRelativeOffsetSize.Items.Add("4");
        }
        private void createFileNameRelativeOffsetEndianList()
        {
            this.comboFileRecordNameRelativeOffsetBytesOrder.Items.Add(VfsExtractorWorker.BIG_ENDIAN);
            this.comboFileRecordNameRelativeOffsetBytesOrder.Items.Add(VfsExtractorWorker.LITTLE_ENDIAN);
        }
        private void createFileNameRelativeOffsetLocationList()
        {
            this.comboFileRecordNameRelativeLocation.Items.Add(VfsExtractorWorker.RELATIVE_TO_START_OF_FILE_RECORD);
            this.comboFileRecordNameRelativeLocation.Items.Add(VfsExtractorWorker.RELATIVE_TO_END_OF_FILE_RECORD);
        }

        private void doFileRecordNameCheckbox()
        {
            if (this.cbFileNameIsPresent.Checked)
            {
                // radio buttons for location/offset
                this.rbFileRecordFileNameInRecord.Enabled = true;
                this.rbFileNameAbsoluteOffset.Enabled = true;
                this.rbFileNameRelativeOffset.Enabled = true;
                
                this.rbFileRecordNameSize.Enabled = true;
                this.rbFileRecordNameTerminator.Enabled = true;

                // name size
                if (this.rbFileRecordNameSize.Checked)
                {
                    this.tbFileRecordNameSize.Enabled = true;
                    this.tbFileRecordNameSize.ReadOnly = false;

                    this.tbFileRecordNameTerminatorBytes.Clear();
                    this.tbFileRecordNameTerminatorBytes.Enabled = false;
                    this.tbFileRecordNameTerminatorBytes.ReadOnly = true;
                    this.lblHexOnly.Enabled = false;
                }
                else if (this.rbFileRecordNameTerminator.Checked)
                {
                    this.tbFileRecordNameSize.Clear();
                    this.tbFileRecordNameSize.Enabled = false;
                    this.tbFileRecordNameSize.ReadOnly = true;

                    this.tbFileRecordNameTerminatorBytes.Enabled = true;
                    this.tbFileRecordNameTerminatorBytes.ReadOnly = false;
                    this.lblHexOnly.Enabled = true;
                }
            }
            else
            {
                // radio buttons for location/offset
                this.rbFileRecordFileNameInRecord.Checked = false;
                this.rbFileRecordFileNameInRecord.Enabled = false;
                this.rbFileNameAbsoluteOffset.Checked = false;
                this.rbFileNameAbsoluteOffset.Enabled = false;
                this.rbFileNameRelativeOffset.Checked = false;
                this.rbFileNameRelativeOffset.Enabled = false;


                // name size
                this.rbFileRecordNameSize.Enabled = false;
                this.tbFileRecordNameSize.Clear();
                this.tbFileRecordNameSize.Enabled = false;
                this.tbFileRecordNameSize.ReadOnly = true;

                this.rbFileRecordNameTerminator.Enabled = false;
                this.tbFileRecordNameTerminatorBytes.Clear();
                this.tbFileRecordNameTerminatorBytes.Enabled = false;
                this.tbFileRecordNameTerminatorBytes.ReadOnly = true;
                this.lblHexOnly.Enabled = false;
            }

            this.doRbFileRecordFileNameInRecord();
            this.doRbFileNameAbsoluteOffset();
            this.doRbFileNameRelativeOffset();
        }
        private void cbFileNameIsPresent_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileRecordNameCheckbox();
        }
        private void rbFileRecordNameSize_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileRecordNameCheckbox();
        }
        private void rbFileRecordNameTerminator_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileRecordNameCheckbox();
        }

        private void doRbFileRecordFileNameInRecord()
        {
            if (this.rbFileRecordFileNameInRecord.Checked)
            {
                this.tbFileRecordNameOffset.Enabled = true;
                this.tbFileRecordNameOffset.ReadOnly = false;
            }
            else
            {
                this.tbFileRecordNameOffset.Enabled = false;
                this.tbFileRecordNameOffset.ReadOnly = true;
            }
        }
        private void rbFileRecordFileNameInRecord_CheckedChanged(object sender, EventArgs e)
        {
            this.doRbFileRecordFileNameInRecord();
        }

        private void doRbFileNameAbsoluteOffset()
        {
            if (this.rbFileNameAbsoluteOffset.Checked)
            {
                this.tbFileRecordNameAbsoluteOffset.Enabled = true;
                this.tbFileRecordNameAbsoluteOffset.ReadOnly = false;
                this.comboFileRecordNameAbsoluteOffsetSize.Enabled = true;
                this.comboFileRecordNameAbsoluteOffsetBytesOrder.Enabled = true;
            }
            else
            {
                this.tbFileRecordNameAbsoluteOffset.Enabled = false;
                this.tbFileRecordNameAbsoluteOffset.ReadOnly = true;
                this.tbFileRecordNameAbsoluteOffset.Clear();
                this.comboFileRecordNameAbsoluteOffsetSize.Enabled = false;
                this.comboFileRecordNameAbsoluteOffsetBytesOrder.Enabled = false;
            }
        }
        private void rbFileNameAbsoluteOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doRbFileNameAbsoluteOffset();
        }

        private void doRbFileNameRelativeOffset()
        {
            if (this.rbFileNameRelativeOffset.Checked)
            {
                this.tbFileRecordNameRelativeOffset.Enabled = true;
                this.tbFileRecordNameRelativeOffset.ReadOnly = false;
                this.comboFileRecordNameRelativeOffsetSize.Enabled = true;
                this.comboFileRecordNameRelativeOffsetBytesOrder.Enabled = true;
                this.comboFileRecordNameRelativeLocation.Enabled = true;
            }
            else
            {
                this.tbFileRecordNameRelativeOffset.Enabled = false;
                this.tbFileRecordNameRelativeOffset.ReadOnly = true;
                this.tbFileRecordNameRelativeOffset.Clear();
                this.comboFileRecordNameRelativeOffsetSize.Enabled = false;
                this.comboFileRecordNameRelativeOffsetBytesOrder.Enabled = false;
                this.comboFileRecordNameRelativeLocation.Enabled = false;
            }
        }
        private void rbFileNameRelativeOffset_CheckedChanged(object sender, EventArgs e)
        {
            this.doRbFileNameRelativeOffset();
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

        private VfsExtractorSettings getPresetFromFile(string filePath)
        {
            VfsExtractorSettings preset = null;

            preset = new VfsExtractorSettings();
            XmlSerializer serializer = new XmlSerializer(preset.GetType());
            using (FileStream xmlFs = File.OpenRead(filePath))
            {
                using (XmlTextReader textReader = new XmlTextReader(xmlFs))
                {
                    preset = (VfsExtractorSettings)serializer.Deserialize(textReader);
                }
            }

            return preset;
        }

        private void loadPresetList()
        {
            comboPresets.Items.Clear();

            foreach (string f in Directory.GetFiles(PLUGIN_PATH, "*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    VfsExtractorSettings preset = getPresetFromFile(f);

                    if ((preset != null) && (!String.IsNullOrEmpty(preset.Header.FormatName)))
                    {
                        comboPresets.Items.Add(preset);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error loading preset file <{0}>", Path.GetFileName(f)), "Error");
                }
            }

            comboPresets.Sorted = true;
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.loadPresetList();
        }        

        private void loadVfsSettings(VfsExtractorSettings vfsSettings)
        {            
            #region HEADER_SETTINGS

            switch (vfsSettings.HeaderParameters.HeaderSizeMethod)
            {
                case HeaderSizeMethod.HeaderSizeValue:
                    this.rbFileCountEndOffset.Checked = true;
                    this.tbFileCountEndOffset.Text = vfsSettings.HeaderParameters.HeaderEndsAtOffset;
                    break;
                
                case HeaderSizeMethod.HeaderSizeOffset:
                    this.rbHeaderSizeOffset.Checked = true;
                    this.tbHeaderSizeOffset.Text = vfsSettings.HeaderParameters.HeaderSizeOffset;
                    this.comboHeaderSizeOffsetSize.SelectedItem = vfsSettings.HeaderParameters.HeaderSizeOffsetSize;
                    if (vfsSettings.HeaderParameters.HeaderSizeOffsetEndianessSpecified)
                    {
                        switch (vfsSettings.HeaderParameters.HeaderSizeOffsetEndianess)
                        {
                            case Endianness.big:
                                this.comboHeaderSizeByteOrder.SelectedItem = VfsExtractorWorker.BIG_ENDIAN;
                                break;
                            case Endianness.little:
                                this.comboHeaderSizeByteOrder.SelectedItem = VfsExtractorWorker.LITTLE_ENDIAN;
                                break;
                        }
                    }
                    break;

                case HeaderSizeMethod.FileCountValue:
                    this.rbUserEnteredFileCount.Checked = true;
                    this.tbFileCountValue.Text = vfsSettings.HeaderParameters.FileCountValue;
                    break;

                case HeaderSizeMethod.FileCountOffset:
                    this.rbOffsetBasedFileCount.Checked = true;
                    this.tbFileCountOffset.Text = vfsSettings.HeaderParameters.FileCountOffset;
                    this.comboFileCountOffsetSize.SelectedItem = vfsSettings.HeaderParameters.FileCountOffsetSize;
                    if (vfsSettings.HeaderParameters.FileCountOffsetEndianessSpecified)
                    {
                        switch (vfsSettings.HeaderParameters.FileCountOffsetEndianess)
                        {
                            case Endianness.big:
                                this.comboFileCountByteOrder.SelectedItem = VfsExtractorWorker.BIG_ENDIAN;
                                break;
                            case Endianness.little:
                                this.comboFileCountByteOrder.SelectedItem = VfsExtractorWorker.LITTLE_ENDIAN;
                                break;
                        }
                    }

                    break;
            }

            #endregion

            #region FILE_RECORD_SETTINGS

            this.tbFileRecordsBeginOffset.Text = vfsSettings.FileRecordParameters.FileRecordsStartOffset;
            this.comboFileRecordSize.Text = vfsSettings.FileRecordParameters.FileRecordSize;

            // Offset
            switch (vfsSettings.FileRecordParameters.FileOffsetMethod)
            { 
                case FileOffsetLengthMethod.offset:
                    this.rbUseVfsFileOffset.Checked = true;
                    this.tbFileRecordOffsetOffset.Text = vfsSettings.FileRecordParameters.FileOffsetOffset;
                    this.comboFileRecordOffsetSize.SelectedItem = vfsSettings.FileRecordParameters.FileOffsetOffsetSize;
                    if (vfsSettings.FileRecordParameters.FileOffsetOffsetEndianessSpecified)
                    {
                        switch (vfsSettings.FileRecordParameters.FileOffsetOffsetEndianess)
                        {
                            case Endianness.big:
                                this.comboFileRecordOffsetByteOrder.SelectedItem = VfsExtractorWorker.BIG_ENDIAN;
                                break;
                            case Endianness.little:
                                this.comboFileRecordOffsetByteOrder.SelectedItem = VfsExtractorWorker.LITTLE_ENDIAN;
                                break;
                        }
                    }                    
                    if (!String.IsNullOrEmpty(vfsSettings.FileRecordParameters.FileOffsetOffsetMultiplier))
                    {
                        this.cbUseOffsetMultiplier.Checked = true;
                        this.tbFileRecordOffsetMultiplier.Text = vfsSettings.FileRecordParameters.FileOffsetOffsetMultiplier;
                    }                    
                    break;
                
                case FileOffsetLengthMethod.length:
                    this.rbUseFileSizeToDetermineOffset.Checked = true;
                    this.tbUseFileLengthBeginOffset.Text = vfsSettings.FileRecordParameters.FileCutStartOffset;
                    break;
            }

            // Length
            switch (vfsSettings.FileRecordParameters.FileLengthMethod)
            {
                case FileOffsetLengthMethod.offset:
                    this.rbUseVfsFileLength.Checked = true;
                    this.tbFileRecordLengthOffset.Text = vfsSettings.FileRecordParameters.FileLengthOffset;
                    this.comboFileRecordLengthSize.SelectedItem = vfsSettings.FileRecordParameters.FileLengthOffsetSize;
                    if (vfsSettings.FileRecordParameters.FileLengthOffsetEndianessSpecified)
                    {
                        switch (vfsSettings.FileRecordParameters.FileLengthOffsetEndianess)
                        {
                            case Endianness.big:
                                this.comboFileRecordLengthByteOrder.SelectedItem = VfsExtractorWorker.BIG_ENDIAN;
                                break;
                            case Endianness.little:
                                this.comboFileRecordLengthByteOrder.SelectedItem = VfsExtractorWorker.LITTLE_ENDIAN;
                                break;
                        }
                    }
                    if (!String.IsNullOrEmpty(vfsSettings.FileRecordParameters.FileLengthMultiplier))
                    {
                        this.cbUseLengthMultiplier.Checked = true;
                        this.tbFileRecordLengthMultiplier.Text = vfsSettings.FileRecordParameters.FileLengthMultiplier;
                    }
                    break;

                case FileOffsetLengthMethod.length:
                    this.rbUseOffsetsToDetermineLength.Checked = true;
                    break;
            }

            // NAME
            if (vfsSettings.FileRecordParameters.ExtractFileName)
            {
                this.cbFileNameIsPresent.Checked = true;

                // file name in file record
                if (!String.IsNullOrEmpty(vfsSettings.FileRecordParameters.FileNameOffset))
                {
                    this.rbFileRecordFileNameInRecord.Checked = true;
                    this.tbFileRecordNameOffset.Text = vfsSettings.FileRecordParameters.FileNameOffset;
                }
                
                // name length
                if (vfsSettings.FileRecordParameters.FileNameLengthMethod.Equals(NameLengthType.staticSize))
                {
                    this.rbFileRecordNameSize.Checked = true;
                    this.tbFileRecordNameSize.Text = vfsSettings.FileRecordParameters.FileNameSize;
                }
                else if (vfsSettings.FileRecordParameters.FileNameLengthMethod.Equals(NameLengthType.terminator))
                {
                    this.rbFileRecordNameTerminator.Checked = true;
                    this.tbFileRecordNameTerminatorBytes.Text = vfsSettings.FileRecordParameters.FileNameTerminator;
                }
            }

            #endregion
        }
        private void loadSelectedPreset()
        {
            VfsExtractorSettings preset = (VfsExtractorSettings)this.comboPresets.SelectedItem;

            if (preset != null)
            {
                this.resetHeaderSection();
                this.resetFileRecordSection();

                this.loadVfsSettings(preset);

                if (!String.IsNullOrEmpty(preset.NotesOrWarnings))
                {
                    MessageBox.Show(preset.NotesOrWarnings, "Notes/Warnings");
                }
            }
        }
        private void btnLoadPreset_Click(object sender, EventArgs e)
        {
            this.loadSelectedPreset();
        }

        private VfsExtractorSettings buildPresetForCurrentValues()
        {
            VfsExtractorSettings vfsSettings = new VfsExtractorSettings();

            #region HEADER_SETTINGS

            if (this.rbFileCountEndOffset.Checked)
            {
                vfsSettings.HeaderParameters.HeaderSizeMethod = HeaderSizeMethod.HeaderSizeValue;
                vfsSettings.HeaderParameters.HeaderEndsAtOffset = this.tbFileCountEndOffset.Text;
            }
            else if (this.rbHeaderSizeOffset.Checked)
            {
                vfsSettings.HeaderParameters.HeaderSizeMethod = HeaderSizeMethod.HeaderSizeOffset;
                vfsSettings.HeaderParameters.HeaderSizeOffset = this.tbHeaderSizeOffset.Text;
                vfsSettings.HeaderParameters.HeaderSizeOffsetSize = this.comboHeaderSizeOffsetSize.Text;

                if (this.comboHeaderSizeByteOrder.Text.Equals(VfsExtractorWorker.BIG_ENDIAN))
                {
                    vfsSettings.HeaderParameters.HeaderSizeOffsetEndianessSpecified = true;
                    vfsSettings.HeaderParameters.HeaderSizeOffsetEndianess = Endianness.big;
                }
                else if (this.comboHeaderSizeByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN))
                {
                    vfsSettings.HeaderParameters.HeaderSizeOffsetEndianessSpecified = true;
                    vfsSettings.HeaderParameters.HeaderSizeOffsetEndianess = Endianness.little;
                }
            }
            else if (this.rbUserEnteredFileCount.Checked)
            {
                vfsSettings.HeaderParameters.HeaderSizeMethod = HeaderSizeMethod.FileCountValue;
                vfsSettings.HeaderParameters.FileCountValue = this.tbFileCountValue.Text;
            }
            else if (this.rbOffsetBasedFileCount.Checked)
            {
                vfsSettings.HeaderParameters.HeaderSizeMethod = HeaderSizeMethod.FileCountOffset;
                vfsSettings.HeaderParameters.FileCountOffset = this.tbFileCountOffset.Text;
                vfsSettings.HeaderParameters.FileCountOffsetSize = this.comboFileCountOffsetSize.Text;

                if (this.comboFileCountByteOrder.Text.Equals(VfsExtractorWorker.BIG_ENDIAN))
                {
                    vfsSettings.HeaderParameters.FileCountOffsetEndianessSpecified = true;
                    vfsSettings.HeaderParameters.FileCountOffsetEndianess = Endianness.big;
                }
                else if (this.comboFileCountByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN))
                {
                    vfsSettings.HeaderParameters.FileCountOffsetEndianessSpecified = true;
                    vfsSettings.HeaderParameters.FileCountOffsetEndianess = Endianness.little;
                }
            }

            #endregion

            #region FILE_RECORD_SETTINGS

            vfsSettings.FileRecordParameters.FileRecordsStartOffset = this.tbFileRecordsBeginOffset.Text;
            vfsSettings.FileRecordParameters.FileRecordSize = this.comboFileRecordSize.Text;

            // Offset
            if (this.rbUseVfsFileOffset.Checked)
            {
                vfsSettings.FileRecordParameters.FileOffsetMethod = FileOffsetLengthMethod.offset;
                vfsSettings.FileRecordParameters.FileOffsetOffset = this.tbFileRecordOffsetOffset.Text;
                vfsSettings.FileRecordParameters.FileOffsetOffsetSize = this.comboFileRecordOffsetSize.Text;

                if (this.comboFileRecordOffsetByteOrder.Text.Equals(VfsExtractorWorker.BIG_ENDIAN))
                {
                    vfsSettings.FileRecordParameters.FileOffsetOffsetEndianessSpecified = true;
                    vfsSettings.FileRecordParameters.FileOffsetOffsetEndianess = Endianness.big;
                }
                else if (this.comboFileRecordOffsetByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN))
                {
                    vfsSettings.FileRecordParameters.FileOffsetOffsetEndianessSpecified = true;
                    vfsSettings.FileRecordParameters.FileOffsetOffsetEndianess = Endianness.little;
                }

                if (this.cbUseOffsetMultiplier.Checked)
                {
                    vfsSettings.FileRecordParameters.FileOffsetOffsetMultiplier = this.tbFileRecordOffsetMultiplier.Text;
                }
            }
            else if (this.rbUseFileSizeToDetermineOffset.Checked)
            {
                vfsSettings.FileRecordParameters.FileOffsetMethod = FileOffsetLengthMethod.length;
                vfsSettings.FileRecordParameters.FileCutStartOffset = this.tbUseFileLengthBeginOffset.Text;
            }

            // Length
            if (this.rbUseVfsFileLength.Checked)
            {
                vfsSettings.FileRecordParameters.FileLengthMethod = FileOffsetLengthMethod.offset;
                vfsSettings.FileRecordParameters.FileLengthOffset = this.tbFileRecordLengthOffset.Text;
                vfsSettings.FileRecordParameters.FileLengthOffsetSize = this.comboFileRecordLengthSize.Text;

                if (this.comboFileRecordLengthByteOrder.Text.Equals(VfsExtractorWorker.BIG_ENDIAN))
                {
                    vfsSettings.FileRecordParameters.FileLengthOffsetEndianessSpecified = true;
                    vfsSettings.FileRecordParameters.FileLengthOffsetEndianess = Endianness.big;
                }
                else if (this.comboFileRecordLengthByteOrder.Text.Equals(VfsExtractorWorker.LITTLE_ENDIAN))
                {
                    vfsSettings.FileRecordParameters.FileLengthOffsetEndianessSpecified = true;
                    vfsSettings.FileRecordParameters.FileLengthOffsetEndianess = Endianness.little;
                }

                if (this.cbUseLengthMultiplier.Checked)
                {
                    vfsSettings.FileRecordParameters.FileLengthMultiplier = this.tbFileRecordLengthMultiplier.Text;
                }
            }
            else if (this.rbUseOffsetsToDetermineLength.Checked)
            {
                vfsSettings.FileRecordParameters.FileLengthMethod = FileOffsetLengthMethod.length;            
            }
            
            // NAME
            if (this.cbFileNameIsPresent.Checked)
            {
                vfsSettings.FileRecordParameters.ExtractFileName = true;
                vfsSettings.FileRecordParameters.FileNameOffset = this.tbFileRecordNameOffset.Text;


                if (this.rbFileRecordNameSize.Checked)
                {
                    vfsSettings.FileRecordParameters.FileNameLengthMethodSpecified = true;
                    vfsSettings.FileRecordParameters.FileNameLengthMethod = NameLengthType.staticSize;
                    vfsSettings.FileRecordParameters.FileNameSize = this.tbFileRecordNameSize.Text;
                }
                else if (this.rbFileRecordNameTerminator.Checked)
                {
                    vfsSettings.FileRecordParameters.FileNameLengthMethodSpecified = true;
                    vfsSettings.FileRecordParameters.FileNameLengthMethod = NameLengthType.terminator;
                    vfsSettings.FileRecordParameters.FileNameTerminator = this.tbFileRecordNameTerminatorBytes.Text;
                }
            }
            
            #endregion

            return vfsSettings;
        }
        private void btnSavePreset_Click(object sender, EventArgs e)
        {
            VfsExtractorSettings preset = buildPresetForCurrentValues();

            if (preset != null)
            {
                SavePresetForm saveForm = new SavePresetForm(preset);
                saveForm.Show();
            }
        }

        private void resetHeaderSection()
        {
            this.cbUseHeaderFile.Checked = false;
            this.doUserHeaderFileCheckbox();

            this.rbFileCountEndOffset.Checked = true;
            this.doFileCountRadioButtons();
        }
        private void resetFileRecordSection()
        {
            this.tbFileRecordsBeginOffset.Clear();
            this.comboFileRecordSize.ResetText();

            this.rbUseVfsFileOffset.Checked = true;
            this.cbUseOffsetMultiplier.Checked = false;
            this.doFileRecordOffsetRadioButtons();

            this.rbUseVfsFileLength.Checked = true;
            this.cbUseLengthMultiplier.Checked = false;
            this.doFileRecordLengthRadioButtons();

            this.cbFileNameIsPresent.Checked = false;
            this.doFileRecordNameCheckbox();
        }

        private void btnBrowseOutputFolder_Click(object sender, EventArgs e)
        {
            this.tbOutputFolder.Text = base.browseForFolder(sender, e);
        }

        private void tbOutputFolder_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void tbOutputFolder_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if ((s.Length == 1) && (Directory.Exists(s[0])))
            {
                this.tbOutputFolder.Text = s[0];
            }
        }

        // combo box key capture
        private void comboHeaderSizeOffsetSize_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboHeaderSizeOffsetSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileCountOffsetSize_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileCountOffsetSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileRecordOffsetSize_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordOffsetSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileRecordLengthSize_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordLengthSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileRecordNameAbsoluteOffsetSize_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordNameAbsoluteOffsetSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileRecordNameAbsoluteOffsetBytesOrder_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordNameAbsoluteOffsetBytesOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileRecordNameAbsoluteRelativeSize_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordNameAbsoluteRelativeSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboFileRecordNameRelativeOffsetBytesOrder_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordNameRelativeOffsetBytesOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
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

        private void comboFileRecordNameRelativeLocation_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboFileRecordNameRelativeLocation_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void doCbDoOffsetByteAlignment()
        {
            if (this.cbDoOffsetByteAlginment.Checked)
            {
                this.tbByteAlignement.Enabled = true;
                this.tbByteAlignement.ReadOnly = false;
            }
            else
            {
                this.tbByteAlignement.Clear();
                this.tbByteAlignement.Enabled = false;
                this.tbByteAlignement.ReadOnly = true;
            }
        }
        private void cbDoOffsetByteAlginment_CheckedChanged(object sender, EventArgs e)
        {
            this.doCbDoOffsetByteAlignment();
        }
    }
}
