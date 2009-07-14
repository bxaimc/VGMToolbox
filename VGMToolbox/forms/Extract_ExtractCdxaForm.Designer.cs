namespace VGMToolbox.forms
{
    partial class Extract_ExtractCdxaForm
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
            this.cbPatchByte0x11 = new System.Windows.Forms.CheckBox();
            this.cbAddRiffHeader = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 400);
            this.pnlLabels.Size = new System.Drawing.Size(779, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(779, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 323);
            this.tbOutput.Size = new System.Drawing.Size(779, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 303);
            this.pnlButtons.Size = new System.Drawing.Size(779, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(719, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(659, 0);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.grpOptions);
            this.grpSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSource.Location = new System.Drawing.Point(0, 23);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(779, 280);
            this.grpSource.TabIndex = 5;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source Files";
            this.grpSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSource_DragDrop);
            this.grpSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.cbPatchByte0x11);
            this.grpOptions.Controls.Add(this.cbAddRiffHeader);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpOptions.Location = new System.Drawing.Point(3, 214);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(773, 63);
            this.grpOptions.TabIndex = 6;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // cbPatchByte0x11
            // 
            this.cbPatchByte0x11.AutoSize = true;
            this.cbPatchByte0x11.Checked = true;
            this.cbPatchByte0x11.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPatchByte0x11.Location = new System.Drawing.Point(7, 42);
            this.cbPatchByte0x11.Name = "cbPatchByte0x11";
            this.cbPatchByte0x11.Size = new System.Drawing.Size(330, 17);
            this.cbPatchByte0x11.TabIndex = 1;
            this.cbPatchByte0x11.Text = "Patch byte at 0x11 to  equal 0x00 (also needed by most players).";
            this.cbPatchByte0x11.UseVisualStyleBackColor = true;
            // 
            // cbAddRiffHeader
            // 
            this.cbAddRiffHeader.AutoSize = true;
            this.cbAddRiffHeader.Checked = true;
            this.cbAddRiffHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAddRiffHeader.Location = new System.Drawing.Point(7, 19);
            this.cbAddRiffHeader.Name = "cbAddRiffHeader";
            this.cbAddRiffHeader.Size = new System.Drawing.Size(261, 17);
            this.cbAddRiffHeader.TabIndex = 0;
            this.cbAddRiffHeader.Text = "Add RIFF header (currently needed by all players).";
            this.cbAddRiffHeader.UseVisualStyleBackColor = true;
            // 
            // Extract_ExtractCdxaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 441);
            this.Controls.Add(this.grpSource);
            this.Name = "Extract_ExtractCdxaForm";
            this.Text = "Extract_ExtractCdxaForm";
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
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox cbPatchByte0x11;
        private System.Windows.Forms.CheckBox cbAddRiffHeader;
    }
}