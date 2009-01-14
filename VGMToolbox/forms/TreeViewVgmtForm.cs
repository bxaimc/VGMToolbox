using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.util;

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

                if (!String.IsNullOrEmpty(vProgressStruct.errorMessage))
                {
                    tbOutput.Text += vProgressStruct.errorMessage;
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

                if (node != null && node.Tag is Constants.NodeTagStruct)
                {
                    Constants.NodeTagStruct nts = (Constants.NodeTagStruct)node.Tag;

                    if (typeof(IEmbeddedTagsFormat).IsAssignableFrom(Type.GetType(nts.objectType)) ||
                        typeof(ISingleTagFormat).IsAssignableFrom(Type.GetType(nts.objectType)))
                    {
                        contextMenuStrip1.Show(treeViewTools, p);
                    }
                }
            }
        }

        private void filePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.selectedNode != null && this.selectedNode.Tag is Constants.NodeTagStruct)
            {
                Constants.NodeTagStruct nts = (Constants.NodeTagStruct) this.selectedNode.Tag;

                if (typeof(IEmbeddedTagsFormat).IsAssignableFrom(Type.GetType(nts.objectType)))
                {
                    EmbeddedTagsUpdateForm etuForm = new EmbeddedTagsUpdateForm(nts);
                    etuForm.Show();
                }
                else if (typeof(ISingleTagFormat).IsAssignableFrom(Type.GetType(nts.objectType)))
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
