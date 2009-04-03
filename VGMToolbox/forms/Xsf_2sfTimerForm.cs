using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_2sfTimerForm : AVgmtForm
    {        
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
            Time2sfWorker.Time2sfStruct t2Struct = new Time2sfWorker.Time2sfStruct();
            t2Struct.pathTo2sf = tbPathTo2sfs.Text;
            t2Struct.pathToSdat = tbSdatPath.Text;
            t2Struct.filePrefix = tbFilePrefix.Text;
            t2Struct.doSingleLoop = cbOneLoop.Checked;

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

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new Time2sfWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_2sfTimer_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_2sfTimer_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_2sfTimer_MessageBegin"];
        }
    }
}
