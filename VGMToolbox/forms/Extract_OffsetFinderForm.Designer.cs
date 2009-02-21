namespace VGMToolbox.forms
{
    partial class Extract_OffsetFinderForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbSourcePaths = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbSearchAsHex = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSearchString = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbOutputExtension = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbOffsetSize = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbByteOrder = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbStaticCutsize = new System.Windows.Forms.TextBox();
            this.tbCutSizeOffset = new System.Windows.Forms.TextBox();
            this.rbOffsetBasedCutSize = new System.Windows.Forms.RadioButton();
            this.rbStaticCutSize = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSearchStringOffset = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 513);
            this.pnlLabels.Size = new System.Drawing.Size(816, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(816, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 436);
            this.tbOutput.Size = new System.Drawing.Size(816, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 416);
            this.pnlButtons.Size = new System.Drawing.Size(816, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(756, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(696, 0);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbSourcePaths);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(816, 61);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Files to Search";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Drag and Drop files to search here.";
            // 
            // tbSourcePaths
            // 
            this.tbSourcePaths.AllowDrop = true;
            this.tbSourcePaths.Location = new System.Drawing.Point(6, 19);
            this.tbSourcePaths.Name = "tbSourcePaths";
            this.tbSourcePaths.Size = new System.Drawing.Size(282, 20);
            this.tbSourcePaths.TabIndex = 0;
            this.tbSourcePaths.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSourcePaths_DragDrop);
            this.tbSourcePaths.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbSearchAsHex);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tbSearchString);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 84);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(816, 45);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Criteria";
            // 
            // cbSearchAsHex
            // 
            this.cbSearchAsHex.AutoSize = true;
            this.cbSearchAsHex.Location = new System.Drawing.Point(294, 18);
            this.cbSearchAsHex.Name = "cbSearchAsHex";
            this.cbSearchAsHex.Size = new System.Drawing.Size(87, 17);
            this.cbSearchAsHex.TabIndex = 2;
            this.cbSearchAsHex.Text = "Treat as Hex";
            this.cbSearchAsHex.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Search String";
            // 
            // tbSearchString
            // 
            this.tbSearchString.Location = new System.Drawing.Point(83, 16);
            this.tbSearchString.Name = "tbSearchString";
            this.tbSearchString.Size = new System.Drawing.Size(205, 20);
            this.tbSearchString.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.tbOutputExtension);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.tbSearchStringOffset);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 129);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(816, 161);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Cut Options (all values are relative to the location of the file to be cut)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(234, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Output File Extension";
            // 
            // tbOutputExtension
            // 
            this.tbOutputExtension.Location = new System.Drawing.Point(347, 19);
            this.tbOutputExtension.Name = "tbOutputExtension";
            this.tbOutputExtension.Size = new System.Drawing.Size(92, 20);
            this.tbOutputExtension.TabIndex = 13;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbOffsetSize);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.cbByteOrder);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.tbStaticCutsize);
            this.groupBox4.Controls.Add(this.tbCutSizeOffset);
            this.groupBox4.Controls.Add(this.rbOffsetBasedCutSize);
            this.groupBox4.Controls.Add(this.rbStaticCutSize);
            this.groupBox4.Location = new System.Drawing.Point(9, 45);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(430, 111);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Cut Size Options";
            // 
            // cbOffsetSize
            // 
            this.cbOffsetSize.FormattingEnabled = true;
            this.cbOffsetSize.Location = new System.Drawing.Point(138, 63);
            this.cbOffsetSize.Name = "cbOffsetSize";
            this.cbOffsetSize.Size = new System.Drawing.Size(100, 21);
            this.cbOffsetSize.TabIndex = 12;
            this.cbOffsetSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbOffsetSize_KeyPress);
            this.cbOffsetSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbOffsetSize_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(244, 89);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "byte order.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(64, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "and stored in";
            // 
            // cbByteOrder
            // 
            this.cbByteOrder.FormattingEnabled = true;
            this.cbByteOrder.Location = new System.Drawing.Point(138, 86);
            this.cbByteOrder.Name = "cbByteOrder";
            this.cbByteOrder.Size = new System.Drawing.Size(100, 21);
            this.cbByteOrder.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(244, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "in bytes.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(244, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "in bytes.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(244, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "from the start of the file to cut.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "and has size";
            // 
            // tbStaticCutsize
            // 
            this.tbStaticCutsize.Location = new System.Drawing.Point(138, 18);
            this.tbStaticCutsize.Name = "tbStaticCutsize";
            this.tbStaticCutsize.Size = new System.Drawing.Size(100, 20);
            this.tbStaticCutsize.TabIndex = 3;
            // 
            // tbCutSizeOffset
            // 
            this.tbCutSizeOffset.Location = new System.Drawing.Point(138, 41);
            this.tbCutSizeOffset.Name = "tbCutSizeOffset";
            this.tbCutSizeOffset.Size = new System.Drawing.Size(100, 20);
            this.tbCutSizeOffset.TabIndex = 2;
            // 
            // rbOffsetBasedCutSize
            // 
            this.rbOffsetBasedCutSize.AutoSize = true;
            this.rbOffsetBasedCutSize.Location = new System.Drawing.Point(6, 42);
            this.rbOffsetBasedCutSize.Name = "rbOffsetBasedCutSize";
            this.rbOffsetBasedCutSize.Size = new System.Drawing.Size(117, 17);
            this.rbOffsetBasedCutSize.TabIndex = 1;
            this.rbOffsetBasedCutSize.TabStop = true;
            this.rbOffsetBasedCutSize.Text = "Cut Size is at Offset";
            this.rbOffsetBasedCutSize.UseVisualStyleBackColor = true;
            this.rbOffsetBasedCutSize.CheckedChanged += new System.EventHandler(this.rbStaticCutSize_CheckedChanged);
            // 
            // rbStaticCutSize
            // 
            this.rbStaticCutSize.AutoSize = true;
            this.rbStaticCutSize.Location = new System.Drawing.Point(6, 19);
            this.rbStaticCutSize.Name = "rbStaticCutSize";
            this.rbStaticCutSize.Size = new System.Drawing.Size(111, 17);
            this.rbStaticCutSize.TabIndex = 0;
            this.rbStaticCutSize.TabStop = true;
            this.rbStaticCutSize.Text = "Use Static Cutsize";
            this.rbStaticCutSize.UseVisualStyleBackColor = true;
            this.rbStaticCutSize.CheckedChanged += new System.EventHandler(this.rbStaticCutSize_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Search String is at Offset";
            // 
            // tbSearchStringOffset
            // 
            this.tbSearchStringOffset.Location = new System.Drawing.Point(136, 19);
            this.tbSearchStringOffset.Name = "tbSearchStringOffset";
            this.tbSearchStringOffset.Size = new System.Drawing.Size(92, 20);
            this.tbSearchStringOffset.TabIndex = 0;
            this.tbSearchStringOffset.Text = "0x00";
            // 
            // Extract_OffsetFinderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 554);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Extract_OffsetFinderForm";
            this.Text = "Extract_OffsetFinderForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbSourcePaths;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSearchString;
        private System.Windows.Forms.CheckBox cbSearchAsHex;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbSearchStringOffset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbOffsetBasedCutSize;
        private System.Windows.Forms.RadioButton rbStaticCutSize;
        private System.Windows.Forms.TextBox tbStaticCutsize;
        private System.Windows.Forms.TextBox tbCutSizeOffset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbByteOrder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbOutputExtension;
        private System.Windows.Forms.ComboBox cbOffsetSize;
    }
}