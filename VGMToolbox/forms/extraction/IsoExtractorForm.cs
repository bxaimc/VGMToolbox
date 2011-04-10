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
    public partial class IsoExtractorForm : AVgmtForm
    {
        public IsoExtractorForm(TreeNode pTreeNode)
            : base(pTreeNode) 
        {
            InitializeComponent();
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

            base.backgroundWorker_Execute(taskStruct);
        }

    }
}
