using System;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.extract;
using VGMToolbox.util;

namespace VGMToolbox.forms.compression
{
    public partial class ZlibCompressionForm : AVgmtForm
    {
        public ZlibCompressionForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();

            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_ZlibCompress_Title"];
            this.tbOutput.Text = String.Format(ConfigurationSettings.AppSettings["Form_ZlibCompress_IntroText1"],
                            CompressionUtil.ZlibDecompressOutputExtension, Environment.NewLine);
            this.tbOutput.Text += String.Format(ConfigurationSettings.AppSettings["Form_ZlibCompress_IntroText2"],
                CompressionUtil.ZlibCompressOutputExtension, Environment.NewLine);

            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            this.grpOptions.Text = ConfigurationSettings.AppSettings["Form_ZlibCompress_GrpOptions"];
            this.rbDecompress.Text = ConfigurationSettings.AppSettings["Form_ZlibCompress_RbDecompress"];
            this.rbCompress.Text = ConfigurationSettings.AppSettings["Form_ZlibCompress_RbCompress"];
            this.lblOffset.Text = ConfigurationSettings.AppSettings["Form_ZlibCompress_LblOffset"];
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new ZlibExtractorWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_ZlibCompress_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_ZlibCompress_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_ZlibCompress_MessageBegin"];
        }

        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (validateInputs())
            {

                ZlibExtractorWorker.ZlibExtractorStruct zlStruct = new ZlibExtractorWorker.ZlibExtractorStruct();
                zlStruct.SourcePaths = s;
                zlStruct.DoDecompress = this.rbDecompress.Checked;
                zlStruct.StartingOffset = VGMToolbox.util.ByteConversion.GetLongValueFromString(this.tbOffset.Text);

                base.backgroundWorker_Execute(zlStruct);
            }
        }
        private bool validateInputs()
        {
            bool ret = true;

            // put 0 in Offset if it is empty
            if (String.IsNullOrEmpty(this.tbOffset.Text))
            {
                this.tbOffset.Text = "0";
            }

            try
            {
                long tempval = VGMToolbox.util.ByteConversion.GetLongValueFromString(this.tbOffset.Text);
            }
            catch
            {
                MessageBox.Show("Cannot parse Offset, please enter an integer.  Be sure to prefix hex values with 0x",
                    ConfigurationSettings.AppSettings["Form_Global_ErrorWindowTitle"]);
                ret = false;
            }

            return ret;
        }
    }
}
