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
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 486);
            this.pnlLabels.Size = new System.Drawing.Size(1149, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(1149, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 409);
            this.tbOutput.Size = new System.Drawing.Size(1149, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 389);
            this.pnlButtons.Size = new System.Drawing.Size(1149, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(1089, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(1029, 0);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.lblDragNDrop);
            this.grpSource.Controls.Add(this.tbSource);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Location = new System.Drawing.Point(0, 23);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(1149, 63);
            this.grpSource.TabIndex = 5;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "groupBox1";
            // 
            // lblDragNDrop
            // 
            this.lblDragNDrop.AutoSize = true;
            this.lblDragNDrop.Location = new System.Drawing.Point(6, 42);
            this.lblDragNDrop.Name = "lblDragNDrop";
            this.lblDragNDrop.Size = new System.Drawing.Size(35, 13);
            this.lblDragNDrop.TabIndex = 1;
            this.lblDragNDrop.Text = "label1";
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
            // Xsf_PsxSeqExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1149, 527);
            this.Controls.Add(this.grpSource);
            this.Name = "Xsf_PsxSeqExtractForm";
            this.Text = "Xsf_PsxSeqExtractForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpSource, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.Label lblDragNDrop;
        private System.Windows.Forms.TextBox tbSource;
    }
}