using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.tools.nds;

namespace VGMToolbox.forms
{
    public partial class Xsf_SdatExtractorForm : VgmtForm
    {
        SdatExtractorWorker sdatExtractorWorker;

        public Xsf_SdatExtractorForm(TreeNode pTreeNode): base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SdatExtractor_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SdatExtractor_IntroText"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            this.groupSource.Text =
                ConfigurationSettings.AppSettings["Form_SdatExtractor_GroupSource"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_SdatExtractor_LblDragNDrop"];
        }

        private void tbNDS_SdatExtractor_Source_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageBegin"];

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            int totalFileCount = 0;

            foreach (string path in s)
            {
                if (File.Exists(path))
                {
                    totalFileCount++;
                }
                else if (Directory.Exists(path))
                {
                    totalFileCount += Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;
                }
            }

            SdatExtractorWorker.SdatExtractorStruct sdexStruct = new SdatExtractorWorker.SdatExtractorStruct();
            sdexStruct.pPaths = s;
            sdexStruct.totalFiles = totalFileCount;

            sdatExtractorWorker = new SdatExtractorWorker();
            sdatExtractorWorker.ProgressChanged += backgroundWorker_ReportProgress;
            sdatExtractorWorker.RunWorkerCompleted += SdatExtractorWorker_WorkComplete;
            sdatExtractorWorker.RunWorkerAsync(sdexStruct);
        }

        private void SdatExtractorWorker_WorkComplete(object sender,
                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageComplete"];                
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (sdatExtractorWorker != null && sdatExtractorWorker.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                sdatExtractorWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}