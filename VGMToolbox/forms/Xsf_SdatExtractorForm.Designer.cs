namespace VGMToolbox.forms
{
    partial class Xsf_SdatExtractorForm
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
            this.label17 = new System.Windows.Forms.Label();
            this.tbNDS_SdatExtractor_Source = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 330);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 253);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.label17);
            this.groupBox10.Controls.Add(this.tbNDS_SdatExtractor_Source);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox10.Location = new System.Drawing.Point(0, 20);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(533, 59);
            this.groupBox10.TabIndex = 6;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Source SDAT";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 42);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(147, 13);
            this.label17.TabIndex = 1;
            this.label17.Text = "Drag SDAT(s) to extract here.";
            // 
            // tbNDS_SdatExtractor_Source
            // 
            this.tbNDS_SdatExtractor_Source.AllowDrop = true;
            this.tbNDS_SdatExtractor_Source.Location = new System.Drawing.Point(6, 19);
            this.tbNDS_SdatExtractor_Source.Name = "tbNDS_SdatExtractor_Source";
            this.tbNDS_SdatExtractor_Source.Size = new System.Drawing.Size(282, 20);
            this.tbNDS_SdatExtractor_Source.TabIndex = 0;
            this.tbNDS_SdatExtractor_Source.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbNDS_SdatExtractor_Source_DragDrop);
            this.tbNDS_SdatExtractor_Source.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // Xsf_SdatExtractorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 393);
            this.Controls.Add(this.groupBox10);
            this.Name = "Xsf_SdatExtractorForm";
            this.Text = "Xsf_SdatExtractorForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.groupBox10, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbNDS_SdatExtractor_Source;
    }
}