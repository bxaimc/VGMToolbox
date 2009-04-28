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
    public partial class Examine_SearchForFileForm : AVgmtForm
    {
        public Examine_SearchForFileForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.lblTitle.Text = "Search for File(s)";
            this.btnDoTask.Hide();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ExamineSearchForFileWorker();
        }
        protected override string getCancelMessage()
        {
            return "Search for Files...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Search for Files...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Search for Files...Begin";
        }

        private void tbSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ExamineSearchForFileWorker.ExamineSearchForFileStruct exStruct =
                new ExamineSearchForFileWorker.ExamineSearchForFileStruct();            
            exStruct.SourcePaths = s;
            exStruct.ExtractFile = cbExtract.Checked;
            exStruct.SearchString = tbSearchString.Text;
            exStruct.CaseSensitive = cbCaseSensitive.Checked;

            base.backgroundWorker_Execute(exStruct);
        }
    }
}
