namespace VGMToolbox.forms.xsf
{
    partial class PsfStubMakerForm
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
            this.cbIncludeReverb = new System.Windows.Forms.CheckBox();
            this.tbDriverText = new System.Windows.Forms.TextBox();
            this.lblDriverText = new System.Windows.Forms.Label();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSourceFiles.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 507);
            this.pnlLabels.Size = new System.Drawing.Size(850, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(850, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 430);
            this.tbOutput.Size = new System.Drawing.Size(850, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 410);
            this.pnlButtons.Size = new System.Drawing.Size(850, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(790, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(730, 0);
            // 
            // grpSourceFiles
            // 
            this.grpSourceFiles.Controls.Add(this.grpOptions);
            this.grpSourceFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSourceFiles.Location = new System.Drawing.Point(0, 23);
            this.grpSourceFiles.Name = "grpSourceFiles";
            this.grpSourceFiles.Size = new System.Drawing.Size(850, 387);
            this.grpSourceFiles.TabIndex = 5;
            this.grpSourceFiles.TabStop = false;
            this.grpSourceFiles.Text = "Drop Files Here";
            this.grpSourceFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.grpSourceFiles_DragDrop);
            this.grpSourceFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.cbIncludeReverb);
            this.grpOptions.Controls.Add(this.tbDriverText);
            this.grpOptions.Controls.Add(this.lblDriverText);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpOptions.Location = new System.Drawing.Point(3, 323);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(844, 61);
            this.grpOptions.TabIndex = 0;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // cbIncludeReverb
            // 
            this.cbIncludeReverb.AutoSize = true;
            this.cbIncludeReverb.Location = new System.Drawing.Point(9, 38);
            this.cbIncludeReverb.Name = "cbIncludeReverb";
            this.cbIncludeReverb.Size = new System.Drawing.Size(441, 17);
            this.cbIncludeReverb.TabIndex = 2;
            this.cbIncludeReverb.Text = "Add reverb information to stub, defaults will be used if reverb parameters are no" +
                "t present.";
            this.cbIncludeReverb.UseVisualStyleBackColor = true;
            // 
            // tbDriverText
            // 
            this.tbDriverText.Location = new System.Drawing.Point(101, 13);
            this.tbDriverText.Name = "tbDriverText";
            this.tbDriverText.Size = new System.Drawing.Size(260, 20);
            this.tbDriverText.TabIndex = 1;
            // 
            // lblDriverText
            // 
            this.lblDriverText.AutoSize = true;
            this.lblDriverText.Location = new System.Drawing.Point(6, 16);
            this.lblDriverText.Name = "lblDriverText";
            this.lblDriverText.Size = new System.Drawing.Size(89, 13);
            this.lblDriverText.TabIndex = 0;
            this.lblDriverText.Text = "Driver Text String";
            // 
            // PsfStubMakerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 548);
            this.Controls.Add(this.grpSourceFiles);
            this.Name = "PsfStubMakerForm";
            this.Text = "PsfStubMakerForm";
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
        private System.Windows.Forms.TextBox tbDriverText;
        private System.Windows.Forms.Label lblDriverText;
        private System.Windows.Forms.CheckBox cbIncludeReverb;
    }
}