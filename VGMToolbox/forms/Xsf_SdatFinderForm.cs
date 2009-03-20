using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SdatFinder_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SdatFinder_IntroText"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_SdatFinder_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_SdatFinder_LblDragNDrop"];
        }

        private void SdatFinderWorker_WorkComplete(object sender,
             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SdatFinder_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SdatFinder_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (sdatFinderWorker != null && sdatFinderWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
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

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SdatFinder_MessageBegin"];

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
