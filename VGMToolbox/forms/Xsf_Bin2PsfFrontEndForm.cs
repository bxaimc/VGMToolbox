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
    public partial class Xsf_Bin2PsfFrontEndForm : VgmtForm
    {
        Bin2PsfWorker bin2PsfWorker;

        public Xsf_Bin2PsfFrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "bin2psf Front End";
            this.btnDoTask.Text = "Make PSFs";

            InitializeComponent();
        }

        private void Bin2PsfWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "PSF Creation...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "PSF Creation...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (bin2PsfWorker != null && bin2PsfWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                bin2PsfWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void btnExeBrowse_Click(object sender, EventArgs e)
        {
            tbExePath.Text = base.browseForFile(sender, e);
        }

        private void btnSourceDirectoryBrowse_Click(object sender, EventArgs e)
        {
            tbSourceFilesPath.Text = base.browseForFolder(sender, e);
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "PSF Creation...Begin";

            Bin2PsfWorker.Bin2PsfStruct bpStruct = new Bin2PsfWorker.Bin2PsfStruct();
            bpStruct.sourcePath = tbSourceFilesPath.Text;
            bpStruct.seqOffset = tbSeqOffset.Text;
            bpStruct.vbOffset = tbVbOffset.Text;
            bpStruct.vhOffset = tbVhOffset.Text;
            bpStruct.exePath = tbExePath.Text;
            bpStruct.outputFolder = tbOutputFolderName.Text;

            bpStruct.makeMiniPsfs = cbMinipsf.Checked;
            bpStruct.psflibName = tbPsflibName.Text;

            bin2PsfWorker = new Bin2PsfWorker();
            bin2PsfWorker.ProgressChanged += backgroundWorker_ReportProgress;
            bin2PsfWorker.RunWorkerCompleted += Bin2PsfWorker_WorkComplete;
            bin2PsfWorker.RunWorkerAsync(bpStruct);
        }

        private void cbMinipsf_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMinipsf.Checked)
            {
                tbPsflibName.ReadOnly = false;
            }
            else
            {
                tbPsflibName.ReadOnly = true;
                tbPsflibName.Clear();
            }
        }
    }
}
