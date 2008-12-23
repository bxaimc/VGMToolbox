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
            this.gbXsf_SdatExtractor = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbNDS_SdatExtractor_Source = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.gbXsf_SdatExtractor.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 295);
            this.pnlLabels.Size = new System.Drawing.Size(582, 18);
            // 
            // label2
            // 
            this.lblProgressLabel.Location = new System.Drawing.Point(547, 0);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 218);
            this.tbOutput.Size = new System.Drawing.Size(582, 77);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(582, 20);
            // 
            // gbXsf_SdatExtractor
            // 
            this.gbXsf_SdatExtractor.Controls.Add(this.label17);
            this.gbXsf_SdatExtractor.Controls.Add(this.tbNDS_SdatExtractor_Source);
            this.gbXsf_SdatExtractor.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbXsf_SdatExtractor.Location = new System.Drawing.Point(0, 20);
            this.gbXsf_SdatExtractor.Name = "gbXsf_SdatExtractor";
            this.gbXsf_SdatExtractor.Size = new System.Drawing.Size(582, 59);
            this.gbXsf_SdatExtractor.TabIndex = 5;
            this.gbXsf_SdatExtractor.TabStop = false;
            this.gbXsf_SdatExtractor.Text = "Source SDAT";
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
            this.tbNDS_SdatExtractor_Source.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbNDS_SdatExtractor_Source_DragEnter);
            // 
            // Xsf_SdatExtractorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 335);
            this.Controls.Add(this.gbXsf_SdatExtractor);
            this.Name = "Xsf_SdatExtractorForm";
            this.Text = "Xsf_SdatExtractorForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.gbXsf_SdatExtractor, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.gbXsf_SdatExtractor.ResumeLayout(false);
            this.gbXsf_SdatExtractor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbXsf_SdatExtractor;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbNDS_SdatExtractor_Source;



    }
}