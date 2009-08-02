using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms.xsf
{
    public partial class NdsTo2sfForm : AVgmtForm
    {
        public NdsTo2sfForm(TreeNode pTreeNode) :
            base(pTreeNode)
        {
            string testpackPath = Path.Combine(
                Path.GetDirectoryName(Application.ExecutablePath),
                NdsTo2sfWorker.TESTPACK_PATH);
            
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();
            
            this.lblTitle.Text = "NDS To 2SF (Fully Automated 2SF Creation)";
            this.tbOutput.Text = "- Create 2SFs directly from NDS rom images -" + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_Make2sf_IntroText1"] + Environment.NewLine;            
            this.tbOutput.Text +=
                String.Format(ConfigurationSettings.AppSettings["Form_Make2sf_IntroText2"], Path.GetDirectoryName(testpackPath)) + Environment.NewLine;
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new NdsTo2sfWorker();
        }
        protected override string getCancelMessage()
        {
            return "NDSTo2SF...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "NDSTo2SF...Complete";
        }
        protected override string getBeginMessage()
        {
            return "NDSTo2SF...Begin";
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            NdsTo2sfWorker.NdsTo2sfStruct bwStruct = new NdsTo2sfWorker.NdsTo2sfStruct();
            bwStruct.SourcePaths = s;

            base.backgroundWorker_Execute(bwStruct);
        }
    }
}
