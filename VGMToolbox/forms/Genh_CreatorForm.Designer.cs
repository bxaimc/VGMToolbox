namespace VGMToolbox.forms
{
    partial class Genh_CreatorForm
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
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.grpSourceFiles = new System.Windows.Forms.GroupBox();
            this.tbSourceDirectory = new System.Windows.Forms.TextBox();
            this.btnBrowseDirectory = new System.Windows.Forms.Button();
            this.grpFormat = new System.Windows.Forms.GroupBox();
            this.comboFormat = new System.Windows.Forms.ComboBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.lblInterleave = new System.Windows.Forms.Label();
            this.tbInterleave = new System.Windows.Forms.TextBox();
            this.lblHeaderSkip = new System.Windows.Forms.Label();
            this.tbHeaderSkip = new System.Windows.Forms.TextBox();
            this.cbHeaderOnly = new System.Windows.Forms.CheckBox();
            this.tbChannels = new System.Windows.Forms.TextBox();
            this.lblChannels = new System.Windows.Forms.Label();
            this.tbFrequency = new System.Windows.Forms.TextBox();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.tbLoopStart = new System.Windows.Forms.TextBox();
            this.lblLoopStart = new System.Windows.Forms.Label();
            this.tbLoopEnd = new System.Windows.Forms.TextBox();
            this.lblLoopEnd = new System.Windows.Forms.Label();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSourceFiles.SuspendLayout();
            this.grpFormat.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 583);
            this.pnlLabels.Size = new System.Drawing.Size(840, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(840, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 506);
            this.tbOutput.Size = new System.Drawing.Size(840, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 486);
            this.pnlButtons.Size = new System.Drawing.Size(840, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(780, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(720, 0);
            // 
            // lbFiles
            // 
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.HorizontalScrollbar = true;
            this.lbFiles.Location = new System.Drawing.Point(6, 43);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFiles.Size = new System.Drawing.Size(254, 303);
            this.lbFiles.TabIndex = 5;
            // 
            // grpSourceFiles
            // 
            this.grpSourceFiles.Controls.Add(this.tbSourceDirectory);
            this.grpSourceFiles.Controls.Add(this.lbFiles);
            this.grpSourceFiles.Controls.Add(this.btnBrowseDirectory);
            this.grpSourceFiles.Location = new System.Drawing.Point(3, 29);
            this.grpSourceFiles.Name = "grpSourceFiles";
            this.grpSourceFiles.Size = new System.Drawing.Size(266, 353);
            this.grpSourceFiles.TabIndex = 6;
            this.grpSourceFiles.TabStop = false;
            this.grpSourceFiles.Text = "Source Files";
            // 
            // tbSourceDirectory
            // 
            this.tbSourceDirectory.Location = new System.Drawing.Point(6, 16);
            this.tbSourceDirectory.Name = "tbSourceDirectory";
            this.tbSourceDirectory.Size = new System.Drawing.Size(216, 20);
            this.tbSourceDirectory.TabIndex = 6;
            this.tbSourceDirectory.TextChanged += new System.EventHandler(this.tbSourceDirectory_TextChanged);
            // 
            // btnBrowseDirectory
            // 
            this.btnBrowseDirectory.Location = new System.Drawing.Point(228, 16);
            this.btnBrowseDirectory.Name = "btnBrowseDirectory";
            this.btnBrowseDirectory.Size = new System.Drawing.Size(32, 20);
            this.btnBrowseDirectory.TabIndex = 9;
            this.btnBrowseDirectory.Text = "...";
            this.btnBrowseDirectory.UseVisualStyleBackColor = true;
            this.btnBrowseDirectory.Click += new System.EventHandler(this.btnBrowseDirectory_Click);
            // 
            // grpFormat
            // 
            this.grpFormat.Controls.Add(this.comboFormat);
            this.grpFormat.Location = new System.Drawing.Point(275, 29);
            this.grpFormat.Name = "grpFormat";
            this.grpFormat.Size = new System.Drawing.Size(258, 41);
            this.grpFormat.TabIndex = 7;
            this.grpFormat.TabStop = false;
            this.grpFormat.Text = "Format";
            // 
            // comboFormat
            // 
            this.comboFormat.FormattingEnabled = true;
            this.comboFormat.Location = new System.Drawing.Point(6, 15);
            this.comboFormat.Name = "comboFormat";
            this.comboFormat.Size = new System.Drawing.Size(246, 21);
            this.comboFormat.TabIndex = 0;
            this.comboFormat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboFormat_KeyPress);
            this.comboFormat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboFormat_KeyDown);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.lblLoopEnd);
            this.grpOptions.Controls.Add(this.tbLoopEnd);
            this.grpOptions.Controls.Add(this.lblLoopStart);
            this.grpOptions.Controls.Add(this.tbLoopStart);
            this.grpOptions.Controls.Add(this.lblFrequency);
            this.grpOptions.Controls.Add(this.tbFrequency);
            this.grpOptions.Controls.Add(this.lblChannels);
            this.grpOptions.Controls.Add(this.tbChannels);
            this.grpOptions.Controls.Add(this.lblInterleave);
            this.grpOptions.Controls.Add(this.tbInterleave);
            this.grpOptions.Controls.Add(this.lblHeaderSkip);
            this.grpOptions.Controls.Add(this.tbHeaderSkip);
            this.grpOptions.Location = new System.Drawing.Point(275, 76);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(258, 283);
            this.grpOptions.TabIndex = 8;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // lblInterleave
            // 
            this.lblInterleave.AutoSize = true;
            this.lblInterleave.Location = new System.Drawing.Point(6, 48);
            this.lblInterleave.Name = "lblInterleave";
            this.lblInterleave.Size = new System.Drawing.Size(54, 13);
            this.lblInterleave.TabIndex = 3;
            this.lblInterleave.Text = "Interleave";
            // 
            // tbInterleave
            // 
            this.tbInterleave.Location = new System.Drawing.Point(152, 45);
            this.tbInterleave.Name = "tbInterleave";
            this.tbInterleave.Size = new System.Drawing.Size(100, 20);
            this.tbInterleave.TabIndex = 2;
            // 
            // lblHeaderSkip
            // 
            this.lblHeaderSkip.AutoSize = true;
            this.lblHeaderSkip.Location = new System.Drawing.Point(6, 22);
            this.lblHeaderSkip.Name = "lblHeaderSkip";
            this.lblHeaderSkip.Size = new System.Drawing.Size(66, 13);
            this.lblHeaderSkip.TabIndex = 1;
            this.lblHeaderSkip.Text = "Header Skip";
            // 
            // tbHeaderSkip
            // 
            this.tbHeaderSkip.Location = new System.Drawing.Point(152, 19);
            this.tbHeaderSkip.Name = "tbHeaderSkip";
            this.tbHeaderSkip.Size = new System.Drawing.Size(100, 20);
            this.tbHeaderSkip.TabIndex = 0;
            // 
            // cbHeaderOnly
            // 
            this.cbHeaderOnly.AutoSize = true;
            this.cbHeaderOnly.Location = new System.Drawing.Point(275, 365);
            this.cbHeaderOnly.Name = "cbHeaderOnly";
            this.cbHeaderOnly.Size = new System.Drawing.Size(117, 17);
            this.cbHeaderOnly.TabIndex = 0;
            this.cbHeaderOnly.Text = "Ouput Header Only";
            this.cbHeaderOnly.UseVisualStyleBackColor = true;
            // 
            // tbChannels
            // 
            this.tbChannels.Location = new System.Drawing.Point(152, 71);
            this.tbChannels.Name = "tbChannels";
            this.tbChannels.Size = new System.Drawing.Size(100, 20);
            this.tbChannels.TabIndex = 4;
            // 
            // lblChannels
            // 
            this.lblChannels.AutoSize = true;
            this.lblChannels.Location = new System.Drawing.Point(6, 74);
            this.lblChannels.Name = "lblChannels";
            this.lblChannels.Size = new System.Drawing.Size(51, 13);
            this.lblChannels.TabIndex = 5;
            this.lblChannels.Text = "Channels";
            // 
            // tbFrequency
            // 
            this.tbFrequency.Location = new System.Drawing.Point(152, 97);
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(100, 20);
            this.tbFrequency.TabIndex = 6;
            // 
            // lblFrequency
            // 
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.Location = new System.Drawing.Point(6, 100);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(57, 13);
            this.lblFrequency.TabIndex = 7;
            this.lblFrequency.Text = "Frequency";
            // 
            // tbLoopStart
            // 
            this.tbLoopStart.Location = new System.Drawing.Point(152, 135);
            this.tbLoopStart.Name = "tbLoopStart";
            this.tbLoopStart.Size = new System.Drawing.Size(100, 20);
            this.tbLoopStart.TabIndex = 8;
            // 
            // lblLoopStart
            // 
            this.lblLoopStart.AutoSize = true;
            this.lblLoopStart.Location = new System.Drawing.Point(6, 138);
            this.lblLoopStart.Name = "lblLoopStart";
            this.lblLoopStart.Size = new System.Drawing.Size(105, 13);
            this.lblLoopStart.TabIndex = 9;
            this.lblLoopStart.Text = "Loop Start (Samples)";
            // 
            // tbLoopEnd
            // 
            this.tbLoopEnd.Location = new System.Drawing.Point(152, 161);
            this.tbLoopEnd.Name = "tbLoopEnd";
            this.tbLoopEnd.Size = new System.Drawing.Size(100, 20);
            this.tbLoopEnd.TabIndex = 10;
            // 
            // lblLoopEnd
            // 
            this.lblLoopEnd.AutoSize = true;
            this.lblLoopEnd.Location = new System.Drawing.Point(6, 164);
            this.lblLoopEnd.Name = "lblLoopEnd";
            this.lblLoopEnd.Size = new System.Drawing.Size(102, 13);
            this.lblLoopEnd.TabIndex = 11;
            this.lblLoopEnd.Text = "Loop End (Samples)";
            // 
            // Genh_CreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 624);
            this.Controls.Add(this.grpFormat);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpSourceFiles);
            this.Controls.Add(this.cbHeaderOnly);
            this.Name = "Genh_CreatorForm";
            this.Text = "Genh_CreatorForm";
            this.Controls.SetChildIndex(this.cbHeaderOnly, 0);
            this.Controls.SetChildIndex(this.grpSourceFiles, 0);
            this.Controls.SetChildIndex(this.grpOptions, 0);
            this.Controls.SetChildIndex(this.grpFormat, 0);
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpSourceFiles.ResumeLayout(false);
            this.grpSourceFiles.PerformLayout();
            this.grpFormat.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.GroupBox grpSourceFiles;
        private System.Windows.Forms.GroupBox grpFormat;
        private System.Windows.Forms.ComboBox comboFormat;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.TextBox tbSourceDirectory;
        private System.Windows.Forms.Button btnBrowseDirectory;
        private System.Windows.Forms.CheckBox cbHeaderOnly;
        private System.Windows.Forms.Label lblHeaderSkip;
        private System.Windows.Forms.TextBox tbHeaderSkip;
        private System.Windows.Forms.Label lblInterleave;
        private System.Windows.Forms.TextBox tbInterleave;
        private System.Windows.Forms.Label lblChannels;
        private System.Windows.Forms.TextBox tbChannels;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.Label lblLoopStart;
        private System.Windows.Forms.TextBox tbLoopStart;
        private System.Windows.Forms.TextBox tbLoopEnd;
        private System.Windows.Forms.Label lblLoopEnd;
    }
}