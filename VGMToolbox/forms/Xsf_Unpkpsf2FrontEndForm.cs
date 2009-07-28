using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Unpkpsf2FrontEndForm : AVgmtForm
    {        
        public Xsf_Unpkpsf2FrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_IntroText"];
            this.btnDoTask.Hide();

            InitializeComponent();

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

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new UnpkPsf2Worker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageBegin"];
        }
    }
}
