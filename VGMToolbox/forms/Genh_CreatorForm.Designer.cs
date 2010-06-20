namespace VGMToolbox.forms
{
    partial class Genh_CreatorForm
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
            this.components = new System.ComponentModel.Container();
            this.grpFormat = new System.Windows.Forms.GroupBox();
            this.comboFormat = new System.Windows.Forms.ComboBox();
            this.cbHeaderOnly = new System.Windows.Forms.CheckBox();
            this.rbCreate = new System.Windows.Forms.RadioButton();
            this.rbEdit = new System.Windows.Forms.RadioButton();
            this.rbExtract = new System.Windows.Forms.RadioButton();
            this.contextMenuRefresh = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshFileListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuBytesToSamples = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bytesToSamplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.grpCoefOptions = new System.Windows.Forms.GroupBox();
            this.lblRightCoef = new System.Windows.Forms.Label();
            this.tbLeftCoef = new System.Windows.Forms.TextBox();
            this.lblLeftCoef = new System.Windows.Forms.Label();
            this.cbCapcomHack = new System.Windows.Forms.CheckBox();
            this.tbRightCoef = new System.Windows.Forms.TextBox();
            this.grpLoopOptions = new System.Windows.Forms.GroupBox();
            this.lblLoopStart = new System.Windows.Forms.Label();
            this.cbUseLoopEndOffset = new System.Windows.Forms.CheckBox();
            this.lblLoopEnd = new System.Windows.Forms.Label();
            this.loopEndOffsetDescription = new VGMToolbox.controls.OffsetDescriptionControl();
            this.tbLoopEnd = new System.Windows.Forms.TextBox();
            this.cbUseLoopStartOffset = new System.Windows.Forms.CheckBox();
            this.tbLoopStart = new System.Windows.Forms.TextBox();
            this.loopStartOffsetDescription = new VGMToolbox.controls.OffsetDescriptionControl();
            this.cbManualEntry = new System.Windows.Forms.RadioButton();
            this.cbFindLoop = new System.Windows.Forms.RadioButton();
            this.cbNoLoops = new System.Windows.Forms.RadioButton();
            this.cbLoopFileEnd = new System.Windows.Forms.RadioButton();
            this.grpGeneralOptions = new System.Windows.Forms.GroupBox();
            this.lblHeaderSkip = new System.Windows.Forms.Label();
            this.cbHeaderSkip = new System.Windows.Forms.ComboBox();
            this.cbInterleave = new System.Windows.Forms.ComboBox();
            this.cbChannels = new System.Windows.Forms.ComboBox();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.cbUseFrequencyOffset = new System.Windows.Forms.CheckBox();
            this.cbFrequency = new System.Windows.Forms.ComboBox();
            this.frequencyOffsetDescription = new VGMToolbox.controls.OffsetDescriptionControl();
            this.lblChannels = new System.Windows.Forms.Label();
            this.lblInterleave = new System.Windows.Forms.Label();
            this.grpFunction = new System.Windows.Forms.GroupBox();
            this.lblFilenameFilter = new System.Windows.Forms.Label();
            this.tbFilenameFilter = new System.Windows.Forms.TextBox();
            this.tbSourceDirectory = new System.Windows.Forms.TextBox();
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.btnBrowseDirectory = new System.Windows.Forms.Button();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpFormat.SuspendLayout();
            this.contextMenuRefresh.SuspendLayout();
            this.contextMenuBytesToSamples.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.grpCoefOptions.SuspendLayout();
            this.grpLoopOptions.SuspendLayout();
            this.grpGeneralOptions.SuspendLayout();
            this.grpFunction.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 677);
            this.pnlLabels.Size = new System.Drawing.Size(789, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(789, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 600);
            this.tbOutput.Size = new System.Drawing.Size(789, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 580);
            this.pnlButtons.Size = new System.Drawing.Size(789, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(729, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(669, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnDoTask_Click);
            // 
            // grpFormat
            // 
            this.grpFormat.Controls.Add(this.comboFormat);
            this.grpFormat.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFormat.Location = new System.Drawing.Point(0, 0);
            this.grpFormat.Name = "grpFormat";
            this.grpFormat.Size = new System.Drawing.Size(783, 41);
            this.grpFormat.TabIndex = 7;
            this.grpFormat.TabStop = false;
            this.grpFormat.Text = "Format";
            // 
            // comboFormat
            // 
            this.comboFormat.FormattingEnabled = true;
            this.comboFormat.Location = new System.Drawing.Point(6, 14);
            this.comboFormat.Name = "comboFormat";
            this.comboFormat.Size = new System.Drawing.Size(527, 21);
            this.comboFormat.TabIndex = 0;
            this.comboFormat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboFormat_KeyPress);
            this.comboFormat.SelectedValueChanged += new System.EventHandler(this.comboFormat_SelectedValueChanged);
            this.comboFormat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboFormat_KeyDown);
            // 
            // cbHeaderOnly
            // 
            this.cbHeaderOnly.AutoSize = true;
            this.cbHeaderOnly.Location = new System.Drawing.Point(10, 40);
            this.cbHeaderOnly.Name = "cbHeaderOnly";
            this.cbHeaderOnly.Size = new System.Drawing.Size(117, 17);
            this.cbHeaderOnly.TabIndex = 0;
            this.cbHeaderOnly.Text = "Ouput Header Only";
            this.cbHeaderOnly.UseVisualStyleBackColor = true;
            // 
            // rbCreate
            // 
            this.rbCreate.AutoSize = true;
            this.rbCreate.Location = new System.Drawing.Point(3, 17);
            this.rbCreate.Name = "rbCreate";
            this.rbCreate.Size = new System.Drawing.Size(56, 17);
            this.rbCreate.TabIndex = 9;
            this.rbCreate.TabStop = true;
            this.rbCreate.Text = "Create";
            this.rbCreate.UseVisualStyleBackColor = true;
            this.rbCreate.CheckedChanged += new System.EventHandler(this.rbCreate_CheckedChanged);
            // 
            // rbEdit
            // 
            this.rbEdit.AutoSize = true;
            this.rbEdit.Location = new System.Drawing.Point(3, 86);
            this.rbEdit.Name = "rbEdit";
            this.rbEdit.Size = new System.Drawing.Size(43, 17);
            this.rbEdit.TabIndex = 10;
            this.rbEdit.TabStop = true;
            this.rbEdit.Text = "Edit";
            this.rbEdit.UseVisualStyleBackColor = true;
            this.rbEdit.CheckedChanged += new System.EventHandler(this.rbEdit_CheckedChanged);
            // 
            // rbExtract
            // 
            this.rbExtract.AutoSize = true;
            this.rbExtract.Location = new System.Drawing.Point(3, 63);
            this.rbExtract.Name = "rbExtract";
            this.rbExtract.Size = new System.Drawing.Size(58, 17);
            this.rbExtract.TabIndex = 11;
            this.rbExtract.TabStop = true;
            this.rbExtract.Text = "Extract";
            this.rbExtract.UseVisualStyleBackColor = true;
            this.rbExtract.CheckedChanged += new System.EventHandler(this.rbExtract_CheckedChanged);
            // 
            // contextMenuRefresh
            // 
            this.contextMenuRefresh.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshFileListToolStripMenuItem});
            this.contextMenuRefresh.Name = "contextMenuRefresh";
            this.contextMenuRefresh.Size = new System.Drawing.Size(151, 26);
            // 
            // refreshFileListToolStripMenuItem
            // 
            this.refreshFileListToolStripMenuItem.Name = "refreshFileListToolStripMenuItem";
            this.refreshFileListToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.refreshFileListToolStripMenuItem.Text = "Refresh File List";
            this.refreshFileListToolStripMenuItem.Click += new System.EventHandler(this.refreshFileListToolStripMenuItem_Click);
            // 
            // contextMenuBytesToSamples
            // 
            this.contextMenuBytesToSamples.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bytesToSamplesToolStripMenuItem});
            this.contextMenuBytesToSamples.Name = "contextMenuBytesToSamples";
            this.contextMenuBytesToSamples.Size = new System.Drawing.Size(157, 26);
            // 
            // bytesToSamplesToolStripMenuItem
            // 
            this.bytesToSamplesToolStripMenuItem.Name = "bytesToSamplesToolStripMenuItem";
            this.bytesToSamplesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.bytesToSamplesToolStripMenuItem.Text = "Bytes to Samples";
            this.bytesToSamplesToolStripMenuItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bytesToSamplesToolStripMenuItem_MouseUp);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.pnlOptions);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOptions.Location = new System.Drawing.Point(0, 178);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(789, 402);
            this.grpOptions.TabIndex = 12;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // pnlOptions
            // 
            this.pnlOptions.AutoScroll = true;
            this.pnlOptions.Controls.Add(this.grpCoefOptions);
            this.pnlOptions.Controls.Add(this.grpLoopOptions);
            this.pnlOptions.Controls.Add(this.grpGeneralOptions);
            this.pnlOptions.Controls.Add(this.grpFormat);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Location = new System.Drawing.Point(3, 16);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(783, 383);
            this.pnlOptions.TabIndex = 0;
            // 
            // grpCoefOptions
            // 
            this.grpCoefOptions.Controls.Add(this.lblRightCoef);
            this.grpCoefOptions.Controls.Add(this.tbLeftCoef);
            this.grpCoefOptions.Controls.Add(this.lblLeftCoef);
            this.grpCoefOptions.Controls.Add(this.cbCapcomHack);
            this.grpCoefOptions.Controls.Add(this.tbRightCoef);
            this.grpCoefOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCoefOptions.Location = new System.Drawing.Point(0, 281);
            this.grpCoefOptions.Name = "grpCoefOptions";
            this.grpCoefOptions.Size = new System.Drawing.Size(783, 67);
            this.grpCoefOptions.TabIndex = 37;
            this.grpCoefOptions.TabStop = false;
            this.grpCoefOptions.Text = "Coefficients";
            // 
            // lblRightCoef
            // 
            this.lblRightCoef.AutoSize = true;
            this.lblRightCoef.Location = new System.Drawing.Point(6, 22);
            this.lblRightCoef.Name = "lblRightCoef";
            this.lblRightCoef.Size = new System.Drawing.Size(102, 13);
            this.lblRightCoef.TabIndex = 15;
            this.lblRightCoef.Text = "Coef: Right Channel";
            // 
            // tbLeftCoef
            // 
            this.tbLeftCoef.Location = new System.Drawing.Point(114, 45);
            this.tbLeftCoef.Name = "tbLeftCoef";
            this.tbLeftCoef.Size = new System.Drawing.Size(100, 20);
            this.tbLeftCoef.TabIndex = 16;
            // 
            // lblLeftCoef
            // 
            this.lblLeftCoef.AutoSize = true;
            this.lblLeftCoef.Location = new System.Drawing.Point(6, 48);
            this.lblLeftCoef.Name = "lblLeftCoef";
            this.lblLeftCoef.Size = new System.Drawing.Size(95, 13);
            this.lblLeftCoef.TabIndex = 17;
            this.lblLeftCoef.Text = "Coef: Left Channel";
            // 
            // cbCapcomHack
            // 
            this.cbCapcomHack.AutoSize = true;
            this.cbCapcomHack.Location = new System.Drawing.Point(221, 47);
            this.cbCapcomHack.Name = "cbCapcomHack";
            this.cbCapcomHack.Size = new System.Drawing.Size(94, 17);
            this.cbCapcomHack.TabIndex = 18;
            this.cbCapcomHack.Text = "Capcom Hack";
            this.cbCapcomHack.UseVisualStyleBackColor = true;
            // 
            // tbRightCoef
            // 
            this.tbRightCoef.Location = new System.Drawing.Point(114, 19);
            this.tbRightCoef.Name = "tbRightCoef";
            this.tbRightCoef.Size = new System.Drawing.Size(100, 20);
            this.tbRightCoef.TabIndex = 14;
            // 
            // grpLoopOptions
            // 
            this.grpLoopOptions.Controls.Add(this.lblLoopStart);
            this.grpLoopOptions.Controls.Add(this.cbUseLoopEndOffset);
            this.grpLoopOptions.Controls.Add(this.lblLoopEnd);
            this.grpLoopOptions.Controls.Add(this.loopEndOffsetDescription);
            this.grpLoopOptions.Controls.Add(this.tbLoopEnd);
            this.grpLoopOptions.Controls.Add(this.cbUseLoopStartOffset);
            this.grpLoopOptions.Controls.Add(this.tbLoopStart);
            this.grpLoopOptions.Controls.Add(this.loopStartOffsetDescription);
            this.grpLoopOptions.Controls.Add(this.cbManualEntry);
            this.grpLoopOptions.Controls.Add(this.cbFindLoop);
            this.grpLoopOptions.Controls.Add(this.cbNoLoops);
            this.grpLoopOptions.Controls.Add(this.cbLoopFileEnd);
            this.grpLoopOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpLoopOptions.Location = new System.Drawing.Point(0, 152);
            this.grpLoopOptions.Name = "grpLoopOptions";
            this.grpLoopOptions.Size = new System.Drawing.Size(783, 129);
            this.grpLoopOptions.TabIndex = 36;
            this.grpLoopOptions.TabStop = false;
            this.grpLoopOptions.Text = "Looping";
            // 
            // lblLoopStart
            // 
            this.lblLoopStart.AutoSize = true;
            this.lblLoopStart.Location = new System.Drawing.Point(12, 16);
            this.lblLoopStart.Name = "lblLoopStart";
            this.lblLoopStart.Size = new System.Drawing.Size(105, 13);
            this.lblLoopStart.TabIndex = 9;
            this.lblLoopStart.Text = "Loop Start (Samples)";
            this.lblLoopStart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblLoopStart_MouseUp);
            // 
            // cbUseLoopEndOffset
            // 
            this.cbUseLoopEndOffset.AutoSize = true;
            this.cbUseLoopEndOffset.Location = new System.Drawing.Point(26, 88);
            this.cbUseLoopEndOffset.Name = "cbUseLoopEndOffset";
            this.cbUseLoopEndOffset.Size = new System.Drawing.Size(76, 17);
            this.cbUseLoopEndOffset.TabIndex = 34;
            this.cbUseLoopEndOffset.Text = "Use Offset";
            this.cbUseLoopEndOffset.UseVisualStyleBackColor = true;
            this.cbUseLoopEndOffset.CheckedChanged += new System.EventHandler(this.cbUseLoopEndOffset_CheckedChanged);
            // 
            // lblLoopEnd
            // 
            this.lblLoopEnd.AutoSize = true;
            this.lblLoopEnd.Location = new System.Drawing.Point(12, 65);
            this.lblLoopEnd.Name = "lblLoopEnd";
            this.lblLoopEnd.Size = new System.Drawing.Size(102, 13);
            this.lblLoopEnd.TabIndex = 11;
            this.lblLoopEnd.Text = "Loop End (Samples)";
            this.lblLoopEnd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblLoopEnd_MouseUp);
            // 
            // loopEndOffsetDescription
            // 
            this.loopEndOffsetDescription.Location = new System.Drawing.Point(108, 83);
            this.loopEndOffsetDescription.Name = "loopEndOffsetDescription";
            this.loopEndOffsetDescription.OffsetByteOrder = "Little Endian";
            this.loopEndOffsetDescription.OffsetSize = "4";
            this.loopEndOffsetDescription.OffsetValue = "";
            this.loopEndOffsetDescription.Size = new System.Drawing.Size(372, 24);
            this.loopEndOffsetDescription.TabIndex = 33;
            // 
            // tbLoopEnd
            // 
            this.tbLoopEnd.Location = new System.Drawing.Point(123, 62);
            this.tbLoopEnd.Name = "tbLoopEnd";
            this.tbLoopEnd.Size = new System.Drawing.Size(100, 20);
            this.tbLoopEnd.TabIndex = 10;
            // 
            // cbUseLoopStartOffset
            // 
            this.cbUseLoopStartOffset.AutoSize = true;
            this.cbUseLoopStartOffset.Location = new System.Drawing.Point(26, 40);
            this.cbUseLoopStartOffset.Name = "cbUseLoopStartOffset";
            this.cbUseLoopStartOffset.Size = new System.Drawing.Size(76, 17);
            this.cbUseLoopStartOffset.TabIndex = 32;
            this.cbUseLoopStartOffset.Text = "Use Offset";
            this.cbUseLoopStartOffset.UseVisualStyleBackColor = true;
            this.cbUseLoopStartOffset.CheckedChanged += new System.EventHandler(this.cbUseLoopStartOffset_CheckedChanged);
            // 
            // tbLoopStart
            // 
            this.tbLoopStart.Location = new System.Drawing.Point(122, 13);
            this.tbLoopStart.Name = "tbLoopStart";
            this.tbLoopStart.Size = new System.Drawing.Size(100, 20);
            this.tbLoopStart.TabIndex = 8;
            // 
            // loopStartOffsetDescription
            // 
            this.loopStartOffsetDescription.Location = new System.Drawing.Point(108, 35);
            this.loopStartOffsetDescription.Name = "loopStartOffsetDescription";
            this.loopStartOffsetDescription.OffsetByteOrder = "Little Endian";
            this.loopStartOffsetDescription.OffsetSize = "4";
            this.loopStartOffsetDescription.OffsetValue = "";
            this.loopStartOffsetDescription.Size = new System.Drawing.Size(372, 24);
            this.loopStartOffsetDescription.TabIndex = 31;
            // 
            // cbManualEntry
            // 
            this.cbManualEntry.AutoSize = true;
            this.cbManualEntry.Checked = true;
            this.cbManualEntry.Location = new System.Drawing.Point(14, 108);
            this.cbManualEntry.Name = "cbManualEntry";
            this.cbManualEntry.Size = new System.Drawing.Size(87, 17);
            this.cbManualEntry.TabIndex = 28;
            this.cbManualEntry.TabStop = true;
            this.cbManualEntry.Text = "Manual Entry";
            this.cbManualEntry.UseVisualStyleBackColor = true;
            this.cbManualEntry.CheckedChanged += new System.EventHandler(this.cbManualEntry_CheckedChanged);
            // 
            // cbFindLoop
            // 
            this.cbFindLoop.AutoSize = true;
            this.cbFindLoop.Location = new System.Drawing.Point(278, 108);
            this.cbFindLoop.Name = "cbFindLoop";
            this.cbFindLoop.Size = new System.Drawing.Size(72, 17);
            this.cbFindLoop.TabIndex = 27;
            this.cbFindLoop.Text = "Find Loop";
            this.cbFindLoop.UseVisualStyleBackColor = true;
            this.cbFindLoop.CheckedChanged += new System.EventHandler(this.cbFindLoop_CheckedChanged);
            // 
            // cbNoLoops
            // 
            this.cbNoLoops.AutoSize = true;
            this.cbNoLoops.Location = new System.Drawing.Point(107, 108);
            this.cbNoLoops.Name = "cbNoLoops";
            this.cbNoLoops.Size = new System.Drawing.Size(71, 17);
            this.cbNoLoops.TabIndex = 25;
            this.cbNoLoops.Text = "No Loops";
            this.cbNoLoops.UseVisualStyleBackColor = true;
            this.cbNoLoops.CheckedChanged += new System.EventHandler(this.cbNoLoops_CheckedChanged);
            // 
            // cbLoopFileEnd
            // 
            this.cbLoopFileEnd.AutoSize = true;
            this.cbLoopFileEnd.Location = new System.Drawing.Point(187, 108);
            this.cbLoopFileEnd.Name = "cbLoopFileEnd";
            this.cbLoopFileEnd.Size = new System.Drawing.Size(85, 17);
            this.cbLoopFileEnd.TabIndex = 26;
            this.cbLoopFileEnd.Text = "Use File End";
            this.cbLoopFileEnd.UseVisualStyleBackColor = true;
            this.cbLoopFileEnd.CheckedChanged += new System.EventHandler(this.cbLoopFileEnd_CheckedChanged);
            // 
            // grpGeneralOptions
            // 
            this.grpGeneralOptions.Controls.Add(this.lblHeaderSkip);
            this.grpGeneralOptions.Controls.Add(this.cbHeaderSkip);
            this.grpGeneralOptions.Controls.Add(this.lblInterleave);
            this.grpGeneralOptions.Controls.Add(this.cbInterleave);
            this.grpGeneralOptions.Controls.Add(this.cbChannels);
            this.grpGeneralOptions.Controls.Add(this.lblFrequency);
            this.grpGeneralOptions.Controls.Add(this.cbUseFrequencyOffset);
            this.grpGeneralOptions.Controls.Add(this.cbFrequency);
            this.grpGeneralOptions.Controls.Add(this.frequencyOffsetDescription);
            this.grpGeneralOptions.Controls.Add(this.lblChannels);
            this.grpGeneralOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpGeneralOptions.Location = new System.Drawing.Point(0, 41);
            this.grpGeneralOptions.Name = "grpGeneralOptions";
            this.grpGeneralOptions.Size = new System.Drawing.Size(783, 111);
            this.grpGeneralOptions.TabIndex = 35;
            this.grpGeneralOptions.TabStop = false;
            this.grpGeneralOptions.Text = "General";
            // 
            // lblHeaderSkip
            // 
            this.lblHeaderSkip.AutoSize = true;
            this.lblHeaderSkip.Location = new System.Drawing.Point(9, 16);
            this.lblHeaderSkip.Name = "lblHeaderSkip";
            this.lblHeaderSkip.Size = new System.Drawing.Size(66, 13);
            this.lblHeaderSkip.TabIndex = 1;
            this.lblHeaderSkip.Text = "Header Skip";
            // 
            // cbHeaderSkip
            // 
            this.cbHeaderSkip.FormattingEnabled = true;
            this.cbHeaderSkip.Location = new System.Drawing.Point(92, 13);
            this.cbHeaderSkip.Name = "cbHeaderSkip";
            this.cbHeaderSkip.Size = new System.Drawing.Size(100, 21);
            this.cbHeaderSkip.TabIndex = 21;
            this.cbHeaderSkip.Text = "0";
            this.cbHeaderSkip.SelectedValueChanged += new System.EventHandler(this.cbHeaderSkip_SelectedValueChanged);
            // 
            // cbInterleave
            // 
            this.cbInterleave.FormattingEnabled = true;
            this.cbInterleave.Location = new System.Drawing.Point(301, 13);
            this.cbInterleave.Name = "cbInterleave";
            this.cbInterleave.Size = new System.Drawing.Size(100, 21);
            this.cbInterleave.TabIndex = 22;
            this.cbInterleave.Text = "0";
            this.cbInterleave.SelectedValueChanged += new System.EventHandler(this.cbInterleave_SelectedValueChanged);
            // 
            // cbChannels
            // 
            this.cbChannels.FormattingEnabled = true;
            this.cbChannels.Location = new System.Drawing.Point(92, 38);
            this.cbChannels.Name = "cbChannels";
            this.cbChannels.Size = new System.Drawing.Size(100, 21);
            this.cbChannels.TabIndex = 23;
            this.cbChannels.SelectedValueChanged += new System.EventHandler(this.cbChannels_SelectedValueChanged);
            // 
            // lblFrequency
            // 
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.Location = new System.Drawing.Point(10, 65);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(57, 13);
            this.lblFrequency.TabIndex = 7;
            this.lblFrequency.Text = "Frequency";
            // 
            // cbUseFrequencyOffset
            // 
            this.cbUseFrequencyOffset.AutoSize = true;
            this.cbUseFrequencyOffset.Location = new System.Drawing.Point(24, 89);
            this.cbUseFrequencyOffset.Name = "cbUseFrequencyOffset";
            this.cbUseFrequencyOffset.Size = new System.Drawing.Size(76, 17);
            this.cbUseFrequencyOffset.TabIndex = 30;
            this.cbUseFrequencyOffset.Text = "Use Offset";
            this.cbUseFrequencyOffset.UseVisualStyleBackColor = true;
            // 
            // cbFrequency
            // 
            this.cbFrequency.FormattingEnabled = true;
            this.cbFrequency.Location = new System.Drawing.Point(92, 62);
            this.cbFrequency.Name = "cbFrequency";
            this.cbFrequency.Size = new System.Drawing.Size(100, 21);
            this.cbFrequency.TabIndex = 24;
            this.cbFrequency.SelectedValueChanged += new System.EventHandler(this.cbFrequency_SelectedValueChanged);
            // 
            // frequencyOffsetDescription
            // 
            this.frequencyOffsetDescription.Location = new System.Drawing.Point(106, 84);
            this.frequencyOffsetDescription.Name = "frequencyOffsetDescription";
            this.frequencyOffsetDescription.OffsetByteOrder = "Little Endian";
            this.frequencyOffsetDescription.OffsetSize = "4";
            this.frequencyOffsetDescription.OffsetValue = "";
            this.frequencyOffsetDescription.Size = new System.Drawing.Size(372, 24);
            this.frequencyOffsetDescription.TabIndex = 29;
            // 
            // lblChannels
            // 
            this.lblChannels.AutoSize = true;
            this.lblChannels.Location = new System.Drawing.Point(9, 41);
            this.lblChannels.Name = "lblChannels";
            this.lblChannels.Size = new System.Drawing.Size(51, 13);
            this.lblChannels.TabIndex = 5;
            this.lblChannels.Text = "Channels";
            // 
            // lblInterleave
            // 
            this.lblInterleave.AutoSize = true;
            this.lblInterleave.Location = new System.Drawing.Point(241, 16);
            this.lblInterleave.Name = "lblInterleave";
            this.lblInterleave.Size = new System.Drawing.Size(54, 13);
            this.lblInterleave.TabIndex = 3;
            this.lblInterleave.Text = "Interleave";
            // 
            // grpFunction
            // 
            this.grpFunction.Controls.Add(this.lblFilenameFilter);
            this.grpFunction.Controls.Add(this.tbFilenameFilter);
            this.grpFunction.Controls.Add(this.tbSourceDirectory);
            this.grpFunction.Controls.Add(this.lbFiles);
            this.grpFunction.Controls.Add(this.btnBrowseDirectory);
            this.grpFunction.Controls.Add(this.rbCreate);
            this.grpFunction.Controls.Add(this.rbEdit);
            this.grpFunction.Controls.Add(this.rbExtract);
            this.grpFunction.Controls.Add(this.cbHeaderOnly);
            this.grpFunction.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFunction.Location = new System.Drawing.Point(0, 23);
            this.grpFunction.Name = "grpFunction";
            this.grpFunction.Size = new System.Drawing.Size(789, 155);
            this.grpFunction.TabIndex = 13;
            this.grpFunction.TabStop = false;
            this.grpFunction.Text = "Functions";
            // 
            // lblFilenameFilter
            // 
            this.lblFilenameFilter.AutoSize = true;
            this.lblFilenameFilter.Location = new System.Drawing.Point(322, 44);
            this.lblFilenameFilter.Name = "lblFilenameFilter";
            this.lblFilenameFilter.Size = new System.Drawing.Size(76, 13);
            this.lblFilenameFilter.TabIndex = 16;
            this.lblFilenameFilter.Text = "FileName Filter";
            this.lblFilenameFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbFilenameFilter
            // 
            this.tbFilenameFilter.Location = new System.Drawing.Point(142, 41);
            this.tbFilenameFilter.Name = "tbFilenameFilter";
            this.tbFilenameFilter.Size = new System.Drawing.Size(174, 20);
            this.tbFilenameFilter.TabIndex = 15;
            // 
            // tbSourceDirectory
            // 
            this.tbSourceDirectory.Location = new System.Drawing.Point(142, 14);
            this.tbSourceDirectory.Name = "tbSourceDirectory";
            this.tbSourceDirectory.Size = new System.Drawing.Size(350, 20);
            this.tbSourceDirectory.TabIndex = 13;
            // 
            // lbFiles
            // 
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.HorizontalScrollbar = true;
            this.lbFiles.Location = new System.Drawing.Point(142, 67);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFiles.Size = new System.Drawing.Size(388, 82);
            this.lbFiles.TabIndex = 12;
            // 
            // btnBrowseDirectory
            // 
            this.btnBrowseDirectory.Location = new System.Drawing.Point(498, 14);
            this.btnBrowseDirectory.Name = "btnBrowseDirectory";
            this.btnBrowseDirectory.Size = new System.Drawing.Size(32, 20);
            this.btnBrowseDirectory.TabIndex = 14;
            this.btnBrowseDirectory.Text = "...";
            this.btnBrowseDirectory.UseVisualStyleBackColor = true;
            // 
            // Genh_CreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 718);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpFunction);
            this.Name = "Genh_CreatorForm";
            this.Text = "Genh_CreatorForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpFunction, 0);
            this.Controls.SetChildIndex(this.grpOptions, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpFormat.ResumeLayout(false);
            this.contextMenuRefresh.ResumeLayout(false);
            this.contextMenuBytesToSamples.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.pnlOptions.ResumeLayout(false);
            this.grpCoefOptions.ResumeLayout(false);
            this.grpCoefOptions.PerformLayout();
            this.grpLoopOptions.ResumeLayout(false);
            this.grpLoopOptions.PerformLayout();
            this.grpGeneralOptions.ResumeLayout(false);
            this.grpGeneralOptions.PerformLayout();
            this.grpFunction.ResumeLayout(false);
            this.grpFunction.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpFormat;
        private System.Windows.Forms.ComboBox comboFormat;
        private System.Windows.Forms.CheckBox cbHeaderOnly;
        private System.Windows.Forms.RadioButton rbCreate;
        private System.Windows.Forms.RadioButton rbEdit;
        private System.Windows.Forms.RadioButton rbExtract;
        private System.Windows.Forms.ContextMenuStrip contextMenuRefresh;
        private System.Windows.Forms.ToolStripMenuItem refreshFileListToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuBytesToSamples;
        private System.Windows.Forms.ToolStripMenuItem bytesToSamplesToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.RadioButton cbFindLoop;
        private System.Windows.Forms.Label lblHeaderSkip;
        private System.Windows.Forms.RadioButton cbLoopFileEnd;
        private System.Windows.Forms.Label lblInterleave;
        private System.Windows.Forms.RadioButton cbNoLoops;
        private System.Windows.Forms.Label lblChannels;
        private System.Windows.Forms.ComboBox cbFrequency;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.RadioButton cbManualEntry;
        private System.Windows.Forms.TextBox tbLoopStart;
        private System.Windows.Forms.ComboBox cbChannels;
        private System.Windows.Forms.Label lblLoopStart;
        private System.Windows.Forms.ComboBox cbInterleave;
        private System.Windows.Forms.TextBox tbLoopEnd;
        private System.Windows.Forms.ComboBox cbHeaderSkip;
        private System.Windows.Forms.Label lblLoopEnd;
        private System.Windows.Forms.CheckBox cbCapcomHack;
        private System.Windows.Forms.TextBox tbRightCoef;
        private System.Windows.Forms.Label lblLeftCoef;
        private System.Windows.Forms.Label lblRightCoef;
        private System.Windows.Forms.TextBox tbLeftCoef;
        private VGMToolbox.controls.OffsetDescriptionControl frequencyOffsetDescription;
        private System.Windows.Forms.GroupBox grpFunction;
        private System.Windows.Forms.CheckBox cbUseFrequencyOffset;
        private System.Windows.Forms.CheckBox cbUseLoopStartOffset;
        private VGMToolbox.controls.OffsetDescriptionControl loopStartOffsetDescription;
        private System.Windows.Forms.CheckBox cbUseLoopEndOffset;
        private VGMToolbox.controls.OffsetDescriptionControl loopEndOffsetDescription;
        private System.Windows.Forms.GroupBox grpGeneralOptions;
        private System.Windows.Forms.GroupBox grpLoopOptions;
        private System.Windows.Forms.GroupBox grpCoefOptions;
        private System.Windows.Forms.Label lblFilenameFilter;
        private System.Windows.Forms.TextBox tbFilenameFilter;
        private System.Windows.Forms.TextBox tbSourceDirectory;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.Button btnBrowseDirectory;
    }
}