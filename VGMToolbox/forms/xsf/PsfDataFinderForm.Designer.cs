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
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 533);
            this.pnlLabels.Size = new System.Drawing.Size(722, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(722, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 456);
            this.tbOutput.Size = new System.Drawing.Size(722, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 436);
            this.pnlButtons.Size = new System.Drawing.Size(722, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(662, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(602, 0);
            // 
            // grpSource
            // 
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSource.Location = new System.Drawing.Point(0, 23);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(722, 413);
            this.grpSource.TabIndex = 5;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Drop Files Here";
            this.grpSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.grpSource_DragDrop);
            this.grpSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // PsfDataFinderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 574);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
    }
}