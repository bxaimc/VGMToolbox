using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Xsf2ExeForm : VgmtForm
    {
        XsfCompressedProgramExtractorWorker xsfCompressedProgramExtractor;
        
        public Xsf_Xsf2ExeForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "xSF to EXE";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();            
            
            InitializeComponent();
        }

        private void tbXsfPsf2Exe_Source_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "PSF2EXE...Begin";

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

            XsfCompressedProgramExtractorWorker.XsfCompressedProgramExtractorStruct xcpeStruct =
                new XsfCompressedProgramExtractorWorker.XsfCompressedProgramExtractorStruct();
            xcpeStruct.pPaths = s;
            xcpeStruct.totalFiles = totalFileCount;
            xcpeStruct.includeExtension = cbXsfPsf2Exe_IncludeOrigExt.Checked;
            xcpeStruct.stripGsfHeader = cbXsfPsf2Exe_StripGsfHeader.Checked;
            xcpeStruct.extractReservedSection = cbExtractReservedSection.Checked;

            xsfCompressedProgramExtractor = new XsfCompressedProgramExtractorWorker();
            xsfCompressedProgramExtractor.ProgressChanged += backgroundWorker_ReportProgress;
            xsfCompressedProgramExtractor.RunWorkerCompleted += XsfCompressedProgramExtractorWorker_WorkComplete;
            xsfCompressedProgramExtractor.RunWorkerAsync(xcpeStruct);
        }

        private void XsfCompressedProgramExtractorWorker_WorkComplete(object sender,
                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "PSF2EXE...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "PSF2EXE...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (xsfCompressedProgramExtractor != null && xsfCompressedProgramExtractor.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                xsfCompressedProgramExtractor.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
