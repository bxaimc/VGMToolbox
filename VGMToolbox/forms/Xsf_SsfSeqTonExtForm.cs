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
    public partial class Xsf_SsfSeqTonExtForm : VgmtForm
    {
        SsfSeqTonExtractorWorker ssfSeqTonExtractorWorker;

        public Xsf_SsfSeqTonExtForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SeqextTonextFE_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SeqextTonextFE_IntroText"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_SeqextTonextFE_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_SeqextTonextFE_LblDragNDrop"];
            this.cbExtractToSubfolder.Text =
                ConfigurationSettings.AppSettings["Form_SeqextTonextFE_CheckBoxExtractToSubfolder"];
            this.lblAuthor.Text =
                ConfigurationSettings.AppSettings["Form_SeqextTonextFE_LblAuthor"];
        }

        private void tbSsfSqTonExtSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SeqextTonextFE_MessageBegin"];

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
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SeqextTonextFE_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SeqextTonextFE_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (ssfSeqTonExtractorWorker != null && ssfSeqTonExtractorWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
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
