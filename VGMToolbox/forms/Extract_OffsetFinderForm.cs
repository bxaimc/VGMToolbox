using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools.extract;

namespace VGMToolbox.forms
{
    public partial class Extract_OffsetFinderForm : VgmtForm
    {
        OffsetFinderWorker offsetFinderWorker;
        
        public Extract_OffsetFinderForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Offset Finder";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
                        
            InitializeComponent();
        }

        private void OffsetFinderWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Searching for Strings...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Searching for Strings...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (offsetFinderWorker != null && offsetFinderWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                offsetFinderWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        private void tbSourcePaths_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "Searching for Strings...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            OffsetFinderWorker.OffsetFinderStruct ofStruct = new OffsetFinderWorker.OffsetFinderStruct();
            ofStruct.sourcePaths = s;
            ofStruct.searchString = tbSearchString.Text;
            ofStruct.treatSearchStringAsHex = cbSearchAsHex.Checked;

            offsetFinderWorker = new OffsetFinderWorker();
            offsetFinderWorker.ProgressChanged += backgroundWorker_ReportProgress;
            offsetFinderWorker.RunWorkerCompleted += OffsetFinderWorker_WorkComplete;
            offsetFinderWorker.RunWorkerAsync(ofStruct);
        }
    }
}
