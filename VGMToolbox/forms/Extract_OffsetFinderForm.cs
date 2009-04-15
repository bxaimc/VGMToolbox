using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Forms;

using VGMToolbox.dbutil;
using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms
{
    public partial class Extract_OffsetFinderForm : VgmtForm
    {
        private static readonly string DB_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "db"), "collection.s3db");        
        
        public Extract_OffsetFinderForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SimpleCutter_Title"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_SimpleCutter_IntroText1"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_SimpleCutter_IntroText2"] + Environment.NewLine;
            this.tbOutput.Text += ConfigurationSettings.AppSettings["Form_SimpleCutter_IntroText3"] + Environment.NewLine;

            InitializeComponent();

            this.grpFiles.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_GroupFiles"];
            this.lblDragNDrop.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblDragNDrop"];
            this.grpCriteria.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_GroupCriteria"];
            this.cbDoCut.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_CheckBoxDoCut"];
            this.lblStringAtOffset.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblStringAtOffset"];
            this.lblOutputExtension.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblOutputExtension"];
            this.gbCutSizeOptions.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_GroupCutSizeOptions"];
            this.rbStaticCutSize.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_RadioStaticCutSize"];
            this.rbOffsetBasedCutSize.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_RadioOffsetBasedCutSize"];
            this.lblHasSize.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblHasSize"];
            this.lblStoredIn.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblStoredIn"];
            this.lblInBytes.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblInBytes"];
            this.lblFromStart.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblFromStart"];
            this.lblInBytes2.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblInBytes2"];
            this.lblByteOrder.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_LblByteOrder"];
            this.rbUseTerminator.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_RadioUseTerminator"];
            this.cbTreatTerminatorAsHex.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_CheckBoxTreatTerminatorAsHex"];
            this.cbIncludeTerminatorInLength.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_CheckBoxIncludeTerminatorInLength"];
            this.cbAddExtraBytes.Text =
                ConfigurationSettings.AppSettings["Form_SimpleCutter_CheckBoxExtracCutSizeBytes"];

            this.createEndianList();
            this.createOffsetSizeList();
            this.resetCutSection();

            this.loadPresetsComboBox();
        }

        private void tbSourcePaths_DragDrop(object sender, DragEventArgs e)
        {
            if (validateInputs())
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                OffsetFinderWorker.OffsetFinderStruct ofStruct = new OffsetFinderWorker.OffsetFinderStruct();
                ofStruct.SourcePaths = s;
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
                    else if (this.rbUseTerminator.Checked)
                    {
                        ofStruct.useTerminatorForCutsize = true;
                        ofStruct.terminatorString = this.tbTerminatorString.Text;
                        ofStruct.treatTerminatorStringAsHex = this.cbTreatTerminatorAsHex.Checked;
                        ofStruct.includeTerminatorLength = this.cbIncludeTerminatorInLength.Checked;
                    }
                    else
                    {
                        ofStruct.cutSize = this.tbStaticCutsize.Text;
                    }

                    if (cbAddExtraBytes.Checked)
                    {
                        ofStruct.extraCutSizeBytes = tbExtraCutSizeBytes.Text;
                    }
                }

                base.backgroundWorker_Execute(ofStruct);
            }
        }
        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }        
        private void doRadioCheckedChanged(object sender, EventArgs e)
        {
            if (rbStaticCutSize.Checked)
            {
                tbStaticCutsize.ReadOnly = false;
                tbCutSizeOffset.ReadOnly = true;
                cbOffsetSize.Enabled = false;
                cbByteOrder.Enabled = false;
                tbTerminatorString.ReadOnly = true;
                cbTreatTerminatorAsHex.Enabled = false;
                cbIncludeTerminatorInLength.Enabled = false;
            }
            else if (rbOffsetBasedCutSize.Checked)
            {
                tbStaticCutsize.ReadOnly = true;
                tbCutSizeOffset.ReadOnly = false;
                cbOffsetSize.Enabled = true;
                cbByteOrder.Enabled = true;
                tbTerminatorString.ReadOnly = true;
                cbTreatTerminatorAsHex.Enabled = false;
                cbIncludeTerminatorInLength.Enabled = false;
            }
            else
            {
                tbStaticCutsize.ReadOnly = true;
                tbCutSizeOffset.ReadOnly = true;
                cbOffsetSize.Enabled = false;
                cbByteOrder.Enabled = false;
                tbTerminatorString.ReadOnly = false;
                cbTreatTerminatorAsHex.Enabled = true;
                cbIncludeTerminatorInLength.Enabled = true;
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
                else if (rbOffsetBasedCutSize.Checked)
                {
                    ret = ret && base.checkTextBox(this.tbCutSizeOffset.Text, "Cut Size Offset");
                    ret = ret && base.checkTextBox((string)this.cbOffsetSize.Text, "Offset Size");
                    ret = ret && base.checkTextBox((string)this.cbByteOrder.Text, "Byte Order");
                }
                else
                {
                    ret = ret && base.checkTextBox(this.tbTerminatorString.Text, "Terminator String");
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
                lblStringAtOffset.Show();
                lblHasSize.Show();
                lblFromStart.Show();
                lblInBytes2.Show();
                lblInBytes.Show();
                lblStoredIn.Show();
                lblByteOrder.Show();
                lblOutputExtension.Show();
                
                this.rbUseTerminator.Show();
                this.tbTerminatorString.Show();
                this.cbTreatTerminatorAsHex.Show();
                this.cbIncludeTerminatorInLength.Show();

                this.cbAddExtraBytes.Show();
                this.tbExtraCutSizeBytes.Show();
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
                lblStringAtOffset.Hide();
                lblHasSize.Hide();
                lblFromStart.Hide();
                lblInBytes2.Hide();
                lblInBytes.Hide();
                lblStoredIn.Hide();
                lblByteOrder.Hide();
                lblOutputExtension.Hide();

                this.rbUseTerminator.Hide();
                this.tbTerminatorString.Hide();
                this.cbTreatTerminatorAsHex.Hide();
                this.cbIncludeTerminatorInLength.Hide();

                this.cbAddExtraBytes.Hide();
                this.tbExtraCutSizeBytes.Hide();
            }        
        }
        private void cbAddExtraBytes_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAddExtraBytes.Checked)
            {
                tbExtraCutSizeBytes.ReadOnly = false;
            }
            else
            {
                tbExtraCutSizeBytes.ReadOnly = true;
            }
        }
        
        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new OffsetFinderWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SimpleCutter_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SimpleCutter_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SimpleCutter_MessageBegin"];
        }

        private void loadPresetsComboBox()
        {
            this.comboPresets.Items.Add(String.Empty);

            this.comboPresets.DataSource = SqlLiteUtil.GetSimpleDataTable(DB_PATH, "OffsetFinder", "OffsetFinderFormatName");
            this.comboPresets.DisplayMember = "OffsetFinderFormatName";
            this.comboPresets.ValueMember = "OffsetFinderId";
        }
        private void loadSelectedItem()
        {
            string warningString;
            DataRowView drv = (DataRowView)this.comboPresets.SelectedItem;
            DataTable dt = SqlLiteUtil.GetSimpleDataItem(DB_PATH, "OffsetFinder", "OffsetFinderId", drv["OffsetFinderId"].ToString());
            DataRow dr = dt.Rows[0];

            this.tbSearchString.Text = dr["SearchString"].ToString();
            this.cbSearchAsHex.Checked = Convert.ToBoolean(dr["TreatSearchStringAsHex"]);
            
            this.tbSearchStringOffset.Text = dr["SearchStringOffset"].ToString();
            this.tbOutputExtension.Text = dr["OuputFileExtension"].ToString();

            this.rbStaticCutSize.Checked = Convert.ToBoolean(dr["UseStaticCutsize"]);
            this.tbStaticCutsize.Text = dr["StaticCutSize"].ToString();

            this.rbOffsetBasedCutSize.Checked = Convert.ToBoolean(dr["UseCutSizeAtOffset"]);
            this.tbCutSizeOffset.Text = dr["CutSizeAtOffset"].ToString();
            this.cbOffsetSize.SelectedItem = dr["CutSizeOffsetSize"];
            this.cbByteOrder.SelectedItem = dr["CutSizeOffsetEndianess"];

            this.rbUseTerminator.Checked = Convert.ToBoolean(dr["UseTerminatorString"]);
            this.tbTerminatorString.Text = dr["TerminatorString"].ToString();
            this.cbTreatTerminatorAsHex.Checked = Convert.ToBoolean(dr["TreatTerminatorStringAsHex"]);
            this.cbIncludeTerminatorInLength.Checked = Convert.ToBoolean(dr["IncludeTerminatorInSize"]);

            this.cbAddExtraBytes.Checked = Convert.ToBoolean(dr["AddExtraBytes"]);
            this.tbExtraCutSizeBytes.Text = dr["AddExtraBytesSize"].ToString();

            warningString = dr["NotesOrWarnings"].ToString();

            if (!String.IsNullOrEmpty(warningString))
            {
                MessageBox.Show(warningString, "Notes/Warnings");
            }
        }

        private void comboPresets_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void comboPresets_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnLoadPreset_Click(object sender, EventArgs e)
        {
            loadSelectedItem();
        }
    }
}
