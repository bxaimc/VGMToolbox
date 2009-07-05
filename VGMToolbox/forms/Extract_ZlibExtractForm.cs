using System;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.extract;
using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class Extract_ZlibExtractForm : AVgmtForm
    {
        public Extract_ZlibExtractForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();
            
            this.lblTitle.Text = "Zlib Extractor";
            this.tbOutput.Text = String.Format("Zlib compressed section must begin at the start of the file.{0}", 
                Environment.NewLine);
            this.tbOutput.Text = String.Format("Files will be output using the original file name with the '{0}' file extension.{1}",
                            CompressionUtil.ZLIB_OUTPUT_EXTENSION, Environment.NewLine);
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ZlibExtractorWorker();
        }
        protected override string getCancelMessage()
        {
            return "Extracting Zlib Compressed Data...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Extracting Zlib Compressed Data...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Extracting Zlib Compressed Data...Begin";
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ZlibExtractorWorker.ZlibExtractorStruct zlStruct = new ZlibExtractorWorker.ZlibExtractorStruct();
            zlStruct.SourcePaths = s;

            base.backgroundWorker_Execute(zlStruct);
        }
    }
}
