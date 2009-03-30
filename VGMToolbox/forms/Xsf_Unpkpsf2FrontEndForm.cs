using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Unpkpsf2FrontEndForm : VgmtForm
    {
        UnpkPsf2Worker unpkPsf2Worker;
        
        public Xsf_Unpkpsf2FrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_IntroText"];
            this.btnDoTask.Hide();

            InitializeComponent();

            this.grpSource.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_GroupSource"];
            this.lblDragNDrop.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_LblDragNDrop"];
            this.lblAuthor.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_LblAuthor"];
        }

        private void tbPsf2Source_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageBegin"];

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            UnpkPsf2Worker.UnpkPsf2Struct unpkStruct = new UnpkPsf2Worker.UnpkPsf2Struct();
            unpkStruct.SourcePaths = s;

            unpkPsf2Worker = new UnpkPsf2Worker();
            unpkPsf2Worker.ProgressChanged += backgroundWorker_ReportProgress;
            unpkPsf2Worker.RunWorkerCompleted += Unpkpsf2Worker_WorkComplete;
            unpkPsf2Worker.RunWorkerAsync(unpkStruct);
        }

        private void Unpkpsf2Worker_WorkComplete(object sender,
             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_UnpkPsf2FE_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (unpkPsf2Worker != null && unpkPsf2Worker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                unpkPsf2Worker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
