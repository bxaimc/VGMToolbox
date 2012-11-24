using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using VGMToolbox.plugin;
using VGMToolbox.tools.xsf;
using VGMToolbox.util;

namespace VGMToolbox.forms.xsf
{
    public partial class SsfMakeAdvancedForm : VgmtForm
    {        
        public const float TOTAL_68000_MEMORY = 0x80000;
        public const float RECT_WIDTH = 140;
        public const float RECT_HEIGHT = 180;

        public static readonly long DATA_START_OFFSET = 0xB000;
        public static readonly float DATA_START_HEIGHT = (DATA_START_OFFSET / TOTAL_68000_MEMORY) * RECT_HEIGHT;

        private Graphics graphicsRenderer;
        private Pen blackPen;

        public SsfMakeAdvancedForm(TreeNode pTreeNode)
            : base(pTreeNode)
        {
            this.lblTitle.Text = "ssfmake Advanced Front End (note: Python must be installed and in your PATH.)";
            this.btnDoTask.Text = "Make SSFs";
            
            InitializeComponent();
            
            // setup colors
            this.tbDriverFile.BackColor = Color.DarkGreen;
            this.tbSequenceFile.BackColor = Color.Green;
            this.tbDspProgram.BackColor = Color.GreenYellow;


            // init memory
            this.InitializeMemoryPictureBox();
            this.RenderMemoryGrid();
        }

        protected void InitializeMemoryPictureBox()
        {
            this.pb68000Memory.Image = new Bitmap((int)RECT_WIDTH, (int)RECT_HEIGHT);
            this.graphicsRenderer = Graphics.FromImage(this.pb68000Memory.Image);
            this.blackPen = new Pen(Color.Black, 1);
        }

        protected void RenderMemoryGrid()
        {
            long? driverFileSize;
            long? seqFileSize;
            long? dspFileSize;

            long currentOffset = DATA_START_OFFSET;
            float currentHeight = DATA_START_HEIGHT;
            float seqHeight;
            float dspHeight;

            // draw white background
            this.graphicsRenderer.FillRectangle(Brushes.White, 0, 0, RECT_WIDTH, RECT_HEIGHT);

            // render Driver
            if (!String.IsNullOrEmpty(this.tbDriverFile.Text))
            {
                driverFileSize = FileUtil.GetFileSize(this.tbDriverFile.Text);

                if (driverFileSize != null)
                {
                    this.graphicsRenderer.FillRectangle(Brushes.DarkGreen, 0, 0, RECT_WIDTH, this.GetRectangleHeight((float)driverFileSize));
                }
            }

            // render SEQ
            if (!String.IsNullOrEmpty(this.tbSequenceFile.Text))
            {
                seqFileSize = FileUtil.GetFileSize(this.tbSequenceFile.Text);

                if (seqFileSize != null)
                {
                    seqHeight = this.GetRectangleHeight((float)seqFileSize);
                    this.graphicsRenderer.FillRectangle(Brushes.Green, 0, currentHeight, RECT_WIDTH, seqHeight);

                    // update current offset/hieght
                    currentOffset += (long)seqFileSize;
                    currentHeight += seqHeight;
                }
            }

            // render DSP
            if (!String.IsNullOrEmpty(this.tbDspProgram.Text))
            {
                dspFileSize = FileUtil.GetFileSize(this.tbDspProgram.Text);

                if (dspFileSize != null)
                {
                    currentOffset = MathUtil.RoundUpToByteAlignment((long)currentOffset, 0x2000);
                    currentHeight = GetRectangleHeight(currentOffset);

                    dspHeight = this.GetRectangleHeight((float)dspFileSize);
                    this.graphicsRenderer.FillRectangle(Brushes.GreenYellow, 0, currentHeight, RECT_WIDTH, dspHeight);

                    // update current offset/height
                    currentOffset += (long)dspFileSize;
                    currentHeight += dspHeight;
                }
            }

            //render outline
            this.graphicsRenderer.DrawRectangle(this.blackPen, 0, 0, RECT_WIDTH - this.blackPen.Width, RECT_HEIGHT - this.blackPen.Width);

            // repaint
            this.pb68000Memory.Refresh();
        }

        protected float GetRectangleHeight(float value)
        {
            return (value / TOTAL_68000_MEMORY) * RECT_HEIGHT;
        }

        protected override IVgmtBackgroundWorker getBackgroundWorker()
        {
            return new SsfMakeWorker();
        }
        protected override string getCancelMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageCancel"];
        }
        protected override string getCompleteMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageComplete"];
        }
        protected override string getBeginMessage()
        {
            return ConfigurationSettings.AppSettings["Form_SsfMakeFE_MessageBegin"];
        }

        protected string getDroppedFile(DragEventArgs e)
        {
            string droppedFile = null;
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if ((s.Length == 1) && (File.Exists(s[0])))
            {
                droppedFile = s[0];
            }

            return droppedFile;
        }
        private void tbDriverFile_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void tbDriverFile_DragDrop(object sender, DragEventArgs e)
        {
            this.tbDriverFile.Text = this.getDroppedFile(e);            
        }
        private void tbDriverFile_TextChanged(object sender, EventArgs e)
        {
            this.RenderMemoryGrid();
        }

        private void tbSequenceFile_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void tbSequenceFile_DragDrop(object sender, DragEventArgs e)
        {
            this.tbSequenceFile.Text = this.getDroppedFile(e);            
        }
        private void tbSequenceFile_TextChanged(object sender, EventArgs e)
        {
            this.RenderMemoryGrid();
        }

        private void tbDspProgram_DragEnter(object sender, DragEventArgs e)
        {
            base.doDragEnter(sender, e);
        }
        private void tbDspProgram_DragDrop(object sender, DragEventArgs e)
        {
            this.tbDspProgram.Text = this.getDroppedFile(e);
        }
        private void tbDspProgram_TextChanged(object sender, EventArgs e)
        {
            this.RenderMemoryGrid();
        }

        
    }
}
