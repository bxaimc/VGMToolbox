using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.format.sdat;
using VGMToolbox.format.util;
using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;
using VGMToolbox.util;

namespace VGMToolbox.forms
{
    public partial class Xsf_Mk2sfForm : AVgmtForm
    {
        public Xsf_Mk2sfForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            string testpackPath =
                Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), 
                Mk2sfWorker.TESTPACK_PATH);
            
            InitializeComponent();

            this.lblTitle.Text = "Make 2SFs";
            this.btnDoTask.Text = "Make 2SFs";

            this.tbOutput.Text = "- 2sfTool.exe written by UNKNOWNFILE and CaitSith2." + Environment.NewLine;
            this.tbOutput.Text +=
                String.Format("- 'testpack.nds' is not included and must be downloaded and placed in <{0}>", Path.GetDirectoryName(testpackPath));
        }
        
        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            this.tbSource.Text = base.browseForFile(sender, e);
        }
        private void btnDoTask_Click(object sender, EventArgs e)
        {
            ArrayList allowedSequences = this.GetAllowedSequences();

            if (CheckSdatPathAndOutputDir() && CheckForTestPackNds())
            {
                Mk2sfWorker.Mk2sfStruct mk2sfStruct = new Mk2sfWorker.Mk2sfStruct();
                mk2sfStruct.AllowedSequences = allowedSequences;
                mk2sfStruct.DestinationFolder = tbOutputPath.Text;
                mk2sfStruct.SourcePath = tbSource.Text;

                mk2sfStruct.TagArtist = tbArtist.Text;
                mk2sfStruct.TagCopyright = tbCopyright.Text;
                mk2sfStruct.TagYear = tbYear.Text;
                mk2sfStruct.TagGame = tbGame.Text;

                base.backgroundWorker_Execute(mk2sfStruct);
            }
        }

        private void tbSource_TextChanged(object sender, EventArgs e)
        {
            this.LoadComboBox(this.tbSource.Text);
        }

        private void LoadComboBox(string pSourcePath)
        {
            Smap smap = SdatUtil.GetSmapFromSdat(pSourcePath);
            DataGridViewRow row = new DataGridViewRow();

            this.dataGridSseq.Rows.Clear();

            if ((smap.SseqSection != null) && (smap.SseqSection.Length > 0))
            {
                foreach (Smap.SmapSeqStruct s in smap.SseqSection)
                {
                    row = new DataGridViewRow();
                    row.CreateCells(this.dataGridSseq);

                    row.Cells[1].Value = s.number.ToString();

                    if (!String.IsNullOrEmpty(s.name))
                    {
                        row.Cells[0].Value = true;
                        row.Cells[2].Value = s.fileID.ToString();
                        row.Cells[3].Value = s.size.ToString();
                        row.Cells[4].Value = s.name.ToString();
                        row.Cells[5].Value = s.bnk.ToString();
                        row.Cells[6].Value = s.vol.ToString();
                        row.Cells[7].Value = s.cpr.ToString();
                        row.Cells[8].Value = s.ppr.ToString();
                        row.Cells[9].Value = s.ply.ToString();
                    }
                    else
                    {
                        row.Cells[0].Value = false;
                    }

                    this.dataGridSseq.Rows.Add(row);
                }
            }
        }
        private ArrayList GetAllowedSequences()
        {
            ArrayList allowedSequences = new ArrayList();

            foreach (DataGridViewRow r in dataGridSseq.Rows)
            {
                if (((bool)r.Cells[0].Value) &&
                    (r.Cells[4].Value != null) &&
                    (!String.IsNullOrEmpty((string)r.Cells[4].Value)))
                {
                    allowedSequences.Add(int.Parse((string)r.Cells[1].Value));
                }
            }

            return allowedSequences;
        }
        private static bool CheckForTestPackNds()
        {
            bool ret = true;
            string testpackPath = 
                Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Mk2sfWorker.TESTPACK_PATH);

            if (!File.Exists(testpackPath))
            {
                ret = false;
                MessageBox.Show(String.Format("{0} not found.  Please put {0} in <{1}>",
                    Path.GetFileName(testpackPath), Path.GetDirectoryName(testpackPath)),
                    String.Format("MISSING <{0}>", Path.GetFileName(testpackPath)));
            }
            else
            {
                using (FileStream fs = File.OpenRead(testpackPath))
                {
                    if (!ChecksumUtil.GetCrc32OfFullFile(fs).Equals(Mk2sfWorker.TESTPACK_CRC32))
                    {
                        ret = false;
                        MessageBox.Show(String.Format("Invalid CRC32 for {0}.  Please put {0} in <{1}>.  The expected CRC32 is {2}.",
                            Path.GetFileName(testpackPath), Path.GetDirectoryName(testpackPath), Mk2sfWorker.TESTPACK_CRC32),
                            String.Format("INVALID CRC32 FOR <{0}>", Path.GetFileName(testpackPath)));                    
                    }
                }                
            }

            return ret;
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            this.tbOutputPath.Text = base.browseForFolder(sender, e);
        }


        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new Mk2sfWorker();
        }
        protected override string getCancelMessage()
        {
            return "Make 2SFs...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Make 2SFs...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Make 2SFs...Begin";
        }

        private bool CheckSdatPathAndOutputDir()
        {
            bool ret = true;
            
            string sdatSourceDirectory = Path.GetDirectoryName(Path.GetFullPath(tbSource.Text));

            if (sdatSourceDirectory.Equals(tbOutputPath.Text.Trim()))
            {
                ret = false;
                MessageBox.Show("Output folder cannot be the same as the folder containing the source SDAT.",
                    "INVALID OUTPUT FOLDER");             
            }

            return ret;
        }
    }
}
