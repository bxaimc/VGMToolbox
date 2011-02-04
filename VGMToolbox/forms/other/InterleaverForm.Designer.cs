namespace VGMToolbox.forms.other
{
    partial class InterleaverForm
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
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tbInterleaveSize = new System.Windows.Forms.TextBox();
            this.rbNoFillBytes = new System.Windows.Forms.RadioButton();
            this.rbUseFillBytes = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.grpFillBytes = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.sortableFileListControl1 = new VGMToolbox.controls.SortableFileListControl();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.grpFillBytes.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 455);
            this.pnlLabels.Size = new System.Drawing.Size(691, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(691, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 378);
            this.tbOutput.Size = new System.Drawing.Size(691, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 358);
            this.pnlButtons.Size = new System.Drawing.Size(691, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(631, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(571, 0);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.sortableFileListControl1);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Location = new System.Drawing.Point(0, 0);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(691, 210);
            this.grpSource.TabIndex = 6;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Files";
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.grpFillBytes);
            this.grpOptions.Controls.Add(this.panel1);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpOptions.Location = new System.Drawing.Point(0, 235);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(691, 100);
            this.grpOptions.TabIndex = 7;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // pnlContent
            // 
            this.pnlContent.AutoScroll = true;
            this.pnlContent.Controls.Add(this.grpOptions);
            this.pnlContent.Controls.Add(this.grpSource);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 23);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(691, 335);
            this.pnlContent.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Interleave Size";
            // 
            // tbInterleaveSize
            // 
            this.tbInterleaveSize.Location = new System.Drawing.Point(86, 4);
            this.tbInterleaveSize.Name = "tbInterleaveSize";
            this.tbInterleaveSize.Size = new System.Drawing.Size(73, 20);
            this.tbInterleaveSize.TabIndex = 1;
            // 
            // rbNoFillBytes
            // 
            this.rbNoFillBytes.AutoSize = true;
            this.rbNoFillBytes.Location = new System.Drawing.Point(6, 18);
            this.rbNoFillBytes.Name = "rbNoFillBytes";
            this.rbNoFillBytes.Size = new System.Drawing.Size(134, 17);
            this.rbNoFillBytes.TabIndex = 3;
            this.rbNoFillBytes.TabStop = true;
            this.rbNoFillBytes.Text = "Do Not Use Filler Bytes";
            this.rbNoFillBytes.UseVisualStyleBackColor = true;
            // 
            // rbUseFillBytes
            // 
            this.rbUseFillBytes.AutoSize = true;
            this.rbUseFillBytes.Location = new System.Drawing.Point(6, 37);
            this.rbUseFillBytes.Name = "rbUseFillBytes";
            this.rbUseFillBytes.Size = new System.Drawing.Size(97, 17);
            this.rbUseFillBytes.TabIndex = 4;
            this.rbUseFillBytes.TabStop = true;
            this.rbUseFillBytes.Text = "Use Filler Bytes";
            this.rbUseFillBytes.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(109, 36);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(41, 20);
            this.textBox1.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(156, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "(byte value: 00, FF, etc...)";
            // 
            // grpFillBytes
            // 
            this.grpFillBytes.Controls.Add(this.rbNoFillBytes);
            this.grpFillBytes.Controls.Add(this.label3);
            this.grpFillBytes.Controls.Add(this.rbUseFillBytes);
            this.grpFillBytes.Controls.Add(this.textBox1);
            this.grpFillBytes.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFillBytes.Location = new System.Drawing.Point(3, 42);
            this.grpFillBytes.Name = "grpFillBytes";
            this.grpFillBytes.Size = new System.Drawing.Size(685, 58);
            this.grpFillBytes.TabIndex = 7;
            this.grpFillBytes.TabStop = false;
            this.grpFillBytes.Text = "When files sizes differ:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbInterleaveSize);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(685, 26);
            this.panel1.TabIndex = 8;
            // 
            // sortableFileListControl1
            // 
            this.sortableFileListControl1.AllowDrop = true;
            this.sortableFileListControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.sortableFileListControl1.Location = new System.Drawing.Point(3, 16);
            this.sortableFileListControl1.Name = "sortableFileListControl1";
            this.sortableFileListControl1.Size = new System.Drawing.Size(685, 193);
            this.sortableFileListControl1.TabIndex = 5;
            // 
            // InterleaverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 496);
            this.Controls.Add(this.pnlContent);
            this.Name = "InterleaverForm";
            this.Text = "InterleaverForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.pnlContent, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpOptions.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.grpFillBytes.ResumeLayout(false);
            this.grpFillBytes.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VGMToolbox.controls.SortableFileListControl sortableFileListControl1;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.TextBox tbInterleaveSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpFillBytes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton rbUseFillBytes;
        private System.Windows.Forms.RadioButton rbNoFillBytes;
        private System.Windows.Forms.Panel panel1;
    }
}