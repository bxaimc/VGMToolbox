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
    public partial class Xsf_Psf2TimerForm : VgmtForm
    {
        Psf2TimerWorker psf2TimerWorker;
        
        public Xsf_Psf2TimerForm(TreeNode pTreeNode)
            : base(pTreeNode) 
        {
            this.lblTitle.Text = "PSF2 Timer";
            this.tbOutput.Text = "Drag and Drop PSF2s onto the box above.  A timing script will be output in each PSF2's directory.";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();
        }

        private void Psf2TimerWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Time PSF2s...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Time PSF2s...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (psf2TimerWorker != null && psf2TimerWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                psf2TimerWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void tbSourcePaths_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "Time PSF2s...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Psf2TimerWorker.Psf2TimerStruct timeStruct = new Psf2TimerWorker.Psf2TimerStruct();
            timeStruct.sourcePaths = s;

            psf2TimerWorker = new Psf2TimerWorker();
            psf2TimerWorker.ProgressChanged += backgroundWorker_ReportProgress;
            psf2TimerWorker.RunWorkerCompleted += Psf2TimerWorker_WorkComplete;
            psf2TimerWorker.RunWorkerAsync(timeStruct);
        }
    }
}
