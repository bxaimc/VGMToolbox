using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.util;
using VGMToolbox.tools;

namespace VGMToolbox.forms
{
    public partial class TreeViewVgmtForm : VgmtForm
    {
        private TreeNode selectedNode;
        private TreeNode oldNode;
        
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
                VGMToolbox.util.ProgressStruct vProgressStruct = (VGMToolbox.util.ProgressStruct)e.UserState;
                tbOutput.Text += vProgressStruct.GenericMessage;
            }
            else if (e.UserState != null)
            {
                VGMToolbox.util.ProgressStruct vProgressStruct = (VGMToolbox.util.ProgressStruct)e.UserState;

                if (vProgressStruct.NewNode != null)
                {
                    treeViewTools.Nodes.Add(vProgressStruct.NewNode);
                }

                lblProgressLabel.Text = vProgressStruct.Filename == null ? String.Empty : vProgressStruct.Filename;

                if (!String.IsNullOrEmpty(vProgressStruct.ErrorMessage))
                {
                    tbOutput.Text += vProgressStruct.ErrorMessage;
                    errorFound = true;
                } 
            }

            this.showElapsedTime();
        }

        protected override void initializeProcessing()
        {
            base.initializeProcessing();
            treeViewTools.Nodes.Clear();
        }

        private void treeViewTools_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                TreeNode node = treeViewTools.GetNodeAt(p);
                this.selectedNode = node;
                this.oldNode = treeViewTools.SelectedNode;
                treeViewTools.SelectedNode = node;

                if (node != null && node.Tag is VGMToolbox.util.NodeTagStruct)
                {
                    VGMToolbox.util.NodeTagStruct nts = (VGMToolbox.util.NodeTagStruct)node.Tag;

                    if (typeof(IEmbeddedTagsFormat).IsAssignableFrom(Type.GetType(nts.ObjectType)) ||
                        typeof(ISingleTagFormat).IsAssignableFrom(Type.GetType(nts.ObjectType)))
                    {
                        contextMenuStrip1.Show(treeViewTools, p);
                    }
                }
            }
        }

        private void filePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.selectedNode != null && this.selectedNode.Tag is VGMToolbox.util.NodeTagStruct)
            {
                VGMToolbox.util.NodeTagStruct nts = (VGMToolbox.util.NodeTagStruct) this.selectedNode.Tag;

                if (typeof(IEmbeddedTagsFormat).IsAssignableFrom(Type.GetType(nts.ObjectType)))
                {
                    EmbeddedTagsUpdateForm etuForm = new EmbeddedTagsUpdateForm(nts);
                    etuForm.Show();
                }
                else if (typeof(ISingleTagFormat).IsAssignableFrom(Type.GetType(nts.ObjectType)))
                {
                    SingleTagUpdateForm stuForm = new SingleTagUpdateForm(nts);
                    stuForm.Show();
                }
                                
                this.selectedNode = this.oldNode;
                this.oldNode = null;
            }
        }
    }
}
