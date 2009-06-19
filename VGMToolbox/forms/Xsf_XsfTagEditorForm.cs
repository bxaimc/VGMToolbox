using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_XsfTagEditorForm : AVgmtForm
    {
        Xsf vgmData;
        bool isBatchMode;
        

        public Xsf_XsfTagEditorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.isBatchMode = false;
            
            InitializeComponent();

            this.lblTitle.Text = "xSF Tag Editor";
            this.btnDoTask.Text = "Update Tags";
        }

        private void btnBrowseDirectory_Click(object sender, EventArgs e)
        {
            this.tbSourceDirectory.Text = base.browseForFolder(sender, e);
        }

        private void tbSourceDirectory_TextChanged(object sender, EventArgs e)
        {
            this.lbFiles.Items.Clear();

            if (Directory.Exists(this.tbSourceDirectory.Text))
            {
                foreach (string f in Directory.GetFiles(this.tbSourceDirectory.Text))
                {
                    if (!String.IsNullOrEmpty(XsfUtil.GetXsfFormatString(f)))
                    {
                        this.lbFiles.Items.Add(Path.GetFileName(f));
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

        private void loadSelectedTrack()
        {
            string selectedFilePath;
            
            if (lbFiles.SelectedIndices.Count == 1)
            {
                selectedFilePath = Path.Combine(this.tbSourceDirectory.Text, this.lbFiles.Items[this.lbFiles.SelectedIndex].ToString());

                using (FileStream fs =
                    File.Open(selectedFilePath, FileMode.Open, FileAccess.Read))
                {
                    this.vgmData = new Xsf();
                    this.vgmData.Initialize(fs, selectedFilePath);

                    this.tbGame.Text = this.vgmData.GetGameTag();
                    this.tbArtist.Text = this.vgmData.GetArtistTag();
                    this.tbCopyright.Text = this.vgmData.GetCopyrightTag();
                    this.tbGenre.Text = this.vgmData.GetGenreTag();
                    this.tbYear.Text = this.vgmData.GetYearTag();
                    this.tbXsfBy.Text = this.vgmData.GetXsfByTag();
                    
                    this.tbTitle.Text = this.vgmData.GetTitleTag();
                    this.tbLength.Text = this.vgmData.GetLengthTag();
                    this.tbFade.Text = this.vgmData.GetFadeTag();
                    this.tbVolume.Text = this.vgmData.GetVolumeTag();

                    this.tbComments.Text = this.vgmData.GetCommentTag();
                }
            }
        }

        private void disableTrackItems()
        {
            this.lblTrackTitle.Enabled = false;            
            this.tbTitle.Clear();
            this.tbTitle.Enabled = false;
            this.tbTitle.ReadOnly = true;

            this.lblLength.Enabled = false;
            this.tbLength.Clear();
            this.tbLength.Enabled = false;
            this.tbLength.ReadOnly = true;

            this.lblFade.Enabled = false;
            this.tbFade.Clear();
            this.tbFade.Enabled = false;
            this.tbFade.ReadOnly = true;

            this.lblVolume.Enabled = false;
            this.tbVolume.Clear();
            this.tbVolume.Enabled = false;
            this.tbVolume.ReadOnly = true;
        }
        private void enableTrackItems()
        {
            this.lblTrackTitle.Enabled = true;
            this.tbTitle.Enabled = true;
            this.tbTitle.ReadOnly = false;

            this.lblLength.Enabled = true;
            this.tbLength.Enabled = true;
            this.tbLength.ReadOnly = false;

            this.lblFade.Enabled = true;
            this.tbFade.Enabled = true;
            this.tbFade.ReadOnly = false;

            this.lblVolume.Enabled = true;
            this.tbVolume.Enabled = true;
            this.tbVolume.ReadOnly = false;
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            int j = 0;
            XsfTagUpdaterWorker.XsfTagUpdaterStruct xtUpdateStruct = new XsfTagUpdaterWorker.XsfTagUpdaterStruct();
                        
            xtUpdateStruct.SourcePaths = new string[this.lbFiles.SelectedIndices.Count];            
            foreach (int i in this.lbFiles.SelectedIndices)
            {
                xtUpdateStruct.SourcePaths[j++] = Path.Combine(this.tbSourceDirectory.Text, this.lbFiles.Items[i].ToString());
            }

            xtUpdateStruct.RemoveEmptyTags = cbDeleteEmpty.Checked;
            xtUpdateStruct.IsBatchMode = this.isBatchMode;

            xtUpdateStruct.TitleTag = this.tbTitle.Text;
            xtUpdateStruct.ArtistTag = this.tbArtist.Text;
            xtUpdateStruct.GameTag = this.tbGame.Text;
            xtUpdateStruct.YearTag = this.tbYear.Text;
            xtUpdateStruct.GenreTag = this.tbGenre.Text;
            xtUpdateStruct.CommentTag = this.tbComments.Text;
            xtUpdateStruct.CopyrightTag = this.tbCopyright.Text;
            xtUpdateStruct.XsfByTag = this.tbXsfBy.Text;
            xtUpdateStruct.VolumeTag = this.tbVolume.Text;
            xtUpdateStruct.LengthTag = this.tbLength.Text;
            xtUpdateStruct.FadeTag = this.tbFade.Text;

            base.backgroundWorker_Execute(xtUpdateStruct);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new XsfTagUpdaterWorker();
        }
        protected override string getCancelMessage()
        {
            return "Updating xSF Tags...Cancelled.";
        }
        protected override string getCompleteMessage()
        {
            return "Updating xSF Tags...Complete.";
        }
        protected override string getBeginMessage()
        {
            return "Updating xSF Tags...Begin.";
        }        
    }
}
