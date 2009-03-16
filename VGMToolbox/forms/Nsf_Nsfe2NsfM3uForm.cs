using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.tools.nsf;

namespace VGMToolbox.forms
{
    public partial class Nsf_Nsfe2NsfM3uForm : VgmtForm
    {
        NsfeM3uBuilderWorker nsfeM3uBuilder;
        
        public Nsf_Nsfe2NsfM3uForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = 
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_Title"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();

            this.grpSourceFiles.Text =
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_GroupSourceFiles"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_LblDragNDrop"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_GroupOptions"];
            this.cbNSFE_OneM3uPerTrack.Text =
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_CheckBoxOneM3uPerTrack"];
        }

        private void tbNSF_nsfe2m3uSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text =
                ConfigurationSettings.AppSettings["Form_Nsfe2M3u_MessageBegin"]; 

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            int totalFileCount = 0;

            foreach (string path in s)
            {
                if (File.Exists(path))
                {
                    totalFileCount++;
                }
                else if (Directory.Exists(path))
                {
                    totalFileCount += Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;
                }
            }

            NsfeM3uBuilderWorker.NsfeM3uBuilderStruct nsfeStruct = new NsfeM3uBuilderWorker.NsfeM3uBuilderStruct();
            nsfeStruct.pPaths = s;
            nsfeStruct.totalFiles = totalFileCount;
            nsfeStruct.onePlaylistPerFile = cbNSFE_OneM3uPerTrack.Checked;

            nsfeM3uBuilder = new NsfeM3uBuilderWorker();
            nsfeM3uBuilder.ProgressChanged += backgroundWorker_ReportProgress;
            nsfeM3uBuilder.RunWorkerCompleted += NsfeM3uBuilderWorker_WorkComplete;
            nsfeM3uBuilder.RunWorkerAsync(nsfeStruct);
        }

        private void NsfeM3uBuilderWorker_WorkComplete(object sender,
                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_Nsfe2M3u_MessageCancel"];
                tbOutput.Text +=
                    ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_Nsfe2M3u_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (nsfeM3uBuilder != null && nsfeM3uBuilder.IsBusy)
            {
                tbOutput.Text += 
                    ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                nsfeM3uBuilder.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
