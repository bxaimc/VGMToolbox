namespace VGMToolbox.forms.extraction
{
    partial class ExtractSonyAdpcmForm
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
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 378);
            this.pnlLabels.Size = new System.Drawing.Size(686, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(686, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 301);
            this.tbOutput.Size = new System.Drawing.Size(686, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 281);
            this.pnlButtons.Size = new System.Drawing.Size(686, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(626, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(566, 0);
            // 
            // grpSourceFiles
            // 
            this.grpSourceFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSourceFiles.Location = new System.Drawing.Point(0, 23);
            this.grpSourceFiles.Name = "grpSourceFiles";
            this.grpSourceFiles.Size = new System.Drawing.Size(686, 258);
            this.grpSourceFiles.TabIndex = 5;
            this.grpSourceFiles.TabStop = false;
            this.grpSourceFiles.Text = "groupBox1";
            this.grpSourceFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.grpSourceFiles_DragDrop);
            this.grpSourceFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // ExtractSonyAdpcmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 419);
            this.Controls.Add(this.grpSourceFiles);
            this.Name = "ExtractSonyAdpcmForm";
            this.Text = "ExtractSonyAdpcmForm";
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSourceFiles;
    }
}