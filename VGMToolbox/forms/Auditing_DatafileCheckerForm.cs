using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.auditing;

namespace VGMToolbox.forms
{
    public partial class Auditing_DatafileCheckerForm : VgmtForm
    {
        DatafileCheckerWorker datafileCheckerWorker;
        
        public Auditing_DatafileCheckerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text =
                ConfigurationSettings.AppSettings["Form_AuditDatafileChecker_Title"];
            this.btnDoTask.Text =
                ConfigurationSettings.AppSettings["Form_AuditDatafileChecker_DoTaskButton"];

            this.btnCancel.Hide();

            InitializeComponent();

            // languages
            this.gbSourceDestination.Text = 
                ConfigurationSettings.AppSettings["Form_AuditDatafileChecker_GroupSource"];
            this.lblSourceDataFile.Text = 
                ConfigurationSettings.AppSettings["Form_AuditDatafileChecker_LblSourceDataFile"];
            this.lblReportDestination.Text = 
                ConfigurationSettings.AppSettings["Form_AuditDatafileChecker_LblReportDestination"];
                    
        }

        private void btnDatafileChecker_Check_Click(object sender, EventArgs e)
        {
            if (base.checkFileExists(tbDatafileChecker_SourceFile.Text, this.lblSourceDataFile.Text) &&
                base.checkFolderExists(tbDatafileChecker_OutputPath.Text, this.lblReportDestination.Text))
            {
                base.initializeProcessing();

                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_AuditDatafileChecker_MessageBegin"];

                DatafileCheckerWorker.DatafileCheckerStruct datafileCheckerStruct = new DatafileCheckerWorker.DatafileCheckerStruct();
                datafileCheckerStruct.datafilePath = tbDatafileChecker_SourceFile.Text;
                datafileCheckerStruct.outputPath = tbDatafileChecker_OutputPath.Text;

                datafileCheckerWorker = new DatafileCheckerWorker();
                datafileCheckerWorker.ProgressChanged += backgroundWorker_ReportProgress;
                datafileCheckerWorker.RunWorkerCompleted += datafileCheckerWorker_WorkComplete;
                datafileCheckerWorker.RunWorkerAsync(datafileCheckerStruct);
            }
        }

        private void datafileCheckerWorker_WorkComplete(object sender,
             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_AuditDatafileChecker_MessageCancel"];
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text =
                    ConfigurationSettings.AppSettings["Form_AuditDatafileChecker_MessageComplete"];
            }

            // update node color
            this.setNodeAsComplete();
        }

        private void tbDatafileChecker_BrowseSource_Click(object sender, EventArgs e)
        {
            tbDatafileChecker_SourceFile.Text = base.browseForFile(sender, e);
        }

        private void tbDatafileChecker_BrowseDestination_Click(object sender, EventArgs e)
        {
            tbDatafileChecker_OutputPath.Text = base.browseForFolder(sender, e);
        }


    }
}
