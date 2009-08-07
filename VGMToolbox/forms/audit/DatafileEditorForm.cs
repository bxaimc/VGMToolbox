using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using VGMToolbox.auditing;
using VGMToolbox.format.auditing;
using VGMToolbox.plugin;

namespace VGMToolbox.forms.audit
{
    public partial class DatafileEditorForm : TreeViewVgmtForm
    {
        private datafile datafileObject;

        public DatafileEditorForm(TreeNode pTreeNode) : 
            base(pTreeNode)
        {
            InitializeComponent();

            btnDoTask.Text = "Save As";
        }

        /// <summary>
        /// Browse to the source file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browseSourceButton_Click(object sender, EventArgs e)
        {
            this.datafileSourcePath.Text = base.browseForFile(sender, e);

            if (base.checkFileExists(this.datafileSourcePath.Text, this.grpSource.Text))
            {
                try
                {
                    this.datafileObject = this.getDataFileObject();
                    this.loadDataFileIntoTree(this.datafileObject);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error parsing data file: {0}{1}", ex.Message, Environment.NewLine), "Error");
                }
            }
        }

        private datafile getDataFileObject()
        {
            datafile datafileObject = new datafile();
            XmlSerializer serializer = new XmlSerializer(typeof(datafile));
            
            using (FileStream xmlFs = File.OpenRead(this.datafileSourcePath.Text))
            {
                using (XmlTextReader textReader = new XmlTextReader(xmlFs))
                {
                    datafileObject = (datafile)serializer.Deserialize(textReader);
                }
            }

            return datafileObject;
        }

        private void loadDataFileIntoTree(datafile datafileObject)
        {
            TreeNode workingGameNode;
            TreeNode workingRomNode;

            // clear tree
            this.treeViewTools.Nodes.Clear();

            foreach (game g in datafileObject.game)
            {
                workingGameNode = new TreeNode(g.name);
                workingGameNode.Tag = g.DeepCopy();

                foreach (rom r in g.rom)
                {
                    workingRomNode = new TreeNode(r.name);
                    workingRomNode.Tag = r.DeepCopy();

                    workingGameNode.Nodes.Add(workingRomNode);
                }

                this.treeViewTools.Nodes.Add(workingGameNode);
            }
        }

        protected override void treeViewTools_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.selectedNode = treeViewTools.GetNodeAt(e.X, e.Y);
                this.oldNode = treeViewTools.SelectedNode;
                treeViewTools.SelectedNode = this.selectedNode;
                treeViewTools.LabelEdit = true;

                if (this.selectedNode != null &&
                    ((this.selectedNode.Tag is game) || (this.selectedNode.Tag is rom)))
                {
                    this.treeNodeContextMenuStrip.Show(treeViewTools, e.X, e.Y);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                treeViewTools.SelectedNode = selectedNode;
                treeViewTools.LabelEdit = true;
                if (!selectedNode.IsEditing)
                {
                    selectedNode.BeginEdit();
                }
            }
            else
            {
                MessageBox.Show("No tree node selected or selected node.", "Invalid selection");
            }

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedNode != null)
            {
                treeViewTools.SelectedNode = selectedNode;

                game sourceGame = null;
                rom sourceRom = null;

                if (selectedNode.Tag is rom)
                {
                    sourceRom = (rom)selectedNode.Tag;
                    sourceGame = (game)selectedNode.Parent.Tag;
                }
                else
                {
                    sourceGame = (game)selectedNode.Tag;
                }

                this.datafileObject = AuditingUtil.DeleteItemFromDatafile(this.datafileObject, sourceGame, sourceRom);
                this.loadDataFileIntoTree(this.datafileObject);
            }
            else
            {
                MessageBox.Show("No tree node selected or selected node.", "Invalid selection");
            }
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            string saveFilePath = base.browseForFileToSave(sender, e);

            if (!String.IsNullOrEmpty(saveFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(this.datafileObject.GetType());

                using (TextWriter textWriter = new StreamWriter(saveFilePath))
                {
                    serializer.Serialize(textWriter, this.datafileObject);

                }
            }
        }

        


    }
}
