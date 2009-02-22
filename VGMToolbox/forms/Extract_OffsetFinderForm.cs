using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.tools.extract;

namespace VGMToolbox.forms
{
    public partial class Extract_OffsetFinderForm : VgmtForm
    {
        
        OffsetFinderWorker offsetFinderWorker;
        
        public Extract_OffsetFinderForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Simple Cutter/Offset Finder";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            this.tbOutput.Text = "Search for offsets, or search for offsets and cut based on input criteria." + Environment.NewLine;
            this.tbOutput.Text += "- Use \"Search String is at Offset\" for file types that have their Magic Bytes at a nonzero offset (i.e. SSF Driver) " + Environment.NewLine;
            this.tbOutput.Text += "- Use \"Cut Size is at Offset\" for file types that store their length in the header." + Environment.NewLine;

            InitializeComponent();

            this.createEndianList();
            this.createOffsetSizeList();
            this.resetCutSection();
        }

        private void OffsetFinderWorker_WorkComplete(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Searching for Strings...Cancelled";
                tbOutput.Text += "Operation cancelled.";
            }
            else
            {
                lblProgressLabel.Text = String.Empty;
                toolStripStatusLabel1.Text = "Searching for Strings...Complete";
            }

            // update node color
            setNodeAsComplete();
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (offsetFinderWorker != null && offsetFinderWorker.IsBusy)
            {
                tbOutput.Text += "CANCEL PENDING...";
                offsetFinderWorker.CancelAsync();
                this.errorFound = true;
            }
        }

        private void tbSourcePaths_DragDrop(object sender, DragEventArgs e)
        {
            base.initializeProcessing();

            if (validateInputs())
            {
                toolStripStatusLabel1.Text = "Searching for Strings...Begin";

                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                OffsetFinderWorker.OffsetFinderStruct ofStruct = new OffsetFinderWorker.OffsetFinderStruct();
                ofStruct.sourcePaths = s;
                ofStruct.searchString = tbSearchString.Text;
                ofStruct.treatSearchStringAsHex = cbSearchAsHex.Checked;

                if (cbDoCut.Checked)
                {
                    ofStruct.cutFile = true;

                    ofStruct.searchStringOffset = this.tbSearchStringOffset.Text;
                    ofStruct.outputFileExtension = this.tbOutputExtension.Text;

                    if (this.rbOffsetBasedCutSize.Checked)
                    {
                        ofStruct.cutSize = this.tbCutSizeOffset.Text;
                        ofStruct.cutSizeOffsetSize = this.cbOffsetSize.Text;
                        ofStruct.isCutSizeAnOffset = true;
                        ofStruct.isLittleEndian =
                            (cbByteOrder.Text.Equals(OffsetFinderWorker.LITTLE_ENDIAN));

                    }
                    else
                    {
                        ofStruct.cutSize = this.tbStaticCutsize.Text;
                    }
                }

                offsetFinderWorker = new OffsetFinderWorker();
                offsetFinderWorker.ProgressChanged += backgroundWorker_ReportProgress;
                offsetFinderWorker.RunWorkerCompleted += OffsetFinderWorker_WorkComplete;
                offsetFinderWorker.RunWorkerAsync(ofStruct);
            }
        }

        private void rbStaticCutSize_CheckedChanged(object sender, EventArgs e)
        {
            if (rbStaticCutSize.Checked)
            {
                tbStaticCutsize.ReadOnly = false;
                tbCutSizeOffset.ReadOnly = true;
                cbOffsetSize.Enabled = false;
                cbByteOrder.Enabled = false;
            }
            else
            {
                tbStaticCutsize.ReadOnly = true;
                tbCutSizeOffset.ReadOnly = false;
                cbOffsetSize.Enabled = true;
                cbByteOrder.Enabled = true;
            }
        }

        private void createEndianList()
        {
            cbByteOrder.Items.Add(OffsetFinderWorker.BIG_ENDIAN);
            cbByteOrder.Items.Add(OffsetFinderWorker.LITTLE_ENDIAN);
        }

        private void createOffsetSizeList()
        {
            cbOffsetSize.Items.Add("1");
            cbOffsetSize.Items.Add("2");
            cbOffsetSize.Items.Add("4");            
        }

        private void cbByteOrder_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void cbByteOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbOffsetSize_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void cbOffsetSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private bool validateInputs()
        {
            bool ret = base.checkTextBox(this.tbSearchString.Text, "Search String");

            if (cbDoCut.Checked)
            {                
                ret = ret && base.checkTextBox(this.tbOutputExtension.Text, "Output Extension");

                if (this.rbStaticCutSize.Checked)
                {
                    ret = ret && base.checkTextBox(this.tbStaticCutsize.Text, "Static Cut Size");
                }
                else
                {
                    ret = ret && base.checkTextBox(this.tbCutSizeOffset.Text, "Cut Size Offset");
                    ret = ret && base.checkTextBox((string) this.cbOffsetSize.Text, "Offset Size");
                    ret = ret && base.checkTextBox((string) this.cbByteOrder.Text, "Byte Order");                                    
                }
            }

            return ret;
        }

        private void cbDoCut_CheckedChanged(object sender, EventArgs e)
        {
            this.resetCutSection();
        }

        private void resetCutSection()
        {
            if (cbDoCut.Checked)
            {
                tbSearchStringOffset.ReadOnly = false;
                tbOutputExtension.ReadOnly = false;

                rbStaticCutSize.Enabled = true;
                rbOffsetBasedCutSize.Enabled = true;

                rbStaticCutSize.Checked = false;
                rbOffsetBasedCutSize.Checked = false;

                tbStaticCutsize.ReadOnly = false;
                tbCutSizeOffset.ReadOnly = false;
                cbOffsetSize.Enabled = true;
                cbByteOrder.Enabled = true;

                tbSearchStringOffset.Show();
                tbOutputExtension.Show();
                rbStaticCutSize.Show();
                rbOffsetBasedCutSize.Show();
                rbStaticCutSize.Show();
                rbOffsetBasedCutSize.Show();
                tbStaticCutsize.Show();
                tbCutSizeOffset.Show();
                cbOffsetSize.Show();
                cbByteOrder.Show();

                gbCutSizeOptions.Show();
                label3.Show();
                label4.Show();
                label5.Show();
                label6.Show();
                label7.Show();
                label8.Show();
                label9.Show();
                label11.Show();
            }
            else
            {
                tbSearchStringOffset.ReadOnly = true;
                tbOutputExtension.ReadOnly = true;

                rbStaticCutSize.Checked = false;
                rbOffsetBasedCutSize.Checked = false;

                rbStaticCutSize.Enabled = false;
                rbOffsetBasedCutSize.Enabled = false;

                tbStaticCutsize.ReadOnly = true;
                tbCutSizeOffset.ReadOnly = true;
                cbOffsetSize.Enabled = false;
                cbByteOrder.Enabled = false;


                tbSearchStringOffset.Hide();
                tbOutputExtension.Hide();
                rbStaticCutSize.Hide();
                rbOffsetBasedCutSize.Hide();
                rbStaticCutSize.Hide();
                rbOffsetBasedCutSize.Hide();
                tbStaticCutsize.Hide();
                tbCutSizeOffset.Hide();
                cbOffsetSize.Hide();
                cbByteOrder.Hide();

                gbCutSizeOptions.Hide();
                label3.Hide();
                label4.Hide();
                label5.Hide();
                label6.Hide();
                label7.Hide();
                label8.Hide();
                label9.Hide();
                label11.Hide();
            }        
        }
    }
}
