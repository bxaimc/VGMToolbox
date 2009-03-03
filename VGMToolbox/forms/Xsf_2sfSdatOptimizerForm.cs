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
    public partial class Xsf_2sfSdatOptimizerForm : VgmtForm
    {
        SdatOptimizerWorker sdatOptimizerWorker;

        public Xsf_2sfSdatOptimizerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = "SDAT Optimizer";
            this.tbOutput.Text = "Optimize SDATs for zlib compression." + Environment.NewLine;
            this.tbOutput.Text += "     Stage 1: Prepare STRMs (Functional - Minor Testing)" + Environment.NewLine;
            this.tbOutput.Text += "     Stage 2: Prepare SWAVs (Functional - Minor Testing)" + Environment.NewLine;
            this.tbOutput.Text += "     Stage 3: Prepare SBNKs (Functional - Minor Testing)" + Environment.NewLine;

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();
        }

        private void tbSourceSdat_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "SDAT Optimization...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            SdatOptimizerWorker.SdatOptimizerStruct soptStruct = new SdatOptimizerWorker.SdatOptimizerStruct();
            soptStruct.sourcePaths = s;
            soptStruct.startSequence = tbStartSequence.Text;
            soptStruct.endSequence = tbEndSequence.Text;

            sdatOptimizerWorker = new SdatOptimizerWorker();
            sdatOptimizerWorker.ProgressChanged += backgroundWorker_ReportProgress;
            sdatOptimizerWorker.RunWorkerCompleted += SdatOptimizerWorker_WorkComplete;
            sdatOptimizerWorker.RunWorkerAsync(soptStruct);
        }

        private void SdatOptimizerWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "SDAT Optimization...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "SDAT Optimization...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (sdatOptimizerWorker != null && sdatOptimizerWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                sdatOptimizerWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
