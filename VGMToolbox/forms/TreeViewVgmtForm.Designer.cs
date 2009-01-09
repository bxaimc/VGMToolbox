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
            this.components = new System.ComponentModel.Container();
            this.treeViewTools = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.filePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 396);
            this.pnlLabels.Size = new System.Drawing.Size(823, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(823, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 319);
            this.tbOutput.Size = new System.Drawing.Size(823, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 299);
            this.pnlButtons.Size = new System.Drawing.Size(823, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(763, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(703, 0);
            // 
            // treeViewTools
            // 
            this.treeViewTools.AllowDrop = true;
            this.treeViewTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeViewTools.Location = new System.Drawing.Point(0, 23);
            this.treeViewTools.Name = "treeViewTools";
            this.treeViewTools.Size = new System.Drawing.Size(823, 186);
            this.treeViewTools.TabIndex = 7;
            this.treeViewTools.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewTools_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filePathToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 48);
            // 
            // filePathToolStripMenuItem
            // 
            this.filePathToolStripMenuItem.Name = "filePathToolStripMenuItem";
            this.filePathToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.filePathToolStripMenuItem.Text = "File Path";
            this.filePathToolStripMenuItem.Click += new System.EventHandler(this.filePathToolStripMenuItem_Click);
            // 
            // TreeViewVgmtForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 437);
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
            this.pnlButtons.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.TreeView treeViewTools;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filePathToolStripMenuItem;
    }
}