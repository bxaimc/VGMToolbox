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
            this.cbKeepTempFiles = new System.Windows.Forms.CheckBox();
            this.cbShowAllExeOutput = new System.Windows.Forms.CheckBox();
            this.grpToWavOptions = new System.Windows.Forms.GroupBox();
            this.cbDoToWav = new System.Windows.Forms.CheckBox();
            this.grpRiffHeaderOptions = new System.Windows.Forms.GroupBox();
            this.cbAddRiffHeader = new System.Windows.Forms.CheckBox();
            this.comboRiffChannels = new System.Windows.Forms.ComboBox();
            this.lblRiffChannels = new System.Windows.Forms.Label();
            this.comboRiffFrequency = new System.Windows.Forms.ComboBox();
            this.lblRiffFrequency = new System.Windows.Forms.Label();
            this.grpXmaParseOptions = new System.Windows.Forms.GroupBox();
            this.grpStartOffset = new System.Windows.Forms.GroupBox();
            this.XmaParseStartOffsetOffsetDescription = new VGMToolbox.forms.OffsetDescriptionControl();
            this.rbXmaParseStartOffsetOffset = new System.Windows.Forms.RadioButton();
            this.rbXmaParseStartOffsetStatic = new System.Windows.Forms.RadioButton();
            this.tbXmaParseStartOffset = new System.Windows.Forms.TextBox();
            this.grpBlockSize = new System.Windows.Forms.GroupBox();
            this.XmaParseBlockSizeOffsetDescription = new VGMToolbox.forms.OffsetDescriptionControl();
            this.rbXmaParseBlockSizeOffset = new System.Windows.Forms.RadioButton();
            this.rbXmaParseBlockSizeStatic = new System.Windows.Forms.RadioButton();
            this.tbXmaParseBlockSize = new System.Windows.Forms.TextBox();
            this.grpXmaParseDataSize = new System.Windows.Forms.GroupBox();
            this.XmaParseDataSizeOffsetDescription = new VGMToolbox.forms.OffsetDescriptionControl();
            this.rbXmaParseDataSizeOffset = new System.Windows.Forms.RadioButton();
            this.rbXmaParseDataSizeStatic = new System.Windows.Forms.RadioButton();
            this.tbXmaParseDataSize = new System.Windows.Forms.TextBox();
            this.cbXmaParseIgnoreErrors = new System.Windows.Forms.CheckBox();
            this.cbXmaParseDoRebuild = new System.Windows.Forms.CheckBox();
            this.cbDoXmaParse = new System.Windows.Forms.CheckBox();
            this.comboXmaParseInputType = new System.Windows.Forms.ComboBox();
            this.lblXmaParseInputType = new System.Windows.Forms.Label();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpOutputOptions.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.groupOtherOptions.SuspendLayout();
            this.grpToWavOptions.SuspendLayout();
            this.grpRiffHeaderOptions.SuspendLayout();
            this.grpXmaParseOptions.SuspendLayout();
            this.grpStartOffset.SuspendLayout();
            this.grpBlockSize.SuspendLayout();
            this.grpXmaParseDataSize.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 437);
            this.pnlLabels.Size = new System.Drawing.Size(789, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(789, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 360);
            this.tbOutput.Size = new System.Drawing.Size(789, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 340);
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
            this.pnlOptions.Controls.Add(this.grpToWavOptions);
            this.pnlOptions.Controls.Add(this.grpRiffHeaderOptions);
            this.pnlOptions.Controls.Add(this.grpXmaParseOptions);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Location = new System.Drawing.Point(0, 133);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(789, 207);
            this.pnlOptions.TabIndex = 7;
            // 
            // groupOtherOptions
            // 
            this.groupOtherOptions.Controls.Add(this.cbKeepTempFiles);
            this.groupOtherOptions.Controls.Add(this.cbShowAllExeOutput);
            this.groupOtherOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupOtherOptions.Location = new System.Drawing.Point(0, 383);
            this.groupOtherOptions.Name = "groupOtherOptions";
            this.groupOtherOptions.Size = new System.Drawing.Size(772, 64);
            this.groupOtherOptions.TabIndex = 2;
            this.groupOtherOptions.TabStop = false;
            this.groupOtherOptions.Text = "Other Options";
            // 
            // cbKeepTempFiles
            // 
            this.cbKeepTempFiles.AutoSize = true;
            this.cbKeepTempFiles.Location = new System.Drawing.Point(6, 42);
            this.cbKeepTempFiles.Name = "cbKeepTempFiles";
            this.cbKeepTempFiles.Size = new System.Drawing.Size(160, 17);
            this.cbKeepTempFiles.TabIndex = 1;
            this.cbKeepTempFiles.Text = "Keep temp/intermediate files";
            this.cbKeepTempFiles.UseVisualStyleBackColor = true;
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
            // grpToWavOptions
            // 
            this.grpToWavOptions.Controls.Add(this.cbDoToWav);
            this.grpToWavOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpToWavOptions.Location = new System.Drawing.Point(0, 339);
            this.grpToWavOptions.Name = "grpToWavOptions";
            this.grpToWavOptions.Size = new System.Drawing.Size(772, 44);
            this.grpToWavOptions.TabIndex = 3;
            this.grpToWavOptions.TabStop = false;
            this.grpToWavOptions.Text = "ToWav.exe Options";
            // 
            // cbDoToWav
            // 
            this.cbDoToWav.AutoSize = true;
            this.cbDoToWav.Location = new System.Drawing.Point(3, 19);
            this.cbDoToWav.Name = "cbDoToWav";
            this.cbDoToWav.Size = new System.Drawing.Size(213, 17);
            this.cbDoToWav.TabIndex = 0;
            this.cbDoToWav.Text = "Execute ToWav.exe on the Ouput Files";
            this.cbDoToWav.UseVisualStyleBackColor = true;
            // 
            // grpRiffHeaderOptions
            // 
            this.grpRiffHeaderOptions.Controls.Add(this.cbAddRiffHeader);
            this.grpRiffHeaderOptions.Controls.Add(this.comboRiffChannels);
            this.grpRiffHeaderOptions.Controls.Add(this.lblRiffChannels);
            this.grpRiffHeaderOptions.Controls.Add(this.comboRiffFrequency);
            this.grpRiffHeaderOptions.Controls.Add(this.lblRiffFrequency);
            this.grpRiffHeaderOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpRiffHeaderOptions.Location = new System.Drawing.Point(0, 276);
            this.grpRiffHeaderOptions.Name = "grpRiffHeaderOptions";
            this.grpRiffHeaderOptions.Size = new System.Drawing.Size(772, 63);
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
            this.grpXmaParseOptions.Controls.Add(this.grpStartOffset);
            this.grpXmaParseOptions.Controls.Add(this.grpBlockSize);
            this.grpXmaParseOptions.Controls.Add(this.grpXmaParseDataSize);
            this.grpXmaParseOptions.Controls.Add(this.cbXmaParseIgnoreErrors);
            this.grpXmaParseOptions.Controls.Add(this.cbXmaParseDoRebuild);
            this.grpXmaParseOptions.Controls.Add(this.cbDoXmaParse);
            this.grpXmaParseOptions.Controls.Add(this.comboXmaParseInputType);
            this.grpXmaParseOptions.Controls.Add(this.lblXmaParseInputType);
            this.grpXmaParseOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpXmaParseOptions.Location = new System.Drawing.Point(0, 0);
            this.grpXmaParseOptions.Name = "grpXmaParseOptions";
            this.grpXmaParseOptions.Size = new System.Drawing.Size(772, 276);
            this.grpXmaParseOptions.TabIndex = 0;
            this.grpXmaParseOptions.TabStop = false;
            this.grpXmaParseOptions.Text = "xma_parse Options";
            // 
            // grpStartOffset
            // 
            this.grpStartOffset.Controls.Add(this.XmaParseStartOffsetOffsetDescription);
            this.grpStartOffset.Controls.Add(this.rbXmaParseStartOffsetOffset);
            this.grpStartOffset.Controls.Add(this.rbXmaParseStartOffsetStatic);
            this.grpStartOffset.Controls.Add(this.tbXmaParseStartOffset);
            this.grpStartOffset.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpStartOffset.Location = new System.Drawing.Point(3, 63);
            this.grpStartOffset.Name = "grpStartOffset";
            this.grpStartOffset.Size = new System.Drawing.Size(766, 74);
            this.grpStartOffset.TabIndex = 13;
            this.grpStartOffset.TabStop = false;
            this.grpStartOffset.Text = "Start Offset";
            // 
            // XmaParseStartOffsetOffsetDescription
            // 
            this.XmaParseStartOffsetOffsetDescription.Location = new System.Drawing.Point(136, 39);
            this.XmaParseStartOffsetOffsetDescription.Name = "XmaParseStartOffsetOffsetDescription";
            this.XmaParseStartOffsetOffsetDescription.OffsetByteOrder = "Little Endian";
            this.XmaParseStartOffsetOffsetDescription.OffsetSize = "4";
            this.XmaParseStartOffsetOffsetDescription.OffsetValue = "";
            this.XmaParseStartOffsetOffsetDescription.Size = new System.Drawing.Size(372, 27);
            this.XmaParseStartOffsetOffsetDescription.TabIndex = 6;
            // 
            // rbXmaParseStartOffsetOffset
            // 
            this.rbXmaParseStartOffsetOffset.AutoSize = true;
            this.rbXmaParseStartOffsetOffset.Location = new System.Drawing.Point(6, 42);
            this.rbXmaParseStartOffsetOffset.Name = "rbXmaParseStartOffsetOffset";
            this.rbXmaParseStartOffsetOffset.Size = new System.Drawing.Size(123, 17);
            this.rbXmaParseStartOffsetOffset.TabIndex = 5;
            this.rbXmaParseStartOffsetOffset.TabStop = true;
            this.rbXmaParseStartOffsetOffset.Text = "Start Size is at Offset";
            this.rbXmaParseStartOffsetOffset.UseVisualStyleBackColor = true;
            this.rbXmaParseStartOffsetOffset.CheckedChanged += new System.EventHandler(this.rbXmaParseStartOffsetOffset_CheckedChanged);
            // 
            // rbXmaParseStartOffsetStatic
            // 
            this.rbXmaParseStartOffsetStatic.AutoSize = true;
            this.rbXmaParseStartOffsetStatic.Location = new System.Drawing.Point(6, 19);
            this.rbXmaParseStartOffsetStatic.Name = "rbXmaParseStartOffsetStatic";
            this.rbXmaParseStartOffsetStatic.Size = new System.Drawing.Size(78, 17);
            this.rbXmaParseStartOffsetStatic.TabIndex = 4;
            this.rbXmaParseStartOffsetStatic.TabStop = true;
            this.rbXmaParseStartOffsetStatic.Text = "Start Offset";
            this.rbXmaParseStartOffsetStatic.UseVisualStyleBackColor = true;
            this.rbXmaParseStartOffsetStatic.CheckedChanged += new System.EventHandler(this.rbXmaParseStartOffsetStatic_CheckedChanged);
            // 
            // tbXmaParseStartOffset
            // 
            this.tbXmaParseStartOffset.Location = new System.Drawing.Point(136, 18);
            this.tbXmaParseStartOffset.Name = "tbXmaParseStartOffset";
            this.tbXmaParseStartOffset.Size = new System.Drawing.Size(70, 20);
            this.tbXmaParseStartOffset.TabIndex = 3;
            // 
            // grpBlockSize
            // 
            this.grpBlockSize.Controls.Add(this.XmaParseBlockSizeOffsetDescription);
            this.grpBlockSize.Controls.Add(this.rbXmaParseBlockSizeOffset);
            this.grpBlockSize.Controls.Add(this.rbXmaParseBlockSizeStatic);
            this.grpBlockSize.Controls.Add(this.tbXmaParseBlockSize);
            this.grpBlockSize.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpBlockSize.Location = new System.Drawing.Point(3, 137);
            this.grpBlockSize.Name = "grpBlockSize";
            this.grpBlockSize.Size = new System.Drawing.Size(766, 69);
            this.grpBlockSize.TabIndex = 12;
            this.grpBlockSize.TabStop = false;
            this.grpBlockSize.Text = "Block Size";
            // 
            // XmaParseBlockSizeOffsetDescription
            // 
            this.XmaParseBlockSizeOffsetDescription.Location = new System.Drawing.Point(136, 40);
            this.XmaParseBlockSizeOffsetDescription.Name = "XmaParseBlockSizeOffsetDescription";
            this.XmaParseBlockSizeOffsetDescription.OffsetByteOrder = "Little Endian";
            this.XmaParseBlockSizeOffsetDescription.OffsetSize = "4";
            this.XmaParseBlockSizeOffsetDescription.OffsetValue = "";
            this.XmaParseBlockSizeOffsetDescription.Size = new System.Drawing.Size(372, 27);
            this.XmaParseBlockSizeOffsetDescription.TabIndex = 6;
            // 
            // rbXmaParseBlockSizeOffset
            // 
            this.rbXmaParseBlockSizeOffset.AutoSize = true;
            this.rbXmaParseBlockSizeOffset.Location = new System.Drawing.Point(6, 45);
            this.rbXmaParseBlockSizeOffset.Name = "rbXmaParseBlockSizeOffset";
            this.rbXmaParseBlockSizeOffset.Size = new System.Drawing.Size(128, 17);
            this.rbXmaParseBlockSizeOffset.TabIndex = 3;
            this.rbXmaParseBlockSizeOffset.TabStop = true;
            this.rbXmaParseBlockSizeOffset.Text = "Block Size is at Offset";
            this.rbXmaParseBlockSizeOffset.UseVisualStyleBackColor = true;
            this.rbXmaParseBlockSizeOffset.CheckedChanged += new System.EventHandler(this.rbXmaParseBlockSizeOffset_CheckedChanged);
            // 
            // rbXmaParseBlockSizeStatic
            // 
            this.rbXmaParseBlockSizeStatic.AutoSize = true;
            this.rbXmaParseBlockSizeStatic.Location = new System.Drawing.Point(6, 19);
            this.rbXmaParseBlockSizeStatic.Name = "rbXmaParseBlockSizeStatic";
            this.rbXmaParseBlockSizeStatic.Size = new System.Drawing.Size(75, 17);
            this.rbXmaParseBlockSizeStatic.TabIndex = 2;
            this.rbXmaParseBlockSizeStatic.TabStop = true;
            this.rbXmaParseBlockSizeStatic.Text = "Block Size";
            this.rbXmaParseBlockSizeStatic.UseVisualStyleBackColor = true;
            this.rbXmaParseBlockSizeStatic.CheckedChanged += new System.EventHandler(this.rbXmaParseBlockSizeStatic_CheckedChanged);
            // 
            // tbXmaParseBlockSize
            // 
            this.tbXmaParseBlockSize.Location = new System.Drawing.Point(136, 18);
            this.tbXmaParseBlockSize.Name = "tbXmaParseBlockSize";
            this.tbXmaParseBlockSize.Size = new System.Drawing.Size(70, 20);
            this.tbXmaParseBlockSize.TabIndex = 5;
            // 
            // grpXmaParseDataSize
            // 
            this.grpXmaParseDataSize.Controls.Add(this.XmaParseDataSizeOffsetDescription);
            this.grpXmaParseDataSize.Controls.Add(this.rbXmaParseDataSizeOffset);
            this.grpXmaParseDataSize.Controls.Add(this.rbXmaParseDataSizeStatic);
            this.grpXmaParseDataSize.Controls.Add(this.tbXmaParseDataSize);
            this.grpXmaParseDataSize.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpXmaParseDataSize.Location = new System.Drawing.Point(3, 206);
            this.grpXmaParseDataSize.Name = "grpXmaParseDataSize";
            this.grpXmaParseDataSize.Size = new System.Drawing.Size(766, 67);
            this.grpXmaParseDataSize.TabIndex = 11;
            this.grpXmaParseDataSize.TabStop = false;
            this.grpXmaParseDataSize.Text = "Data Size";
            // 
            // XmaParseDataSizeOffsetDescription
            // 
            this.XmaParseDataSizeOffsetDescription.Location = new System.Drawing.Point(136, 40);
            this.XmaParseDataSizeOffsetDescription.Name = "XmaParseDataSizeOffsetDescription";
            this.XmaParseDataSizeOffsetDescription.OffsetByteOrder = "Little Endian";
            this.XmaParseDataSizeOffsetDescription.OffsetSize = "4";
            this.XmaParseDataSizeOffsetDescription.OffsetValue = "";
            this.XmaParseDataSizeOffsetDescription.Size = new System.Drawing.Size(372, 27);
            this.XmaParseDataSizeOffsetDescription.TabIndex = 10;
            // 
            // rbXmaParseDataSizeOffset
            // 
            this.rbXmaParseDataSizeOffset.AutoSize = true;
            this.rbXmaParseDataSizeOffset.Location = new System.Drawing.Point(6, 44);
            this.rbXmaParseDataSizeOffset.Name = "rbXmaParseDataSizeOffset";
            this.rbXmaParseDataSizeOffset.Size = new System.Drawing.Size(124, 17);
            this.rbXmaParseDataSizeOffset.TabIndex = 1;
            this.rbXmaParseDataSizeOffset.TabStop = true;
            this.rbXmaParseDataSizeOffset.Text = "Data Size is at Offset";
            this.rbXmaParseDataSizeOffset.UseVisualStyleBackColor = true;
            this.rbXmaParseDataSizeOffset.CheckedChanged += new System.EventHandler(this.rbXmaParseDataSizeOffset_CheckedChanged);
            // 
            // rbXmaParseDataSizeStatic
            // 
            this.rbXmaParseDataSizeStatic.AutoSize = true;
            this.rbXmaParseDataSizeStatic.Location = new System.Drawing.Point(6, 19);
            this.rbXmaParseDataSizeStatic.Name = "rbXmaParseDataSizeStatic";
            this.rbXmaParseDataSizeStatic.Size = new System.Drawing.Size(71, 17);
            this.rbXmaParseDataSizeStatic.TabIndex = 0;
            this.rbXmaParseDataSizeStatic.TabStop = true;
            this.rbXmaParseDataSizeStatic.Text = "Data Size";
            this.rbXmaParseDataSizeStatic.UseVisualStyleBackColor = true;
            this.rbXmaParseDataSizeStatic.CheckedChanged += new System.EventHandler(this.rbXmaParseDataSizeStatic_CheckedChanged);
            // 
            // tbXmaParseDataSize
            // 
            this.tbXmaParseDataSize.Location = new System.Drawing.Point(136, 18);
            this.tbXmaParseDataSize.Name = "tbXmaParseDataSize";
            this.tbXmaParseDataSize.Size = new System.Drawing.Size(70, 20);
            this.tbXmaParseDataSize.TabIndex = 9;
            // 
            // cbXmaParseIgnoreErrors
            // 
            this.cbXmaParseIgnoreErrors.AutoSize = true;
            this.cbXmaParseIgnoreErrors.Location = new System.Drawing.Point(315, 36);
            this.cbXmaParseIgnoreErrors.Name = "cbXmaParseIgnoreErrors";
            this.cbXmaParseIgnoreErrors.Size = new System.Drawing.Size(86, 17);
            this.cbXmaParseIgnoreErrors.TabIndex = 10;
            this.cbXmaParseIgnoreErrors.Text = "Ignore Errors";
            this.cbXmaParseIgnoreErrors.UseVisualStyleBackColor = true;
            // 
            // cbXmaParseDoRebuild
            // 
            this.cbXmaParseDoRebuild.AutoSize = true;
            this.cbXmaParseDoRebuild.Location = new System.Drawing.Point(148, 36);
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
            // XmaConvertForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 478);
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
            this.grpToWavOptions.ResumeLayout(false);
            this.grpToWavOptions.PerformLayout();
            this.grpRiffHeaderOptions.ResumeLayout(false);
            this.grpRiffHeaderOptions.PerformLayout();
            this.grpXmaParseOptions.ResumeLayout(false);
            this.grpXmaParseOptions.PerformLayout();
            this.grpStartOffset.ResumeLayout(false);
            this.grpStartOffset.PerformLayout();
            this.grpBlockSize.ResumeLayout(false);
            this.grpBlockSize.PerformLayout();
            this.grpXmaParseDataSize.ResumeLayout(false);
            this.grpXmaParseDataSize.PerformLayout();
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
        private System.Windows.Forms.TextBox tbXmaParseStartOffset;
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
        private System.Windows.Forms.TextBox tbXmaParseDataSize;
        private System.Windows.Forms.CheckBox cbXmaParseIgnoreErrors;
        private System.Windows.Forms.CheckBox cbKeepTempFiles;
        private System.Windows.Forms.GroupBox grpToWavOptions;
        private System.Windows.Forms.CheckBox cbDoToWav;
        private System.Windows.Forms.GroupBox grpXmaParseDataSize;
        private System.Windows.Forms.RadioButton rbXmaParseDataSizeOffset;
        private System.Windows.Forms.RadioButton rbXmaParseDataSizeStatic;
        private System.Windows.Forms.GroupBox grpBlockSize;
        private System.Windows.Forms.RadioButton rbXmaParseBlockSizeOffset;
        private System.Windows.Forms.RadioButton rbXmaParseBlockSizeStatic;
        private System.Windows.Forms.GroupBox grpStartOffset;
        private System.Windows.Forms.RadioButton rbXmaParseStartOffsetOffset;
        private System.Windows.Forms.RadioButton rbXmaParseStartOffsetStatic;
        private OffsetDescriptionControl XmaParseStartOffsetOffsetDescription;
        private OffsetDescriptionControl XmaParseBlockSizeOffsetDescription;
        private OffsetDescriptionControl XmaParseDataSizeOffsetDescription;
    }
}