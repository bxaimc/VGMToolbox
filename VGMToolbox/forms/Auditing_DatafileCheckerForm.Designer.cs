namespace VGMToolbox.forms
{
    partial class Auditing_DatafileCheckerForm
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbDatafileChecker_BrowseDestination = new System.Windows.Forms.Button();
            this.tbDatafileChecker_BrowseSource = new System.Windows.Forms.Button();
            this.tbDatafileChecker_OutputPath = new System.Windows.Forms.TextBox();
            this.tbDatafileChecker_SourceFile = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 430);
            this.pnlLabels.Size = new System.Drawing.Size(756, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(756, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 353);
            this.tbOutput.Size = new System.Drawing.Size(756, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 333);
            this.pnlButtons.Size = new System.Drawing.Size(756, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(696, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(636, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnDatafileChecker_Check_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbDatafileChecker_BrowseDestination);
            this.groupBox1.Controls.Add(this.tbDatafileChecker_BrowseSource);
            this.groupBox1.Controls.Add(this.tbDatafileChecker_OutputPath);
            this.groupBox1.Controls.Add(this.tbDatafileChecker_SourceFile);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 98);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source and Destination";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Report(s) Destination Folder";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Source Datafile";
            // 
            // tbDatafileChecker_BrowseDestination
            // 
            this.tbDatafileChecker_BrowseDestination.Location = new System.Drawing.Point(271, 71);
            this.tbDatafileChecker_BrowseDestination.Name = "tbDatafileChecker_BrowseDestination";
            this.tbDatafileChecker_BrowseDestination.Size = new System.Drawing.Size(28, 20);
            this.tbDatafileChecker_BrowseDestination.TabIndex = 3;
            this.tbDatafileChecker_BrowseDestination.Text = "...";
            this.tbDatafileChecker_BrowseDestination.UseVisualStyleBackColor = true;
            this.tbDatafileChecker_BrowseDestination.Click += new System.EventHandler(this.tbDatafileChecker_BrowseDestination_Click);
            // 
            // tbDatafileChecker_BrowseSource
            // 
            this.tbDatafileChecker_BrowseSource.Location = new System.Drawing.Point(270, 32);
            this.tbDatafileChecker_BrowseSource.Name = "tbDatafileChecker_BrowseSource";
            this.tbDatafileChecker_BrowseSource.Size = new System.Drawing.Size(28, 20);
            this.tbDatafileChecker_BrowseSource.TabIndex = 1;
            this.tbDatafileChecker_BrowseSource.Text = "...";
            this.tbDatafileChecker_BrowseSource.UseVisualStyleBackColor = true;
            this.tbDatafileChecker_BrowseSource.Click += new System.EventHandler(this.tbDatafileChecker_BrowseSource_Click);
            // 
            // tbDatafileChecker_OutputPath
            // 
            this.tbDatafileChecker_OutputPath.Location = new System.Drawing.Point(6, 71);
            this.tbDatafileChecker_OutputPath.Name = "tbDatafileChecker_OutputPath";
            this.tbDatafileChecker_OutputPath.Size = new System.Drawing.Size(259, 20);
            this.tbDatafileChecker_OutputPath.TabIndex = 2;
            // 
            // tbDatafileChecker_SourceFile
            // 
            this.tbDatafileChecker_SourceFile.Location = new System.Drawing.Point(6, 32);
            this.tbDatafileChecker_SourceFile.Name = "tbDatafileChecker_SourceFile";
            this.tbDatafileChecker_SourceFile.Size = new System.Drawing.Size(259, 20);
            this.tbDatafileChecker_SourceFile.TabIndex = 0;
            // 
            // Auditing_DatafileCheckerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 471);
            this.Controls.Add(this.groupBox1);
            this.Name = "Auditing_DatafileCheckerForm";
            this.Text = "Auditing_DatafileCheckerForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button tbDatafileChecker_BrowseDestination;
        private System.Windows.Forms.Button tbDatafileChecker_BrowseSource;
        private System.Windows.Forms.TextBox tbDatafileChecker_OutputPath;
        private System.Windows.Forms.TextBox tbDatafileChecker_SourceFile;
    }
}