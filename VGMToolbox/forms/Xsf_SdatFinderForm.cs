using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools.nds;

namespace VGMToolbox.forms
{
    public partial class Xsf_SdatFinderForm : VgmtForm
    {
        SdatFinderWorker sdatFinderWorker;
        
        public Xsf_SdatFinderForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Nintendo DS SDAT Finder";
            this.tbOutput.Text = "Find and Extract SDATs from files.";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();
        }

        private void SdatFinderWorker_WorkComplete(object sender,
             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Searching for SDATs...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Searching for SDATs...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (sdatFinderWorker != null && sdatFinderWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                sdatFinderWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void tbSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "Searching for SDATs...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            SdatFinderWorker.SdatFinderStruct sfStruct = new SdatFinderWorker.SdatFinderStruct();
            sfStruct.sourcePaths = s;

            sdatFinderWorker = new SdatFinderWorker();
            sdatFinderWorker.ProgressChanged += backgroundWorker_ReportProgress;
            sdatFinderWorker.RunWorkerCompleted += SdatFinderWorker_WorkComplete;
            sdatFinderWorker.RunWorkerAsync(sfStruct);
        }
    }
}
