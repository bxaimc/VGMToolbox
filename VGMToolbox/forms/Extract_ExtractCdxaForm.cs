using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.tools.extract;

namespace VGMToolbox.forms
{
    public partial class Extract_ExtractCdxaForm : VgmtForm
    {
        ExtractCdxaWorker extractCdxaWorker;
        
        public Extract_ExtractCdxaForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_CdxaExtractor_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_CdxaExtractor_IntroText"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_CdxaExtractor_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_CdxaExtractor_LblDragNDrop"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_CdxaExtractor_GroupOptions"];
            this.cbAddRiffHeader.Text =
                ConfigurationSettings.AppSettings["Form_CdxaExtractor_CheckBoxAddRiffHeader"];
            this.cbPatchByte0x11.Text =
                ConfigurationSettings.AppSettings["Form_CdxaExtractor_CheckBoxPatchByte0x11"];
        }

        private void tbSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "Extract XA data...Begin";

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExtractCdxaWorker.ExtractCdxaStruct extStruct = new ExtractCdxaWorker.ExtractCdxaStruct();
            extStruct.SourcePaths = s;
            extStruct.AddRiffHeader = cbAddRiffHeader.Checked;
            extStruct.PatchByte0x11 = cbPatchByte0x11.Checked;

            extractCdxaWorker = new ExtractCdxaWorker();
            extractCdxaWorker.ProgressChanged += backgroundWorker_ReportProgress;
            extractCdxaWorker.RunWorkerCompleted += ExtractCdxaWorker_WorkComplete;
            extractCdxaWorker.RunWorkerAsync(extStruct);
        }

        private void ExtractCdxaWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Extract XA data...Cancelled";
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Extract XA data...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (extractCdxaWorker != null && extractCdxaWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                extractCdxaWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }        
    }
}
