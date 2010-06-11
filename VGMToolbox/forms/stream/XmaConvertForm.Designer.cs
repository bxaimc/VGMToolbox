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
            this.grpXmaParseOptions = new System.Windows.Forms.GroupBox();
            this.tbXmaParseBlockSize = new System.Windows.Forms.TextBox();
            this.lblXmaParseBlockSize = new System.Windows.Forms.Label();
            this.tbXmaParseStartOffset = new System.Windows.Forms.TextBox();
            this.lblXmaParseOffset = new System.Windows.Forms.Label();
            this.comboXmaParseInputType = new System.Windows.Forms.ComboBox();
            this.lblXmaParseInputType = new System.Windows.Forms.Label();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.grpXmaParseOptions.SuspendLayout();
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
            this.grpRiffHeaderOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpRiffHeaderOptions.Location = new System.Drawing.Point(0, 46);
            this.grpRiffHeaderOptions.Name = "grpRiffHeaderOptions";
            this.grpRiffHeaderOptions.Size = new System.Drawing.Size(789, 56);
            this.grpRiffHeaderOptions.TabIndex = 1;
            this.grpRiffHeaderOptions.TabStop = false;
            this.grpRiffHeaderOptions.Text = "RIFF Header Options";
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
            this.lblXmaParseInputType.Location = new System.Drawing.Point(6, 22);
            this.lblXmaParseInputType.Name = "lblXmaParseInputType";
            this.lblXmaParseInputType.Size = new System.Drawing.Size(57, 13);
            this.lblXmaParseInputType.TabIndex = 0;
            this.lblXmaParseInputType.Text = "XMA Type";
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
            this.grpXmaParseOptions.ResumeLayout(false);
            this.grpXmaParseOptions.PerformLayout();
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
    }
}