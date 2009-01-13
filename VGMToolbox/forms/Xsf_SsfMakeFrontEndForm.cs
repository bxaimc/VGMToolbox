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
    public partial class Xsf_SsfMakeFrontEndForm : VgmtForm
    {
        SsfMakeWorker ssfMakeWorker;
        
        public Xsf_SsfMakeFrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "ssfmake Front End (note: Python must be installed and in your PATH.)";
            this.btnDoTask.Text = "Build SSFs";
            this.tbOutput.Text = "This tool is incomplete and does not function.";

            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();
            toolStripStatusLabel1.Text = "Build SSFs...Begin";

            SsfMakeWorker.SsfMakeStruct smStruct = new SsfMakeWorker.SsfMakeStruct();
            smStruct.sequenceBank = tbSequenceBank.Text;
            smStruct.sequenceTrack = tbSequenceTrack.Text;
            smStruct.volume = tbVolume.Text;
            smStruct.mixerBank = tbMixerBank.Text;
            smStruct.mixerNumber = tbMixerNumber.Text;
            smStruct.effectNumber = tbEffect.Text;
            smStruct.useDsp = String.IsNullOrEmpty(tbDsp.Text)? "0" : "1";
            smStruct.driver = tbDriver.Text;
            smStruct.toneData = tbToneData.Text;
            smStruct.sequenceData = tbSequence.Text;
            smStruct.dspProgram = tbDsp.Text;

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
                toolStripStatusLabel1.Text = "Build SSFs...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                toolStripStatusLabel1.Text = "Build SSFs...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (ssfMakeWorker != null && ssfMakeWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
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
    }
}
