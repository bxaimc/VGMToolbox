using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools;
using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class Examine_TagViewerForm : TreeViewVgmtForm
    {
        TreeBuilderWorker treeBuilder;
        
        public Examine_TagViewerForm(TreeNode pTreeNode) : base(pTreeNode) 
        {
            this.lblTitle.Text =
                ConfigurationSettings.AppSettings["Form_ExamineTags_Title"];
            this.btnDoTask.Text =
                ConfigurationSettings.AppSettings["Form_ExamineTags_DoTaskButton"];

            this.tbOutput.Text =
                ConfigurationSettings.AppSettings["Form_ExamineTags_IntroText1"] + System.Environment.NewLine;
            this.tbOutput.Text +=
                ConfigurationSettings.AppSettings["Form_ExamineTags_IntroText2"] + System.Environment.NewLine;

            InitializeComponent();

            this.cbCheckForLibs.Text =
                ConfigurationSettings.AppSettings["Form_ExamineTags_CheckBoxCheckForLibs"];
        }

        private void tbXsfSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = 
                ConfigurationSettings.AppSettings["Form_ExamineTags_MessageBegin"];

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            int totalFileCount = FileUtil.GetFileCount(s);

            TreeBuilderWorker.TreeBuilderStruct tbStruct = new TreeBuilderWorker.TreeBuilderStruct();
            tbStruct.pPaths = s;
            tbStruct.totalFiles = totalFileCount;
            tbStruct.checkForLibs = cbCheckForLibs.Checked;

            treeBuilder = new TreeBuilderWorker();
            treeBuilder.ProgressChanged += backgroundWorker_ReportProgress;
            treeBuilder.RunWorkerCompleted += TreeBuilderWorker_WorkComplete;
            treeBuilder.RunWorkerAsync(tbStruct);
        }

        private void TreeBuilderWorker_WorkComplete(object sender,
                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text =
                    ConfigurationSettings.AppSettings["Form_ExamineTags_MessageCancel"];
                tbOutput.Text +=
                    ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_ExamineTags_MessageComplete"]; 
            }
            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (treeBuilder != null && treeBuilder.IsBusy)
            {
                tbOutput.Text += 
                    ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                treeBuilder.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            this.treeViewTools.ExpandAll();
        }
    }
}
