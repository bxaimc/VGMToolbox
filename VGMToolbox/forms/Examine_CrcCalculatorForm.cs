using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.examine;

namespace VGMToolbox.forms
{
    public partial class Examine_CrcCalculatorForm : AVgmtForm
    {
        public Examine_CrcCalculatorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            this.tbOutput.Text = "Calculate CRC32/MD5/SHA1 of input files.  Full file support only, VGMT special checksums not yet added.";

        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExamineChecksumGeneratorWorker.ExamineChecksumGeneratorStruct crcStruct =
                new ExamineChecksumGeneratorWorker.ExamineChecksumGeneratorStruct();
            crcStruct.SourcePaths = s;

            base.backgroundWorker_Execute(crcStruct);
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ExamineChecksumGeneratorWorker();
        }
        protected override string getCancelMessage()
        {
            return "Generating Checksums...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Generating Checksums...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Generating Checksums...Begin";
        }
    }
}
