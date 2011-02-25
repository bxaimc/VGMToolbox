namespace VGMToolbox.forms.stream
{
    partial class MpegDemuxForm
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
            this.grpSourceFiles = new System.Windows.Forms.GroupBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboFormat = new System.Windows.Forms.ComboBox();
            this.cbAddHeader = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSourceFiles.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 487);
            this.pnlLabels.Size = new System.Drawing.Size(830, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(830, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 410);
            this.tbOutput.Size = new System.Drawing.Size(830, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 390);
            this.pnlButtons.Size = new System.Drawing.Size(830, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(770, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(710, 0);
            // 
            // grpSourceFiles
            // 
            this.grpSourceFiles.Controls.Add(this.grpOptions);
            this.grpSourceFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSourceFiles.Location = new System.Drawing.Point(0, 23);
            this.grpSourceFiles.Name = "grpSourceFiles";
            this.grpSourceFiles.Size = new System.Drawing.Size(830, 367);
            this.grpSourceFiles.TabIndex = 5;
            this.grpSourceFiles.TabStop = false;
            this.grpSourceFiles.Text = "Drop Files Here";
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.cbAddHeader);
            this.grpOptions.Controls.Add(this.label1);
            this.grpOptions.Controls.Add(this.comboFormat);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpOptions.Location = new System.Drawing.Point(3, 300);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(824, 64);
            this.grpOptions.TabIndex = 0;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Format";
            // 
            // comboFormat
            // 
            this.comboFormat.FormattingEnabled = true;
            this.comboFormat.Location = new System.Drawing.Point(51, 13);
            this.comboFormat.Name = "comboFormat";
            this.comboFormat.Size = new System.Drawing.Size(121, 21);
            this.comboFormat.TabIndex = 0;
            this.comboFormat.SelectedIndexChanged += new System.EventHandler(this.comboFormat_SelectedIndexChanged);
            // 
            // cbAddHeader
            // 
            this.cbAddHeader.AutoSize = true;
            this.cbAddHeader.Location = new System.Drawing.Point(9, 40);
            this.cbAddHeader.Name = "cbAddHeader";
            this.cbAddHeader.Size = new System.Drawing.Size(130, 17);
            this.cbAddHeader.TabIndex = 2;
            this.cbAddHeader.Text = "Add Header to Output";
            this.cbAddHeader.UseVisualStyleBackColor = true;
            // 
            // MpegDemuxForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 528);
            this.Controls.Add(this.grpSourceFiles);
            this.Name = "MpegDemuxForm";
            this.Text = "MpegDemuxForm";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MpegDemuxForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MpegDemuxForm_DragEnter);
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpSourceFiles, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpSourceFiles.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSourceFiles;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboFormat;
        private System.Windows.Forms.CheckBox cbAddHeader;
    }
}