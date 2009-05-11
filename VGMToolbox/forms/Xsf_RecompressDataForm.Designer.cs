namespace VGMToolbox.forms
{
    partial class Xsf_RecompressDataForm
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
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.tbSource = new System.Windows.Forms.TextBox();
            this.cbCompressionLevel = new System.Windows.Forms.ComboBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 384);
            this.pnlLabels.Size = new System.Drawing.Size(711, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(711, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 307);
            this.tbOutput.Size = new System.Drawing.Size(711, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 287);
            this.pnlButtons.Size = new System.Drawing.Size(711, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(651, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(591, 0);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.tbSource);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Location = new System.Drawing.Point(0, 23);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(711, 60);
            this.grpSource.TabIndex = 5;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source";
            // 
            // tbSource
            // 
            this.tbSource.AllowDrop = true;
            this.tbSource.Location = new System.Drawing.Point(6, 19);
            this.tbSource.Name = "tbSource";
            this.tbSource.Size = new System.Drawing.Size(297, 20);
            this.tbSource.TabIndex = 0;
            this.tbSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSource_DragDrop);
            this.tbSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // cbCompressionLevel
            // 
            this.cbCompressionLevel.FormattingEnabled = true;
            this.cbCompressionLevel.Location = new System.Drawing.Point(6, 19);
            this.cbCompressionLevel.Name = "cbCompressionLevel";
            this.cbCompressionLevel.Size = new System.Drawing.Size(297, 21);
            this.cbCompressionLevel.TabIndex = 6;
            this.cbCompressionLevel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbCompressionLevel_KeyPress);
            this.cbCompressionLevel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbCompressionLevel_KeyDown);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.cbCompressionLevel);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpOptions.Location = new System.Drawing.Point(0, 83);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(711, 65);
            this.grpOptions.TabIndex = 7;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // Xsf_RecompressDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 425);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpSource);
            this.Name = "Xsf_RecompressDataForm";
            this.Text = "XsfRecompressDataForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpSource, 0);
            this.Controls.SetChildIndex(this.grpOptions, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.grpOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.ComboBox cbCompressionLevel;
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.GroupBox grpOptions;
    }
}