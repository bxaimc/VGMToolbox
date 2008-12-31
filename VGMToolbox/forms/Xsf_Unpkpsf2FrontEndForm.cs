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
    public partial class Xsf_Unpkpsf2FrontEndForm : VgmtForm
    {
        UnpkPsf2Worker unpkPsf2Worker;
        
        public Xsf_Unpkpsf2FrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "unpkpsf2 Front End";
            this.btnDoTask.Hide();

            InitializeComponent();
        }

        private void tbPsf2Source_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "PSF2 Unpacking...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            UnpkPsf2Worker.UnpkPsf2Struct unpkStruct = new UnpkPsf2Worker.UnpkPsf2Struct();
            unpkStruct.sourcePaths = s;

            unpkPsf2Worker = new UnpkPsf2Worker();
            unpkPsf2Worker.ProgressChanged += backgroundWorker_ReportProgress;
            unpkPsf2Worker.RunWorkerCompleted += Unpkpsf2Worker_WorkComplete;
            unpkPsf2Worker.RunWorkerAsync(unpkStruct);
        }

        private void Unpkpsf2Worker_WorkComplete(object sender,
             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "PSF2 Unpacking...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "PSF2 Unpacking...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (unpkPsf2Worker != null && unpkPsf2Worker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                unpkPsf2Worker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
