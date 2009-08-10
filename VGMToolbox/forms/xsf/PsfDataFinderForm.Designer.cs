namespace VGMToolbox.forms.xsf
{
    partial class PsfDataFinderForm
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
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.cbUseMinimum = new System.Windows.Forms.CheckBox();
            this.tbMinimumSize = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 351);
            this.pnlLabels.Size = new System.Drawing.Size(768, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(768, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 274);
            this.tbOutput.Size = new System.Drawing.Size(768, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 254);
            this.pnlButtons.Size = new System.Drawing.Size(768, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(708, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(648, 0);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.grpOptions);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSource.Location = new System.Drawing.Point(0, 23);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(768, 231);
            this.grpSource.TabIndex = 5;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Drop Files Here";
            this.grpSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.grpSource_DragDrop);
            this.grpSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.tbMinimumSize);
            this.grpOptions.Controls.Add(this.cbUseMinimum);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpOptions.Location = new System.Drawing.Point(3, 182);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(762, 46);
            this.grpOptions.TabIndex = 0;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // cbUseMinimum
            // 
            this.cbUseMinimum.AutoSize = true;
            this.cbUseMinimum.Location = new System.Drawing.Point(6, 19);
            this.cbUseMinimum.Name = "cbUseMinimum";
            this.cbUseMinimum.Size = new System.Drawing.Size(130, 17);
            this.cbUseMinimum.TabIndex = 0;
            this.cbUseMinimum.Text = "Minimum Size on SEQ";
            this.cbUseMinimum.UseVisualStyleBackColor = true;
            // 
            // tbMinimumSize
            // 
            this.tbMinimumSize.Location = new System.Drawing.Point(142, 17);
            this.tbMinimumSize.Name = "tbMinimumSize";
            this.tbMinimumSize.Size = new System.Drawing.Size(100, 20);
            this.tbMinimumSize.TabIndex = 1;
            // 
            // PsfDataFinderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 392);
            this.Controls.Add(this.grpSource);
            this.Name = "PsfDataFinderForm";
            this.Text = "PsfDataFinderForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpSource, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox cbUseMinimum;
        private System.Windows.Forms.TextBox tbMinimumSize;
    }
}