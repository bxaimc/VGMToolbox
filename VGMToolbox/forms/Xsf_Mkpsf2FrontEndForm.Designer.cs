namespace VGMToolbox.forms
{
    partial class Xsf_Mkpsf2FrontEndForm
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
            this.btnModulesDirectoryBrowse = new System.Windows.Forms.Button();
            this.tbModulesDirectory = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSourceDirectoryBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSourceDirectory = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbVolume = new System.Windows.Forms.TextBox();
            this.tbTempo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbTickInterval = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbDepth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbReverb = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 533);
            this.pnlLabels.Size = new System.Drawing.Size(777, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(777, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 456);
            this.tbOutput.Size = new System.Drawing.Size(777, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 436);
            this.pnlButtons.Size = new System.Drawing.Size(777, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(717, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(657, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnModulesDirectoryBrowse);
            this.groupBox1.Controls.Add(this.tbModulesDirectory);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnSourceDirectoryBrowse);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbSourceDirectory);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(777, 102);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Directories";
            // 
            // btnModulesDirectoryBrowse
            // 
            this.btnModulesDirectoryBrowse.Location = new System.Drawing.Point(250, 71);
            this.btnModulesDirectoryBrowse.Name = "btnModulesDirectoryBrowse";
            this.btnModulesDirectoryBrowse.Size = new System.Drawing.Size(25, 21);
            this.btnModulesDirectoryBrowse.TabIndex = 14;
            this.btnModulesDirectoryBrowse.Text = "...";
            this.btnModulesDirectoryBrowse.UseVisualStyleBackColor = true;
            this.btnModulesDirectoryBrowse.Click += new System.EventHandler(this.btnModulesDirectoryBrowse_Click);
            // 
            // tbModulesDirectory
            // 
            this.tbModulesDirectory.Location = new System.Drawing.Point(9, 71);
            this.tbModulesDirectory.Name = "tbModulesDirectory";
            this.tbModulesDirectory.Size = new System.Drawing.Size(235, 20);
            this.tbModulesDirectory.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Modules Directory";
            // 
            // btnSourceDirectoryBrowse
            // 
            this.btnSourceDirectoryBrowse.Location = new System.Drawing.Point(250, 32);
            this.btnSourceDirectoryBrowse.Name = "btnSourceDirectoryBrowse";
            this.btnSourceDirectoryBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnSourceDirectoryBrowse.TabIndex = 11;
            this.btnSourceDirectoryBrowse.Text = "...";
            this.btnSourceDirectoryBrowse.UseVisualStyleBackColor = true;
            this.btnSourceDirectoryBrowse.Click += new System.EventHandler(this.btnSourceDirectoryBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Source Directory";
            // 
            // tbSourceDirectory
            // 
            this.tbSourceDirectory.Location = new System.Drawing.Point(9, 32);
            this.tbSourceDirectory.Name = "tbSourceDirectory";
            this.tbSourceDirectory.Size = new System.Drawing.Size(235, 20);
            this.tbSourceDirectory.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tbVolume);
            this.groupBox2.Controls.Add(this.tbTempo);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tbTickInterval);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tbDepth);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbReverb);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(777, 73);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options (Leave empty for defaults)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(151, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Volume";
            // 
            // tbVolume
            // 
            this.tbVolume.Location = new System.Drawing.Point(199, 15);
            this.tbVolume.MaxLength = 3;
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(36, 20);
            this.tbVolume.TabIndex = 8;
            // 
            // tbTempo
            // 
            this.tbTempo.Location = new System.Drawing.Point(49, 43);
            this.tbTempo.MaxLength = 5;
            this.tbTempo.Name = "tbTempo";
            this.tbTempo.Size = new System.Drawing.Size(50, 20);
            this.tbTempo.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Tempo";
            // 
            // tbTickInterval
            // 
            this.tbTickInterval.Location = new System.Drawing.Point(174, 43);
            this.tbTickInterval.MaxLength = 4;
            this.tbTickInterval.Name = "tbTickInterval";
            this.tbTickInterval.Size = new System.Drawing.Size(47, 20);
            this.tbTickInterval.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(102, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Tick Interval";
            // 
            // tbDepth
            // 
            this.tbDepth.Location = new System.Drawing.Point(105, 15);
            this.tbDepth.MaxLength = 5;
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(41, 20);
            this.tbDepth.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(68, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Depth";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Reverb";
            // 
            // tbReverb
            // 
            this.tbReverb.Location = new System.Drawing.Point(49, 15);
            this.tbReverb.MaxLength = 1;
            this.tbReverb.Name = "tbReverb";
            this.tbReverb.Size = new System.Drawing.Size(19, 20);
            this.tbReverb.TabIndex = 0;
            // 
            // Xsf_Mkpsf2FrontEndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 574);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Xsf_Mkpsf2FrontEndForm";
            this.Text = "Xsf_Mkpsf2FrontEndForm";
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
        private System.Windows.Forms.Button btnModulesDirectoryBrowse;
        private System.Windows.Forms.TextBox tbModulesDirectory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSourceDirectoryBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbSourceDirectory;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbVolume;
        private System.Windows.Forms.TextBox tbTempo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbTickInterval;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbDepth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbReverb;
    }
}