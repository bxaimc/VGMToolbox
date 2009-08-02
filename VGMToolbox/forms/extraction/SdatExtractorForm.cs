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

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new SdatExtractorWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SdatExtractor_MessageBegin"];
        }
    }
}