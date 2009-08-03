using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class TwoSfTimerForm : AVgmtForm
    {        
        public TwoSfTimerForm(TreeNode pTreeNode)
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

            // messages
            this.BackgroundWorker = new Time2sfWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_2sfTimer_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_2sfTimer_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_2sfTimer_MessageCancel"];

            this.grpSourcePaths.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_GroupSourcePaths"];
            this.lblPathTo2sfFiles.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_LblPathTo2sfFiles"];
            this.lblPathToSdat.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_LblPathToSdat"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_GroupOptions"];
            this.cbOneLoop.Text =
                ConfigurationSettings.AppSettings["Form_2sfTimer_CheckboxSingleLoop"];
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            Time2sfStruct t2Struct = new Time2sfStruct();
            t2Struct.Mini2sfDirectory = tbPathTo2sfs.Text;
            t2Struct.SdatPath = tbSdatPath.Text;
            t2Struct.DoSingleLoop = cbOneLoop.Checked;

            base.backgroundWorker_Execute(t2Struct);
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
