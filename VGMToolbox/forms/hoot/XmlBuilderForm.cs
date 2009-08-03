using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.hoot;

namespace VGMToolbox.forms.hoot
{
    public partial class XmlBuilderForm : AVgmtForm
    {        
        public XmlBuilderForm(TreeNode pTreeNode) : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = 
                ConfigurationSettings.AppSettings["Form_HootXmlBuilder_Title"];
            this.tbOutput.Text =
                ConfigurationSettings.AppSettings["Form_HootXmlBuilder_Intro"];
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            // messages
            this.BackgroundWorker = new HootXmlBuilderWorker();
            this.BeginMessage = ConfigurationSettings.AppSettings["Form_HootXmlBuilder_MessageBegin"];
            this.CompleteMessage = ConfigurationSettings.AppSettings["Form_HootXmlBuilder_MessageComplete"];
            this.CancelMessage = ConfigurationSettings.AppSettings["Form_HootXmlBuilder_MessageCancel"];

            this.gbHootXML_Source.AllowDrop = true;

            this.gbHootXML_Source.Text =
                ConfigurationSettings.AppSettings["Form_Global_DropSourceFiles"];
            this.gbHootXML_Options.Text =
                ConfigurationSettings.AppSettings["Form_HootXmlBuilder_GroupOptions"];
            this.cbHootXML_CombineOutput.Text =
                ConfigurationSettings.AppSettings["Form_HootXmlBuilder_CheckBoxCombineOutput"];
            this.cbHootXML_SplitOutput.Text =
                ConfigurationSettings.AppSettings["Form_HootXmlBuilder_CheckBoxSplitOutput"];
        }

        private void tbHootXML_Path_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            HootXmlBuilderWorker.HootXmlBuilderStruct xbStruct = new HootXmlBuilderWorker.HootXmlBuilderStruct();
            xbStruct.SourcePaths = s;
            xbStruct.combineOutput = cbHootXML_CombineOutput.Checked;
            xbStruct.splitOutput = cbHootXML_SplitOutput.Checked;
            xbStruct.parseFileName = cbParseFile.Checked;

            base.backgroundWorker_Execute(xbStruct);
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
    }
}
