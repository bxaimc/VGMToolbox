namespace VGMToolbox.forms
{
    partial class Xsf_2sfTimerForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.tbFilePrefix = new System.Windows.Forms.TextBox();
            this.btnSdatBrowse = new System.Windows.Forms.Button();
            this.btn2sfBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSdatPath = new System.Windows.Forms.TextBox();
            this.tbPathTo2sfs = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbOneLoop = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 479);
            this.pnlLabels.Size = new System.Drawing.Size(936, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(936, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 402);
            this.tbOutput.Size = new System.Drawing.Size(936, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 382);
            this.pnlButtons.Size = new System.Drawing.Size(936, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(876, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(816, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnDoTask_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbFilePrefix);
            this.groupBox1.Controls.Add(this.btnSdatBrowse);
            this.groupBox1.Controls.Add(this.btn2sfBrowse);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbSdatPath);
            this.groupBox1.Controls.Add(this.tbPathTo2sfs);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(936, 100);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Paths";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "File Prefix of Input Files";
            // 
            // tbFilePrefix
            // 
            this.tbFilePrefix.Location = new System.Drawing.Point(127, 74);
            this.tbFilePrefix.Name = "tbFilePrefix";
            this.tbFilePrefix.Size = new System.Drawing.Size(248, 20);
            this.tbFilePrefix.TabIndex = 6;
            // 
            // btnSdatBrowse
            // 
            this.btnSdatBrowse.Location = new System.Drawing.Point(381, 48);
            this.btnSdatBrowse.Name = "btnSdatBrowse";
            this.btnSdatBrowse.Size = new System.Drawing.Size(29, 20);
            this.btnSdatBrowse.TabIndex = 5;
            this.btnSdatBrowse.Text = "...";
            this.btnSdatBrowse.UseVisualStyleBackColor = true;
            this.btnSdatBrowse.Click += new System.EventHandler(this.btnSdatBrowse_Click);
            // 
            // btn2sfBrowse
            // 
            this.btn2sfBrowse.Location = new System.Drawing.Point(381, 22);
            this.btn2sfBrowse.Name = "btn2sfBrowse";
            this.btn2sfBrowse.Size = new System.Drawing.Size(29, 20);
            this.btn2sfBrowse.TabIndex = 4;
            this.btn2sfBrowse.Text = "...";
            this.btn2sfBrowse.UseVisualStyleBackColor = true;
            this.btn2sfBrowse.Click += new System.EventHandler(this.btn2sfBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path to SDAT";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Path to 2SF files";
            // 
            // tbSdatPath
            // 
            this.tbSdatPath.Location = new System.Drawing.Point(127, 48);
            this.tbSdatPath.Name = "tbSdatPath";
            this.tbSdatPath.Size = new System.Drawing.Size(248, 20);
            this.tbSdatPath.TabIndex = 1;
            // 
            // tbPathTo2sfs
            // 
            this.tbPathTo2sfs.Location = new System.Drawing.Point(127, 22);
            this.tbPathTo2sfs.Name = "tbPathTo2sfs";
            this.tbPathTo2sfs.Size = new System.Drawing.Size(248, 20);
            this.tbPathTo2sfs.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbOneLoop);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(936, 47);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // cbOneLoop
            // 
            this.cbOneLoop.AutoSize = true;
            this.cbOneLoop.Location = new System.Drawing.Point(9, 19);
            this.cbOneLoop.Name = "cbOneLoop";
            this.cbOneLoop.Size = new System.Drawing.Size(237, 17);
            this.cbOneLoop.TabIndex = 0;
            this.cbOneLoop.Text = "Single Loop (leave unchecked for two loops)";
            this.cbOneLoop.UseVisualStyleBackColor = true;
            // 
            // Xsf_2sfTimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 520);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Xsf_2sfTimerForm";
            this.Text = "Xsf_2sfTimerForm";
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
        private System.Windows.Forms.TextBox tbSdatPath;
        private System.Windows.Forms.TextBox tbPathTo2sfs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSdatBrowse;
        private System.Windows.Forms.Button btn2sfBrowse;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFilePrefix;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbOneLoop;
    }
}