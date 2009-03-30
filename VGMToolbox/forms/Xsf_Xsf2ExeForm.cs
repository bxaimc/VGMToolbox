using System;
using System.ComponentModel;
using System.Configuration;
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
            this.lblTitle.Text = 
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_Title"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();            
            
            InitializeComponent();

            this.grpXsfPsf2Exe_Source.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_GroupSourceFiles"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_LblDragNDrop"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_GroupOptions"];
            this.cbExtractReservedSection.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxExtractReservedSection"];
            this.cbXsfPsf2Exe_IncludeOrigExt.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxIncludeOriginalExtension"];
            this.cbXsfPsf2Exe_StripGsfHeader.Text =
                ConfigurationSettings.AppSettings["Form_Xsf2Exe_CheckBoxStripGsfHeader"];
        }

        private void tbXsfPsf2Exe_Source_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageBegin"];

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            XsfCompressedProgramExtractorWorker.XsfCompressedProgramExtractorStruct xcpeStruct =
                new XsfCompressedProgramExtractorWorker.XsfCompressedProgramExtractorStruct();
            xcpeStruct.SourcePaths = s;
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
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = ConfigurationSettings.AppSettings["Form_Xsf2Exe_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (xsfCompressedProgramExtractor != null && xsfCompressedProgramExtractor.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
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
