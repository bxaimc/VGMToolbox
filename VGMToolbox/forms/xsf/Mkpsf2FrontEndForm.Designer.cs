namespace VGMToolbox.forms.xsf
{
    partial class Mkpsf2FrontEndForm
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
            this.grpDirectory = new System.Windows.Forms.GroupBox();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.tbOutputFolderName = new System.Windows.Forms.TextBox();
            this.btnModulesDirectoryBrowse = new System.Windows.Forms.Button();
            this.tbModulesDirectory = new System.Windows.Forms.TextBox();
            this.lblModulesDirectory = new System.Windows.Forms.Label();
            this.btnSourceDirectoryBrowse = new System.Windows.Forms.Button();
            this.lblSourceDirectory = new System.Windows.Forms.Label();
            this.tbSourceDirectory = new System.Windows.Forms.TextBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.lblVolume = new System.Windows.Forms.Label();
            this.tbVolume = new System.Windows.Forms.TextBox();
            this.tbTempo = new System.Windows.Forms.TextBox();
            this.lblTempo = new System.Windows.Forms.Label();
            this.tbTickInterval = new System.Windows.Forms.TextBox();
            this.lblTickInterval = new System.Windows.Forms.Label();
            this.tbDepth = new System.Windows.Forms.TextBox();
            this.lblDepth = new System.Windows.Forms.Label();
            this.lblReverb = new System.Windows.Forms.Label();
            this.tbReverb = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.cbTryCombinations = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpDirectory.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 507);
            this.pnlLabels.Size = new System.Drawing.Size(638, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(638, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 430);
            this.tbOutput.Size = new System.Drawing.Size(638, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 410);
            this.pnlButtons.Size = new System.Drawing.Size(638, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(578, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(518, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // grpDirectory
            // 
            this.grpDirectory.Controls.Add(this.cbTryCombinations);
            this.grpDirectory.Controls.Add(this.lblOutputFolder);
            this.grpDirectory.Controls.Add(this.tbOutputFolderName);
            this.grpDirectory.Controls.Add(this.btnModulesDirectoryBrowse);
            this.grpDirectory.Controls.Add(this.tbModulesDirectory);
            this.grpDirectory.Controls.Add(this.lblModulesDirectory);
            this.grpDirectory.Controls.Add(this.btnSourceDirectoryBrowse);
            this.grpDirectory.Controls.Add(this.lblSourceDirectory);
            this.grpDirectory.Controls.Add(this.tbSourceDirectory);
            this.grpDirectory.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDirectory.Location = new System.Drawing.Point(0, 23);
            this.grpDirectory.Name = "grpDirectory";
            this.grpDirectory.Size = new System.Drawing.Size(638, 158);
            this.grpDirectory.TabIndex = 5;
            this.grpDirectory.TabStop = false;
            this.grpDirectory.Text = "Directories";
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(5, 94);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(102, 13);
            this.lblOutputFolder.TabIndex = 16;
            this.lblOutputFolder.Text = "Output Folder Name";
            // 
            // tbOutputFolderName
            // 
            this.tbOutputFolderName.Location = new System.Drawing.Point(9, 110);
            this.tbOutputFolderName.Name = "tbOutputFolderName";
            this.tbOutputFolderName.Size = new System.Drawing.Size(235, 20);
            this.tbOutputFolderName.TabIndex = 15;
            this.tbOutputFolderName.Text = "myPSF2Set";
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
            // lblModulesDirectory
            // 
            this.lblModulesDirectory.AutoSize = true;
            this.lblModulesDirectory.Location = new System.Drawing.Point(6, 55);
            this.lblModulesDirectory.Name = "lblModulesDirectory";
            this.lblModulesDirectory.Size = new System.Drawing.Size(92, 13);
            this.lblModulesDirectory.TabIndex = 12;
            this.lblModulesDirectory.Text = "Modules Directory";
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
            // lblSourceDirectory
            // 
            this.lblSourceDirectory.AutoSize = true;
            this.lblSourceDirectory.Location = new System.Drawing.Point(6, 16);
            this.lblSourceDirectory.Name = "lblSourceDirectory";
            this.lblSourceDirectory.Size = new System.Drawing.Size(86, 13);
            this.lblSourceDirectory.TabIndex = 10;
            this.lblSourceDirectory.Text = "Source Directory";
            // 
            // tbSourceDirectory
            // 
            this.tbSourceDirectory.Location = new System.Drawing.Point(9, 32);
            this.tbSourceDirectory.Name = "tbSourceDirectory";
            this.tbSourceDirectory.Size = new System.Drawing.Size(235, 20);
            this.tbSourceDirectory.TabIndex = 9;
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.lblVolume);
            this.grpOptions.Controls.Add(this.tbVolume);
            this.grpOptions.Controls.Add(this.tbTempo);
            this.grpOptions.Controls.Add(this.lblTempo);
            this.grpOptions.Controls.Add(this.tbTickInterval);
            this.grpOptions.Controls.Add(this.lblTickInterval);
            this.grpOptions.Controls.Add(this.tbDepth);
            this.grpOptions.Controls.Add(this.lblDepth);
            this.grpOptions.Controls.Add(this.lblReverb);
            this.grpOptions.Controls.Add(this.tbReverb);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpOptions.Location = new System.Drawing.Point(0, 181);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(638, 73);
            this.grpOptions.TabIndex = 10;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options (Leave empty for defaults)";
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(233, 15);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(42, 13);
            this.lblVolume.TabIndex = 9;
            this.lblVolume.Text = "Volume";
            // 
            // tbVolume
            // 
            this.tbVolume.Location = new System.Drawing.Point(281, 11);
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
            // lblTempo
            // 
            this.lblTempo.AutoSize = true;
            this.lblTempo.Location = new System.Drawing.Point(6, 47);
            this.lblTempo.Name = "lblTempo";
            this.lblTempo.Size = new System.Drawing.Size(40, 13);
            this.lblTempo.TabIndex = 6;
            this.lblTempo.Text = "Tempo";
            // 
            // tbTickInterval
            // 
            this.tbTickInterval.Location = new System.Drawing.Point(174, 43);
            this.tbTickInterval.MaxLength = 4;
            this.tbTickInterval.Name = "tbTickInterval";
            this.tbTickInterval.Size = new System.Drawing.Size(47, 20);
            this.tbTickInterval.TabIndex = 5;
            // 
            // lblTickInterval
            // 
            this.lblTickInterval.AutoSize = true;
            this.lblTickInterval.Location = new System.Drawing.Point(102, 47);
            this.lblTickInterval.Name = "lblTickInterval";
            this.lblTickInterval.Size = new System.Drawing.Size(66, 13);
            this.lblTickInterval.TabIndex = 4;
            this.lblTickInterval.Text = "Tick Interval";
            // 
            // tbDepth
            // 
            this.tbDepth.Location = new System.Drawing.Point(174, 12);
            this.tbDepth.MaxLength = 5;
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(47, 20);
            this.tbDepth.TabIndex = 3;
            // 
            // lblDepth
            // 
            this.lblDepth.AutoSize = true;
            this.lblDepth.Location = new System.Drawing.Point(102, 18);
            this.lblDepth.Name = "lblDepth";
            this.lblDepth.Size = new System.Drawing.Size(36, 13);
            this.lblDepth.TabIndex = 2;
            this.lblDepth.Text = "Depth";
            // 
            // lblReverb
            // 
            this.lblReverb.AutoSize = true;
            this.lblReverb.Location = new System.Drawing.Point(5, 19);
            this.lblReverb.Name = "lblReverb";
            this.lblReverb.Size = new System.Drawing.Size(42, 13);
            this.lblReverb.TabIndex = 1;
            this.lblReverb.Text = "Reverb";
            // 
            // tbReverb
            // 
            this.tbReverb.Location = new System.Drawing.Point(49, 15);
            this.tbReverb.MaxLength = 1;
            this.tbReverb.Name = "tbReverb";
            this.tbReverb.Size = new System.Drawing.Size(19, 20);
            this.tbReverb.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblAuthor);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 254);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 25);
            this.panel1.TabIndex = 11;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAuthor.Location = new System.Drawing.Point(0, 0);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Padding = new System.Windows.Forms.Padding(5);
            this.lblAuthor.Size = new System.Drawing.Size(330, 23);
            this.lblAuthor.TabIndex = 0;
            this.lblAuthor.Text = "mkpsf2.exe is written by Neill Corlett (http://www.neillcorlett.com/).";
            // 
            // cbTryCombinations
            // 
            this.cbTryCombinations.AutoSize = true;
            this.cbTryCombinations.Location = new System.Drawing.Point(9, 136);
            this.cbTryCombinations.Name = "cbTryCombinations";
            this.cbTryCombinations.Size = new System.Drawing.Size(365, 17);
            this.cbTryCombinations.TabIndex = 17;
            this.cbTryCombinations.Text = "Try all combinations of SQ and HD/BD (good for finding matching pairs).";
            this.cbTryCombinations.UseVisualStyleBackColor = true;
            // 
            // Mkpsf2FrontEndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 548);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpDirectory);
            this.Name = "Mkpsf2FrontEndForm";
            this.Text = "Mkpsf2FrontEndForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpDirectory, 0);
            this.Controls.SetChildIndex(this.grpOptions, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpDirectory.ResumeLayout(false);
            this.grpDirectory.PerformLayout();
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDirectory;
        private System.Windows.Forms.Button btnModulesDirectoryBrowse;
        private System.Windows.Forms.TextBox tbModulesDirectory;
        private System.Windows.Forms.Label lblModulesDirectory;
        private System.Windows.Forms.Button btnSourceDirectoryBrowse;
        private System.Windows.Forms.Label lblSourceDirectory;
        private System.Windows.Forms.TextBox tbSourceDirectory;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.TextBox tbVolume;
        private System.Windows.Forms.TextBox tbTempo;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.TextBox tbTickInterval;
        private System.Windows.Forms.Label lblTickInterval;
        private System.Windows.Forms.TextBox tbDepth;
        private System.Windows.Forms.Label lblDepth;
        private System.Windows.Forms.Label lblReverb;
        private System.Windows.Forms.TextBox tbReverb;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.TextBox tbOutputFolderName;
        private System.Windows.Forms.CheckBox cbTryCombinations;
    }
}