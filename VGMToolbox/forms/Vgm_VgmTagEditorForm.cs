﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.dbutil;
using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.vgm;

namespace VGMToolbox.forms
{    
    public partial class Vgm_VgmTagEditorForm : AVgmtForm
    {
        private static readonly string DB_PATH =
            Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "db"), "collection.s3db");
        
        IGd3TagFormat vgmData;
        bool isBatchMode;
        
        public Vgm_VgmTagEditorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.isBatchMode = false;
            
            InitializeComponent();

            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_DoTaskButton"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_Info"];

            this.grpSourceFiles.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_GrpSourceFiles"];
            this.grpSetTags.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_GrpSetTags"];
            this.lblGameEn.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblGameEn"];
            this.lblGameJp.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblGameJp"];
            this.lblSystemEn.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblSystemEn"];
            this.lblSystemJp.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblSystemJp"];
            this.lblArtistEn.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblArtistEn"];
            this.lblArtistJp.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblArtistJp"];
            this.lblGameDate.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblGameDate"];
            this.lblRipper.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblRipper"];
            this.grpTrackTags.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_GrpTrackTags"];
            this.lblTrackTitleEn.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblTrackTitleEn"];
            this.lblTrackTitleJp.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_LblTrackTitleJp"];
            this.grpComments.Text = ConfigurationSettings.AppSettings["Form_VgmTagEditor_GrpComments"];

            this.loadSystems();
        }


        private void loadSystems()
        {
            System.Text.Encoding unicode = System.Text.Encoding.Unicode;

            DataTable dtEn = SqlLiteUtil.GetSimpleDataTable(DB_PATH, "VgmSystemNames", "SystemName");
            DataRow drEn = dtEn.NewRow();
            dtEn.Rows.InsertAt(drEn, 0);
            this.cbSystemEn.DataSource = dtEn;
            this.cbSystemEn.DisplayMember = "SystemName";
            this.cbSystemEn.ValueMember = "SystemName";


            DataTable dtJp = SqlLiteUtil.GetSimpleDataTable(DB_PATH, "VgmSystemNames", "SystemName");
            DataRow drJp = dtJp.NewRow();
            dtJp.Rows.InsertAt(drJp, 0);
            this.cbSystemJp.DataSource = dtJp;
            this.cbSystemJp.DisplayMember = "SystemName";
            this.cbSystemJp.ValueMember = "SystemName";
            
        }

        private void btnBrowseDirectory_Click(object sender, EventArgs e)
        {
            this.tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }
        private void tbSourceDirectory_TextChanged(object sender, EventArgs e)
        {
            this.lbFiles.Items.Clear();
            Type formatType;

            if (Directory.Exists(this.tbSourceDirectory.Text))
            {
                foreach (string f in Directory.GetFiles(this.tbSourceDirectory.Text))
                {
                    formatType = null;

                    using (FileStream fs = File.OpenRead(f))
                    {
                        formatType = FormatUtil.getObjectType(fs);
                        if ((formatType != null) &&
                            typeof(IGd3TagFormat).IsAssignableFrom(formatType))
                        {
                            this.lbFiles.Items.Add(Path.GetFileName(f));
                        }
                    }
                }
            }
            else
            {
                this.lbFiles.Items.Clear();
            }
        }

        private void lbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFiles.SelectedIndices.Count > 1)
            {
                this.isBatchMode = true;
                this.disableTrackItems();
            }
            else
            {
                this.isBatchMode = false;
                this.enableTrackItems();
                this.loadSelectedTrack();
            }
        }
        private void disableTrackItems()
        {
            this.lblTrackTitleEn.Enabled = false;
            this.tbTitleEn.Enabled = false;
            this.tbTitleEn.ReadOnly = true;

            this.lblTrackTitleJp.Enabled = false;
            this.tbTitleJp.Enabled = false;
            this.tbTitleJp.ReadOnly = true;
        }
        private void enableTrackItems()
        {
            this.lblTrackTitleEn.Enabled = true;
            this.tbTitleEn.Enabled = true;
            this.tbTitleEn.ReadOnly = false;

            this.lblTrackTitleJp.Enabled = true;
            this.tbTitleJp.Enabled = true;
            this.tbTitleJp.ReadOnly = false;
        }
        private void loadSelectedTrack()
        {
            string selectedFilePath;

            if (lbFiles.SelectedIndices.Count == 1)
            {
                selectedFilePath = Path.Combine(this.tbSourceDirectory.Text, this.lbFiles.Items[this.lbFiles.SelectedIndex].ToString());

                using (FileStream fs =
                    File.Open(selectedFilePath, FileMode.Open, FileAccess.Read))
                {
                    Type formatType = FormatUtil.getObjectType(fs);
                    if (formatType != null)
                    {
                        this.vgmData = (IGd3TagFormat)Activator.CreateInstance(formatType);
                        this.vgmData.Initialize(fs, selectedFilePath);

                        this.tbGameEn.Text = this.vgmData.GetGameTagEn();
                        this.tbGameJp.Text = this.vgmData.GetGameTagJp();
                        this.cbSystemEn.Text = this.vgmData.GetSystemTagEn();
                        this.cbSystemJp.Text = this.vgmData.GetSystemTagJp();
                        this.tbArtistEn.Text = this.vgmData.GetArtistTagEn();
                        this.tbArtistJp.Text = this.vgmData.GetArtistTagJp();
                        this.tbGameDate.Text = this.vgmData.GetDateTag();
                        this.tbRipper.Text = this.vgmData.GetRipperTag();
                        this.tbTitleEn.Text = this.vgmData.GetTitleTagEn();
                        this.tbTitleJp.Text = this.vgmData.GetTitleTagJp();
                        this.tbComments.Text = this.vgmData.GetCommentTag();
                    }
                }
            }
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new VgmTagUpdaterWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_VgmTagEditor_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_VgmTagEditor_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_VgmTagEditor_MessageBegin"];
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            int j = 0;
            VgmTagUpdaterWorker.VgmTagUpdaterStruct vtUpdateStruct = new VgmTagUpdaterWorker.VgmTagUpdaterStruct();

            vtUpdateStruct.SourcePaths = new string[this.lbFiles.SelectedIndices.Count];
            foreach (int i in this.lbFiles.SelectedIndices)
            {
                vtUpdateStruct.SourcePaths[j++] = Path.Combine(this.tbSourceDirectory.Text, this.lbFiles.Items[i].ToString());
            }

            vtUpdateStruct.IsBatchMode = this.isBatchMode;

            vtUpdateStruct.TitleTagEn = this.tbTitleEn.Text;
            vtUpdateStruct.TitleTagJp = this.tbTitleJp.Text;
            vtUpdateStruct.GameTagEn = this.tbGameEn.Text;
            vtUpdateStruct.GameTagJp = this.tbGameJp.Text;
            vtUpdateStruct.SystemTagEn = this.cbSystemEn.Text;
            vtUpdateStruct.SystemTagJp = this.cbSystemJp.Text;
            vtUpdateStruct.ArtistTagEn = this.tbArtistEn.Text;
            vtUpdateStruct.ArtistTagJp = this.tbArtistJp.Text;
            vtUpdateStruct.DateTag = this.tbGameDate.Text;
            vtUpdateStruct.RipperTag = this.tbRipper.Text;
            vtUpdateStruct.CommentTag = this.tbComments.Text;

            base.backgroundWorker_Execute(vtUpdateStruct);
        }

        private void cbSystemEn_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void cbSystemEn_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbSystemJp_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void cbSystemJp_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void lbFiles_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // show menu
                contextMenuRefresh.Show(lbFiles, p);
            }
        }
        private void tsmRefresh_Click(object sender, EventArgs e)
        {
            this.tbSourceDirectory_TextChanged(sender, e);
        }
    }
}