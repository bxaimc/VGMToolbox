using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.stream;

namespace VGMToolbox.forms.stream
{
    public partial class PosFileCreatorForm : AVgmtForm
    {
        public PosFileCreatorForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = ".POS File Maker";
            this.tbOutput.Text = "Create .POS files for looping .WAV files.";

            InitializeComponent();

            this.tbOutputFileMask.Text = "$B.$E.WAV";

            this.rbLoopEnd.Checked = true;

            this.rbLoopShiftWavCompare.Checked = true;
            this.cbLoopShiftPredictShift.Checked = true;
        }





        //----------------------------
        // background worker defaults
        //----------------------------
        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new PosFileCreatorWorker();
        }
        protected override string getCancelMessage()
        {
            return "Creating POS File...Cancelled";
        }
        protected override string getCompleteMessage()
        {
            return "Creating POS File...Complete";
        }
        protected override string getBeginMessage()
        {
            return "Creating POS File...Begin";
        }

        private void PosFileCreatorForm_DragDrop(object sender, DragEventArgs e)
        {
            PosFileCreatorWorker.PosFileCreatorStruct taskStruct = new PosFileCreatorWorker.PosFileCreatorStruct();

            // paths
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            taskStruct.SourcePaths = s;

            // public string OutputFolder { set; get; }
            taskStruct.OutputFileMask = this.tbOutputFileMask.Text;

            // Loop Start
            taskStruct.DoLoopStartStatic = this.rbLoopStartStatic.Checked;
            taskStruct.DoLoopStartOffset = this.rbLoopStartOffset.Checked;
            taskStruct.DoLoopStartRiffOffset = this.rbLoopStartRiffOffset.Checked;

            taskStruct.LoopStartStaticValue = this.tbLoopStartStatic.Text;
            taskStruct.LoopStartCalculatingOffset = this.LoopStartCalculatingOffsetDescriptionControl.GetOffsetValues();
            taskStruct.LoopStartRiffCalculatingOffset = this.LoopStartRiffCalculatingOffsetDescriptionControl.GetOffsetValues();

            // Loop End
            taskStruct.LoopEndIsLoopEnd = this.rbLoopEnd.Checked;
            taskStruct.LoopEndIsLoopLength = this.rbLoopLength.Checked;
            
            taskStruct.DoLoopEndStatic = this.rbLoopEndStatic.Checked;
            taskStruct.DoLoopEndOffset = this.rbLoopEndOffset.Checked;
            taskStruct.DoLoopEndRiffOffset = this.rbLoopEndRiffOffset.Checked;

            taskStruct.LoopEndStaticValue = this.tbLoopEndStatic.Text;
            taskStruct.LoopEndCalculatingOffset = this.LoopEndCalculatingOffsetDescriptionControl.GetOffsetValues();
            taskStruct.LoopEndRiffCalculatingOffset = this.LoopEndRiffCalculatingOffsetDescriptionControl.GetOffsetValues();

            // Loop Shift
            taskStruct.DoStaticLoopShift = this.rbLoopShiftStatic.Checked;
            taskStruct.DoLoopShiftWavCompare = this.rbLoopShiftWavCompare.Checked;
            taskStruct.PredictLoopShiftForBatch = this.cbLoopShiftPredictShift.Checked;
            taskStruct.LoopStartStaticValue = this.tbLoopShiftStatic.Text;

            // Other Options
            taskStruct.CreateM3u = this.cbOptionsCreateM3u.Checked;

            // Execute Worker
            base.backgroundWorker_Execute(taskStruct);
        }

        private void PosFileCreatorForm_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }

        // radio buttons
        private void doLoopShiftRadios()
        {
            if (this.rbLoopShiftStatic.Checked)
            {
                this.tbLoopShiftStatic.Enabled = true;
                this.tbLoopShiftStatic.ReadOnly = false;

                this.cbLoopShiftPredictShift.Enabled = false;
                this.cbLoopShiftPredictShift.Checked = false;
            }
            else if (this.rbLoopShiftWavCompare.Checked)
            {
                this.tbLoopShiftStatic.Enabled = false;
                this.tbLoopShiftStatic.ReadOnly = true;

                this.cbLoopShiftPredictShift.Enabled = true;            
            }
        }        
        private void rbLoopShiftStatic_CheckedChanged(object sender, EventArgs e)
        {
            this.doLoopShiftRadios();
        }
        private void rbLoopShiftWavCompare_CheckedChanged(object sender, EventArgs e)
        {
            this.doLoopShiftRadios();
        }



        //------------
        // Validation
        //------------
    }
}
