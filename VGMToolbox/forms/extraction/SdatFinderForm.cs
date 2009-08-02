using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.nds;

namespace VGMToolbox.forms.extraction
{
    public partial class SdatFinderForm : AVgmtForm
    {        
        public SdatFinderForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SdatFinder_Title"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SdatFinder_IntroText"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            this.grpSource.AllowDrop = true;

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void tbSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            SdatFinderWorker.SdatFinderStruct sfStruct = new SdatFinderWorker.SdatFinderStruct();
            sfStruct.SourcePaths = s;

            base.backgroundWorker_Execute(sfStruct);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new SdatFinderWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SdatFinder_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SdatFinder_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SdatFinder_MessageBegin"];
        }
    }
}
