using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.other;
using VGMToolbox.util;

namespace VGMToolbox.forms.other
{
    public partial class ExternalListFileRenamerForm : AVgmtForm
    {
        public ExternalListFileRenamerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "External List File Renamer";
            this.tbOutput.Text = "Rename a directory of files according to names contained in an external an index or header file.";
            this.btnDoTask.Text = "Rename Files";            
            
            InitializeComponent();

            this.initializeFileNameSizeRadios();
            this.InitializeTextFormatComboBox();
        }
        
        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ExternalListFileRenamerWorker();
        }
        protected override string getCancelMessage()
        {
            return "Renaming Files...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Renaming Files...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Renaming Files...Begin";
        }

        
        

        private bool validateAll()
        {
            bool isValid = true;

            //isValid &= AVgmtForm.checkTextBox(this.tbNameOffset.Text, this.lblNameOffset.Text);

            //if (this.rbNameLength.Checked)
            //{
            //    isValid &= AVgmtForm.checkTextBox(this.tbNameLength.Text, this.rbNameLength.Text);
            //    AVgmtForm.checkIfTextIsParsableAsLong(this.tbNameLength.Text, this.rbNameLength.Text);
            //}

            //if (this.rbTerminatorBytes.Checked)
            //{
            //    isValid &= AVgmtForm.checkTextBox(this.tbTerminatorBytes.Text, this.rbTerminatorBytes.Text);
            //    AVgmtForm.checkIfTextIsParsableAsLong("0x" + this.tbTerminatorBytes.Text, this.rbTerminatorBytes.Text);            
            //}

            return isValid;
        }

        #region List File

        private void btnBrowseListFile_Click(object sender, EventArgs e)
        {
            this.tbListFile.Text = base.browseForFile(sender, e);
        }

        private void tbListFile_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void tbListFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if ((s.Length == 1) && (File.Exists(s[0])))
            {
                this.tbListFile.Text = s[0];
            }
        }

        private void tbListFile_TextChanged(object sender, EventArgs e)
        {
            //this.loadDestinationFiles();
        }

        private void tbListFileOffsetToFileList_TextChanged(object sender, EventArgs e)
        {
            //this.loadDestinationFiles();
        }


        private void comboListFileNameTextFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboListFileNameTextFormat.SelectedItem.Equals("Code Page"))
            {
                this.tbListFilesCodePage.Enabled = true;
                this.tbListFilesCodePage.ReadOnly = false;
            }
            else
            {
                this.tbListFilesCodePage.Enabled = false;
                this.tbListFilesCodePage.ReadOnly = true;
            }
        }

        private void comboListFileNameTextFormat_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboListFileNameTextFormat_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        

        
        private void InitializeTextFormatComboBox()
        {
            // this is hokey, but I don't feel like doing it the right way right now
            this.comboListFileNameTextFormat.Items.Add("ASCII");
            this.comboListFileNameTextFormat.Items.Add("UTF8");
            this.comboListFileNameTextFormat.Items.Add("UTF16-LE");
            this.comboListFileNameTextFormat.Items.Add("UTF16-BE");
            this.comboListFileNameTextFormat.Items.Add("UTF32-LE");
            this.comboListFileNameTextFormat.Items.Add("Code Page");

            this.comboListFileNameTextFormat.SelectedItem = "ASCII";
        }

        private void loadDestinationFiles()
        {
            long listOffset;
            
            int fileCount;
            int rowCount = this.dgFilePreview.RowCount;

            if (rowCount > 0)
            {
                // clear destination files
                for (int i = 0; i < rowCount; i++)
                {
                    this.dgFilePreview.Rows[i].Cells[2].Value = String.Empty;
                }

                // load list from file
                if (!String.IsNullOrEmpty(this.tbListFileOffsetToFileList.Text.Trim()) &&
                    File.Exists(this.tbListFile.Text.Trim()))
                {
                    try
                    {
                        listOffset = ByteConversion.GetLongValueFromString(this.tbListFileOffsetToFileList.Text.Trim());

                        // read files
                    }
                    catch (Exception ex)
                    { 
                    
                    }
                }
            } // if (rowCount > 0)
        }






        # endregion

        #region File Name Size

        private void initializeFileNameSizeRadios()
        {
            this.rbListFileStaticSize.Checked = false;
            this.rbListFileTerminator.Checked = true;
            
            this.tbListFileTerminator.Enabled = true;
            this.tbListFileTerminator.ReadOnly = false;

            this.tbListFileStaticSize.Enabled = false;
            this.tbListFileStaticSize.ReadOnly = true;
        }

        private void rbListFileTerminator_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileListFileNameSizeRadios();
        }

        private void rbListFileStaticSize_CheckedChanged(object sender, EventArgs e)
        {
            this.doFileListFileNameSizeRadios();
        }

        private void doFileListFileNameSizeRadios()
        {
            if (this.rbListFileTerminator.Checked)
            {
                this.tbListFileTerminator.Enabled = true;
                this.tbListFileTerminator.ReadOnly = false;

                this.tbListFileStaticSize.Enabled = false;
                this.tbListFileStaticSize.ReadOnly = true;
            }
            else
            {
                this.tbListFileTerminator.Enabled = false;
                this.tbListFileTerminator.ReadOnly = true;

                this.tbListFileStaticSize.Enabled = true;
                this.tbListFileStaticSize.ReadOnly = false;
            }
        }

        #endregion

        #region Source Folder

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            this.tbSourceFolder.Text = base.browseForFolder(sender, e);
        }

        private void tbSourceFolder_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void tbSourceFolder_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if ((s.Length == 1) && (Directory.Exists(s[0])))
            {
                this.tbSourceFolder.Text = s[0];
            }
        }

        private void tbSourceFolder_TextChanged(object sender, EventArgs e)
        {
            //this.loadSourceFilesIntoPreview();
        }

        private void tbSourceFileMask_TextChanged(object sender, EventArgs e)
        {
            //this.loadSourceFilesIntoPreview();
        }

        private void loadSourceFilesIntoPreview()
        {
            DataGridViewRow row;
            string fileMask;

            // clear existing preview
            this.dgFilePreview.Rows.Clear();

            // verify directory exists
            if (Directory.Exists(this.tbSourceFolder.Text))
            {
                // build file mask
                fileMask = this.tbSourceFileMask.Text.Trim();
                fileMask = String.IsNullOrEmpty(fileMask) ? "*.*" : fileMask;

                // loop over source files
                foreach (string fileName in Directory.GetFiles(this.tbSourceFolder.Text, fileMask))
                {
                    // @TODO: Add limit to file count to prevent issues?

                    // build row
                    row = new DataGridViewRow();
                    row.CreateCells(this.dgFilePreview);

                    row.Cells[0].Value = true; // checkbox
                    row.Cells[1].Value = Path.GetFileName(fileName); // source file

                    // add row to grid
                    this.dgFilePreview.Rows.Add(row);
                }

            } // if (Directory.Exists(this.tbSourceFolder.Text))
        }

        #endregion

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            // @TODO: Validate Entries

            // build input struct
            ExternalListFileRenamerWorker.ExternalListFileRenamerStruct taskStruct = new ExternalListFileRenamerWorker.ExternalListFileRenamerStruct();
            taskStruct.SourceFolder = this.tbSourceFolder.Text;
            taskStruct.SourceFileMask = this.tbSourceFileMask.Text;
            taskStruct.KeepOriginalFileExtension = this.cbKeepExtension.Checked;

            taskStruct.ListFileLocation = this.tbListFile.Text;
            taskStruct.OffsetToFileNames = this.tbListFileOffsetToFileList.Text;
            taskStruct.FileNameEncoding = (string)this.comboListFileNameTextFormat.SelectedItem;
            taskStruct.FileNameCodePage = this.tbListFilesCodePage.Text;
            
            taskStruct.FileNameHasTerminator = this.rbListFileTerminator.Checked;
            taskStruct.FileNameTerminator = this.tbListFileTerminator.Text;
            taskStruct.FileNameHasStaticSize = this.rbListFileStaticSize.Checked;
            taskStruct.FileNameStaticSize = this.tbListFileStaticSize.Text;

            base.backgroundWorker_Execute(taskStruct);
        }


        

        



        

        

        

        
    }
}
