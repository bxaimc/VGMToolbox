namespace VGMToolbox.forms.xsf
{
    partial class SsfMakeAdvancedForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbDriverFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbDspProgram = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSequenceFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbToneData = new System.Windows.Forms.ListBox();
            this.grpFiles = new System.Windows.Forms.GroupBox();
            this.pb68000Memory = new System.Windows.Forms.PictureBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb68000Memory)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 282);
            this.pnlLabels.Size = new System.Drawing.Size(755, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(755, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 205);
            this.tbOutput.Size = new System.Drawing.Size(755, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 185);
            this.pnlButtons.Size = new System.Drawing.Size(755, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(695, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(635, 0);
            // 
            // tbDriverFile
            // 
            this.tbDriverFile.AllowDrop = true;
            this.tbDriverFile.Location = new System.Drawing.Point(84, 13);
            this.tbDriverFile.Name = "tbDriverFile";
            this.tbDriverFile.Size = new System.Drawing.Size(229, 20);
            this.tbDriverFile.TabIndex = 5;
            this.tbDriverFile.TextChanged += new System.EventHandler(this.tbDriverFile_TextChanged);
            this.tbDriverFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbDriverFile_DragDrop);
            this.tbDriverFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbDriverFile_DragEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Driver";
            // 
            // tbDspProgram
            // 
            this.tbDspProgram.AllowDrop = true;
            this.tbDspProgram.Location = new System.Drawing.Point(84, 39);
            this.tbDspProgram.Name = "tbDspProgram";
            this.tbDspProgram.Size = new System.Drawing.Size(229, 20);
            this.tbDspProgram.TabIndex = 7;
            this.tbDspProgram.TextChanged += new System.EventHandler(this.tbDspProgram_TextChanged);
            this.tbDspProgram.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbDspProgram_DragDrop);
            this.tbDspProgram.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbDspProgram_DragEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "DSP Program";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Sequence File";
            // 
            // tbSequenceFile
            // 
            this.tbSequenceFile.AllowDrop = true;
            this.tbSequenceFile.Location = new System.Drawing.Point(84, 65);
            this.tbSequenceFile.Name = "tbSequenceFile";
            this.tbSequenceFile.Size = new System.Drawing.Size(229, 20);
            this.tbSequenceFile.TabIndex = 10;
            this.tbSequenceFile.TextChanged += new System.EventHandler(this.tbSequenceFile_TextChanged);
            this.tbSequenceFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSequenceFile_DragDrop);
            this.tbSequenceFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbSequenceFile_DragEnter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Tone Data";
            // 
            // lbToneData
            // 
            this.lbToneData.FormattingEnabled = true;
            this.lbToneData.Location = new System.Drawing.Point(84, 98);
            this.lbToneData.Name = "lbToneData";
            this.lbToneData.Size = new System.Drawing.Size(229, 95);
            this.lbToneData.TabIndex = 12;
            // 
            // grpFiles
            // 
            this.grpFiles.Controls.Add(this.pb68000Memory);
            this.grpFiles.Controls.Add(this.label1);
            this.grpFiles.Controls.Add(this.label4);
            this.grpFiles.Controls.Add(this.tbDriverFile);
            this.grpFiles.Controls.Add(this.tbDspProgram);
            this.grpFiles.Controls.Add(this.lbToneData);
            this.grpFiles.Controls.Add(this.label3);
            this.grpFiles.Controls.Add(this.label2);
            this.grpFiles.Controls.Add(this.tbSequenceFile);
            this.grpFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFiles.Location = new System.Drawing.Point(0, 23);
            this.grpFiles.Name = "grpFiles";
            this.grpFiles.Size = new System.Drawing.Size(755, 202);
            this.grpFiles.TabIndex = 13;
            this.grpFiles.TabStop = false;
            this.grpFiles.Text = "Files";
            // 
            // pb68000Memory
            // 
            this.pb68000Memory.Location = new System.Drawing.Point(389, 13);
            this.pb68000Memory.Name = "pb68000Memory";
            this.pb68000Memory.Size = new System.Drawing.Size(140, 180);
            this.pb68000Memory.TabIndex = 13;
            this.pb68000Memory.TabStop = false;
            // 
            // SsfMakeAdvancedForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 323);
            this.Controls.Add(this.grpFiles);
            this.Name = "SsfMakeAdvancedForm";
            this.Text = "SsfMakeAdvancedForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpFiles, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpFiles.ResumeLayout(false);
            this.grpFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb68000Memory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDriverFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbDspProgram;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSequenceFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbToneData;
        private System.Windows.Forms.GroupBox grpFiles;
        private System.Windows.Forms.PictureBox pb68000Memory;
    }
}