using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_2sfRipperForm : VgmtForm
    {
        Rip2sfWorker rip2sfWorker;
        
        public Xsf_2sfRipperForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "2SF Ripper (Old Format)";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide(); 
            
            InitializeComponent();
        }

        private void tb2sf_Source_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();
            do2sfRipCheck();

            toolStripStatusLabel1.Text = "2SF Ripping...Begin";

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

            Rip2sfWorker.Rip2sfStruct rip2sfStruct = new Rip2sfWorker.Rip2sfStruct();
            rip2sfStruct.pPaths = s;
            rip2sfStruct.totalFiles = totalFileCount;
            rip2sfStruct.containerRomPath = tb2sf_ContainerPath.Text;
            rip2sfStruct.filePrefix = tb2sf_FilePrefix.Text.Trim().Replace(' ', '_');

            rip2sfWorker = new Rip2sfWorker();
            rip2sfWorker.ProgressChanged += backgroundWorker_ReportProgress;
            rip2sfWorker.RunWorkerCompleted += Rip2sfWorker_WorkComplete;
            rip2sfWorker.RunWorkerAsync(rip2sfStruct);
        }

        private void Rip2sfWorker_WorkComplete(object sender,
             RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "2SF Ripping...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "2SF Ripping...Complete";
            }
            
            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (rip2sfWorker != null && rip2sfWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                rip2sfWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void do2sfRipCheck()
        {
            // Verify container file exists
            if (!File.Exists(tb2sf_ContainerPath.Text))
            {
                MessageBox.Show("Container file does not exist.");
            }

            // Replace invalid filename chars with underscore
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                tb2sf_FilePrefix.Text = tb2sf_FilePrefix.Text.Replace(c, '_');
            }
        }

        private void btn2sf_BrowseContainerRom_Click(object sender, EventArgs e)
        {
            tb2sf_ContainerPath.Text = base.browseForFile(sender, e);
        }
    }
}
