using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

using VGMToolbox.auditing;

namespace VGMToolbox.forms
{
    public partial class Auditing_DatafileCreatorForm : VgmtForm
    {
        DatafileCreatorWorker datCreator;
        
        public Auditing_DatafileCreatorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            // set title
            this.lblTitle.Text = ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_Title"];
            this.btnDoTask.Text = ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_DoTaskButton"];
            this.tbOutput.Text = ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_IntroText"];

            InitializeComponent();

            this.gbHeader.Text = ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_GroupHeader"];
            this.gbSourceDestPaths.Text = ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_GroupSourceDestination"];

            this.lblHeaderName.Text = ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_LblHeaderName"];
        }

        private void btnDatCreator_BuildDat_Click(object sender, EventArgs e)
        {
            base.initializeProcessing();

            if (checkDatafileCreatorInputs())
            {
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_MessageBegin"];

                DatafileCreatorWorker.GetGameParamsStruct vGetGameParamsStruct = new DatafileCreatorWorker.GetGameParamsStruct();
                vGetGameParamsStruct.pDir = tbDatCreator_SourceFolder.Text;
                vGetGameParamsStruct.pOutputMessage = "";
                vGetGameParamsStruct.totalFiles = Directory.GetFiles(tbDatCreator_SourceFolder.Text, "*.*", SearchOption.AllDirectories).Length;

                datCreator = new DatafileCreatorWorker();
                datCreator.ProgressChanged += backgroundWorker_ReportProgress;
                datCreator.RunWorkerCompleted += datafileCreatorWorker_WorkComplete;
                datCreator.RunWorkerAsync(vGetGameParamsStruct);
            }
        }
        
        private void datafileCreatorWorker_WorkComplete(object sender,
                                    RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_MessageCancel"];
                tbOutput.Text +=
                    ConfigurationSettings.AppSettings["Form_Global_OperationCancelled"];
            }
            else
            {
                lblProgressLabel.Text = String.Empty;

                datafile dataFile = new datafile();
                dataFile.header = DatafileCreatorWorker.buildHeader(tbDatCreator_Author.Text, tbDatCreator_Category.Text,
                    tbDatCreator_Comment.Text, tbDatCreator_Date.Text, tbDatCreator_Description.Text,
                    tbDatCreator_Email.Text, tbDatCreator_Homepage.Text, tbDatCreator_Name.Text,
                    tbDatCreator_Url.Text, tbDatCreator_Version.Text);

                dataFile.game = (VGMToolbox.auditing.game[])e.Result;

                XmlSerializer serializer = new XmlSerializer(dataFile.GetType());

                TextWriter textWriter = new StreamWriter(tbDatCreator_OutputDat.Text);
                serializer.Serialize(textWriter, dataFile);
                textWriter.Close();
                textWriter.Dispose();

                toolStripStatusLabel1.Text = 
                    ConfigurationSettings.AppSettings["Form_AuditDatafileCreator_MessageComplete"];
            }

            // update node color
            setNodeAsComplete();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (datCreator != null && datCreator.IsBusy)
            {
                tbOutput.Text += ConfigurationSettings.AppSettings["Form_Global_CancelPending"];
                datCreator.CancelAsync();
                this.errorFound = true;
            }
        }

        private void btnDatCreator_BrowseSource_Click(object sender, EventArgs e)
        {
            tbDatCreator_SourceFolder.Text = base.browseForFolder(sender, e);
        }

        private void btnDatCreator_BrowseDestination_Click(object sender, EventArgs e)
        {
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.DefaultExt = "xml";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.Filter = "XML File (*.xml)|*.xml|Datafile (*.dat)| *.dat";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbDatCreator_OutputDat.Text = saveFileDialog1.FileName;
            }
        }

        private bool checkDatafileCreatorInputs()
        {
            bool ret = true;

            if (!(checkTextBox(tbDatCreator_Name.Text, "Datafile Name") &&
                checkTextBox(tbDatCreator_Description.Text, "Description") &&
                checkTextBox(tbDatCreator_SourceFolder.Text, "Source Directory") &&
                checkTextBox(tbDatCreator_OutputDat.Text, "Destination Datafile") &&
                checkFolderExists(tbDatCreator_SourceFolder.Text, "Source Directory")))
            {
                ret = false;
            }

            return ret;
        }
    }
}
