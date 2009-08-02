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
            this.pnlLabels.Location = new System.Drawing.Point(0, 501);
            this.pnlLabels.Size = new System.Drawing.Size(857, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(857, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 424);
            this.tbOutput.Size = new System.Drawing.Size(857, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 404);
            this.pnlButtons.Size = new System.Drawing.Size(857, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(797, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(737, 0);
            // 
            // grpSourceFiles
            // 
            this.grpSourceFiles.Controls.Add(this.grpOptions);
            this.grpSourceFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSourceFiles.Location = new System.Drawing.Point(0, 23);
            this.grpSourceFiles.Name = "grpSourceFiles";
            this.grpSourceFiles.Size = new System.Drawing.Size(857, 381);
            this.grpSourceFiles.TabIndex = 5;
            this.grpSourceFiles.TabStop = false;
            this.grpSourceFiles.Text = "Drop Files Here";
            this.grpSourceFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.grpSourceFiles_DragDrop);
            this.grpSourceFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.tbDriverText);
            this.grpOptions.Controls.Add(this.lblDriverText);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpOptions.Location = new System.Drawing.Point(3, 337);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(851, 41);
            this.grpOptions.TabIndex = 0;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
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
            this.ClientSize = new System.Drawing.Size(857, 542);
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
    }
}