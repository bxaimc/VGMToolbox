using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_SsfMakeFrontEndForm : VgmtForm
    {
        SsfMakeWorker ssfMakeWorker;
        
        public Xsf_SsfMakeFrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SsfMakeFE_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_SsfMakeFE_DoTaskButton"];
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_SsfMakeFE_IntroText1"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_SsfMakeFE_IntroText2"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_SsfMakeFE_IntroText3"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_SsfMakeFE_IntroText4"] + Environment.NewLine;

            InitializeComponent();

            this.groupFiles.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_GroupFiles"];
            this.lblDriver.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblDriver"];
            this.lblSourcePath.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblSourcePath"];
            this.lblOutputFolder.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblOutputFolder"];
            this.lblSingleDspFile.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblSingleDspFile"];
            this.grpSettings.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_GroupSettings"];
            this.lblSequenceBank.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblSequenceBank"];
            this.lblSequenceTrack.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblSequenceTrack"];
            this.lblVolume.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblVolume"];
            this.lblMixerBank.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblMixerBank"];
            this.lblMixerNumber.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblMixerNumber"];
            this.lblEffect.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_LblEffect"];
            this.cbMatchSeqBank.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_CheckBoxMatchSeqBank"];
            this.cbSeekData.Text =
                ConfigurationSettings.AppSettings["Form_SsfMakeFE_CheckBoxSeekData"];

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();
            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageBegin"];

            SsfMakeWorker.SsfMakeStruct smStruct = new SsfMakeWorker.SsfMakeStruct();
            smStruct.sequenceBank = tbSequenceBank.Text;
            smStruct.sequenceTrack = tbSequenceTrack.Text;
            smStruct.volume = tbVolume.Text;
            smStruct.mixerBank = tbMixerBank.Text;
            smStruct.mixerNumber = tbMixerNumber.Text;
            smStruct.effectNumber = tbEffect.Text;
            smStruct.useDsp = "0"; // logic within worker will set this
            smStruct.driver = tbDriver.Text;
            // smStruct.map = tbMapFile.Text.Trim();
            smStruct.dspFile = tbDspFile.Text;

            smStruct.sourcePath = tbSourcePath.Text;
            smStruct.outputFolder = tbOutputFolder.Text;
            smStruct.findData = cbSeekData.Checked;

            ssfMakeWorker = new SsfMakeWorker();
            ssfMakeWorker.ProgressChanged += backgroundWorker_ReportProgress;
            ssfMakeWorker.RunWorkerCompleted += SsfMakeWorker_WorkComplete;
            ssfMakeWorker.RunWorkerAsync(smStruct);
        }

        private void SsfMakeWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (ssfMakeWorker != null && ssfMakeWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                ssfMakeWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        private void tbSequenceBank_TextChanged(object sender, EventArgs e)
        {
            if (cbMatchSeqBank.Checked)
            {
                tbMixerBank.Text = tbSequenceBank.Text;
            }
        }

        private void cbMatchSeqBank_CheckedChanged(object sender, EventArgs e)
        {            
            if (cbMatchSeqBank.Checked)
            {
                tbMixerBank.Text = tbSequenceBank.Text;
            }

            tbMixerBank.ReadOnly = cbMatchSeqBank.Checked;
        }

        private void btnBrowseDriver_Click(object sender, EventArgs e)
        {
            tbDriver.Text = base.browseForFile(sender, e);
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            tbSourcePath.Text = base.browseForFolder(sender, e);
        }

        private void btnBrowseDsp_Click(object sender, EventArgs e)
        {
            tbDspFile.Text = base.browseForFile(sender, e);
        }
    }
}
