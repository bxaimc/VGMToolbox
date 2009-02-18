using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_SsfSeqTonExtForm : VgmtForm
    {
        SsfSeqTonExtractorWorker ssfSeqTonExtractorWorker;

        public Xsf_SsfSeqTonExtForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "seqext.py/tonext.py Front End  (note: Python must be installed and in your PATH.)";
            this.tbOutput.Text = "Extract SEQ/TON data from files for SSF creation.";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();
        }

        private void tbSsfSqTonExtSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "SEQ/TON Extraction...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            SsfSeqTonExtractorWorker.SsfSeqTonExtractorStruct stexStruct = new SsfSeqTonExtractorWorker.SsfSeqTonExtractorStruct();
            stexStruct.sourcePaths = s;
            stexStruct.extractToSubFolder = cbExtractToSubfolder.Checked;

            ssfSeqTonExtractorWorker = new SsfSeqTonExtractorWorker();
            ssfSeqTonExtractorWorker.ProgressChanged += backgroundWorker_ReportProgress;
            ssfSeqTonExtractorWorker.RunWorkerCompleted += SsfSeqTonExtractorWorker_WorkComplete;
            ssfSeqTonExtractorWorker.RunWorkerAsync(stexStruct);
        }

        private void SsfSeqTonExtractorWorker_WorkComplete(object sender,
             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "SEQ/TON Extraction...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "SEQ/TON Extraction...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (ssfSeqTonExtractorWorker != null && ssfSeqTonExtractorWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                ssfSeqTonExtractorWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
