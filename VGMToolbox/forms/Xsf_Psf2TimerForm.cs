using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_Psf2Timer_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_Psf2Timer_IntroText"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();

            this.gbSource.Text =
                ConfigurationSettings.AppSettings["Form_Psf2Timer_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_Psf2Timer_LblDragNDrop"];
        }

        private void Psf2TimerWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2Timer_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2Timer_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (psf2TimerWorker != null && psf2TimerWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
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

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2Timer_MessageBegin"];

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Psf2TimerWorker.Psf2TimerStruct timeStruct = new Psf2TimerWorker.Psf2TimerStruct();
            timeStruct.SourcePaths = s;

            psf2TimerWorker = new Psf2TimerWorker();
            psf2TimerWorker.ProgressChanged += backgroundWorker_ReportProgress;
            psf2TimerWorker.RunWorkerCompleted += Psf2TimerWorker_WorkComplete;
            psf2TimerWorker.RunWorkerAsync(timeStruct);
        }
    }
}
