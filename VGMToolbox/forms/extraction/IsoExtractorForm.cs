using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format.iso;
using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms.extraction
{
    public partial class IsoExtractorForm : AVgmtForm
    {
        protected TreeNode selectedNode;
        protected TreeNode oldNode;

        protected ListViewItem selectedListViewItem;
        protected ListViewItem oldListViewItem;

        protected string sourceFile;
        protected string destinationFolder;
        protected bool isTreeSelected;

        public IsoExtractorForm(TreeNode pTreeNode)
            : base(pTreeNode) 
        {
            InitializeComponent();

            this.lblTitle.Text = "ISO Browser/Extractor";
            this.btnDoTask.Hide();

        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new IsoExtractorWorker();
        }
        protected override string getCancelMessage()
        {
            return "Extract ISO...Cancelled.";
        }
        protected override string getCompleteMessage()
        {
            return "Extract ISO...Complete.";
        }
        protected override string getBeginMessage()
        {
            return "Extract ISO...Begin.";
        }

        private void IsoExtractorForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            IsoExtractorWorker.IsoExtractorStruct taskStruct = 
                new IsoExtractorWorker.IsoExtractorStruct();
            taskStruct.SourcePaths = s;

            IVolume[] volumes = XDvdFs.GetVolumes(s[0]);
            try
            {
                this.IsoFolderTreeView.Nodes.Clear();
                this.fileListView.Items.Clear();
                
                LoadTreeWithVolumes(volumes);
                this.sourceFile = s[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private void LoadTreeWithVolumes(IVolume[] volumes)
        {
            TreeNode volumeNode;
            TreeNode directoryNode;
            string volumeName;

            foreach (IVolume v in volumes)
            {
                volumeName = String.IsNullOrEmpty(v.VolumeIdentifier) ? "-" : v.VolumeIdentifier;
                volumeNode = new TreeNode(volumeName);

                foreach (IDirectoryStructure d in v.Directories)
                {
                    directoryNode = GetDirectoryNode(d);
                    directoryNode.Text = "..";
                    volumeNode.Nodes.Add(directoryNode);
                }
                
                IsoFolderTreeView.Nodes.Add(volumeNode);
            }
        }

        private TreeNode GetDirectoryNode(IDirectoryStructure directory)
        {
            TreeNode directoryNode;
            TreeNode subDirectoryNode;
            TreeNode fileNode;

            directoryNode = new TreeNode(directory.DirectoryName);
            directoryNode.Tag = directory;

            foreach (IDirectoryStructure d in directory.SubDirectories)
            {                
                subDirectoryNode = GetDirectoryNode(d);
                directoryNode.Nodes.Add(subDirectoryNode);
            }

            //foreach (IFileStructure f in directory.Files)
            //{
            //    fileNode = GetFileNode(f);
            //    fileNode.Tag = fileNode;
            //    directoryNode.Nodes.Add(fileNode);
            //}

            return directoryNode;
        }

        private TreeNode GetFileNode(IFileStructure file)
        {
            TreeNode fileNode = new TreeNode(file.FileName);
            fileNode.Tag = file;

            return fileNode;
        }

        private void IsoFolderTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ListViewItem fileItem;
            ListViewItem.ListViewSubItem offsetItem;
            ListViewItem.ListViewSubItem sizeItem;
            ListViewItem.ListViewSubItem dateItem;
            
            if (this.IsoFolderTreeView.SelectedNode != null)
            {
                IDirectoryStructure dirStructure = (IDirectoryStructure)this.IsoFolderTreeView.SelectedNode.Tag;

                this.fileListView.Items.Clear();

                if (dirStructure != null &&
                    dirStructure.Files != null)
                {
                    foreach (IFileStructure f in dirStructure.Files)
                    {
                        fileItem = new ListViewItem(f.FileName);
                        offsetItem = new ListViewItem.ListViewSubItem(fileItem, String.Format("0x{0}", f.Offset.ToString("X")));
                        sizeItem = new ListViewItem.ListViewSubItem(fileItem, f.Size.ToString());
                        dateItem = new ListViewItem.ListViewSubItem(fileItem, f.FileDateTime.ToString());
                        fileItem.Tag = f;

                        fileItem.SubItems.Add(offsetItem);
                        fileItem.SubItems.Add(sizeItem);
                        fileItem.SubItems.Add(dateItem);

                        this.fileListView.Items.Add(fileItem);
                    }
                }
            }
        }

        private void IsoFolderTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            isTreeSelected = true;
            
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                TreeNode node = IsoFolderTreeView.GetNodeAt(p);
                this.selectedNode = node;
                this.oldNode = IsoFolderTreeView.SelectedNode;
                IsoFolderTreeView.SelectedNode = node;

                if (node != null && node.Tag != null)
                {
                    if ((node.Tag is IFileStructure) || 
                        (node.Tag is IDirectoryStructure))
                    {
                        contextMenuStrip1.Show(IsoFolderTreeView, p);
                    }
                }
            }
        }

        private void fileListView_MouseUp(object sender, MouseEventArgs e)
        {
            bool showList = true;
            isTreeSelected = false;
            
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                if (this.fileListView.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem lvi in this.fileListView.SelectedItems)
                    {
                        if (lvi.Tag == null ||
                            !(lvi.Tag is IFileStructure))
                        {
                            showList = false;
                            break;
                        }
                    }
                }
                else
                {
                    showList = false;
                }

                if (showList)
                {
                    contextMenuStrip1.Show(this.fileListView, p);
                }
            }
        }

        private void extractToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string destinationFolder = base.browseForFolder(sender, e);

            this.extractFiles(destinationFolder);
        }

        private void extractToSubfolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string destinationFolder =
                Path.Combine(Path.GetDirectoryName(this.sourceFile), String.Format("{0}_isodump", Path.GetFileNameWithoutExtension(this.sourceFile)));

            this.extractFiles(destinationFolder);
        }

        private void extractFiles(string destinationFolder)
        {
            ArrayList files = new ArrayList();
            ArrayList directories = new ArrayList();

            if (this.isTreeSelected &&
                this.IsoFolderTreeView.SelectedNode.Tag != null &&
                this.IsoFolderTreeView.SelectedNode.Tag is IDirectoryStructure)
            {
                directories.Add(this.IsoFolderTreeView.SelectedNode.Tag);
            }
            else if (!this.isTreeSelected)
            {
                foreach (ListViewItem lvi in this.fileListView.SelectedItems)
                {
                    if (lvi.Tag != null &&
                        lvi.Tag is IFileStructure)
                    {
                        files.Add(lvi.Tag);
                    }
                }
            }

            IsoExtractorWorker.IsoExtractorStruct taskStruct = 
                new IsoExtractorWorker.IsoExtractorStruct();
            taskStruct.SourcePaths = new string[] { this.sourceFile };           
            taskStruct.DestinationFolder = destinationFolder;
            taskStruct.IsoFormat = IsoExtractorWorker.IsoFormatType.Iso9660;
            taskStruct.Files = (IFileStructure[])files.ToArray(typeof(IFileStructure));
            taskStruct.Directories = (IDirectoryStructure[])directories.ToArray(typeof(IDirectoryStructure));

            base.backgroundWorker_Execute(taskStruct);
        }
    }
}
