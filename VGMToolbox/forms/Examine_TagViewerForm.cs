using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools;
using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class Examine_TagViewerForm : TreeViewVgmtForm
    {
        TreeBuilderWorker treeBuilder;
        
        public Examine_TagViewerForm() : base() 
        {
            // set title
            this.lblTitle.Text = "Tag/Info Viewer";
            this.btnDoTask.Text = "Expand All";

            this.tbOutput.Text = "Drag Files or Folders onto the box above to View Information." + System.Environment.NewLine;
            this.tbOutput.Text += "Right Click on filenames to update the tags (GBS/NSF/MDX/PSID only)." + System.Environment.NewLine;

            InitializeComponent();
        }

        public Examine_TagViewerForm(TreeNode pTreeNode) : base(pTreeNode) 
        {
            // set title
            this.lblTitle.Text = "Tag/Info Viewer";
            this.btnDoTask.Text = "Expand All";

            this.tbOutput.Text = "Drag Files or Folders onto the box above to View Information." + System.Environment.NewLine;
            this.tbOutput.Text += "Right Click on filenames to update the tags (GBS/NSF/MDX/PSID only)." + System.Environment.NewLine;
            
            InitializeComponent();
        }

        private void tbXsfSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();
            
            toolStripStatusLabel1.Text = "Examination...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            int totalFileCount = FileUtil.GetFileCount(s);

            TreeBuilderWorker.TreeBuilderStruct tbStruct = new TreeBuilderWorker.TreeBuilderStruct();
            tbStruct.pPaths = s;
            tbStruct.totalFiles = totalFileCount;
            tbStruct.checkForLibs = cbCheckForLibs.Checked;

            treeBuilder = new TreeBuilderWorker();
            treeBuilder.ProgressChanged += backgroundWorker_ReportProgress;
            treeBuilder.RunWorkerCompleted += TreeBuilderWorker_WorkComplete;
            treeBuilder.RunWorkerAsync(tbStruct);
        }

        private void TreeBuilderWorker_WorkComplete(object sender,
                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Examination...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Examination...Complete";
            }
            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (treeBuilder != null && treeBuilder.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                treeBuilder.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            this.treeViewTools.ExpandAll();
        }
    }
}
