namespace VGMToolbox.forms
{
    partial class Xsf_Psf2SqExtractorForm
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
            this.tbPsf2Source = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 415);
            this.pnlLabels.Size = new System.Drawing.Size(846, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(846, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 338);
            this.tbOutput.Size = new System.Drawing.Size(846, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 318);
            this.pnlButtons.Size = new System.Drawing.Size(846, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(786, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(726, 0);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.lblDragNDrop);
            this.grpSource.Controls.Add(this.tbPsf2Source);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSource.Location = new System.Drawing.Point(0, 23);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(846, 59);
            this.grpSource.TabIndex = 6;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source Files/Folders";
            // 
            // lblDragNDrop
            // 
            this.lblDragNDrop.AutoSize = true;
            this.lblDragNDrop.Location = new System.Drawing.Point(6, 42);
            this.lblDragNDrop.Name = "lblDragNDrop";
            this.lblDragNDrop.Size = new System.Drawing.Size(164, 13);
            this.lblDragNDrop.TabIndex = 1;
            this.lblDragNDrop.Text = "Drag PSF2s to Extract From Here";
            // 
            // tbPsf2Source
            // 
            this.tbPsf2Source.AllowDrop = true;
            this.tbPsf2Source.Location = new System.Drawing.Point(6, 19);
            this.tbPsf2Source.Name = "tbPsf2Source";
            this.tbPsf2Source.Size = new System.Drawing.Size(282, 20);
            this.tbPsf2Source.TabIndex = 0;
            this.tbPsf2Source.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbPsf2Source_DragDrop);
            this.tbPsf2Source.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // Xsf_Psf2SqExtractorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 456);
            this.Controls.Add(this.grpSource);
            this.Name = "Xsf_Psf2SqExtractorForm";
            this.Text = "Xsf_Psf2SqExtractorForm";
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
        private System.Windows.Forms.TextBox tbPsf2Source;

    }
}