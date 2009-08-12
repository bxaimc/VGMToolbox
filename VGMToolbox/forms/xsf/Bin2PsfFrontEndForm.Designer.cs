namespace VGMToolbox.forms.xsf
{
    partial class Bin2PsfFrontEndForm
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
            this.cbMinipsf = new System.Windows.Forms.CheckBox();
            this.lblPsfLibName = new System.Windows.Forms.Label();
            this.tbPsflibName = new System.Windows.Forms.TextBox();
            this.tbOutputFolderName = new System.Windows.Forms.TextBox();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.btnExeBrowse = new System.Windows.Forms.Button();
            this.btnSourceDirectoryBrowse = new System.Windows.Forms.Button();
            this.lblSourceFiles = new System.Windows.Forms.Label();
            this.tbSourceFilesPath = new System.Windows.Forms.TextBox();
            this.lblDriverPath = new System.Windows.Forms.Label();
            this.tbExePath = new System.Windows.Forms.TextBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.cbTryMixing = new System.Windows.Forms.CheckBox();
            this.cbAllowZeroLengthSequences = new System.Windows.Forms.CheckBox();
            this.lblVbOffset = new System.Windows.Forms.Label();
            this.tbVbOffset = new System.Windows.Forms.TextBox();
            this.lblVhOffset = new System.Windows.Forms.Label();
            this.tbVhOffset = new System.Windows.Forms.TextBox();
            this.lblSeqOffset = new System.Windows.Forms.Label();
            this.tbSeqOffset = new System.Windows.Forms.TextBox();
            this.grpGenericDrivers = new System.Windows.Forms.GroupBox();
            this.lblGenericDriver = new System.Windows.Forms.Label();
            this.genericDriver = new System.Windows.Forms.ComboBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.grpGenericDrivers.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 431);
            this.pnlLabels.Size = new System.Drawing.Size(857, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(857, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 354);
            this.tbOutput.Size = new System.Drawing.Size(857, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 334);
            this.pnlButtons.Size = new System.Drawing.Size(857, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(797, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(737, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnDoTask_Click);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.cbMinipsf);
            this.grpSource.Controls.Add(this.lblPsfLibName);
            this.grpSource.Controls.Add(this.tbPsflibName);
            this.grpSource.Controls.Add(this.tbOutputFolderName);
            this.grpSource.Controls.Add(this.lblOutputFolder);
            this.grpSource.Controls.Add(this.btnExeBrowse);
            this.grpSource.Controls.Add(this.btnSourceDirectoryBrowse);
            this.grpSource.Controls.Add(this.lblSourceFiles);
            this.grpSource.Controls.Add(this.tbSourceFilesPath);
            this.grpSource.Controls.Add(this.lblDriverPath);
            this.grpSource.Controls.Add(this.tbExePath);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Location = new System.Drawing.Point(0, 70);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(857, 117);
            this.grpSource.TabIndex = 5;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source";
            // 
            // cbMinipsf
            // 
            this.cbMinipsf.AutoSize = true;
            this.cbMinipsf.Location = new System.Drawing.Point(310, 92);
            this.cbMinipsf.Name = "cbMinipsf";
            this.cbMinipsf.Size = new System.Drawing.Size(101, 17);
            this.cbMinipsf.TabIndex = 18;
            this.cbMinipsf.Text = "Output .minipsfs";
            this.cbMinipsf.UseVisualStyleBackColor = true;
            this.cbMinipsf.CheckedChanged += new System.EventHandler(this.cbMinipsf_CheckedChanged);
            // 
            // lblPsfLibName
            // 
            this.lblPsfLibName.AutoSize = true;
            this.lblPsfLibName.Location = new System.Drawing.Point(3, 93);
            this.lblPsfLibName.Name = "lblPsfLibName";
            this.lblPsfLibName.Size = new System.Drawing.Size(72, 13);
            this.lblPsfLibName.TabIndex = 17;
            this.lblPsfLibName.Text = "PSFLib Name";
            // 
            // tbPsflibName
            // 
            this.tbPsflibName.Location = new System.Drawing.Point(86, 90);
            this.tbPsflibName.Name = "tbPsflibName";
            this.tbPsflibName.ReadOnly = true;
            this.tbPsflibName.Size = new System.Drawing.Size(218, 20);
            this.tbPsflibName.TabIndex = 16;
            // 
            // tbOutputFolderName
            // 
            this.tbOutputFolderName.Location = new System.Drawing.Point(86, 64);
            this.tbOutputFolderName.Name = "tbOutputFolderName";
            this.tbOutputFolderName.Size = new System.Drawing.Size(218, 20);
            this.tbOutputFolderName.TabIndex = 15;
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(3, 67);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(71, 13);
            this.lblOutputFolder.TabIndex = 14;
            this.lblOutputFolder.Text = "Output Folder";
            // 
            // btnExeBrowse
            // 
            this.btnExeBrowse.Location = new System.Drawing.Point(310, 12);
            this.btnExeBrowse.Name = "btnExeBrowse";
            this.btnExeBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnExeBrowse.TabIndex = 13;
            this.btnExeBrowse.Text = "...";
            this.btnExeBrowse.UseVisualStyleBackColor = true;
            this.btnExeBrowse.Click += new System.EventHandler(this.btnExeBrowse_Click);
            // 
            // btnSourceDirectoryBrowse
            // 
            this.btnSourceDirectoryBrowse.Location = new System.Drawing.Point(310, 38);
            this.btnSourceDirectoryBrowse.Name = "btnSourceDirectoryBrowse";
            this.btnSourceDirectoryBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnSourceDirectoryBrowse.TabIndex = 12;
            this.btnSourceDirectoryBrowse.Text = "...";
            this.btnSourceDirectoryBrowse.UseVisualStyleBackColor = true;
            this.btnSourceDirectoryBrowse.Click += new System.EventHandler(this.btnSourceDirectoryBrowse_Click);
            // 
            // lblSourceFiles
            // 
            this.lblSourceFiles.AutoSize = true;
            this.lblSourceFiles.Location = new System.Drawing.Point(3, 42);
            this.lblSourceFiles.Name = "lblSourceFiles";
            this.lblSourceFiles.Size = new System.Drawing.Size(65, 13);
            this.lblSourceFiles.TabIndex = 3;
            this.lblSourceFiles.Text = "Source Files";
            // 
            // tbSourceFilesPath
            // 
            this.tbSourceFilesPath.Location = new System.Drawing.Point(86, 39);
            this.tbSourceFilesPath.Name = "tbSourceFilesPath";
            this.tbSourceFilesPath.Size = new System.Drawing.Size(218, 20);
            this.tbSourceFilesPath.TabIndex = 2;
            // 
            // lblDriverPath
            // 
            this.lblDriverPath.AutoSize = true;
            this.lblDriverPath.Location = new System.Drawing.Point(3, 16);
            this.lblDriverPath.Name = "lblDriverPath";
            this.lblDriverPath.Size = new System.Drawing.Size(60, 13);
            this.lblDriverPath.TabIndex = 1;
            this.lblDriverPath.Text = "Driver Path";
            // 
            // tbExePath
            // 
            this.tbExePath.AllowDrop = true;
            this.tbExePath.Location = new System.Drawing.Point(86, 13);
            this.tbExePath.Name = "tbExePath";
            this.tbExePath.Size = new System.Drawing.Size(218, 20);
            this.tbExePath.TabIndex = 0;
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.cbTryMixing);
            this.grpOptions.Controls.Add(this.cbAllowZeroLengthSequences);
            this.grpOptions.Controls.Add(this.lblVbOffset);
            this.grpOptions.Controls.Add(this.tbVbOffset);
            this.grpOptions.Controls.Add(this.lblVhOffset);
            this.grpOptions.Controls.Add(this.tbVhOffset);
            this.grpOptions.Controls.Add(this.lblSeqOffset);
            this.grpOptions.Controls.Add(this.tbSeqOffset);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpOptions.Location = new System.Drawing.Point(0, 187);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(857, 200);
            this.grpOptions.TabIndex = 6;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // cbTryMixing
            // 
            this.cbTryMixing.Location = new System.Drawing.Point(6, 136);
            this.cbTryMixing.Name = "cbTryMixing";
            this.cbTryMixing.Size = new System.Drawing.Size(501, 31);
            this.cbTryMixing.TabIndex = 7;
            this.cbTryMixing.Text = "[BETA] Try all possible combinations of SEQ and VH (Good for finding matches in u" +
                "nmatched data) [Need to fix Progress Meter].";
            this.cbTryMixing.UseVisualStyleBackColor = true;
            // 
            // cbAllowZeroLengthSequences
            // 
            this.cbAllowZeroLengthSequences.AutoSize = true;
            this.cbAllowZeroLengthSequences.Location = new System.Drawing.Point(6, 113);
            this.cbAllowZeroLengthSequences.Name = "cbAllowZeroLengthSequences";
            this.cbAllowZeroLengthSequences.Size = new System.Drawing.Size(169, 17);
            this.cbAllowZeroLengthSequences.TabIndex = 6;
            this.cbAllowZeroLengthSequences.Text = "Allow Zero Length Sequences";
            this.cbAllowZeroLengthSequences.UseVisualStyleBackColor = true;
            // 
            // lblVbOffset
            // 
            this.lblVbOffset.AutoSize = true;
            this.lblVbOffset.Location = new System.Drawing.Point(3, 75);
            this.lblVbOffset.Name = "lblVbOffset";
            this.lblVbOffset.Size = new System.Drawing.Size(52, 13);
            this.lblVbOffset.TabIndex = 5;
            this.lblVbOffset.Text = "VB Offset";
            // 
            // tbVbOffset
            // 
            this.tbVbOffset.Location = new System.Drawing.Point(86, 72);
            this.tbVbOffset.Name = "tbVbOffset";
            this.tbVbOffset.Size = new System.Drawing.Size(218, 20);
            this.tbVbOffset.TabIndex = 4;
            // 
            // lblVhOffset
            // 
            this.lblVhOffset.AutoSize = true;
            this.lblVhOffset.Location = new System.Drawing.Point(3, 48);
            this.lblVhOffset.Name = "lblVhOffset";
            this.lblVhOffset.Size = new System.Drawing.Size(53, 13);
            this.lblVhOffset.TabIndex = 3;
            this.lblVhOffset.Text = "VH Offset";
            // 
            // tbVhOffset
            // 
            this.tbVhOffset.Location = new System.Drawing.Point(86, 45);
            this.tbVhOffset.Name = "tbVhOffset";
            this.tbVhOffset.Size = new System.Drawing.Size(218, 20);
            this.tbVhOffset.TabIndex = 2;
            // 
            // lblSeqOffset
            // 
            this.lblSeqOffset.AutoSize = true;
            this.lblSeqOffset.Location = new System.Drawing.Point(3, 22);
            this.lblSeqOffset.Name = "lblSeqOffset";
            this.lblSeqOffset.Size = new System.Drawing.Size(60, 13);
            this.lblSeqOffset.TabIndex = 1;
            this.lblSeqOffset.Text = "SEQ Offset";
            // 
            // tbSeqOffset
            // 
            this.tbSeqOffset.Location = new System.Drawing.Point(86, 19);
            this.tbSeqOffset.Name = "tbSeqOffset";
            this.tbSeqOffset.Size = new System.Drawing.Size(218, 20);
            this.tbSeqOffset.TabIndex = 0;
            // 
            // grpGenericDrivers
            // 
            this.grpGenericDrivers.Controls.Add(this.lblGenericDriver);
            this.grpGenericDrivers.Controls.Add(this.genericDriver);
            this.grpGenericDrivers.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpGenericDrivers.Location = new System.Drawing.Point(0, 23);
            this.grpGenericDrivers.Name = "grpGenericDrivers";
            this.grpGenericDrivers.Size = new System.Drawing.Size(857, 47);
            this.grpGenericDrivers.TabIndex = 7;
            this.grpGenericDrivers.TabStop = false;
            this.grpGenericDrivers.Text = "Presets";
            // 
            // lblGenericDriver
            // 
            this.lblGenericDriver.AutoSize = true;
            this.lblGenericDriver.Location = new System.Drawing.Point(3, 22);
            this.lblGenericDriver.Name = "lblGenericDriver";
            this.lblGenericDriver.Size = new System.Drawing.Size(35, 13);
            this.lblGenericDriver.TabIndex = 1;
            this.lblGenericDriver.Text = "Driver";
            // 
            // genericDriver
            // 
            this.genericDriver.FormattingEnabled = true;
            this.genericDriver.Location = new System.Drawing.Point(86, 19);
            this.genericDriver.Name = "genericDriver";
            this.genericDriver.Size = new System.Drawing.Size(249, 21);
            this.genericDriver.TabIndex = 0;
            this.genericDriver.SelectedIndexChanged += new System.EventHandler(this.genericDriver_SelectedIndexChanged);
            this.genericDriver.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.genericDriver_KeyPress);
            this.genericDriver.KeyDown += new System.Windows.Forms.KeyEventHandler(this.genericDriver_KeyDown);
            // 
            // Bin2PsfFrontEndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 472);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpSource);
            this.Controls.Add(this.grpGenericDrivers);
            this.Name = "Bin2PsfFrontEndForm";
            this.Text = "Bin2PsfFrontEndForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpGenericDrivers, 0);
            this.Controls.SetChildIndex(this.grpSource, 0);
            this.Controls.SetChildIndex(this.grpOptions, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.grpGenericDrivers.ResumeLayout(false);
            this.grpGenericDrivers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.TextBox tbExePath;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.TextBox tbSeqOffset;
        private System.Windows.Forms.Label lblSeqOffset;
        private System.Windows.Forms.Label lblVbOffset;
        private System.Windows.Forms.TextBox tbVbOffset;
        private System.Windows.Forms.Label lblVhOffset;
        private System.Windows.Forms.TextBox tbVhOffset;
        private System.Windows.Forms.Label lblDriverPath;
        private System.Windows.Forms.Label lblSourceFiles;
        private System.Windows.Forms.TextBox tbSourceFilesPath;
        private System.Windows.Forms.Button btnExeBrowse;
        private System.Windows.Forms.Button btnSourceDirectoryBrowse;
        private System.Windows.Forms.TextBox tbOutputFolderName;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.CheckBox cbMinipsf;
        private System.Windows.Forms.Label lblPsfLibName;
        private System.Windows.Forms.TextBox tbPsflibName;
        private System.Windows.Forms.CheckBox cbAllowZeroLengthSequences;
        private System.Windows.Forms.GroupBox grpGenericDrivers;
        private System.Windows.Forms.ComboBox genericDriver;
        private System.Windows.Forms.Label lblGenericDriver;
        private System.Windows.Forms.CheckBox cbTryMixing;
    }
}