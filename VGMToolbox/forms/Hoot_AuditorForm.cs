using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.hoot;


namespace VGMToolbox.forms
{
    public partial class Hoot_AuditorForm : TreeViewVgmtForm
    {
        static readonly string AUDIT_OUTPUT_FILE = 
            Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "hoot"), "audit.txt");

        
        public Hoot_AuditorForm(TreeNode pTreeNode)
            : base(pTreeNode) 
        {
            base.outputToText = true;
            base.outputTextFile = AUDIT_OUTPUT_FILE;

            NodePrintMessageStruct n = new NodePrintMessageStruct();
            n.NodeColor = HootAuditorWorker.HOOT_MISSING_FILE_COLOR;
            n.Message = "(MISSING)";
            base.outputColorToMessageRules = new NodePrintMessageStruct[] { n };

            InitializeComponent();

            this.lblTitle.Text = "Hoot Collection Auditor (Hoot Auditor Result Verification Engine Yes!)";
            this.btnDoTask.Text = "Audit Collection";
            this.tbOutput.Text = "- Warning: Duplicate archive names in different folders can lead to inaccurate results." + Environment.NewLine;
            this.tbOutput.Text += "- Results will also be output to the 'hoot' subfolder as 'audit.txt'." + Environment.NewLine;
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
            hootStruct.IncludeSubDirectories = cbIncludeSubDirs.Checked;

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
