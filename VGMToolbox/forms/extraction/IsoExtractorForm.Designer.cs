namespace VGMToolbox.forms.extraction
{
    partial class IsoExtractorForm
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
            this.IsoFolderTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.fileListView = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnLba = new System.Windows.Forms.ColumnHeader();
            this.columnSize = new System.Windows.Forms.ColumnHeader();
            this.columnDate = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToSubfolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 479);
            this.pnlLabels.Size = new System.Drawing.Size(795, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(795, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 402);
            this.tbOutput.Size = new System.Drawing.Size(795, 77);
            this.toolTip1.SetToolTip(this.tbOutput, "Double-Click to view in your default text editor.");
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 382);
            this.pnlButtons.Size = new System.Drawing.Size(795, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(735, 0);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(675, 0);
            // 
            // IsoFolderTreeView
            // 
            this.IsoFolderTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IsoFolderTreeView.Location = new System.Drawing.Point(0, 0);
            this.IsoFolderTreeView.Name = "IsoFolderTreeView";
            this.IsoFolderTreeView.Size = new System.Drawing.Size(263, 359);
            this.IsoFolderTreeView.TabIndex = 5;
            this.IsoFolderTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.IsoFolderTreeView_MouseUp);
            this.IsoFolderTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.IsoFolderTreeView_AfterSelect);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 23);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.IsoFolderTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.fileListView);
            this.splitContainer1.Size = new System.Drawing.Size(795, 359);
            this.splitContainer1.SplitterDistance = 263;
            this.splitContainer1.TabIndex = 6;
            // 
            // fileListView
            // 
            this.fileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnLba,
            this.columnSize,
            this.columnDate});
            this.fileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListView.FullRowSelect = true;
            this.fileListView.Location = new System.Drawing.Point(0, 0);
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(528, 359);
            this.fileListView.TabIndex = 0;
            this.fileListView.UseCompatibleStateImageBehavior = false;
            this.fileListView.View = System.Windows.Forms.View.Details;
            this.fileListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fileListView_MouseDoubleClick);
            this.fileListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fileListView_MouseUp);
            this.fileListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.fileListView_ColumnClick);
            this.fileListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.fileListView_KeyUp);
            // 
            // columnName
            // 
            this.columnName.Text = "Filename";
            this.columnName.Width = 150;
            // 
            // columnLba
            // 
            this.columnLba.Text = "LBA";
            this.columnLba.Width = 80;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Size";
            this.columnSize.Width = 80;
            // 
            // columnDate
            // 
            this.columnDate.Text = "Date";
            this.columnDate.Width = 80;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToToolStripMenuItem,
            this.extractToSubfolderToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(172, 48);
            // 
            // extractToToolStripMenuItem
            // 
            this.extractToToolStripMenuItem.Name = "extractToToolStripMenuItem";
            this.extractToToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.extractToToolStripMenuItem.Text = "Extract to...";
            this.extractToToolStripMenuItem.Click += new System.EventHandler(this.extractToToolStripMenuItem_Click);
            // 
            // extractToSubfolderToolStripMenuItem
            // 
            this.extractToSubfolderToolStripMenuItem.Name = "extractToSubfolderToolStripMenuItem";
            this.extractToSubfolderToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.extractToSubfolderToolStripMenuItem.Text = "Extract to Subfolder";
            this.extractToSubfolderToolStripMenuItem.Click += new System.EventHandler(this.extractToSubfolderToolStripMenuItem_Click);
            // 
            // IsoExtractorForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 520);
            this.Controls.Add(this.splitContainer1);
            this.Name = "IsoExtractorForm";
            this.Text = "IsoExtractorForm";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.IsoExtractorForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView IsoFolderTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView fileListView;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ColumnHeader columnDate;
        private System.Windows.Forms.ColumnHeader columnLba;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem extractToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractToSubfolderToolStripMenuItem;
    }
}