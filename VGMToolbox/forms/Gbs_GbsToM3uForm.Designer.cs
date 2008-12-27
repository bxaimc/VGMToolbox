namespace VGMToolbox.forms
{
    partial class Gbs_GbsToM3uForm
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbGBS_gbsm3uSource = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbGBS_OneM3uPerTrack = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.tbGBS_gbsm3uSource);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 23);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(756, 61);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "GBS  .m3u Builder";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 42);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(171, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "Drag and Drop folders or files here.";
            // 
            // tbGBS_gbsm3uSource
            // 
            this.tbGBS_gbsm3uSource.AllowDrop = true;
            this.tbGBS_gbsm3uSource.Location = new System.Drawing.Point(6, 19);
            this.tbGBS_gbsm3uSource.Name = "tbGBS_gbsm3uSource";
            this.tbGBS_gbsm3uSource.Size = new System.Drawing.Size(259, 20);
            this.tbGBS_gbsm3uSource.TabIndex = 0;
            this.tbGBS_gbsm3uSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbGBS_gbsm3uSource_DragDrop);
            this.tbGBS_gbsm3uSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbGBS_OneM3uPerTrack);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox6.Location = new System.Drawing.Point(0, 84);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(756, 61);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Options";
            // 
            // cbGBS_OneM3uPerTrack
            // 
            this.cbGBS_OneM3uPerTrack.AutoSize = true;
            this.cbGBS_OneM3uPerTrack.Location = new System.Drawing.Point(6, 19);
            this.cbGBS_OneM3uPerTrack.Name = "cbGBS_OneM3uPerTrack";
            this.cbGBS_OneM3uPerTrack.Size = new System.Drawing.Size(177, 17);
            this.cbGBS_OneM3uPerTrack.TabIndex = 0;
            this.cbGBS_OneM3uPerTrack.Text = "Output additional .m3u per track";
            this.cbGBS_OneM3uPerTrack.UseVisualStyleBackColor = true;
            // 
            // Gbs_GbsToM3uForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 437);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Name = "Gbs_GbsToM3uForm";
            this.Text = "Gbs_GbsToM3uForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox6, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbGBS_gbsm3uSource;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbGBS_OneM3uPerTrack;
    }
}