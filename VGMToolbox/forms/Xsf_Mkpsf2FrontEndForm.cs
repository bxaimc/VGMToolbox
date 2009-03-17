using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Mkpsf2FrontEndForm : VgmtForm
    {
        MkPsf2Worker mkPsf2Worker;
        
        public Xsf_Mkpsf2FrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_DoTaskButton"];

            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_IntroText1"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_MkPsf2FE_IntroText2"] + Environment.NewLine + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_MkPsf2FE_IntroText3"];

            InitializeComponent();

            this.grpDirectory.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_GroupDirectory"];
            this.lblSourceDirectory.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblSourceDirectory"];
            this.lblModulesDirectory.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblModulesDirectory"];
            this.lblOutputFolder.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblOutputFolder"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_GroupOptions"];
            this.lblReverb.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblReverb"];
            this.lblDepth.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblDepth"];
            this.lblVolume.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblVolume"];
            this.lblTempo.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblTempo"];
            this.lblTickInterval.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblTickInterval"];
            this.lblAuthor.Text =
                ConfigurationSettings.AppSettings["Form_MkPsf2FE_LblAuthor"];

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();
            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_MessageBegin"];

            MkPsf2Worker.MkPsf2Struct mkStruct = new MkPsf2Worker.MkPsf2Struct();
            mkStruct.sourcePath = tbSourceDirectory.Text;
            mkStruct.modulePath = tbModulesDirectory.Text;
            mkStruct.outputFolder = tbOutputFolderName.Text;

            mkStruct.depth = tbDepth.Text;
            mkStruct.reverb = tbReverb.Text;
            mkStruct.tempo = tbTempo.Text;
            mkStruct.tickInterval = tbTickInterval.Text;
            mkStruct.volume = tbVolume.Text;

            mkPsf2Worker = new MkPsf2Worker();
            mkPsf2Worker.ProgressChanged += backgroundWorker_ReportProgress;
            mkPsf2Worker.RunWorkerCompleted += MkPsf2Worker_WorkComplete;
            mkPsf2Worker.RunWorkerAsync(mkStruct);
        }

        private void MkPsf2Worker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mkPsf2Worker != null && mkPsf2Worker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                mkPsf2Worker.CancelAsync();
                this.errorFound = true;
            }
        }

        private void btnSourceDirectoryBrowse_Click(object sender, EventArgs e)
        {
            tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }

        private void btnModulesDirectoryBrowse_Click(object sender, EventArgs e)
        {
            tbModulesDirectory.Text = base.browseForFolder(sender, e);
        }
    }
}
