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
    public partial class Xsf_Psf2ToPsf2LibForm : VgmtForm
    {
        Psf2toPsf2LibWorker psf2toPsf2LibWorker;

        public Xsf_Psf2ToPsf2LibForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_DoTaskButton"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_IntroText1"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_IntroText2"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_IntroText3"] + Environment.NewLine;

            InitializeComponent();

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_GroupSource"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_GroupOptions"];
            this.lblOutputFilePrefix.Text =
                ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_LblOutputFilePrefix"];

        }

        private void Psf2toPsf2LibWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (psf2toPsf2LibWorker != null && psf2toPsf2LibWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                psf2toPsf2LibWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2ToPsf2Lib_MessageBegin"];

            Psf2toPsf2LibWorker.Psf2ToPsf2LibStruct psf2Struct = new Psf2toPsf2LibWorker.Psf2ToPsf2LibStruct();
            psf2Struct.sourcePath = tbSourceDirectory.Text;
            psf2Struct.libraryName = tbFilePrefix.Text;

            psf2toPsf2LibWorker = new Psf2toPsf2LibWorker();
            psf2toPsf2LibWorker.ProgressChanged += backgroundWorker_ReportProgress;
            psf2toPsf2LibWorker.RunWorkerCompleted += Psf2toPsf2LibWorker_WorkComplete;
            psf2toPsf2LibWorker.RunWorkerAsync(psf2Struct);
        }

        private void btnSourceDirBrowse_Click(object sender, EventArgs e)
        {
            tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }

    }
}
