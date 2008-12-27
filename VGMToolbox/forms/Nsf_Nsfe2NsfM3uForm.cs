using System;
using System.ComponentModel;
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
            this.lblTitle.Text = "NSFE to NSF + .m3u";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();
        }

        private void tbNSF_nsfe2m3uSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "NSFE to .M3U Conversion...Begin";

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
                toolStripStatusLabel1.Text = "NSFE to .M3U Conversion...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "NSFE to .M3U Conversion...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (nsfeM3uBuilder != null && nsfeM3uBuilder.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
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
