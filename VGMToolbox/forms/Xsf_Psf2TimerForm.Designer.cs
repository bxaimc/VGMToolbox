namespace VGMToolbox.forms
{
    partial class Xsf_Psf2TimerForm
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
            this.tbSourcePaths = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 529);
            this.pnlLabels.Size = new System.Drawing.Size(868, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(868, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 452);
            this.tbOutput.Size = new System.Drawing.Size(868, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 432);
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
            // tbSourcePaths
            // 
            this.tbSourcePaths.AllowDrop = true;
            this.tbSourcePaths.Location = new System.Drawing.Point(3, 29);
            this.tbSourcePaths.Name = "tbSourcePaths";
            this.tbSourcePaths.Size = new System.Drawing.Size(197, 20);
            this.tbSourcePaths.TabIndex = 5;
            this.tbSourcePaths.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSourcePaths_DragDrop);
            this.tbSourcePaths.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // Xsf_Psf2TimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 570);
            this.Controls.Add(this.tbSourcePaths);
            this.Name = "Xsf_Psf2TimerForm";
            this.Text = "Xsf_Psf2TimerForm";
            this.Controls.SetChildIndex(this.tbSourcePaths, 0);
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSourcePaths;
    }
}