using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format.iso;
using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms.extraction
{
    public partial class IsoExtractorForm : VgmtForm
    {
        protected TreeNode selectedNode;
        
        public IsoExtractorForm(TreeNode pTreeNode)
            : base(pTreeNode) 
        {
            InitializeComponent();

            this.lblTitle.Text = "ISO Browser/Extractor";
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

            IVolume[] volumes = Iso9660.GetVolumes(s[0]);
            try
            {
                IsoFolderTreeView.Nodes.Clear();
                LoadTreeWithVolumes(volumes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // base.backgroundWorker_Execute(taskStruct);
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
                        offsetItem = new ListViewItem.ListViewSubItem(fileItem, f.Offset.ToString());
                        sizeItem = new ListViewItem.ListViewSubItem(fileItem, f.Size.ToString());
                        dateItem = new ListViewItem.ListViewSubItem(fileItem, f.FileDateTime.ToString());

                        fileItem.SubItems.Add(offsetItem);
                        fileItem.SubItems.Add(sizeItem);
                        fileItem.SubItems.Add(dateItem);

                        this.fileListView.Items.Add(fileItem);
                    }
                }
            }
        }

    }
}
