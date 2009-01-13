using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class EmbeddedTagsUpdateForm : Form
    {
        Constants.NodeTagStruct nodeTagInfo;
        IEmbeddedTagsFormat vgmData;

        public EmbeddedTagsUpdateForm(Constants.NodeTagStruct pNts)
        {
            nodeTagInfo = pNts;
            
            InitializeComponent();

            loadCurrentTagInformation();
        }

        private void loadCurrentTagInformation()
        {
            using (FileStream fs = 
                File.Open(this.nodeTagInfo.filePath, FileMode.Open, FileAccess.Read))
            {
                this.vgmData =
                    (IEmbeddedTagsFormat)Activator.CreateInstance(Type.GetType(this.nodeTagInfo.objectType));
                this.vgmData.Initialize(fs);

                this.tbName.Text = this.vgmData.GetSongNameAsText();
                this.tbArtist.Text = this.vgmData.GetArtistAsText();
                this.tbCopyright.Text = this.vgmData.GetCopyrightAsText();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.vgmData.UpdateSongName(this.nodeTagInfo.filePath, this.tbName.Text);
            this.vgmData.UpdateArtist(this.nodeTagInfo.filePath, this.tbArtist.Text);
            this.vgmData.UpdateCopyright(this.nodeTagInfo.filePath, this.tbCopyright.Text);

            MessageBox.Show(String.Format("Tags for \"{0}\" have been updated.  Changes will not be displayed in the tree until you add the files again.", Path.GetFileName(this.nodeTagInfo.filePath)));
            this.Close();
            this.Dispose();
        }
    }
}
