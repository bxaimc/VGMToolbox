using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;

namespace VGMToolbox.forms
{
    public partial class Xsf_2sfTagsMigratorForm : AVgmtForm
    {
        public Xsf_2sfTagsMigratorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            InitializeComponent();

            this.lblTitle.Text = "2SF - V1 to V2 Tag Copier";
            this.btnDoTask.Text = "Copy Tags";
        }

        private void btnBrowseV1Source_Click(object sender, EventArgs e)
        {
            this.tbV1Source.Text = base.browseForFolder(sender, e);
        }
        private void btnBrowseV2Source_Click(object sender, EventArgs e)
        {
            this.tbV2Source.Text = base.browseForFolder(sender, e);
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new Xsf2sfTagMigratorWorker();
        }
        protected override string getCancelMessage()
        {
            return "Migrating Tags...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Migrating Tags...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Migrating Tags...Begin";
        }

        private void btnDoTask_Click(object sender, EventArgs e)
        {
            if (validateInputs())
            {
                Xsf2sfTagMigratorWorker.Xsf2sfTagMigratorStruct xtStruct = new Xsf2sfTagMigratorWorker.Xsf2sfTagMigratorStruct();

                xtStruct.SourceDirectory = this.tbV1Source.Text;
                xtStruct.DestinationDirectory = this.tbV2Source.Text;

                xtStruct.CopyEmptyTags = this.cbEmptyTags.Checked;
                xtStruct.UpdateArtistTag = this.cbArtist.Checked;
                xtStruct.UpdateCommentTag = this.cbComment.Checked;
                xtStruct.UpdateCopyrightTag = this.cbCopyright.Checked;
                xtStruct.UpdateFadeTag = this.cbFade.Checked;
                xtStruct.UpdateFileName = this.cbFileName.Checked;
                xtStruct.UpdateGameTag = this.cbGame.Checked;
                xtStruct.UpdateGenreTag = this.cbGenre.Checked;
                xtStruct.UpdateLengthTag = this.cbLength.Checked;
                xtStruct.UpdateTitleTag = this.cbTitle.Checked;
                xtStruct.UpdateVolumeTag = this.cbVolume.Checked;
                xtStruct.UpdateXsfByTag = this.cb2sfBy.Checked;
                xtStruct.UpdateYearTag = this.cbYear.Checked;

                base.backgroundWorker_Execute(xtStruct);
            }
        }

        private void btnCheckNone_Click(object sender, EventArgs e)
        {
            this.cb2sfBy.Checked = false;
            this.cbArtist.Checked = false;
            this.cbComment.Checked = false;
            this.cbCopyright.Checked = false;
            this.cbEmptyTags.Checked = false;
            this.cbFade.Checked = false;
            this.cbFileName.Checked = false;
            this.cbGame.Checked = false;
            this.cbGenre.Checked = false;
            this.cbLength.Checked = false;
            this.cbTitle.Checked = false;
            this.cbVolume.Checked = false;
            this.cbYear.Checked = false;
        }
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            this.cb2sfBy.Checked = true;
            this.cbArtist.Checked = true;
            this.cbComment.Checked = true;
            this.cbCopyright.Checked = true;
            this.cbEmptyTags.Checked = true;
            this.cbFade.Checked = true;
            this.cbFileName.Checked = true;
            this.cbGame.Checked = true;
            this.cbGenre.Checked = true;
            this.cbLength.Checked = true;
            this.cbTitle.Checked = true;
            this.cbVolume.Checked = true;
            this.cbYear.Checked = true;
        }
        private void btnDefault_Click(object sender, EventArgs e)
        {
            this.cb2sfBy.Checked = false;
            this.cbArtist.Checked = true;
            this.cbComment.Checked = false;
            this.cbCopyright.Checked = true;
            this.cbEmptyTags.Checked = false;
            this.cbFade.Checked = false;
            this.cbFileName.Checked = false;
            this.cbGame.Checked = true;
            this.cbGenre.Checked = true;
            this.cbLength.Checked = false;
            this.cbTitle.Checked = true;
            this.cbVolume.Checked = false;
            this.cbYear.Checked = true;
        }

        private bool validateInputs()
        {
            bool ret = false;

            ret &= base.checkFolderExists(this.tbV1Source.Text, this.lblV1Folder.Text);
            ret &= base.checkFolderExists(this.tbV2Source.Text, this.lblV2Folder.Text);

            if (this.tbV1Source.Text.Equals(this.tbV2Source.Text))
            {
                MessageBox.Show(String.Format("{0} cannot match {1}", this.lblV1Folder.Text, this.lblV2Folder.Text), "ERROR");
                ret = false;
            }

            return ret;
        }
    }
}
