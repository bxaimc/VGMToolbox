using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_PsxSeqExtractForm : VgmtForm
    {
        PsxSeqExtractWorker psxSeqExtractWorker;
        
        public Xsf_PsxSeqExtractForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SeqExtractor_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SeqExtractor_IntroText"];
            this.btnDoTask.Hide();
            
            InitializeComponent();

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_LblDragNDrop"];            
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_GroupOptions"];
            this.cbForce2Loops.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_CheckBoxForce2Loops"];
            this.cbForceType.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_CheckBoxForceType"];
            this.rbForceSepType.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_RadioForceSepType"];
            this.rbForceSeqType.Text =
                ConfigurationSettings.AppSettings["Form_SeqExtractor_RadioForceSeqType"];
        }

        private void PsxSeqExtractWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SeqExtractor_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SeqExtractor_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (psxSeqExtractWorker != null && psxSeqExtractWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                psxSeqExtractWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void tbSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SeqExtractor_MessageBegin"];

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            PsxSeqExtractWorker.PsxSeqExtractStruct psxStruct = new PsxSeqExtractWorker.PsxSeqExtractStruct();
            psxStruct.SourcePaths = s;
            psxStruct.force2Loops = cbForce2Loops.Checked;
            psxStruct.forceSepType = rbForceSepType.Checked;
            psxStruct.forceSeqType = rbForceSeqType.Checked;

            psxSeqExtractWorker = new PsxSeqExtractWorker();
            psxSeqExtractWorker.ProgressChanged += backgroundWorker_ReportProgress;
            psxSeqExtractWorker.RunWorkerCompleted += PsxSeqExtractWorker_WorkComplete;
            psxSeqExtractWorker.RunWorkerAsync(psxStruct);
        }

        private void cbForceType_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbForceType.Checked)
            {
                this.rbForceSepType.Enabled = true;
                this.rbForceSeqType.Enabled = true;
            }
            else
            {
                this.rbForceSepType.Enabled = false;
                this.rbForceSeqType.Enabled = false;

                this.rbForceSepType.Checked = false;
                this.rbForceSeqType.Checked = false;
            }
        }
    }
}
