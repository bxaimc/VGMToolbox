using System;
using System.Windows.Forms;

using VGMToolbox.format;
using VGMToolbox.plugin;
using VGMToolbox.tools.stream;

namespace VGMToolbox.forms.stream
{
    public partial class MpegDemuxForm : AVgmtForm
    {
        public MpegDemuxForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = "Video Demultiplexer";

            // hide the DoTask button since this is a drag and drop form
            this.btnDoTask.Hide();            
            
            InitializeComponent();

            this.tbOutput.Text = "Demultiplex streams from movies." + Environment.NewLine;
            this.tbOutput.Text += "- Currently supported formats: DVD Video, MO (Wii Only), MPEG1, MPEG2, PAM, PMF, PSS, SFD, USM, XMV" + Environment.NewLine;
            this.tbOutput.Text += "- If the MPEG does not work for your file, be sure to try DVD Video, since it can handle specialized audio types." + Environment.NewLine;
            this.tbOutput.Text += "- MKVMerge can be used to add raw .264 data to a container file for playback." + Environment.NewLine;
            this.tbOutput.Text += "- Adding header to raw XMV video not currently supported." + Environment.NewLine;


            this.initializeFormatList();
        }

        private void initializeFormatList()
        {
            this.comboFormat.Items.Clear();                        
            this.comboFormat.Items.Add("DVD Video (VOB)");
            this.comboFormat.Items.Add("MO (Mobiclip)");
            this.comboFormat.Items.Add("MPEG");
            this.comboFormat.Items.Add("PAM (PlayStation Advanced Movie)");
            this.comboFormat.Items.Add("PMF (PSP Movie Format)");
            this.comboFormat.Items.Add("PSS (PlayStation Stream)");
            this.comboFormat.Items.Add("SFD (CRI Sofdec Video)");
            this.comboFormat.Items.Add("USM (CRI Movie 2)");
            this.comboFormat.Items.Add("XMV (Xbox Media Video)");

            this.comboFormat.SelectedItem = "DVD Video (VOB)";
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new MpegDemuxWorker();
        }
        protected override string getCancelMessage()
        {
            return "Demultiplexing Streams...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Demultiplexing Streams...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Demultiplexing Streams...Begin";
        }

        private void MpegDemuxForm_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        private void MpegDemuxForm_DragDrop(object sender, DragEventArgs e)
        {
            MpegDemuxWorker.MpegDemuxStruct taskStruct = new MpegDemuxWorker.MpegDemuxStruct();

            // paths
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            taskStruct.SourcePaths = s;

            // format
            taskStruct.SourceFormat = this.comboFormat.SelectedItem.ToString();
            
            // options
            taskStruct.AddHeader = this.cbAddHeader.Checked;
            taskStruct.ExtractAudio = (this.rbExtractAudioAndVideo.Checked || 
                                       this.rbExtractAudioOnly.Checked);
            taskStruct.ExtractVideo = (this.rbExtractAudioAndVideo.Checked ||
                                       this.rbExtractVideoOnly.Checked);

            base.backgroundWorker_Execute(taskStruct);
        }

        private void comboFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboFormat.SelectedItem.ToString())
            {
                case "MO (Mobiclip)":
                case "PAM (PlayStation Advanced Movie)":
                case "PMF (PSP Movie Format)":
                case "XMV (Xbox Media Video)":                
                    this.cbAddHeader.Enabled = true;
                    this.cbAddHeader.Checked = true;
                    break;
                default:
                    this.cbAddHeader.Checked = false;
                    this.cbAddHeader.Enabled = false;
                    break;
            }
        }
    }
}
