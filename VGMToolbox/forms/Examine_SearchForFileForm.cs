using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.examine;

namespace VGMToolbox.forms
{
    public partial class Examine_SearchForFileForm : AVgmtForm
    {
        public Examine_SearchForFileForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.lblTitle.Text = "Search for File(s)";
            this.tbOutput.Text = "Search for files in a folder and archives.  Enter Search String, then select a folder and click 'Search' or Drag and Drop the folders you want to search.";
            this.btnDoTask.Text = "Search";
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ExamineSearchForFileWorker();
        }
        protected override string getCancelMessage()
        {
            return "Search for Files...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Search for Files...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Search for Files...Begin";
        }

        private void tbSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExamineSearchForFileWorker.ExamineSearchForFileStruct exStruct =
                new ExamineSearchForFileWorker.ExamineSearchForFileStruct();            
            exStruct.SourcePaths = s;
            exStruct.ExtractFile = cbExtract.Checked;
            exStruct.SearchString = tbSearchString.Text;
            exStruct.CaseSensitive = cbCaseSensitive.Checked;
            exStruct.OutputFolder = tbOutputFolder.Text;

            base.backgroundWorker_Execute(exStruct);
        }
        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            tbSource.Text = base.browseForFolder(sender, e);
        }
        private void btnDoTask_Click(object sender, EventArgs e)
        {
            if (base.checkFolderExists(tbSource.Text, "Source Folder"))
            {
                string[] s = new string[1];
                s[0] = tbSource.Text;

                ExamineSearchForFileWorker.ExamineSearchForFileStruct exStruct =
                    new ExamineSearchForFileWorker.ExamineSearchForFileStruct();
                exStruct.SourcePaths = s;
                exStruct.ExtractFile = cbExtract.Checked;
                exStruct.SearchString = tbSearchString.Text;
                exStruct.CaseSensitive = cbCaseSensitive.Checked;
                exStruct.OutputFolder = tbOutputFolder.Text;

                base.backgroundWorker_Execute(exStruct);
            }
        }
        private void cbExtract_CheckedChanged(object sender, EventArgs e)
        {
            if (cbExtract.Checked)
            {
                tbOutputFolder.Enabled = true;
                btnOutputDir.Enabled = true;
                lblOutputFolder.Enabled = true;
            }
            else
            {
                tbOutputFolder.Enabled = false;
                btnOutputDir.Enabled = false;
                lblOutputFolder.Enabled = false;
            }
        }
    }
}
