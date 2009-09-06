using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using VGMToolbox.dbutil;
using VGMToolbox.plugin;
using VGMToolbox.tools.extract;

namespace VGMToolbox.forms.extraction
{
    public partial class OffsetFinderForm : AVgmtForm
    {
        private static readonly string DB_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "db"), "collection.s3db");
        private static readonly string PLUGIN_PATH = 
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "plugins"), "AdvancedCutter");        
        
        public OffsetFinderForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_SimpleCutter_Title"];

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();

            this.tbOutput.Text = String.Format(ConfigurationSettings.AppSettings["Form_SimpleCutter_IntroText"], PLUGIN_PATH);
           
            InitializeComponent();

            this.toolTip1.SetToolTip(this.btnRefresh, "Refresh Presets");

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

            // this.lblStartingOffset.Text
            // this.lblMinCutSize.Text
            // this.lblMinCutSizeBytes.Text

            this.createEndianList();
            this.createOffsetSizeList();
            this.doOffsetModuloSearchStringCheckbox();
            this.resetCutSection();
            
            //this.loadPresetsComboBox();
            this.loadOffsetPlugins();
        }

        private void tbSourcePaths_DragDrop(object sender, DragEventArgs e)
        {
            if (validateInputs())
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                OffsetFinderWorker.OffsetFinderStruct ofStruct = new OffsetFinderWorker.OffsetFinderStruct();
                ofStruct.SourcePaths = s;
                ofStruct.searchString = tbSearchString.Text;
                ofStruct.startingOffset = 
                    String.IsNullOrEmpty(tbStartingOffset.Text) ? "0" : tbStartingOffset.Text;
                ofStruct.treatSearchStringAsHex = cbSearchAsHex.Checked;

                if (cbModOffsetSearchString.Checked)
                {
                    ofStruct.DoSearchStringModulo = true;
                    ofStruct.SearchStringModuloDivisor = this.tbOffsetModuloSearchStringDivisor.Text;
                    ofStruct.SearchStringModuloResult = this.tbOffsetModuloSearchStringResult.Text;
                }

                if (cbDoCut.Checked)
                {
                    ofStruct.cutFile = true;

                    ofStruct.searchStringOffset = this.tbSearchStringOffset.Text;
                    ofStruct.outputFileExtension = this.tbOutputExtension.Text;
                    ofStruct.MinimumSize = this.tbMinSizeForCut.Text;

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
                        
                        if (cbModOffsetTerminator.Checked)
                        {
                            ofStruct.DoTerminatorModulo = true;
                            ofStruct.TerminatorStringModuloDivisor = this.tbOffsetModuloTerminatorDivisor.Text;
                            ofStruct.TerminatorStringModuloResult = this.tbOffsetModuloTerminatorResult.Text;
                        }
                    }
                    else if (this.rbStaticCutSize.Checked)
                    {
                        ofStruct.cutSize = this.tbStaticCutsize.Text;
                    }
                    else
                    {
                        MessageBox.Show("Please select a radio button indicating the Cut Size Options to use.");
                        return; // hokey, but oh well
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
                this.cbModOffsetTerminator.Enabled = false;
                this.tbOffsetModuloTerminatorDivisor.ReadOnly = true;
                this.tbOffsetModuloTerminatorDivisor.Enabled = false;                
                this.tbOffsetModuloTerminatorResult.ReadOnly = true;
                this.tbOffsetModuloTerminatorResult.Enabled = false;
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
                this.cbModOffsetTerminator.Enabled = false;
                this.tbOffsetModuloTerminatorDivisor.ReadOnly = true;
                this.tbOffsetModuloTerminatorDivisor.Enabled = false;
                this.tbOffsetModuloTerminatorResult.ReadOnly = true;
                this.tbOffsetModuloTerminatorResult.Enabled = false;
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
                this.cbModOffsetTerminator.Enabled = true;
                this.tbOffsetModuloTerminatorDivisor.ReadOnly = false;
                this.tbOffsetModuloTerminatorDivisor.Enabled = true;
                this.tbOffsetModuloTerminatorResult.ReadOnly = false;
                this.tbOffsetModuloTerminatorResult.Enabled = true;
            }

            this.doOffsetModuloTerminatorCheckbox();
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

                this.lblMinCutSize.Show();
                this.tbMinSizeForCut.ReadOnly = false;
                this.tbMinSizeForCut.Enabled = true;
                this.tbMinSizeForCut.Show();                
                this.lblMinCutSizeBytes.Show();

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

                this.cbModOffsetTerminator.Checked = false;
                this.cbModOffsetTerminator.Show();
                this.tbOffsetModuloTerminatorDivisor.Show();
                this.tbOffsetModuloTerminatorResult.Show();
                this.lblOffsetModuloEquals.Show();
            }
            else
            {
                tbSearchStringOffset.ReadOnly = true;
                tbOutputExtension.ReadOnly = true;

                this.lblMinCutSize.Hide();
                this.tbMinSizeForCut.ReadOnly = true;
                this.tbMinSizeForCut.Enabled = false;
                this.tbMinSizeForCut.Hide();
                this.lblMinCutSizeBytes.Hide();

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

                this.cbModOffsetTerminator.Checked = false;
                this.cbModOffsetTerminator.Hide();
                this.tbOffsetModuloTerminatorDivisor.Hide();
                this.tbOffsetModuloTerminatorResult.Hide();
                this.lblOffsetModuloEquals.Hide();
            }

            this.doOffsetModuloTerminatorCheckbox();
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
        private void loadSelectedItem2()
        {
            string warningString;
            DataRowView drv = (DataRowView)this.comboPresets.SelectedItem;
            DataTable dt = SqlLiteUtil.GetSimpleDataItem(DB_PATH, "OffsetFinder", "OffsetFinderId", drv["OffsetFinderId"].ToString());
            DataRow dr = dt.Rows[0];

            this.cbDoCut.Checked = true;

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
        
        private void loadSelectedItem()
        {
            OffsetFinderTemplate preset = (OffsetFinderTemplate)this.comboPresets.SelectedItem;

            if (preset != null)
            {
                this.resetCutSection();
                this.loadOffsetFinderPreset(preset);

                if (!String.IsNullOrEmpty(preset.NotesOrWarnings))
                {
                    MessageBox.Show(preset.NotesOrWarnings, "Notes/Warnings");
                }
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

        private void loadOffsetPlugins()
        {
            comboPresets.Items.Clear();
            
            foreach (string f in Directory.GetFiles(PLUGIN_PATH, "*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    OffsetFinderTemplate preset = getPresetFromFile(f);

                    if ((preset != null) && (!String.IsNullOrEmpty(preset.Header.FormatName)))
                    {
                        comboPresets.Items.Add(preset);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error loading preset file <{0}>", Path.GetFileName(f)), "Error");
                }
            }

            comboPresets.Sorted = true;           
        }        
        private OffsetFinderTemplate getPresetFromFile(string filePath)
        {
            OffsetFinderTemplate preset = null;
            
            preset = new OffsetFinderTemplate();
            XmlSerializer serializer = new XmlSerializer(preset.GetType());
            using (FileStream xmlFs = File.OpenRead(filePath))
            {
                using (XmlTextReader textReader = new XmlTextReader(xmlFs))
                {
                    preset = (OffsetFinderTemplate)serializer.Deserialize(textReader);
                }
            }

            return preset;
        }
        private void loadOffsetFinderPreset(OffsetFinderTemplate presets)
        {           
            this.cbDoCut.Checked = true;

            this.tbSearchString.Text = presets.SearchParameters.SearchString;
            this.cbSearchAsHex.Checked = presets.SearchParameters.TreatSearchStringAsHex;

            this.tbSearchStringOffset.Text = presets.SearchParameters.SearchStringOffset;
            this.tbOutputExtension.Text = presets.SearchParameters.OutputFileExtension;

            switch (presets.SearchParameters.CutParameters.CutStyle)
            {
                case CutStyle.@static:
                    this.rbStaticCutSize.Checked = true;
                    this.tbStaticCutsize.Text = presets.SearchParameters.CutParameters.StaticCutSize;
                    break;
                case CutStyle.offset:
                    this.rbOffsetBasedCutSize.Checked = true;
                    this.tbCutSizeOffset.Text = presets.SearchParameters.CutParameters.CutSizeAtOffset;
                    this.cbOffsetSize.SelectedItem = presets.SearchParameters.CutParameters.CutSizeOffsetSize;
                    switch (presets.SearchParameters.CutParameters.CutSizeOffsetEndianess)
                    {
                        case Endianness.big:
                            this.cbByteOrder.SelectedItem = "Big Endian";
                            break;
                        case Endianness.little:
                            this.cbByteOrder.SelectedItem = "Little Endian";
                            break;
                    }
                    break;
                case CutStyle.terminator:
                    this.rbUseTerminator.Checked = true;
                    this.tbTerminatorString.Text = presets.SearchParameters.CutParameters.TerminatorString;
                    this.cbTreatTerminatorAsHex.Checked = presets.SearchParameters.CutParameters.TreatTerminatorStringAsHex;
                    this.cbIncludeTerminatorInLength.Checked = presets.SearchParameters.CutParameters.IncludeTerminatorInSize;
                    break;
            }
            
            this.cbAddExtraBytes.Checked = presets.SearchParameters.AddExtraBytes;
            this.tbExtraCutSizeBytes.Text = presets.SearchParameters.AddExtraByteSize;        
        }

        private OffsetFinderTemplate getPresetForCurrentValues()
        { 
            OffsetFinderTemplate preset = new OffsetFinderTemplate();

            preset.SearchParameters.SearchString = this.tbSearchString.Text;
            preset.SearchParameters.TreatSearchStringAsHex = this.cbSearchAsHex.Checked;

            preset.SearchParameters.SearchStringOffset = this.tbSearchStringOffset.Text;
            preset.SearchParameters.OutputFileExtension = this.tbOutputExtension.Text;

            if (this.rbStaticCutSize.Checked)
            {
                preset.SearchParameters.CutParameters.CutStyle = CutStyle.@static;
                preset.SearchParameters.CutParameters.StaticCutSize = this.tbStaticCutsize.Text;

            }
            else if (this.rbOffsetBasedCutSize.Checked)
            {
                preset.SearchParameters.CutParameters.CutStyle = CutStyle.offset;
                preset.SearchParameters.CutParameters.CutSizeAtOffset = this.tbCutSizeOffset.Text;
                preset.SearchParameters.CutParameters.CutSizeOffsetSize = this.cbOffsetSize.SelectedItem.ToString();

                if (this.cbByteOrder.SelectedItem.Equals("Little Endian"))
                {
                    preset.SearchParameters.CutParameters.CutSizeOffsetEndianess = Endianness.little;
                }
                else if (this.cbByteOrder.SelectedItem.Equals("Big Endian"))
                {
                    preset.SearchParameters.CutParameters.CutSizeOffsetEndianess = Endianness.big;
                }
            }
            else if (this.rbUseTerminator.Checked)
            {
                preset.SearchParameters.CutParameters.CutStyle = CutStyle.terminator;
                preset.SearchParameters.CutParameters.TerminatorString = this.tbTerminatorString.Text;
                preset.SearchParameters.CutParameters.TreatTerminatorStringAsHexSpecified = this.cbTreatTerminatorAsHex.Checked;
                preset.SearchParameters.CutParameters.TreatTerminatorStringAsHex = this.cbTreatTerminatorAsHex.Checked;
                preset.SearchParameters.CutParameters.IncludeTerminatorInSize = this.cbIncludeTerminatorInLength.Checked;
            }

            preset.SearchParameters.AddExtraBytes = this.cbAddExtraBytes.Checked;
            preset.SearchParameters.AddExtraByteSize = this.tbExtraCutSizeBytes.Text;

            return preset;
        }
        private void btnSavePreset_Click(object sender, EventArgs e)
        {
            OffsetFinderTemplate preset = getPresetForCurrentValues();

            if (preset != null)
            {
                SavePresetForm saveForm = new SavePresetForm(preset);
                saveForm.Show();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.loadOffsetPlugins();
        }

        private void cbModOffsetTerminator_CheckedChanged(object sender, EventArgs e)
        {
            doOffsetModuloTerminatorCheckbox();
        }

        private void doOffsetModuloTerminatorCheckbox()
        {
            if (!cbModOffsetTerminator.Checked)
            {
                this.tbOffsetModuloTerminatorDivisor.ReadOnly = true;
                this.tbOffsetModuloTerminatorDivisor.Enabled = false;
                this.tbOffsetModuloTerminatorDivisor.Clear();

                this.tbOffsetModuloTerminatorResult.ReadOnly = true;
                this.tbOffsetModuloTerminatorResult.Enabled = false;
                this.tbOffsetModuloTerminatorResult.Clear();
            }
            else
            {
                this.tbOffsetModuloTerminatorDivisor.ReadOnly = false;
                this.tbOffsetModuloTerminatorDivisor.Enabled = true;
                this.tbOffsetModuloTerminatorDivisor.Clear();

                this.tbOffsetModuloTerminatorResult.ReadOnly = false;
                this.tbOffsetModuloTerminatorResult.Enabled = true;
                this.tbOffsetModuloTerminatorResult.Clear();
            }
        }

        private void doOffsetModuloSearchStringCheckbox()
        {
            if (!cbModOffsetSearchString.Checked)
            {
                this.tbOffsetModuloSearchStringDivisor.ReadOnly = true;
                this.tbOffsetModuloSearchStringDivisor.Enabled = false;
                this.tbOffsetModuloSearchStringDivisor.Clear();

                this.tbOffsetModuloSearchStringResult.ReadOnly = true;
                this.tbOffsetModuloSearchStringResult.Enabled = false;
                this.tbOffsetModuloSearchStringResult.Clear();
            }
            else
            {
                this.tbOffsetModuloSearchStringDivisor.ReadOnly = false;
                this.tbOffsetModuloSearchStringDivisor.Enabled = true;
                this.tbOffsetModuloSearchStringDivisor.Clear();

                this.tbOffsetModuloSearchStringResult.ReadOnly = false;
                this.tbOffsetModuloSearchStringResult.Enabled = true;
                this.tbOffsetModuloSearchStringResult.Clear();
            }
        }

        private void cbModOffsetSearchString_CheckedChanged(object sender, EventArgs e)
        {
            this.doOffsetModuloSearchStringCheckbox();
        }
    }
}
