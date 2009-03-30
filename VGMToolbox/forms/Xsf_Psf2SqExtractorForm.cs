using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Psf2SqExtractorForm : VgmtForm
    {
        Psf2SqExtractorWorker psf2SqExtractorWorker;
        
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

        private void Psf2SqExtractorWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (psf2SqExtractorWorker != null && psf2SqExtractorWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                psf2SqExtractorWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void tbPsf2Source_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Psf2SqExtractor_MessageBegin"];

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Psf2SqExtractorWorker.Psf2SqExtractorStruct sqExtStruct = new Psf2SqExtractorWorker.Psf2SqExtractorStruct();
            sqExtStruct.SourcePaths = s;

            psf2SqExtractorWorker = new Psf2SqExtractorWorker();
            psf2SqExtractorWorker.ProgressChanged += backgroundWorker_ReportProgress;
            psf2SqExtractorWorker.RunWorkerCompleted += Psf2SqExtractorWorker_WorkComplete;
            psf2SqExtractorWorker.RunWorkerAsync(sqExtStruct);
        }
    }
}
