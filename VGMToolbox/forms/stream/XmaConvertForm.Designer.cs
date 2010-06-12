namespace VGMToolbox.forms.stream
{
    partial class XmaConvertForm
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
            this.grpPresets = new System.Windows.Forms.GroupBox();
            this.grpOutputOptions = new System.Windows.Forms.GroupBox();
            this.btnBrowseOutputFolder = new System.Windows.Forms.Button();
            this.lblOutputFolderDefault = new System.Windows.Forms.Label();
            this.tbOutputFolder = new System.Windows.Forms.TextBox();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.groupOtherOptions = new System.Windows.Forms.GroupBox();
            this.cbShowAllExeOutput = new System.Windows.Forms.CheckBox();
            this.grpRiffHeaderOptions = new System.Windows.Forms.GroupBox();
            this.cbAddRiffHeader = new System.Windows.Forms.CheckBox();
            this.comboRiffChannels = new System.Windows.Forms.ComboBox();
            this.lblRiffChannels = new System.Windows.Forms.Label();
            this.comboRiffFrequency = new System.Windows.Forms.ComboBox();
            this.lblRiffFrequency = new System.Windows.Forms.Label();
            this.grpXmaParseOptions = new System.Windows.Forms.GroupBox();
            this.tbXmaParseDataSize = new System.Windows.Forms.TextBox();
            this.lblXmaParseDataSize = new System.Windows.Forms.Label();
            this.cbXmaParseDoRebuild = new System.Windows.Forms.CheckBox();
            this.cbDoXmaParse = new System.Windows.Forms.CheckBox();
            this.tbXmaParseBlockSize = new System.Windows.Forms.TextBox();
            this.lblXmaParseBlockSize = new System.Windows.Forms.Label();
            this.tbXmaParseStartOffset = new System.Windows.Forms.TextBox();
            this.lblXmaParseOffset = new System.Windows.Forms.Label();
            this.comboXmaParseInputType = new System.Windows.Forms.ComboBox();
            this.lblXmaParseInputType = new System.Windows.Forms.Label();
            this.cbXmaParseIgnoreErrors = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpOutputOptions.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.groupOtherOptions.SuspendLayout();
            this.grpRiffHeaderOptions.SuspendLayout();
            this.grpXmaParseOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 622);
            this.pnlLabels.Size = new System.Drawing.Size(789, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(789, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 545);
            this.tbOutput.Size = new System.Drawing.Size(789, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 525);
            this.pnlButtons.Size = new System.Drawing.Size(789, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(729, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(669, 0);
            // 
            // grpPresets
            // 
            this.grpPresets.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpPresets.Location = new System.Drawing.Point(0, 23);
            this.grpPresets.Name = "grpPresets";
            this.grpPresets.Size = new System.Drawing.Size(789, 40);
            this.grpPresets.TabIndex = 5;
            this.grpPresets.TabStop = false;
            this.grpPresets.Text = "Presets";
            // 
            // grpOutputOptions
            // 
            this.grpOutputOptions.Controls.Add(this.btnBrowseOutputFolder);
            this.grpOutputOptions.Controls.Add(this.lblOutputFolderDefault);
            this.grpOutputOptions.Controls.Add(this.tbOutputFolder);
            this.grpOutputOptions.Controls.Add(this.lblOutputFolder);
            this.grpOutputOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpOutputOptions.Location = new System.Drawing.Point(0, 63);
            this.grpOutputOptions.Name = "grpOutputOptions";
            this.grpOutputOptions.Size = new System.Drawing.Size(789, 70);
            this.grpOutputOptions.TabIndex = 6;
            this.grpOutputOptions.TabStop = false;
            this.grpOutputOptions.Text = "Output Options";
            // 
            // btnBrowseOutputFolder
            // 
            this.btnBrowseOutputFolder.Location = new System.Drawing.Point(412, 19);
            this.btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
            this.btnBrowseOutputFolder.Size = new System.Drawing.Size(25, 20);
            this.btnBrowseOutputFolder.TabIndex = 3;
            this.btnBrowseOutputFolder.Text = "...";
            this.btnBrowseOutputFolder.UseVisualStyleBackColor = true;
            this.btnBrowseOutputFolder.Click += new System.EventHandler(this.btnBrowseOutputFolder_Click);
            // 
            // lblOutputFolderDefault
            // 
            this.lblOutputFolderDefault.AutoSize = true;
            this.lblOutputFolderDefault.Location = new System.Drawing.Point(84, 42);
            this.lblOutputFolderDefault.Name = "lblOutputFolderDefault";
            this.lblOutputFolderDefault.Size = new System.Drawing.Size(127, 13);
            this.lblOutputFolderDefault.TabIndex = 2;
            this.lblOutputFolderDefault.Text = "(Leave empty for default.)";
            // 
            // tbOutputFolder
            // 
            this.tbOutputFolder.AllowDrop = true;
            this.tbOutputFolder.Location = new System.Drawing.Point(83, 19);
            this.tbOutputFolder.Name = "tbOutputFolder";
            this.tbOutputFolder.Size = new System.Drawing.Size(323, 20);
            this.tbOutputFolder.TabIndex = 1;
            this.tbOutputFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbOutputFolder_DragDrop);
            this.tbOutputFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbOutputFolder_DragEnter);
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.AutoSize = true;
            this.lblOutputFolder.Location = new System.Drawing.Point(6, 22);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(71, 13);
            this.lblOutputFolder.TabIndex = 0;
            this.lblOutputFolder.Text = "Output Folder";
            // 
            // pnlOptions
            // 
            this.pnlOptions.AutoScroll = true;
            this.pnlOptions.Controls.Add(this.groupOtherOptions);
            this.pnlOptions.Controls.Add(this.grpRiffHeaderOptions);
            this.pnlOptions.Controls.Add(this.grpXmaParseOptions);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Location = new System.Drawing.Point(0, 133);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(789, 392);
            this.pnlOptions.TabIndex = 7;
            // 
            // groupOtherOptions
            // 
            this.groupOtherOptions.Controls.Add(this.cbShowAllExeOutput);
            this.groupOtherOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupOtherOptions.Location = new System.Drawing.Point(0, 171);
            this.groupOtherOptions.Name = "groupOtherOptions";
            this.groupOtherOptions.Size = new System.Drawing.Size(789, 39);
            this.groupOtherOptions.TabIndex = 2;
            this.groupOtherOptions.TabStop = false;
            this.groupOtherOptions.Text = "Other Options";
            // 
            // cbShowAllExeOutput
            // 
            this.cbShowAllExeOutput.AutoSize = true;
            this.cbShowAllExeOutput.Location = new System.Drawing.Point(6, 19);
            this.cbShowAllExeOutput.Name = "cbShowAllExeOutput";
            this.cbShowAllExeOutput.Size = new System.Drawing.Size(226, 17);
            this.cbShowAllExeOutput.TabIndex = 0;
            this.cbShowAllExeOutput.Text = "Show All Output from External Applications";
            this.cbShowAllExeOutput.UseVisualStyleBackColor = true;
            // 
            // grpRiffHeaderOptions
            // 
            this.grpRiffHeaderOptions.Controls.Add(this.cbAddRiffHeader);
            this.grpRiffHeaderOptions.Controls.Add(this.comboRiffChannels);
            this.grpRiffHeaderOptions.Controls.Add(this.lblRiffChannels);
            this.grpRiffHeaderOptions.Controls.Add(this.comboRiffFrequency);
            this.grpRiffHeaderOptions.Controls.Add(this.lblRiffFrequency);
            this.grpRiffHeaderOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpRiffHeaderOptions.Location = new System.Drawing.Point(0, 108);
            this.grpRiffHeaderOptions.Name = "grpRiffHeaderOptions";
            this.grpRiffHeaderOptions.Size = new System.Drawing.Size(789, 63);
            this.grpRiffHeaderOptions.TabIndex = 1;
            this.grpRiffHeaderOptions.TabStop = false;
            this.grpRiffHeaderOptions.Text = "RIFF Header Options";
            // 
            // cbAddRiffHeader
            // 
            this.cbAddRiffHeader.AutoSize = true;
            this.cbAddRiffHeader.Location = new System.Drawing.Point(3, 19);
            this.cbAddRiffHeader.Name = "cbAddRiffHeader";
            this.cbAddRiffHeader.Size = new System.Drawing.Size(109, 17);
            this.cbAddRiffHeader.TabIndex = 4;
            this.cbAddRiffHeader.Text = "Add RIFF Header";
            this.cbAddRiffHeader.UseVisualStyleBackColor = true;
            this.cbAddRiffHeader.CheckedChanged += new System.EventHandler(this.cbAddRiffHeader_CheckedChanged);
            // 
            // comboRiffChannels
            // 
            this.comboRiffChannels.FormattingEnabled = true;
            this.comboRiffChannels.Location = new System.Drawing.Point(230, 36);
            this.comboRiffChannels.Name = "comboRiffChannels";
            this.comboRiffChannels.Size = new System.Drawing.Size(79, 21);
            this.comboRiffChannels.TabIndex = 3;
            this.comboRiffChannels.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboRiffChannels_KeyPress);
            this.comboRiffChannels.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboRiffChannels_KeyDown);
            // 
            // lblRiffChannels
            // 
            this.lblRiffChannels.AutoSize = true;
            this.lblRiffChannels.Location = new System.Drawing.Point(173, 39);
            this.lblRiffChannels.Name = "lblRiffChannels";
            this.lblRiffChannels.Size = new System.Drawing.Size(51, 13);
            this.lblRiffChannels.TabIndex = 2;
            this.lblRiffChannels.Text = "Channels";
            // 
            // comboRiffFrequency
            // 
            this.comboRiffFrequency.FormattingEnabled = true;
            this.comboRiffFrequency.Location = new System.Drawing.Point(83, 36);
            this.comboRiffFrequency.Name = "comboRiffFrequency";
            this.comboRiffFrequency.Size = new System.Drawing.Size(77, 21);
            this.comboRiffFrequency.TabIndex = 1;
            this.comboRiffFrequency.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboRiffFrequency_KeyPress);
            this.comboRiffFrequency.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboRiffFrequency_KeyDown);
            // 
            // lblRiffFrequency
            // 
            this.lblRiffFrequency.AutoSize = true;
            this.lblRiffFrequency.Location = new System.Drawing.Point(20, 39);
            this.lblRiffFrequency.Name = "lblRiffFrequency";
            this.lblRiffFrequency.Size = new System.Drawing.Size(57, 13);
            this.lblRiffFrequency.TabIndex = 0;
            this.lblRiffFrequency.Text = "Frequency";
            // 
            // grpXmaParseOptions
            // 
            this.grpXmaParseOptions.Controls.Add(this.cbXmaParseIgnoreErrors);
            this.grpXmaParseOptions.Controls.Add(this.tbXmaParseDataSize);
            this.grpXmaParseOptions.Controls.Add(this.lblXmaParseDataSize);
            this.grpXmaParseOptions.Controls.Add(this.cbXmaParseDoRebuild);
            this.grpXmaParseOptions.Controls.Add(this.cbDoXmaParse);
            this.grpXmaParseOptions.Controls.Add(this.tbXmaParseBlockSize);
            this.grpXmaParseOptions.Controls.Add(this.lblXmaParseBlockSize);
            this.grpXmaParseOptions.Controls.Add(this.tbXmaParseStartOffset);
            this.grpXmaParseOptions.Controls.Add(this.lblXmaParseOffset);
            this.grpXmaParseOptions.Controls.Add(this.comboXmaParseInputType);
            this.grpXmaParseOptions.Controls.Add(this.lblXmaParseInputType);
            this.grpXmaParseOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpXmaParseOptions.Location = new System.Drawing.Point(0, 0);
            this.grpXmaParseOptions.Name = "grpXmaParseOptions";
            this.grpXmaParseOptions.Size = new System.Drawing.Size(789, 108);
            this.grpXmaParseOptions.TabIndex = 0;
            this.grpXmaParseOptions.TabStop = false;
            this.grpXmaParseOptions.Text = "xma_parse Options";
            // 
            // tbXmaParseDataSize
            // 
            this.tbXmaParseDataSize.Location = new System.Drawing.Point(363, 62);
            this.tbXmaParseDataSize.Name = "tbXmaParseDataSize";
            this.tbXmaParseDataSize.Size = new System.Drawing.Size(60, 20);
            this.tbXmaParseDataSize.TabIndex = 9;
            // 
            // lblXmaParseDataSize
            // 
            this.lblXmaParseDataSize.AutoSize = true;
            this.lblXmaParseDataSize.Location = new System.Drawing.Point(300, 65);
            this.lblXmaParseDataSize.Name = "lblXmaParseDataSize";
            this.lblXmaParseDataSize.Size = new System.Drawing.Size(53, 13);
            this.lblXmaParseDataSize.TabIndex = 8;
            this.lblXmaParseDataSize.Text = "Data Size";
            // 
            // cbXmaParseDoRebuild
            // 
            this.cbXmaParseDoRebuild.AutoSize = true;
            this.cbXmaParseDoRebuild.Location = new System.Drawing.Point(23, 64);
            this.cbXmaParseDoRebuild.Name = "cbXmaParseDoRebuild";
            this.cbXmaParseDoRebuild.Size = new System.Drawing.Size(161, 17);
            this.cbXmaParseDoRebuild.TabIndex = 7;
            this.cbXmaParseDoRebuild.Text = "Use Rebuild XMA1 Mode (-r)";
            this.cbXmaParseDoRebuild.UseVisualStyleBackColor = true;
            // 
            // cbDoXmaParse
            // 
            this.cbDoXmaParse.AutoSize = true;
            this.cbDoXmaParse.Location = new System.Drawing.Point(3, 19);
            this.cbDoXmaParse.Name = "cbDoXmaParse";
            this.cbDoXmaParse.Size = new System.Drawing.Size(119, 17);
            this.cbDoXmaParse.TabIndex = 6;
            this.cbDoXmaParse.Text = "Use xma_parse.exe";
            this.cbDoXmaParse.UseVisualStyleBackColor = true;
            this.cbDoXmaParse.CheckedChanged += new System.EventHandler(this.cbDoXmaParse_CheckedChanged);
            // 
            // tbXmaParseBlockSize
            // 
            this.tbXmaParseBlockSize.Location = new System.Drawing.Point(363, 36);
            this.tbXmaParseBlockSize.Name = "tbXmaParseBlockSize";
            this.tbXmaParseBlockSize.Size = new System.Drawing.Size(60, 20);
            this.tbXmaParseBlockSize.TabIndex = 5;
            // 
            // lblXmaParseBlockSize
            // 
            this.lblXmaParseBlockSize.AutoSize = true;
            this.lblXmaParseBlockSize.Location = new System.Drawing.Point(300, 39);
            this.lblXmaParseBlockSize.Name = "lblXmaParseBlockSize";
            this.lblXmaParseBlockSize.Size = new System.Drawing.Size(57, 13);
            this.lblXmaParseBlockSize.TabIndex = 4;
            this.lblXmaParseBlockSize.Text = "Block Size";
            // 
            // tbXmaParseStartOffset
            // 
            this.tbXmaParseStartOffset.Location = new System.Drawing.Point(221, 36);
            this.tbXmaParseStartOffset.Name = "tbXmaParseStartOffset";
            this.tbXmaParseStartOffset.Size = new System.Drawing.Size(60, 20);
            this.tbXmaParseStartOffset.TabIndex = 3;
            // 
            // lblXmaParseOffset
            // 
            this.lblXmaParseOffset.AutoSize = true;
            this.lblXmaParseOffset.Location = new System.Drawing.Point(155, 39);
            this.lblXmaParseOffset.Name = "lblXmaParseOffset";
            this.lblXmaParseOffset.Size = new System.Drawing.Size(60, 13);
            this.lblXmaParseOffset.TabIndex = 2;
            this.lblXmaParseOffset.Text = "Start Offset";
            // 
            // comboXmaParseInputType
            // 
            this.comboXmaParseInputType.FormattingEnabled = true;
            this.comboXmaParseInputType.Location = new System.Drawing.Point(86, 36);
            this.comboXmaParseInputType.Name = "comboXmaParseInputType";
            this.comboXmaParseInputType.Size = new System.Drawing.Size(50, 21);
            this.comboXmaParseInputType.TabIndex = 1;
            this.comboXmaParseInputType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboXmaParseInputType_KeyPress);
            this.comboXmaParseInputType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboXmaParseInputType_KeyDown);
            // 
            // lblXmaParseInputType
            // 
            this.lblXmaParseInputType.AutoSize = true;
            this.lblXmaParseInputType.Location = new System.Drawing.Point(20, 39);
            this.lblXmaParseInputType.Name = "lblXmaParseInputType";
            this.lblXmaParseInputType.Size = new System.Drawing.Size(57, 13);
            this.lblXmaParseInputType.TabIndex = 0;
            this.lblXmaParseInputType.Text = "XMA Type";
            // 
            // cbXmaParseIgnoreErrors
            // 
            this.cbXmaParseIgnoreErrors.AutoSize = true;
            this.cbXmaParseIgnoreErrors.Location = new System.Drawing.Point(23, 87);
            this.cbXmaParseIgnoreErrors.Name = "cbXmaParseIgnoreErrors";
            this.cbXmaParseIgnoreErrors.Size = new System.Drawing.Size(86, 17);
            this.cbXmaParseIgnoreErrors.TabIndex = 10;
            this.cbXmaParseIgnoreErrors.Text = "Ignore Errors";
            this.cbXmaParseIgnoreErrors.UseVisualStyleBackColor = true;
            // 
            // XmaConvertForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 663);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.grpOutputOptions);
            this.Controls.Add(this.grpPresets);
            this.Name = "XmaConvertForm";
            this.Text = "XmaConvertForm";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.XmaConvertForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.XmaConvertForm_DragEnter);
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpPresets, 0);
            this.Controls.SetChildIndex(this.grpOutputOptions, 0);
            this.Controls.SetChildIndex(this.pnlOptions, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpOutputOptions.ResumeLayout(false);
            this.grpOutputOptions.PerformLayout();
            this.pnlOptions.ResumeLayout(false);
            this.groupOtherOptions.ResumeLayout(false);
            this.groupOtherOptions.PerformLayout();
            this.grpRiffHeaderOptions.ResumeLayout(false);
            this.grpRiffHeaderOptions.PerformLayout();
            this.grpXmaParseOptions.ResumeLayout(false);
            this.grpXmaParseOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPresets;
        private System.Windows.Forms.GroupBox grpOutputOptions;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.GroupBox grpXmaParseOptions;
        private System.Windows.Forms.Label lblXmaParseInputType;
        private System.Windows.Forms.ComboBox comboXmaParseInputType;
        private System.Windows.Forms.TextBox tbXmaParseBlockSize;
        private System.Windows.Forms.Label lblXmaParseBlockSize;
        private System.Windows.Forms.TextBox tbXmaParseStartOffset;
        private System.Windows.Forms.Label lblXmaParseOffset;
        private System.Windows.Forms.GroupBox grpRiffHeaderOptions;
        private System.Windows.Forms.ComboBox comboRiffFrequency;
        private System.Windows.Forms.Label lblRiffFrequency;
        private System.Windows.Forms.Label lblRiffChannels;
        private System.Windows.Forms.ComboBox comboRiffChannels;
        private System.Windows.Forms.GroupBox groupOtherOptions;
        private System.Windows.Forms.CheckBox cbShowAllExeOutput;
        private System.Windows.Forms.Label lblOutputFolderDefault;
        private System.Windows.Forms.TextBox tbOutputFolder;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.Button btnBrowseOutputFolder;
        private System.Windows.Forms.CheckBox cbDoXmaParse;
        private System.Windows.Forms.CheckBox cbAddRiffHeader;
        private System.Windows.Forms.CheckBox cbXmaParseDoRebuild;
        private System.Windows.Forms.Label lblXmaParseDataSize;
        private System.Windows.Forms.TextBox tbXmaParseDataSize;
        private System.Windows.Forms.CheckBox cbXmaParseIgnoreErrors;
    }
}