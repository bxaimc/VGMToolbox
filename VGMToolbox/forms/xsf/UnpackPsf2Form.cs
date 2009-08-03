using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class UnpackPsf2Form : AVgmtForm
    {        
        public UnpackPsf2Form(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_IntroText"];
            this.btnDoTask.Hide();

            InitializeComponent();

            // messages
            this.BackgroundWorker = new UnpkPsf2Worker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageCancel"];

            this.grpSource.AllowDrop = true;
            this.grpSource.Text = ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
        }

        private void tbPsf2Source_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            UnpkPsf2Worker.UnpkPsf2Struct unpkStruct = new UnpkPsf2Worker.UnpkPsf2Struct();
            unpkStruct.SourcePaths = s;

            base.backgroundWorker_Execute(unpkStruct);
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
