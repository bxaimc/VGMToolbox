using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.tools.hoot;

namespace VGMToolbox.forms
{
    public partial class Hoot_XmlBuilderForm : VgmtForm
    {
        HootXmlBuilderWorker hootXmlBuilder;
        
        public Hoot_XmlBuilderForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Hoot XML Builder";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();
        }

        private void tbHootXML_Path_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            toolStripStatusLabel1.Text = "Hoot XML Generation...Begin";

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

            HootXmlBuilderWorker.HootXmlBuilderStruct xbStruct = new HootXmlBuilderWorker.HootXmlBuilderStruct();
            xbStruct.pPaths = s;
            xbStruct.totalFiles = totalFileCount;
            xbStruct.combineOutput = cbHootXML_CombineOutput.Checked;
            xbStruct.splitOutput = cbHootXML_SplitOutput.Checked;

            hootXmlBuilder = new HootXmlBuilderWorker();
            hootXmlBuilder.ProgressChanged += backgroundWorker_ReportProgress;
            hootXmlBuilder.RunWorkerCompleted += HootXmlBuilderWorker_WorkComplete;
            hootXmlBuilder.RunWorkerAsync(xbStruct);
        }

        private void HootXmlBuilderWorker_WorkComplete(object sender,
                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Hoot XML Generation...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Hoot XML Generation...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (hootXmlBuilder != null && hootXmlBuilder.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                hootXmlBuilder.CancelAsync();
                this.errorFound = true;
            }
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
