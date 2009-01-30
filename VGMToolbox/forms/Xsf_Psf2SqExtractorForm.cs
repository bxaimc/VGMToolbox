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
    public partial class Xsf_Psf2SqExtractorForm : VgmtForm
    {
        Psf2SqExtractorWorker psf2SqExtractorWorker;
        
        public Xsf_Psf2SqExtractorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "PSF2 SQ Extractor";          
            this.btnDoTask.Hide();

            InitializeComponent();

            this.tbOutput.Text = "Extract only the SQ files from CSL PSF2s and rename" +
                " them with the same name as the source.  Useful when timing tracks.";
        }

        private void Psf2SqExtractorWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "SQ Extraction...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "SQ Extraction...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (psf2SqExtractorWorker != null && psf2SqExtractorWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                psf2SqExtractorWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void tbPsf2Source_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "SQ Extraction...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Psf2SqExtractorWorker.Psf2SqExtractorStruct sqExtStruct = new Psf2SqExtractorWorker.Psf2SqExtractorStruct();
            sqExtStruct.sourcePaths = s;

            psf2SqExtractorWorker = new Psf2SqExtractorWorker();
            psf2SqExtractorWorker.ProgressChanged += backgroundWorker_ReportProgress;
            psf2SqExtractorWorker.RunWorkerCompleted += Psf2SqExtractorWorker_WorkComplete;
            psf2SqExtractorWorker.RunWorkerAsync(sqExtStruct);
        }
    }
}
