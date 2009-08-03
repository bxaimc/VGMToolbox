using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class Psf2SqExtractorForm : AVgmtForm
    {        
        public Psf2SqExtractorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_Title"];          
            this.btnDoTask.Hide();

            InitializeComponent();

            // messages
            this.BackgroundWorker = new Psf2SqExtractorWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageCancel"];
            
            this.grpSource.AllowDrop = true;
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_IntroText"];

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
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
    }
}
