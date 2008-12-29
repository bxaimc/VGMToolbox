using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            this.lblTitle.Text = "Datafile Checker";
            this.btnDoTask.Text = "Check";

            this.btnCancel.Hide();

            InitializeComponent();
        }

        private void btnDatafileChecker_Check_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "Checking Datafile...Begin";

            DatafileCheckerWorker.DatafileCheckerStruct datafileCheckerStruct = new DatafileCheckerWorker.DatafileCheckerStruct();
            datafileCheckerStruct.datafilePath = tbDatafileChecker_SourceFile.Text;
            datafileCheckerStruct.outputPath = tbDatafileChecker_OutputPath.Text;

            datafileCheckerWorker = new DatafileCheckerWorker();
            datafileCheckerWorker.ProgressChanged += backgroundWorker_ReportProgress;
            datafileCheckerWorker.RunWorkerCompleted += datafileCheckerWorker_WorkComplete;
            datafileCheckerWorker.RunWorkerAsync(datafileCheckerStruct);
        }

        private void datafileCheckerWorker_WorkComplete(object sender,
             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Checking Datafile...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Checking Datafile...Complete";
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
