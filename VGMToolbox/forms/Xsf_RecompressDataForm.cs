using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.dbutil;
using VGMToolbox.tools.xsf;
using VGMToolbox.plugin;

namespace VGMToolbox.forms
{
    public partial class Xsf_RecompressDataForm : AVgmtForm
    {
        private static readonly string DB_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "db"), "collection.s3db");        
        
        public Xsf_RecompressDataForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.loadCompressionComboBox();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new XsfRecompressDataWorker();
        }
        protected override string getCancelMessage()
        {
            return "Recompress Data...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Recompress Data...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Recompress Data...Begin";
        }

        private void loadCompressionComboBox()
        {
            this.cbCompressionLevel.Items.Add(String.Empty);
            this.cbCompressionLevel.DataSource = SqlLiteUtil.GetSimpleDataTable(DB_PATH, "ZlibCompressionLevels", "CompressionLevelId");
            this.cbCompressionLevel.DisplayMember = "CompressionLevelDescription";
            this.cbCompressionLevel.ValueMember = "CompressionLevelId";        
        }

        private void tbSource_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            XsfRecompressDataWorker.XsfRecompressDataStruct xrdStruct = new XsfRecompressDataWorker.XsfRecompressDataStruct();
            xrdStruct.SourcePaths = s;
            xrdStruct.CompressionLevel = Convert.ToInt32(this.cbCompressionLevel.SelectedValue);

            base.backgroundWorker_Execute(xrdStruct);            
        }
        private void cbCompressionLevel_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void cbCompressionLevel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
