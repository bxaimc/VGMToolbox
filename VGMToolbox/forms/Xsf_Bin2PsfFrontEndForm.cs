using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Bin2PsfFrontEndForm : AVgmtForm
    {
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
            this.lblVbOffset.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblVbOffset"];
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            Bin2PsfWorker.Bin2PsfStruct bpStruct = new Bin2PsfWorker.Bin2PsfStruct();
            bpStruct.sourcePath = tbSourceFilesPath.Text;
            bpStruct.seqOffset = tbSeqOffset.Text;
            bpStruct.vbOffset = tbVbOffset.Text;
            bpStruct.vhOffset = tbVhOffset.Text;
            bpStruct.exePath = tbExePath.Text;
            bpStruct.outputFolder = tbOutputFolderName.Text;
            bpStruct.makeMiniPsfs = cbMinipsf.Checked;
            bpStruct.psflibName = tbPsflibName.Text;

            base.backgroundWorker_Execute(bpStruct);
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

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new Bin2PsfWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageBegin"];
        }
    }
}
