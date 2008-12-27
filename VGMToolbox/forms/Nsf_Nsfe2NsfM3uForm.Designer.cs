namespace VGMToolbox.forms
{
    partial class Nsf_Nsfe2NsfM3uForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbNSF_nsfe2m3uSource = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbNSFE_OneM3uPerTrack = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 396);
            this.pnlLabels.Size = new System.Drawing.Size(756, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(756, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 319);
            this.tbOutput.Size = new System.Drawing.Size(756, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 299);
            this.pnlButtons.Size = new System.Drawing.Size(756, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(696, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(636, 0);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.tbNSF_nsfe2m3uSource);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(756, 61);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Files";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(171, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Drag and Drop folders or files here.";
            // 
            // tbNSF_nsfe2m3uSource
            // 
            this.tbNSF_nsfe2m3uSource.AllowDrop = true;
            this.tbNSF_nsfe2m3uSource.Location = new System.Drawing.Point(6, 19);
            this.tbNSF_nsfe2m3uSource.Name = "tbNSF_nsfe2m3uSource";
            this.tbNSF_nsfe2m3uSource.Size = new System.Drawing.Size(259, 20);
            this.tbNSF_nsfe2m3uSource.TabIndex = 0;
            this.tbNSF_nsfe2m3uSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbNSF_nsfe2m3uSource_DragDrop);
            this.tbNSF_nsfe2m3uSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbNSFE_OneM3uPerTrack);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox5.Location = new System.Drawing.Point(0, 84);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(756, 61);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Options";
            // 
            // cbNSFE_OneM3uPerTrack
            // 
            this.cbNSFE_OneM3uPerTrack.AutoSize = true;
            this.cbNSFE_OneM3uPerTrack.Location = new System.Drawing.Point(6, 19);
            this.cbNSFE_OneM3uPerTrack.Name = "cbNSFE_OneM3uPerTrack";
            this.cbNSFE_OneM3uPerTrack.Size = new System.Drawing.Size(177, 17);
            this.cbNSFE_OneM3uPerTrack.TabIndex = 0;
            this.cbNSFE_OneM3uPerTrack.Text = "Output additional .m3u per track";
            this.cbNSFE_OneM3uPerTrack.UseVisualStyleBackColor = true;
            // 
            // Nsf_Nsfe2NsfM3uForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 437);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Name = "Nsf_Nsfe2NsfM3uForm";
            this.Text = "Nsf_Nsfe2NsfM3uForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox5, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbNSF_nsfe2m3uSource;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cbNSFE_OneM3uPerTrack;
    }
}