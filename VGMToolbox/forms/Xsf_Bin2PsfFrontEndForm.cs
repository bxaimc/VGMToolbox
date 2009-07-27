using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_Bin2PsfFrontEndForm : AVgmtForm
    {
        public Xsf_Bin2PsfFrontEndForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_DoTaskButton"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_Bin2PsfFE_IntroText"];

            InitializeComponent();

            this.grpSource.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_GroupSource"];
            this.lblDriverPath.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblDriverPath"];
            this.lblSourceFiles.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblSourceFiles"];
            this.lblOutputFolder.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblOutputFolder"];
            this.lblPsfLibName.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblPsfLibName"];
            this.cbMinipsf.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_CheckBoxMinipsf"];
            this.grpOptions.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_GroupOptions"];
            this.lblSeqOffset.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblSeqOffset"];
            this.lblVhOffset.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblVhOffset"];
            this.lblVbOffset.Text =
                ConfigurationSettings.AppSettings["Form_Bin2PsfFE_LblVbOffset"];

            this.cbAllowZeroLengthSequences.Text = "Allow Zero Length Sequences";
            this.grpGenericDrivers.Text = "Generic Driver Presets";
            this.lblGenericDriver.Text = "Generic Driver";

            this.loadGenericDriversList();

            //this.grpGenericDrivers.Hide();
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            Bin2PsfWorker.Bin2PsfStruct bpStruct = new Bin2PsfWorker.Bin2PsfStruct();
            bpStruct.sourcePath = tbSourceFilesPath.Text;
            bpStruct.seqOffset = tbSeqOffset.Text;
            bpStruct.vbOffset = tbVbOffset.Text;
            bpStruct.vhOffset = tbVhOffset.Text;
            bpStruct.exePath = tbExePath.Text;
            bpStruct.outputFolder = tbOutputFolderName.Text;
            bpStruct.makeMiniPsfs = cbMinipsf.Checked;
            bpStruct.psflibName = tbPsflibName.Text;
            bpStruct.AllowZeroLengthSequences = this.cbAllowZeroLengthSequences.Checked;
            bpStruct.DriverName = (string)this.genericDriver.SelectedItem;

            base.backgroundWorker_Execute(bpStruct);
        }

        protected override void doDragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void btnExeBrowse_Click(object sender, EventArgs e)
        {
            tbExePath.Text = base.browseForFile(sender, e);
        }
        private void btnSourceDirectoryBrowse_Click(object sender, EventArgs e)
        {
            tbSourceFilesPath.Text = base.browseForFolder(sender, e);
        }
        private void cbMinipsf_CheckedChanged(object sender, EventArgs e)
        {
            string selectedItem = (string)this.genericDriver.SelectedItem;

            if ((!String.IsNullOrEmpty(selectedItem)) || (cbMinipsf.Checked))
            {
                tbPsflibName.ReadOnly = false;
            }
            else
            {
                tbPsflibName.ReadOnly = true;
                tbPsflibName.Clear();
            }
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new Bin2PsfWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_Bin2PsfFE_MessageBegin"];
        }

        private void loadGenericDriversList()
        {
            this.genericDriver.Items.Add(String.Empty);
            this.genericDriver.Items.Add(Bin2PsfWorker.GENERIC_DRIVER_MGRASS);
            // this.genericDriver.Items.Add(Bin2PsfWorker.GENERIC_DRIVER_DAVIRONICA);
        }
        private void loadGenericDriverPreset()
        {
            string selectedItem = (string)this.genericDriver.SelectedItem;

            switch (selectedItem)
            {
                case Bin2PsfWorker.GENERIC_DRIVER_MGRASS:
                    this.disablePresetFields();
                    this.loadMarkGrassGenericV21Presets();
                    break;
                
                case Bin2PsfWorker.GENERIC_DRIVER_DAVIRONICA:                   
                    this.disablePresetFields();
                    this.loadDavironicaGenericV014Presets();
                    break;
                
                default:
                    this.enablePresetFields();
                    break;
            }
        }
        private void loadMarkGrassGenericV21Presets()
        {            
            this.tbExePath.Text = Bin2PsfWorker.MGRASS_EXE_PATH;
            this.tbPsflibName.Text = String.Empty;
            this.cbMinipsf.Enabled = false;
            
            this.tbSeqOffset.Text = "0x800A0000";
            this.tbVhOffset.Text = "0x800E0000";
            this.tbVbOffset.Text = "0x80160000";
        }
        private void loadDavironicaGenericV014Presets()
        {
            this.tbExePath.Text = Bin2PsfWorker.DAVIRONICA_EXE_PATH;
            this.tbPsflibName.Text = String.Empty;
            this.cbMinipsf.Enabled = false;

            this.tbSeqOffset.Text = "0x80100000";
            this.tbVhOffset.Text = "0x800FFFF8";
            this.tbVbOffset.Text = "0x800FFFFC";
        }

        private void disablePresetFields()
        {
            this.tbExePath.Enabled = false;
            this.tbExePath.ReadOnly = true;
            this.btnExeBrowse.Enabled = false;
            this.tbPsflibName.Enabled = false;
            this.tbPsflibName.ReadOnly = true;            

            this.tbSeqOffset.Enabled = false;
            this.tbSeqOffset.ReadOnly = true;
            this.tbVhOffset.Enabled = false;
            this.tbVhOffset.ReadOnly = true;
            this.tbVbOffset.Enabled = false;
            this.tbVbOffset.ReadOnly = true;
        }
        private void enablePresetFields()
        {            
            this.tbExePath.Enabled = true;
            this.tbExePath.ReadOnly = false;
            this.tbExePath.Text = String.Empty;
            this.btnExeBrowse.Enabled = true;
            this.tbPsflibName.Enabled = true;
            this.tbPsflibName.ReadOnly = false;
            this.cbMinipsf.Enabled = true;
            this.cbMinipsf_CheckedChanged(null, null);

            this.tbSeqOffset.Enabled = true;
            this.tbSeqOffset.ReadOnly = false;
            this.tbSeqOffset.Text = String.Empty;
            this.tbVhOffset.Enabled = true;
            this.tbVhOffset.ReadOnly = false;
            this.tbVhOffset.Text = String.Empty;
            this.tbVbOffset.Enabled = true;
            this.tbVbOffset.ReadOnly = false;
            this.tbVbOffset.Text = String.Empty;
        }
        
        private void genericDriver_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadGenericDriverPreset();
        }
        private void genericDriver_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void genericDriver_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
