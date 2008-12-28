namespace VGMToolbox.forms
{
    partial class Xsf_Xsf2ExeForm
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
            this.grpXsfPsf2Exe_Source = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbXsfPsf2Exe_Source = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbXsfPsf2Exe_StripGsfHeader = new System.Windows.Forms.CheckBox();
            this.cbXsfPsf2Exe_IncludeOrigExt = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpXsfPsf2Exe_Source.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 396);
            this.pnlLabels.Size = new System.Drawing.Size(756, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(756, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 319);
            this.tbOutput.Size = new System.Drawing.Size(756, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 299);
            this.pnlButtons.Size = new System.Drawing.Size(756, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(696, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(636, 0);
            // 
            // grpXsfPsf2Exe_Source
            // 
            this.grpXsfPsf2Exe_Source.Controls.Add(this.label12);
            this.grpXsfPsf2Exe_Source.Controls.Add(this.tbXsfPsf2Exe_Source);
            this.grpXsfPsf2Exe_Source.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpXsfPsf2Exe_Source.Location = new System.Drawing.Point(0, 23);
            this.grpXsfPsf2Exe_Source.Name = "grpXsfPsf2Exe_Source";
            this.grpXsfPsf2Exe_Source.Size = new System.Drawing.Size(756, 61);
            this.grpXsfPsf2Exe_Source.TabIndex = 5;
            this.grpXsfPsf2Exe_Source.TabStop = false;
            this.grpXsfPsf2Exe_Source.Text = "Source";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 42);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(171, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Drag and Drop folders or files here.";
            // 
            // tbXsfPsf2Exe_Source
            // 
            this.tbXsfPsf2Exe_Source.AllowDrop = true;
            this.tbXsfPsf2Exe_Source.Location = new System.Drawing.Point(6, 19);
            this.tbXsfPsf2Exe_Source.Name = "tbXsfPsf2Exe_Source";
            this.tbXsfPsf2Exe_Source.Size = new System.Drawing.Size(259, 20);
            this.tbXsfPsf2Exe_Source.TabIndex = 1;
            this.tbXsfPsf2Exe_Source.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbXsfPsf2Exe_Source_DragDrop);
            this.tbXsfPsf2Exe_Source.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbXsfPsf2Exe_StripGsfHeader);
            this.groupBox4.Controls.Add(this.cbXsfPsf2Exe_IncludeOrigExt);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(0, 84);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(756, 61);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Options";
            // 
            // cbXsfPsf2Exe_StripGsfHeader
            // 
            this.cbXsfPsf2Exe_StripGsfHeader.AutoSize = true;
            this.cbXsfPsf2Exe_StripGsfHeader.Location = new System.Drawing.Point(6, 38);
            this.cbXsfPsf2Exe_StripGsfHeader.Name = "cbXsfPsf2Exe_StripGsfHeader";
            this.cbXsfPsf2Exe_StripGsfHeader.Size = new System.Drawing.Size(209, 17);
            this.cbXsfPsf2Exe_StripGsfHeader.TabIndex = 1;
            this.cbXsfPsf2Exe_StripGsfHeader.Text = "Strip GSF Header after Decompression";
            this.cbXsfPsf2Exe_StripGsfHeader.UseVisualStyleBackColor = true;
            // 
            // cbXsfPsf2Exe_IncludeOrigExt
            // 
            this.cbXsfPsf2Exe_IncludeOrigExt.AutoSize = true;
            this.cbXsfPsf2Exe_IncludeOrigExt.Location = new System.Drawing.Point(6, 19);
            this.cbXsfPsf2Exe_IncludeOrigExt.Name = "cbXsfPsf2Exe_IncludeOrigExt";
            this.cbXsfPsf2Exe_IncludeOrigExt.Size = new System.Drawing.Size(234, 17);
            this.cbXsfPsf2Exe_IncludeOrigExt.TabIndex = 0;
            this.cbXsfPsf2Exe_IncludeOrigExt.Text = "Include original file extension in output name";
            this.cbXsfPsf2Exe_IncludeOrigExt.UseVisualStyleBackColor = true;
            // 
            // Xsf_Xsf2ExeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 437);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.grpXsfPsf2Exe_Source);
            this.Name = "Xsf_Xsf2ExeForm";
            this.Text = "Xsf_Xsf2ExeForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpXsfPsf2Exe_Source, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.grpXsfPsf2Exe_Source.ResumeLayout(false);
            this.grpXsfPsf2Exe_Source.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpXsfPsf2Exe_Source;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbXsfPsf2Exe_Source;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbXsfPsf2Exe_StripGsfHeader;
        private System.Windows.Forms.CheckBox cbXsfPsf2Exe_IncludeOrigExt;
    }
}