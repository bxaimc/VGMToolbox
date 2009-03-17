using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Bin2PsfFrontEndForm : VgmtForm
    {
        Bin2PsfWorker bin2PsfWorker;

        public Xsf_Bin2PsfFrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_DoTaskButton"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_IntroText"];

            InitializeComponent();

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_GroupSource"];
            this.lblDriverPath.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblDriverPath"];
            this.lblSourceFiles.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblSourceFiles"];
            this.lblOutputFolder.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblOutputFolder"];
            this.lblPsfLibName.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblPsfLibName"];
            this.cbMinipsf.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_CheckBoxMinipsf"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_GroupOptions"];
            this.lblSeqOffset.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblSeqOffset"];
            this.lblVhOffset.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblVhOffset"];
            this.lblSeqOffset.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblVbOffset"];
        }

        private void Bin2PsfWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (bin2PsfWorker != null && bin2PsfWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                bin2PsfWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void btnExeBrowse_Click(object sender, EventArgs e)
        {
            tbExePath.Text = base.browseForFile(sender, e);
        }

        private void btnSourceDirectoryBrowse_Click(object sender, EventArgs e)
        {
            tbSourceFilesPath.Text = base.browseForFolder(sender, e);
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageBegin"];

            Bin2PsfWorker.Bin2PsfStruct bpStruct = new Bin2PsfWorker.Bin2PsfStruct();
            bpStruct.sourcePath = tbSourceFilesPath.Text;
            bpStruct.seqOffset = tbSeqOffset.Text;
            bpStruct.vbOffset = tbVbOffset.Text;
            bpStruct.vhOffset = tbVhOffset.Text;
            bpStruct.exePath = tbExePath.Text;
            bpStruct.outputFolder = tbOutputFolderName.Text;

            bpStruct.makeMiniPsfs = cbMinipsf.Checked;
            bpStruct.psflibName = tbPsflibName.Text;

            bin2PsfWorker = new Bin2PsfWorker();
            bin2PsfWorker.ProgressChanged += backgroundWorker_ReportProgress;
            bin2PsfWorker.RunWorkerCompleted += Bin2PsfWorker_WorkComplete;
            bin2PsfWorker.RunWorkerAsync(bpStruct);
        }

        private void cbMinipsf_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMinipsf.Checked)
            {
                tbPsflibName.ReadOnly = false;
            }
            else
            {
                tbPsflibName.ReadOnly = true;
                tbPsflibName.Clear();
            }
        }
    }
}
