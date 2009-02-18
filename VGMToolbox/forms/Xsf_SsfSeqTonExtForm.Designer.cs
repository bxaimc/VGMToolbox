namespace VGMToolbox.forms
{
    partial class Xsf_SsfSeqTonExtForm
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
            this.label17 = new System.Windows.Forms.Label();
            this.tbSsfSqTonExtSource = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbExtractToSubfolder = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 384);
            this.pnlLabels.Size = new System.Drawing.Size(868, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(868, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 307);
            this.tbOutput.Size = new System.Drawing.Size(868, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 287);
            this.pnlButtons.Size = new System.Drawing.Size(868, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(808, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(748, 0);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 42);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(129, 13);
            this.label17.TabIndex = 6;
            this.label17.Text = "Drag file(s) to check here.";
            // 
            // tbSsfSqTonExtSource
            // 
            this.tbSsfSqTonExtSource.AllowDrop = true;
            this.tbSsfSqTonExtSource.Location = new System.Drawing.Point(6, 19);
            this.tbSsfSqTonExtSource.Name = "tbSsfSqTonExtSource";
            this.tbSsfSqTonExtSource.Size = new System.Drawing.Size(282, 20);
            this.tbSsfSqTonExtSource.TabIndex = 5;
            this.tbSsfSqTonExtSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSsfSqTonExtSource_DragDrop);
            this.tbSsfSqTonExtSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbSsfSqTonExtSource);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(868, 63);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Files";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(417, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "seqext.py and tonext.py are written by kingshriek (snesmusic.org/hoot/kingshriek/" +
                "ssf/).";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbExtractToSubfolder);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(868, 40);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // cbExtractToSubfolder
            // 
            this.cbExtractToSubfolder.AutoSize = true;
            this.cbExtractToSubfolder.Location = new System.Drawing.Point(6, 19);
            this.cbExtractToSubfolder.Name = "cbExtractToSubfolder";
            this.cbExtractToSubfolder.Size = new System.Drawing.Size(172, 17);
            this.cbExtractToSubfolder.TabIndex = 0;
            this.cbExtractToSubfolder.Text = "Extract each file to a subfolder.";
            this.cbExtractToSubfolder.UseVisualStyleBackColor = true;
            // 
            // Xsf_SsfSeqTonExtForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 425);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Xsf_SsfSeqTonExtForm";
            this.Text = "Xsf_SsfSeqTonExtForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbSsfSqTonExtSource;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbExtractToSubfolder;
    }
}