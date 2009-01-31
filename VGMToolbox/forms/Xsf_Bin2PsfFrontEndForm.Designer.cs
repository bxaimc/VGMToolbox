namespace VGMToolbox.forms
{
    partial class Xsf_Bin2PsfFrontEndForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbOutputFolderName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnExeBrowse = new System.Windows.Forms.Button();
            this.btnSourceDirectoryBrowse = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSourceFilesPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbExePath = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbVbOffset = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbVhOffset = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSeqOffset = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 470);
            this.pnlLabels.Size = new System.Drawing.Size(844, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(844, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 393);
            this.tbOutput.Size = new System.Drawing.Size(844, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 373);
            this.pnlButtons.Size = new System.Drawing.Size(844, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(784, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(724, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnDoTask_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbOutputFolderName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnExeBrowse);
            this.groupBox1.Controls.Add(this.btnSourceDirectoryBrowse);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbSourceFilesPath);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbExePath);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(844, 96);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source PSX-EXEs";
            // 
            // tbOutputFolderName
            // 
            this.tbOutputFolderName.Location = new System.Drawing.Point(86, 70);
            this.tbOutputFolderName.Name = "tbOutputFolderName";
            this.tbOutputFolderName.Size = new System.Drawing.Size(218, 20);
            this.tbOutputFolderName.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Output Folder";
            // 
            // btnExeBrowse
            // 
            this.btnExeBrowse.Location = new System.Drawing.Point(310, 18);
            this.btnExeBrowse.Name = "btnExeBrowse";
            this.btnExeBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnExeBrowse.TabIndex = 13;
            this.btnExeBrowse.Text = "...";
            this.btnExeBrowse.UseVisualStyleBackColor = true;
            this.btnExeBrowse.Click += new System.EventHandler(this.btnExeBrowse_Click);
            // 
            // btnSourceDirectoryBrowse
            // 
            this.btnSourceDirectoryBrowse.Location = new System.Drawing.Point(310, 44);
            this.btnSourceDirectoryBrowse.Name = "btnSourceDirectoryBrowse";
            this.btnSourceDirectoryBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnSourceDirectoryBrowse.TabIndex = 12;
            this.btnSourceDirectoryBrowse.Text = "...";
            this.btnSourceDirectoryBrowse.UseVisualStyleBackColor = true;
            this.btnSourceDirectoryBrowse.Click += new System.EventHandler(this.btnSourceDirectoryBrowse_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Source Files";
            // 
            // tbSourceFilesPath
            // 
            this.tbSourceFilesPath.Location = new System.Drawing.Point(86, 45);
            this.tbSourceFilesPath.Name = "tbSourceFilesPath";
            this.tbSourceFilesPath.Size = new System.Drawing.Size(218, 20);
            this.tbSourceFilesPath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "PSX-EXE Path";
            // 
            // tbExePath
            // 
            this.tbExePath.AllowDrop = true;
            this.tbExePath.Location = new System.Drawing.Point(86, 19);
            this.tbExePath.Name = "tbExePath";
            this.tbExePath.Size = new System.Drawing.Size(218, 20);
            this.tbExePath.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbVbOffset);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tbVhOffset);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tbSeqOffset);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(844, 100);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "VB Offset";
            // 
            // tbVbOffset
            // 
            this.tbVbOffset.Location = new System.Drawing.Point(86, 72);
            this.tbVbOffset.Name = "tbVbOffset";
            this.tbVbOffset.Size = new System.Drawing.Size(218, 20);
            this.tbVbOffset.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "VH Offset";
            // 
            // tbVhOffset
            // 
            this.tbVhOffset.Location = new System.Drawing.Point(86, 45);
            this.tbVhOffset.Name = "tbVhOffset";
            this.tbVhOffset.Size = new System.Drawing.Size(218, 20);
            this.tbVhOffset.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "SEQ Offset";
            // 
            // tbSeqOffset
            // 
            this.tbSeqOffset.Location = new System.Drawing.Point(86, 19);
            this.tbSeqOffset.Name = "tbSeqOffset";
            this.tbSeqOffset.Size = new System.Drawing.Size(218, 20);
            this.tbSeqOffset.TabIndex = 0;
            // 
            // Xsf_Bin2PsfFrontEndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 511);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Xsf_Bin2PsfFrontEndForm";
            this.Text = "Xsf_Bin2PsfFrontEndForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
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

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbExePath;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbSeqOffset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbVbOffset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbVhOffset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSourceFilesPath;
        private System.Windows.Forms.Button btnExeBrowse;
        private System.Windows.Forms.Button btnSourceDirectoryBrowse;
        private System.Windows.Forms.TextBox tbOutputFolderName;
        private System.Windows.Forms.Label label6;
    }
}