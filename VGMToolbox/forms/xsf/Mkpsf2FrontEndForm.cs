using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class Mkpsf2FrontEndForm : AVgmtForm
    {        
        public Mkpsf2FrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_DoTaskButton"];

            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_MkPsf2FE_IntroText1"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_MkPsf2FE_IntroText2"] + Environment.NewLine + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_MkPsf2FE_IntroText3"];

            InitializeComponent();

            // messages
            this.BackgroundWorker = new MkPsf2Worker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_MkPsf2FE_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_MkPsf2FE_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_MkPsf2FE_MessageCancel"];

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
            MkPsf2Worker.MkPsf2Struct mkStruct = new MkPsf2Worker.MkPsf2Struct();
            mkStruct.sourcePath = tbSourceDirectory.Text;
            mkStruct.modulePath = tbModulesDirectory.Text;
            mkStruct.outputFolder = tbOutputFolderName.Text;

            mkStruct.depth = tbDepth.Text;
            mkStruct.reverb = tbReverb.Text;
            mkStruct.tempo = tbTempo.Text;
            mkStruct.tickInterval = tbTickInterval.Text;
            mkStruct.volume = tbVolume.Text;

            base.backgroundWorker_Execute(mkStruct);
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
