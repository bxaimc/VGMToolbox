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
    public partial class Xsf_2sfSdatOptimizerForm : VgmtForm
    {
        SdatOptimizerWorker sdatOptimizerWorker;

        public Xsf_2sfSdatOptimizerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SdatOptimizer_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SdatOptimizer_IntroText"] + Environment.NewLine;

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_LblDragNDrop"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_GroupOptions"];
            this.lblStartingSequence.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_LblStartingSequence"];
            this.lblEndingSequence.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_LblEndingSequence"];
            this.cbIncludeAllSseq.Text =
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_CheckBoxIncludeAllSequences"];
        }

        private void tbSourceSdat_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = 
                ConfigurationSettings.AppSettings["Form_SdatOptimizer_MessageBegin"];

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            SdatOptimizerWorker.SdatOptimizerStruct soptStruct = new SdatOptimizerWorker.SdatOptimizerStruct();
            soptStruct.SourcePaths = s;

            if (!cbIncludeAllSseq.Checked)
            {
                soptStruct.startSequence = tbStartSequence.Text;
                soptStruct.endSequence = tbEndSequence.Text;
            }

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
                toolStripStatusLabel1.Text =
                    ConfigurationSettings.AppSettings["Form_SdatOptimizer_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_SdatOptimizer_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (sdatOptimizerWorker != null && sdatOptimizerWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                sdatOptimizerWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void cbIncludeAllSseq_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIncludeAllSseq.Checked)
            {
                tbStartSequence.ReadOnly = true;
                tbEndSequence.ReadOnly = true;
            }
            else
            {
                tbStartSequence.ReadOnly = false;
                tbEndSequence.ReadOnly = false;            
            }
        }
    }
}
