namespace VGMToolbox.forms
{
    partial class TreeViewVgmtForm
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
            this.treeViewTools = new System.Windows.Forms.TreeView();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 430);
            this.pnlLabels.Size = new System.Drawing.Size(777, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(777, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 353);
            this.tbOutput.Size = new System.Drawing.Size(777, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 333);
            this.pnlButtons.Size = new System.Drawing.Size(777, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(717, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(657, 0);
            // 
            // treeViewTools
            // 
            this.treeViewTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeViewTools.Location = new System.Drawing.Point(0, 23);
            this.treeViewTools.Name = "treeViewTools";
            this.treeViewTools.Size = new System.Drawing.Size(777, 186);
            this.treeViewTools.TabIndex = 7;
            // 
            // TreeViewVgmtForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 471);
            this.Controls.Add(this.treeViewTools);
            this.Name = "TreeViewVgmtForm";
            this.Text = "TreeViewVgmtForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.treeViewTools, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.TreeView treeViewTools;
    }
}