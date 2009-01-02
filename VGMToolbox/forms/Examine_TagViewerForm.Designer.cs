namespace VGMToolbox.forms
{
    partial class Examine_TagViewerForm
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
            this.cbCheckForLibs = new System.Windows.Forms.CheckBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewTools
            // 
            this.treeViewTools.LineColor = System.Drawing.Color.Black;
            this.treeViewTools.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbXsfSource_DragDrop);
            this.treeViewTools.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 472);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 395);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 375);
            // 
            // btnCancel
            // 
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Click += new System.EventHandler(this.btnDoTask_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbCheckForLibs);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 209);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(777, 44);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // cbCheckForLibs
            // 
            this.cbCheckForLibs.AutoSize = true;
            this.cbCheckForLibs.Location = new System.Drawing.Point(6, 19);
            this.cbCheckForLibs.Name = "cbCheckForLibs";
            this.cbCheckForLibs.Size = new System.Drawing.Size(171, 17);
            this.cbCheckForLibs.TabIndex = 0;
            this.cbCheckForLibs.Text = "Check for Missing Library Files ";
            this.cbCheckForLibs.UseVisualStyleBackColor = true;
            // 
            // Examine_TagViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 513);
            this.Controls.Add(this.groupBox1);
            this.Name = "Examine_TagViewerForm";
            this.Text = "Examine_TagViewerForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.treeViewTools, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbCheckForLibs;

    }
}