using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.stream;

namespace VGMToolbox.forms.stream
{
    public partial class XmaConvertForm : AVgmtForm
    {
        public XmaConvertForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "XMA Converter";
            
            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            InitializeComponent();

            this.initializeXmaParseInputCombo();


        }

        private void initializeXmaParseInputCombo()
        {
            this.comboXmaParseInputType.Items.Add("1");
            this.comboXmaParseInputType.Items.Add("2");
            this.comboXmaParseInputType.SelectedItem = "2";
        }

     
        
        
        
        //-----------------------------
        // drag and drop functionality
        //-----------------------------
        private void XmaConvertForm_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void XmaConvertForm_DragDrop(object sender, DragEventArgs e)
        {            
            // validate inputs and check for prerequisites
            if (this.validateAll())
            {                
                XmaConverterWorker.XmaConverterStruct taskStruct = new XmaConverterWorker.XmaConverterStruct();
                
                // paths
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                taskStruct.SourcePaths = s;

                // XMA PARSE
                taskStruct.XmaParseXmaType = this.comboXmaParseInputType.Text;
                taskStruct.XmaParseStartOffset = this.tbXmaParseStartOffset.Text;
                taskStruct.XmaParseBlockSize = this.tbXmaParseBlockSize.Text;

                base.backgroundWorker_Execute(taskStruct);
            }
        }

        //------------
        // validation
        //------------
        private bool validateAll()
        {
            bool isValid = true;

            isValid &= this.checkPrerequisites();
            isValid &= this.validateXmaParseOptions();
            
            return isValid;
        }

        private bool checkPrerequisites()
        {
            bool isValid = true;
            string errorMessage;

            if (!File.Exists(XmaConverterWorker.TOWAV_FULL_PATH))
            {
                errorMessage = 
                    String.Format(
                        "Cannot find \"{0}\".  \"{0}\" is not included and should be placed in the following directory: <{1}>",
                        Path.GetFileName(XmaConverterWorker.TOWAV_FULL_PATH),
                        Path.GetDirectoryName(XmaConverterWorker.TOWAV_FULL_PATH));

                MessageBox.Show(errorMessage, "Missing File");

                isValid = false;
            }
            
            if (!File.Exists(XmaConverterWorker.XMAPARSE_FULL_PATH))
            {
                errorMessage =
                    String.Format(
                        "Cannot find \"{0}\".  \"{0}\" is not included and should be placed in the following directory: <{1}>",
                        Path.GetFileName(XmaConverterWorker.XMAPARSE_FULL_PATH),
                        Path.GetDirectoryName(XmaConverterWorker.XMAPARSE_FULL_PATH));

                MessageBox.Show(errorMessage, "Missing File");

                isValid = false;
            }
                        
            return isValid;
        }

        private bool validateXmaParseOptions()
        {
            bool isValid = true;

            isValid &= base.checkTextBox(this.comboXmaParseInputType.Text, this.lblXmaParseInputType.Text);
            isValid &= base.checkIfTextIsParsableAsLong(this.tbXmaParseStartOffset.Text, this.lblXmaParseOffset.Text);
            isValid &= base.checkIfTextIsParsableAsLong(this.tbXmaParseBlockSize.Text, this.lblXmaParseBlockSize.Text);

            return isValid;
        }

        //----------------------------
        // background worker defaults
        //----------------------------
        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new XmaConverterWorker();
        }
        protected override string getCancelMessage()
        {
            return "Converting XMA...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Converting XMA...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Converting XMA...Begin";
        }

        //--------------------
        // combo box handlers
        //--------------------
        private void comboXmaParseInputType_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboXmaParseInputType_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
