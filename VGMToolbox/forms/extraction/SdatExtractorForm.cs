using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.nds;

namespace VGMToolbox.forms.extraction
{
    public partial class SdatExtractorForm : AVgmtForm
    {
        public SdatExtractorForm(TreeNode pTreeNode): base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SdatExtractor_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SdatExtractor_IntroText"];
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            // messages
            this.BackgroundWorker = new SdatExtractorWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageCancel"];

            this.groupSource.AllowDrop = true;

            this.groupSource.Text =
                ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
        }

        private void tbNDS_SdatExtractor_Source_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            SdatExtractorWorker.SdatExtractorStruct sdexStruct = new SdatExtractorWorker.SdatExtractorStruct();
            sdexStruct.SourcePaths = s;

            base.backgroundWorker_Execute(sdexStruct);
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}