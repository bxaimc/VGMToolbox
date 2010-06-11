namespace VGMToolbox.forms.stream
{
    partial class XmaConvertForm
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
            this.grpPresets = new System.Windows.Forms.GroupBox();
            this.grpOutputOptions = new System.Windows.Forms.GroupBox();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.grpRiffHeaderOptions = new System.Windows.Forms.GroupBox();
            this.comboRiffChannels = new System.Windows.Forms.ComboBox();
            this.lblRiffChannels = new System.Windows.Forms.Label();
            this.comboRiffFrequency = new System.Windows.Forms.ComboBox();
            this.lblRiffFrequency = new System.Windows.Forms.Label();
            this.grpXmaParseOptions = new System.Windows.Forms.GroupBox();
            this.tbXmaParseBlockSize = new System.Windows.Forms.TextBox();
            this.lblXmaParseBlockSize = new System.Windows.Forms.Label();
            this.tbXmaParseStartOffset = new System.Windows.Forms.TextBox();
            this.lblXmaParseOffset = new System.Windows.Forms.Label();
            this.comboXmaParseInputType = new System.Windows.Forms.ComboBox();
            this.lblXmaParseInputType = new System.Windows.Forms.Label();
            this.groupOtherOptions = new System.Windows.Forms.GroupBox();
            this.cbShowAllExeOutput = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.grpRiffHeaderOptions.SuspendLayout();
            this.grpXmaParseOptions.SuspendLayout();
            this.groupOtherOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 422);
            this.pnlLabels.Size = new System.Drawing.Size(789, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(789, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 345);
            this.tbOutput.Size = new System.Drawing.Size(789, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 325);
            this.pnlButtons.Size = new System.Drawing.Size(789, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(729, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(669, 0);
            // 
            // grpPresets
            // 
            this.grpPresets.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPresets.Location = new System.Drawing.Point(0, 23);
            this.grpPresets.Name = "grpPresets";
            this.grpPresets.Size = new System.Drawing.Size(789, 40);
            this.grpPresets.TabIndex = 5;
            this.grpPresets.TabStop = false;
            this.grpPresets.Text = "Presets";
            // 
            // grpOutputOptions
            // 
            this.grpOutputOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpOutputOptions.Location = new System.Drawing.Point(0, 63);
            this.grpOutputOptions.Name = "grpOutputOptions";
            this.grpOutputOptions.Size = new System.Drawing.Size(789, 57);
            this.grpOutputOptions.TabIndex = 6;
            this.grpOutputOptions.TabStop = false;
            this.grpOutputOptions.Text = "Output Options";
            // 
            // pnlOptions
            // 
            this.pnlOptions.AutoScroll = true;
            this.pnlOptions.Controls.Add(this.groupOtherOptions);
            this.pnlOptions.Controls.Add(this.grpRiffHeaderOptions);
            this.pnlOptions.Controls.Add(this.grpXmaParseOptions);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Location = new System.Drawing.Point(0, 120);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(789, 205);
            this.pnlOptions.TabIndex = 7;
            // 
            // grpRiffHeaderOptions
            // 
            this.grpRiffHeaderOptions.Controls.Add(this.comboRiffChannels);
            this.grpRiffHeaderOptions.Controls.Add(this.lblRiffChannels);
            this.grpRiffHeaderOptions.Controls.Add(this.comboRiffFrequency);
            this.grpRiffHeaderOptions.Controls.Add(this.lblRiffFrequency);
            this.grpRiffHeaderOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpRiffHeaderOptions.Location = new System.Drawing.Point(0, 46);
            this.grpRiffHeaderOptions.Name = "grpRiffHeaderOptions";
            this.grpRiffHeaderOptions.Size = new System.Drawing.Size(789, 48);
            this.grpRiffHeaderOptions.TabIndex = 1;
            this.grpRiffHeaderOptions.TabStop = false;
            this.grpRiffHeaderOptions.Text = "RIFF Header Options";
            // 
            // comboRiffChannels
            // 
            this.comboRiffChannels.FormattingEnabled = true;
            this.comboRiffChannels.Location = new System.Drawing.Point(220, 19);
            this.comboRiffChannels.Name = "comboRiffChannels";
            this.comboRiffChannels.Size = new System.Drawing.Size(79, 21);
            this.comboRiffChannels.TabIndex = 3;
            this.comboRiffChannels.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboRiffChannels_KeyPress);
            this.comboRiffChannels.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboRiffChannels_KeyDown);
            // 
            // lblRiffChannels
            // 
            this.lblRiffChannels.AutoSize = true;
            this.lblRiffChannels.Location = new System.Drawing.Point(163, 22);
            this.lblRiffChannels.Name = "lblRiffChannels";
            this.lblRiffChannels.Size = new System.Drawing.Size(51, 13);
            this.lblRiffChannels.TabIndex = 2;
            this.lblRiffChannels.Text = "Channels";
            // 
            // comboRiffFrequency
            // 
            this.comboRiffFrequency.FormattingEnabled = true;
            this.comboRiffFrequency.Location = new System.Drawing.Point(69, 19);
            this.comboRiffFrequency.Name = "comboRiffFrequency";
            this.comboRiffFrequency.Size = new System.Drawing.Size(77, 21);
            this.comboRiffFrequency.TabIndex = 1;
            this.comboRiffFrequency.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboRiffFrequency_KeyPress);
            this.comboRiffFrequency.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboRiffFrequency_KeyDown);
            // 
            // lblRiffFrequency
            // 
            this.lblRiffFrequency.AutoSize = true;
            this.lblRiffFrequency.Location = new System.Drawing.Point(3, 22);
            this.lblRiffFrequency.Name = "lblRiffFrequency";
            this.lblRiffFrequency.Size = new System.Drawing.Size(57, 13);
            this.lblRiffFrequency.TabIndex = 0;
            this.lblRiffFrequency.Text = "Frequency";
            // 
            // grpXmaParseOptions
            // 
            this.grpXmaParseOptions.Controls.Add(this.tbXmaParseBlockSize);
            this.grpXmaParseOptions.Controls.Add(this.lblXmaParseBlockSize);
            this.grpXmaParseOptions.Controls.Add(this.tbXmaParseStartOffset);
            this.grpXmaParseOptions.Controls.Add(this.lblXmaParseOffset);
            this.grpXmaParseOptions.Controls.Add(this.comboXmaParseInputType);
            this.grpXmaParseOptions.Controls.Add(this.lblXmaParseInputType);
            this.grpXmaParseOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpXmaParseOptions.Location = new System.Drawing.Point(0, 0);
            this.grpXmaParseOptions.Name = "grpXmaParseOptions";
            this.grpXmaParseOptions.Size = new System.Drawing.Size(789, 46);
            this.grpXmaParseOptions.TabIndex = 0;
            this.grpXmaParseOptions.TabStop = false;
            this.grpXmaParseOptions.Text = "xma_parse Options";
            // 
            // tbXmaParseBlockSize
            // 
            this.tbXmaParseBlockSize.Location = new System.Drawing.Point(346, 19);
            this.tbXmaParseBlockSize.Name = "tbXmaParseBlockSize";
            this.tbXmaParseBlockSize.Size = new System.Drawing.Size(60, 20);
            this.tbXmaParseBlockSize.TabIndex = 5;
            // 
            // lblXmaParseBlockSize
            // 
            this.lblXmaParseBlockSize.AutoSize = true;
            this.lblXmaParseBlockSize.Location = new System.Drawing.Point(283, 22);
            this.lblXmaParseBlockSize.Name = "lblXmaParseBlockSize";
            this.lblXmaParseBlockSize.Size = new System.Drawing.Size(57, 13);
            this.lblXmaParseBlockSize.TabIndex = 4;
            this.lblXmaParseBlockSize.Text = "Block Size";
            // 
            // tbXmaParseStartOffset
            // 
            this.tbXmaParseStartOffset.Location = new System.Drawing.Point(204, 19);
            this.tbXmaParseStartOffset.Name = "tbXmaParseStartOffset";
            this.tbXmaParseStartOffset.Size = new System.Drawing.Size(60, 20);
            this.tbXmaParseStartOffset.TabIndex = 3;
            // 
            // lblXmaParseOffset
            // 
            this.lblXmaParseOffset.AutoSize = true;
            this.lblXmaParseOffset.Location = new System.Drawing.Point(138, 22);
            this.lblXmaParseOffset.Name = "lblXmaParseOffset";
            this.lblXmaParseOffset.Size = new System.Drawing.Size(60, 13);
            this.lblXmaParseOffset.TabIndex = 2;
            this.lblXmaParseOffset.Text = "Start Offset";
            // 
            // comboXmaParseInputType
            // 
            this.comboXmaParseInputType.FormattingEnabled = true;
            this.comboXmaParseInputType.Location = new System.Drawing.Point(69, 19);
            this.comboXmaParseInputType.Name = "comboXmaParseInputType";
            this.comboXmaParseInputType.Size = new System.Drawing.Size(50, 21);
            this.comboXmaParseInputType.TabIndex = 1;
            this.comboXmaParseInputType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboXmaParseInputType_KeyPress);
            this.comboXmaParseInputType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboXmaParseInputType_KeyDown);
            // 
            // lblXmaParseInputType
            // 
            this.lblXmaParseInputType.AutoSize = true;
            this.lblXmaParseInputType.Location = new System.Drawing.Point(3, 22);
            this.lblXmaParseInputType.Name = "lblXmaParseInputType";
            this.lblXmaParseInputType.Size = new System.Drawing.Size(57, 13);
            this.lblXmaParseInputType.TabIndex = 0;
            this.lblXmaParseInputType.Text = "XMA Type";
            // 
            // groupOtherOptions
            // 
            this.groupOtherOptions.Controls.Add(this.cbShowAllExeOutput);
            this.groupOtherOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupOtherOptions.Location = new System.Drawing.Point(0, 94);
            this.groupOtherOptions.Name = "groupOtherOptions";
            this.groupOtherOptions.Size = new System.Drawing.Size(789, 58);
            this.groupOtherOptions.TabIndex = 2;
            this.groupOtherOptions.TabStop = false;
            this.groupOtherOptions.Text = "Other Options";
            // 
            // cbShowAllExeOutput
            // 
            this.cbShowAllExeOutput.AutoSize = true;
            this.cbShowAllExeOutput.Location = new System.Drawing.Point(6, 19);
            this.cbShowAllExeOutput.Name = "cbShowAllExeOutput";
            this.cbShowAllExeOutput.Size = new System.Drawing.Size(226, 17);
            this.cbShowAllExeOutput.TabIndex = 0;
            this.cbShowAllExeOutput.Text = "Show All Output from External Applications";
            this.cbShowAllExeOutput.UseVisualStyleBackColor = true;
            // 
            // XmaConvertForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 463);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.grpOutputOptions);
            this.Controls.Add(this.grpPresets);
            this.Name = "XmaConvertForm";
            this.Text = "XmaConvertForm";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.XmaConvertForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.XmaConvertForm_DragEnter);
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpPresets, 0);
            this.Controls.SetChildIndex(this.grpOutputOptions, 0);
            this.Controls.SetChildIndex(this.pnlOptions, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.pnlOptions.ResumeLayout(false);
            this.grpRiffHeaderOptions.ResumeLayout(false);
            this.grpRiffHeaderOptions.PerformLayout();
            this.grpXmaParseOptions.ResumeLayout(false);
            this.grpXmaParseOptions.PerformLayout();
            this.groupOtherOptions.ResumeLayout(false);
            this.groupOtherOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPresets;
        private System.Windows.Forms.GroupBox grpOutputOptions;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.GroupBox grpXmaParseOptions;
        private System.Windows.Forms.Label lblXmaParseInputType;
        private System.Windows.Forms.ComboBox comboXmaParseInputType;
        private System.Windows.Forms.TextBox tbXmaParseBlockSize;
        private System.Windows.Forms.Label lblXmaParseBlockSize;
        private System.Windows.Forms.TextBox tbXmaParseStartOffset;
        private System.Windows.Forms.Label lblXmaParseOffset;
        private System.Windows.Forms.GroupBox grpRiffHeaderOptions;
        private System.Windows.Forms.ComboBox comboRiffFrequency;
        private System.Windows.Forms.Label lblRiffFrequency;
        private System.Windows.Forms.Label lblRiffChannels;
        private System.Windows.Forms.ComboBox comboRiffChannels;
        private System.Windows.Forms.GroupBox groupOtherOptions;
        private System.Windows.Forms.CheckBox cbShowAllExeOutput;
    }
}