using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class TreeViewVgmtForm : VgmtForm
    {
        public TreeViewVgmtForm() : base() 
        {
            InitializeComponent();
        }
        
        public TreeViewVgmtForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            InitializeComponent();
        }

        protected override void backgroundWorker_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != Constants.IGNORE_PROGRESS &&
                e.ProgressPercentage != Constants.PROGRESS_MSG_ONLY)
            {
                toolStripProgressBar.Value = e.ProgressPercentage;
            }

            if ((e.ProgressPercentage == Constants.PROGRESS_MSG_ONLY) && e.UserState != null)
            {
                Constants.ProgressStruct vProgressStruct = (Constants.ProgressStruct)e.UserState;
                tbOutput.Text += vProgressStruct.genericMessage;
            }
            else if (e.UserState != null)
            {
                Constants.ProgressStruct vProgressStruct = (Constants.ProgressStruct)e.UserState;

                if (vProgressStruct.newNode != null)
                {
                    treeViewTools.Nodes.Add(vProgressStruct.newNode);
                }

                lblProgressLabel.Text = vProgressStruct.filename == null ? String.Empty : vProgressStruct.filename;
                tbOutput.Text += vProgressStruct.errorMessage == null ? String.Empty : vProgressStruct.errorMessage;
                errorFound = true;
            }

            this.showElapsedTime();
        }

        protected override void initializeProcessing()
        {
            base.initializeProcessing();
            treeViewTools.Nodes.Clear();
        }
    }
}
