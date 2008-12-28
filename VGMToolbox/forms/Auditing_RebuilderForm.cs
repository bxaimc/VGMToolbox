using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

using VGMToolbox.auditing;

namespace VGMToolbox.forms
{
    public partial class Auditing_RebuilderForm : VgmtForm
    {
        RebuilderWorker rebuilder;
        
        public Auditing_RebuilderForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "File Rebuilder";
            this.btnDoTask.Text = "Rebuild";

            InitializeComponent();
        }

        private void btnRebuilder_Rebuild_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();

            if (checkRebuilderInputs())
            {
                toolStripStatusLabel1.Text = "Rebuilding...";

                datafile dataFile = new datafile();
                XmlSerializer serializer = new XmlSerializer(typeof(datafile));
                TextReader textReader = new StreamReader(tbRebuilder_Datafile.Text);
                dataFile = (datafile)serializer.Deserialize(textReader);
                textReader.Close();

                RebuilderWorker.RebuildSetsStruct vRebuildSetsStruct = new RebuilderWorker.RebuildSetsStruct();
                vRebuildSetsStruct.pSourceDir = tbRebuilder_SourceDir.Text;
                vRebuildSetsStruct.pDestinationDir = tbRebuilder_DestinationDir.Text;
                vRebuildSetsStruct.pDatFile = dataFile;
                vRebuildSetsStruct.pRemoveSource = cbRebuilder_RemoveSource.Checked;
                vRebuildSetsStruct.pOverwriteExisting = cbRebuilder_Overwrite.Checked;
                vRebuildSetsStruct.ScanOnly = cbRebuilder_ScanOnly.Checked;
                vRebuildSetsStruct.pCompressOutput = cbRebuilder_CompressOutput.Checked;

                try
                {
                    vRebuildSetsStruct.totalFiles = Directory.GetFiles(tbRebuilder_SourceDir.Text, "*.*", SearchOption.AllDirectories).Length;

                    rebuilder = new RebuilderWorker();
                    rebuilder.ProgressChanged += backgroundWorker_ReportProgress;
                    rebuilder.RunWorkerCompleted += rebuilderWorker_WorkComplete;
                    rebuilder.RunWorkerAsync(vRebuildSetsStruct);
                }
                catch (Exception exception2)
                {
                    tbOutput.Text += exception2.Message;
                }
            }
        }

        private void rebuilderWorker_WorkComplete(object sender,
                             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Rebuilding...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Rebuilding...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (rebuilder != null && rebuilder.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                rebuilder.CancelAsync();
                this.errorFound = true;
            }
        }

        private void btnRebuilder_BrowseSourceDir_Click(object sender, EventArgs e)
        {
            tbRebuilder_SourceDir.Text = base.browseForFolder(sender, e);
        }

        private void btnRebuilder_BrowseDestinationDir_Click(object sender, EventArgs e)
        {
            tbRebuilder_DestinationDir.Text = base.browseForFolder(sender, e);
        }

        private void btnRebuilder_BrowseDatafile_Click(object sender, EventArgs e)
        {
            tbRebuilder_Datafile.Text = base.browseForFile(sender, e);
        }

        private bool checkRebuilderInputs()
        {
            bool ret = true;

            if (checkTextBox(tbRebuilder_SourceDir.Text, "Source Directory") &&
                checkTextBox(tbRebuilder_DestinationDir.Text, "Destination Directory") &&
                checkTextBox(tbRebuilder_Datafile.Text, "Datafile Path") &&
                checkFolderExists(tbRebuilder_SourceDir.Text, "Source Directory") &&
                checkFolderExists(tbRebuilder_DestinationDir.Text, "Destination Directory") &&
                checkFileExists(tbRebuilder_Datafile.Text, "Datafile Path"))
            {

                if (tbRebuilder_SourceDir.Text.Trim().Equals(tbRebuilder_DestinationDir.Text.Trim()))
                {
                    MessageBox.Show("Source directory cannot be the same as the Destination directory");
                    ret = false;
                }
            }
            else
            {
                ret = false;
            }

            return ret;
        }

        private void cbRebuilder_CompressOutput_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRebuilder_CompressOutput.Checked)
            {
                cbRebuilder_Overwrite.Checked = false;
                cbRebuilder_ScanOnly.Checked = false;
            }
        }

        private void cbRebuilder_Overwrite_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRebuilder_Overwrite.Checked)
            {
                cbRebuilder_CompressOutput.Checked = false;
                cbRebuilder_ScanOnly.Checked = false;
            }
        }

        private void cbRebuilder_RemoveSource_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRebuilder_RemoveSource.Checked)
            {
                cbRebuilder_ScanOnly.Checked = false;
            }
        }

        private void cbRebuilder_ScanOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRebuilder_ScanOnly.Checked)
            {
                cbRebuilder_RemoveSource.Checked = false;
                cbRebuilder_Overwrite.Checked = false;
                cbRebuilder_CompressOutput.Checked = false;
            }
        }
    }
}
