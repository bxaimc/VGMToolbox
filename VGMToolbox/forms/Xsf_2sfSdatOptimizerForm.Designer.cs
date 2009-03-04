namespace VGMToolbox.forms
{
    partial class Xsf_2sfSdatOptimizerForm
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
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.tbSourceSdat = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbIncludeAllSseq = new System.Windows.Forms.CheckBox();
            this.tbEndSequence = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbStartSequence = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 487);
            this.pnlLabels.Size = new System.Drawing.Size(868, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(868, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 410);
            this.tbOutput.Size = new System.Drawing.Size(868, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 390);
            this.pnlButtons.Size = new System.Drawing.Size(868, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(808, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(748, 0);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.tbSourceSdat);
            this.groupBox10.Controls.Add(this.label17);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox10.Location = new System.Drawing.Point(0, 23);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(868, 59);
            this.groupBox10.TabIndex = 7;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Source SDAT";
            // 
            // tbSourceSdat
            // 
            this.tbSourceSdat.AllowDrop = true;
            this.tbSourceSdat.Location = new System.Drawing.Point(6, 19);
            this.tbSourceSdat.Name = "tbSourceSdat";
            this.tbSourceSdat.Size = new System.Drawing.Size(282, 20);
            this.tbSourceSdat.TabIndex = 2;
            this.tbSourceSdat.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSourceSdat_DragDrop);
            this.tbSourceSdat.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 42);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(153, 13);
            this.label17.TabIndex = 1;
            this.label17.Text = "Drag SDAT(s) to optimize here.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbIncludeAllSseq);
            this.groupBox1.Controls.Add(this.tbEndSequence);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbStartSequence);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(868, 63);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // cbIncludeAllSseq
            // 
            this.cbIncludeAllSseq.AutoSize = true;
            this.cbIncludeAllSseq.Checked = true;
            this.cbIncludeAllSseq.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIncludeAllSseq.Location = new System.Drawing.Point(165, 15);
            this.cbIncludeAllSseq.Name = "cbIncludeAllSseq";
            this.cbIncludeAllSseq.Size = new System.Drawing.Size(132, 17);
            this.cbIncludeAllSseq.TabIndex = 4;
            this.cbIncludeAllSseq.Text = "Include All Sequences";
            this.cbIncludeAllSseq.UseVisualStyleBackColor = true;
            this.cbIncludeAllSseq.CheckedChanged += new System.EventHandler(this.cbIncludeAllSseq_CheckedChanged);
            // 
            // tbEndSequence
            // 
            this.tbEndSequence.Location = new System.Drawing.Point(107, 37);
            this.tbEndSequence.Name = "tbEndSequence";
            this.tbEndSequence.ReadOnly = true;
            this.tbEndSequence.Size = new System.Drawing.Size(52, 20);
            this.tbEndSequence.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ending Sequence";
            // 
            // tbStartSequence
            // 
            this.tbStartSequence.Location = new System.Drawing.Point(107, 13);
            this.tbStartSequence.Name = "tbStartSequence";
            this.tbStartSequence.ReadOnly = true;
            this.tbStartSequence.Size = new System.Drawing.Size(52, 20);
            this.tbStartSequence.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Starting Sequence";
            // 
            // Xsf_2sfSdatOptimizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 528);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox10);
            this.Name = "Xsf_2sfSdatOptimizerForm";
            this.Text = "Xsf_2sfSdatOptimizer";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.groupBox10, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbSourceSdat;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbEndSequence;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbStartSequence;
        private System.Windows.Forms.CheckBox cbIncludeAllSseq;
    }
}