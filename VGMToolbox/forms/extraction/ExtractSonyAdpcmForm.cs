using System;
using System.Configuration;
using System.Windows.Forms;
using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms.extraction
{
    public partial class ExtractSonyAdpcmForm : AVgmtForm
    {
        public ExtractSonyAdpcmForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Sony ADPCM Extractor";
            this.tbOutput.Text = "Extract Sony ADPCM Data" + Environment.NewLine;
            this.tbOutput.Text += "- Tool still WIP, results are still imperfect.";
            
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();
            
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            this.grpSourceFiles.Text =  ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ExtractSonyAdpcmWorker();
        }
        protected override string getCancelMessage()
        {
            return "Extracting Sony ADPCM...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Extracting Sony ADPCM...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Extracting Sony ADPCM...Begin";
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExtractSonyAdpcmWorker.ExtractSonyAdpcmStruct bwStruct = new ExtractSonyAdpcmWorker.ExtractSonyAdpcmStruct();
            bwStruct.SourcePaths = s;

            base.backgroundWorker_Execute(bwStruct);
        }
    }
}
