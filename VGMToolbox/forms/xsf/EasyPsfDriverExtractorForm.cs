using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class EasyPsfDriverExtractorForm : AVgmtForm
    {
        public EasyPsfDriverExtractorForm(TreeNode pTreeNode) : 
            base(pTreeNode)

        {
            InitializeComponent();
            
            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();

            this.lblTitle.Text = "Easy PSF Driver Data Extractor";
            this.tbOutput.Text = "- Extract SEQ/VH/VB from Easy PSF Driver sets for use with rebuilding with the original driver." + Environment.NewLine;
            this.tbOutput.Text += "- If a set uses Davironica's Easy PSF Driver, this will extract data much faster than the PSF Data Extractor.";
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new EasyPsfDriverExtractionWorker();
        }
        protected override string getCancelMessage()
        {
            return "Extracting PSF Data...Cancel";
        }
        protected override string getCompleteMessage()
        {
            return "Extracting PSF Data...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Extracting PSF Data...Begin";
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            EasyPsfDriverExtractionStruct bwStruct = new EasyPsfDriverExtractionStruct();
            bwStruct.SourcePaths = s;

            base.backgroundWorker_Execute(bwStruct);
        }
    }
}
