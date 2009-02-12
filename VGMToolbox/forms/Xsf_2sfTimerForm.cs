using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_2sfTimerForm : VgmtForm
    {
        Time2sfWorker time2sfWorker;
        
        public Xsf_2sfTimerForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "2SF Timer";
            this.btnDoTask.Text = "Time 2SFs";
            this.tbOutput.Text = "File Prefix is the first part of the file name for every file in the " +
                "2SF set for the input SDAT.  For example, given the inputs 'foo-0001.mini2sf', " +
                "'foo-0002.mini2sf'... the file prefix would be 'foo'.";

            InitializeComponent();


        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();
            toolStripStatusLabel1.Text = "Time 2SFs...Begin";

            Time2sfWorker.Time2sfStruct t2Struct = new Time2sfWorker.Time2sfStruct();
            t2Struct.pathTo2sf = tbPathTo2sfs.Text;
            t2Struct.pathToSdat = tbSdatPath.Text;
            t2Struct.filePrefix = tbFilePrefix.Text;
            t2Struct.doSingleLoop = cbOneLoop.Checked;

            time2sfWorker = new Time2sfWorker();
            time2sfWorker.ProgressChanged += backgroundWorker_ReportProgress;
            time2sfWorker.RunWorkerCompleted += Time2sfWorker_WorkComplete;
            time2sfWorker.RunWorkerAsync(t2Struct);
        } 
        
        private void Time2sfWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Time 2SFs...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                toolStripStatusLabel1.Text = "Time 2SFs...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (time2sfWorker != null && time2sfWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                time2sfWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        private void btn2sfBrowse_Click(object sender, EventArgs e)
        {
            tbPathTo2sfs.Text = base.browseForFolder(sender, e);
        }

        private void btnSdatBrowse_Click(object sender, EventArgs e)
        {
            tbSdatPath.Text = base.browseForFile(sender, e);
        }       
    }
}
