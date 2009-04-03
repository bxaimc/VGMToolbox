namespace VGMToolbox.forms
{
    partial class Xsf_PsxSeqExtractForm
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
            this.lblDragNDrop = new System.Windows.Forms.Label();
            this.tbSource = new System.Windows.Forms.TextBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.rbForceSeqType = new System.Windows.Forms.RadioButton();
            this.rbForceSepType = new System.Windows.Forms.RadioButton();
            this.cbForceType = new System.Windows.Forms.CheckBox();
            this.cbForce2Loops = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 354);
            this.pnlLabels.Size = new System.Drawing.Size(776, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(776, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 277);
            this.tbOutput.Size = new System.Drawing.Size(776, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 257);
            this.pnlButtons.Size = new System.Drawing.Size(776, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(716, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(656, 0);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.lblDragNDrop);
            this.grpSource.Controls.Add(this.tbSource);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Location = new System.Drawing.Point(0, 23);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(776, 63);
            this.grpSource.TabIndex = 5;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source Files";
            // 
            // lblDragNDrop
            // 
            this.lblDragNDrop.AutoSize = true;
            this.lblDragNDrop.Location = new System.Drawing.Point(6, 42);
            this.lblDragNDrop.Name = "lblDragNDrop";
            this.lblDragNDrop.Size = new System.Drawing.Size(163, 13);
            this.lblDragNDrop.TabIndex = 1;
            this.lblDragNDrop.Text = "Drag and Drop files to scan here.";
            // 
            // tbSource
            // 
            this.tbSource.AllowDrop = true;
            this.tbSource.Location = new System.Drawing.Point(6, 19);
            this.tbSource.Name = "tbSource";
            this.tbSource.Size = new System.Drawing.Size(282, 20);
            this.tbSource.TabIndex = 0;
            this.tbSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSource_DragDrop);
            this.tbSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.rbForceSeqType);
            this.grpOptions.Controls.Add(this.rbForceSepType);
            this.grpOptions.Controls.Add(this.cbForceType);
            this.grpOptions.Controls.Add(this.cbForce2Loops);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpOptions.Location = new System.Drawing.Point(0, 86);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(776, 115);
            this.grpOptions.TabIndex = 6;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // rbForceSeqType
            // 
            this.rbForceSeqType.AutoSize = true;
            this.rbForceSeqType.Enabled = false;
            this.rbForceSeqType.Location = new System.Drawing.Point(28, 87);
            this.rbForceSeqType.Name = "rbForceSeqType";
            this.rbForceSeqType.Size = new System.Drawing.Size(74, 17);
            this.rbForceSeqType.TabIndex = 3;
            this.rbForceSeqType.TabStop = true;
            this.rbForceSeqType.Text = "SEQ Type";
            this.rbForceSeqType.UseVisualStyleBackColor = true;
            // 
            // rbForceSepType
            // 
            this.rbForceSepType.AutoSize = true;
            this.rbForceSepType.Enabled = false;
            this.rbForceSepType.Location = new System.Drawing.Point(28, 64);
            this.rbForceSepType.Name = "rbForceSepType";
            this.rbForceSepType.Size = new System.Drawing.Size(73, 17);
            this.rbForceSepType.TabIndex = 2;
            this.rbForceSepType.TabStop = true;
            this.rbForceSepType.Text = "SEP Type";
            this.rbForceSepType.UseVisualStyleBackColor = true;
            // 
            // cbForceType
            // 
            this.cbForceType.AutoSize = true;
            this.cbForceType.Location = new System.Drawing.Point(6, 42);
            this.cbForceType.Name = "cbForceType";
            this.cbForceType.Size = new System.Drawing.Size(111, 17);
            this.cbForceType.TabIndex = 1;
            this.cbForceType.Text = "Force source type";
            this.cbForceType.UseVisualStyleBackColor = true;
            this.cbForceType.CheckedChanged += new System.EventHandler(this.cbForceType_CheckedChanged);
            // 
            // cbForce2Loops
            // 
            this.cbForce2Loops.AutoSize = true;
            this.cbForce2Loops.Location = new System.Drawing.Point(6, 19);
            this.cbForce2Loops.Name = "cbForce2Loops";
            this.cbForce2Loops.Size = new System.Drawing.Size(413, 17);
            this.cbForce2Loops.TabIndex = 0;
            this.cbForce2Loops.Text = "Force 2 loops for all loops found (Not recommended, use only for overtimed tracks" +
                ")";
            this.cbForce2Loops.UseVisualStyleBackColor = true;
            // 
            // Xsf_PsxSeqExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 395);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.grpSource);
            this.Name = "Xsf_PsxSeqExtractForm";
            this.Text = "Xsf_PsxSeqExtractForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.Label lblDragNDrop;
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox cbForce2Loops;
        private System.Windows.Forms.CheckBox cbForceType;
        private System.Windows.Forms.RadioButton rbForceSeqType;
        private System.Windows.Forms.RadioButton rbForceSepType;
    }
}