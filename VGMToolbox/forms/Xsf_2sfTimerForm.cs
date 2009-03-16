using System;
using System.ComponentModel;
using System.Configuration;
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
            this.lblTitle.Text = 
                ConfigurationSettings.AppSettings["Form_2sfTimer_Title"];
            this.btnDoTask.Text = 
                ConfigurationSettings.AppSettings["Form_2sfTimer_DoTaskButton"];
            this.tbOutput.Text = 
                ConfigurationSettings.AppSettings["Form_2sfTimer_IntroText"];

            InitializeComponent();

            this.grpSourcePaths.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_GroupSourcePaths"];
            this.lblPathTo2sfFiles.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_LblPathTo2sfFiles"];
            this.lblPathToSdat.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_LblPathToSdat"];
            this.lblFilePrefix.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_LblFilePrefix"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_GroupOptions"];
            this.cbOneLoop.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_CheckboxSingleLoop"];
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();
            toolStripStatusLabel1.Text = 
                ConfigurationSettings.AppSettings["Form_2sfTimer_MessageBegin"];

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
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_2sfTimer_MessageCancel"];
                tbOutput.Text +=
                    ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_2sfTimer_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (time2sfWorker != null && time2sfWorker.IsBusy)
            {
                tbOutput.Text += 
                    ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
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
