using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.tools.gbs;

namespace VGMToolbox.forms
{
    public partial class Gbs_GbsToM3uForm : VgmtForm
    {
        GbsM3uBuilderWorker gbsM3uBuilder;
        
        public Gbs_GbsToM3uForm(TreeNode pTreeNode): base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "GBS .m3u Builder";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();
        }

        private void tbGBS_gbsm3uSource_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "GBS .M3U Creation...Begin";

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

            GbsM3uBuilderWorker.GbsM3uBuilderStruct gbStruct = new GbsM3uBuilderWorker.GbsM3uBuilderStruct();
            gbStruct.pPaths = s;
            gbStruct.totalFiles = totalFileCount;
            gbStruct.onePlaylistPerFile = cbGBS_OneM3uPerTrack.Checked;

            gbsM3uBuilder = new GbsM3uBuilderWorker();
            gbsM3uBuilder.ProgressChanged += backgroundWorker_ReportProgress;
            gbsM3uBuilder.RunWorkerCompleted += GbsM3uBuilderWorker_WorkComplete;
            gbsM3uBuilder.RunWorkerAsync(gbStruct);
        }
        
        private void GbsM3uBuilderWorker_WorkComplete(object sender,
                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "GBS .M3U Creation...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "GBS .M3U Creation...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (gbsM3uBuilder != null && gbsM3uBuilder.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                gbsM3uBuilder.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
