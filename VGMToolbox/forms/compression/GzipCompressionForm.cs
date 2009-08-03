using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.extract;
using VGMToolbox.util;

namespace VGMToolbox.forms.compression
{
    public partial class GzipCompressionForm : AVgmtForm
    {
        public GzipCompressionForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            // messages
            this.BackgroundWorker = new GzipExtractorWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_GzipCompress_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_GzipCompress_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_GzipCompress_MessageCancel"];

            this.grpSourceFiles.AllowDrop = true;
            this.btnDoTask.Hide();

            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_GzipCompress_Title"];
            this.tbOutput.Text = String.Format(ConfigurationSettings.AppSettings["Form_GzipCompress_IntroText1"],
                            CompressionUtil.GzipDecompressOutputExtension, Environment.NewLine);
            this.tbOutput.Text += String.Format(ConfigurationSettings.AppSettings["Form_GzipCompress_IntroText2"],
                CompressionUtil.GzipCompressOutputExtension, Environment.NewLine);

            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            this.grpOptions.Text = ConfigurationSettings.AppSettings["Form_GzipCompress_GrpOptions"];
            this.rbDecompress.Text = ConfigurationSettings.AppSettings["Form_GzipCompress_RbDecompress"];
            this.rbCompress.Text = ConfigurationSettings.AppSettings["Form_GzipCompress_RbCompress"];
            this.lblOffset.Text = ConfigurationSettings.AppSettings["Form_GzipCompress_LblOffset"];
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
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
                long tempval = VGMToolbox.util.Encoding.GetLongValueFromString(this.tbOffset.Text);
            }
            catch
            {
                MessageBox.Show(ConfigurationSettings.AppSettings["Form_GzipCompress_ErrorIntParse"],
                    ConfigurationSettings.AppSettings["Form_Global_ErrorWindowTitle"]);
                ret = false;
            }

            return ret;
        }
        private void grpSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (validateInputs())
            {

                GzipExtractorWorker.GzipExtractorStruct bwStruct = new GzipExtractorWorker.GzipExtractorStruct();
                bwStruct.SourcePaths = s;
                bwStruct.DoDecompress = this.rbDecompress.Checked;
                bwStruct.StartingOffset = VGMToolbox.util.Encoding.GetLongValueFromString(this.tbOffset.Text);

                base.backgroundWorker_Execute(bwStruct);
            }
        }
    }
}
