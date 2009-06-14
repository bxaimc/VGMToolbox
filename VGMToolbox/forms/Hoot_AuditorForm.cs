using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.hoot;


namespace VGMToolbox.forms
{
    public partial class Hoot_AuditorForm : TreeViewVgmtForm
    {
        public Hoot_AuditorForm(TreeNode pTreeNode)
            : base(pTreeNode) 
        {
            InitializeComponent();

            this.tbOutput.Text = "Work in Progress, non-functional.";
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new HootAuditorWorker();
        }
        protected override string getCancelMessage()
        {
            return "Auditing Hoot...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Auditing Hoot...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Auditing Hoot...Begin";
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            string[] sourcePaths = new string[] { tbXmlPath.Text };
            HootAuditorWorker.HootAuditorStruct hootStruct = new HootAuditorWorker.HootAuditorStruct();
            hootStruct.SourcePaths = sourcePaths;
            hootStruct.SetArchivePaths = 
                tbArchiveFolders.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            
            base.backgroundWorker_Execute(hootStruct);
        }

        private void btnBrowseXmlFolder_Click(object sender, EventArgs e)
        {
            tbXmlPath.Text = base.browseForFolder(sender, e);
        }

        private void btnBrowseArchiveFolders_Click(object sender, EventArgs e)
        {
            string newFolder = base.browseForFolder(sender, e);
            tbArchiveFolders.Text += newFolder + Environment.NewLine;
        }
    }
}
