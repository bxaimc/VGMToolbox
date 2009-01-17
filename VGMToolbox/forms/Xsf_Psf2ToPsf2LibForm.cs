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
    public partial class Xsf_Psf2ToPsf2LibForm : VgmtForm
    {
        Psf2toPsf2LibWorker psf2toPsf2LibWorker;

        public Xsf_Psf2ToPsf2LibForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = "PSF2 to PSF2LIB (Library Builder)";
            this.btnDoTask.Text = "Combine PSF2s";
            
            InitializeComponent();
        }

        private void Psf2toPsf2LibWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Combine PSF2s...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Combine PSF2s...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (psf2toPsf2LibWorker != null && psf2toPsf2LibWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                psf2toPsf2LibWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "Combine PSF2s...Begin";

            Psf2toPsf2LibWorker.Psf2ToPsf2LibStruct psf2Struct = new Psf2toPsf2LibWorker.Psf2ToPsf2LibStruct();
            psf2Struct.sourcePath = tbSourceDirectory.Text;
            psf2Struct.libraryName = tbFilePrefix.Text;

            psf2toPsf2LibWorker = new Psf2toPsf2LibWorker();
            psf2toPsf2LibWorker.ProgressChanged += backgroundWorker_ReportProgress;
            psf2toPsf2LibWorker.RunWorkerCompleted += Psf2toPsf2LibWorker_WorkComplete;
            psf2toPsf2LibWorker.RunWorkerAsync(psf2Struct);
        }

        private void btnSourceDirBrowse_Click(object sender, EventArgs e)
        {
            tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }

    }
}
