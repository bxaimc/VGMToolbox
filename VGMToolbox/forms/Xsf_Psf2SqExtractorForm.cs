using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Psf2SqExtractorForm : AVgmtForm
    {        
        public Xsf_Psf2SqExtractorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_Title"];          
            this.btnDoTask.Hide();

            InitializeComponent();

            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_IntroText"];

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_LblDragNDrop"];
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void tbPsf2Source_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Psf2SqExtractorWorker.Psf2SqExtractorStruct sqExtStruct = new Psf2SqExtractorWorker.Psf2SqExtractorStruct();
            sqExtStruct.SourcePaths = s;

            base.backgroundWorker_Execute(sqExtStruct);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new Psf2SqExtractorWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageBegin"];
        }
    }
}
