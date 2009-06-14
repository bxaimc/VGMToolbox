namespace VGMToolbox.forms
{
    partial class Hoot_AuditorForm
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
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.lblFolder = new System.Windows.Forms.Label();
            this.tbXmlPath = new System.Windows.Forms.TextBox();
            this.btnBrowseXmlFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbArchiveFolders = new System.Windows.Forms.TextBox();
            this.btnBrowseArchiveFolders = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewTools
            // 
            this.treeViewTools.LineColor = System.Drawing.Color.Black;
            this.treeViewTools.Size = new System.Drawing.Size(939, 218);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grpOptions);
            this.splitContainer1.Size = new System.Drawing.Size(939, 393);
            this.splitContainer1.SplitterDistance = 218;
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 513);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 436);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 416);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Click += new System.EventHandler(this.btnDoTask_Click);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.btnBrowseArchiveFolders);
            this.grpOptions.Controls.Add(this.tbArchiveFolders);
            this.grpOptions.Controls.Add(this.label1);
            this.grpOptions.Controls.Add(this.btnBrowseXmlFolder);
            this.grpOptions.Controls.Add(this.lblFolder);
            this.grpOptions.Controls.Add(this.tbXmlPath);
            this.grpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOptions.Location = new System.Drawing.Point(0, 0);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(939, 171);
            this.grpOptions.TabIndex = 0;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(6, 22);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(61, 13);
            this.lblFolder.TabIndex = 1;
            this.lblFolder.Text = "XML Folder";
            // 
            // tbXmlPath
            // 
            this.tbXmlPath.Location = new System.Drawing.Point(92, 19);
            this.tbXmlPath.Name = "tbXmlPath";
            this.tbXmlPath.Size = new System.Drawing.Size(321, 20);
            this.tbXmlPath.TabIndex = 0;
            // 
            // btnBrowseXmlFolder
            // 
            this.btnBrowseXmlFolder.Location = new System.Drawing.Point(419, 19);
            this.btnBrowseXmlFolder.Name = "btnBrowseXmlFolder";
            this.btnBrowseXmlFolder.Size = new System.Drawing.Size(24, 20);
            this.btnBrowseXmlFolder.TabIndex = 2;
            this.btnBrowseXmlFolder.Text = "...";
            this.btnBrowseXmlFolder.UseVisualStyleBackColor = true;
            this.btnBrowseXmlFolder.Click += new System.EventHandler(this.btnBrowseXmlFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Archive Folders";
            // 
            // tbArchiveFolders
            // 
            this.tbArchiveFolders.Location = new System.Drawing.Point(92, 48);
            this.tbArchiveFolders.Multiline = true;
            this.tbArchiveFolders.Name = "tbArchiveFolders";
            this.tbArchiveFolders.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbArchiveFolders.Size = new System.Drawing.Size(321, 65);
            this.tbArchiveFolders.TabIndex = 4;
            // 
            // btnBrowseArchiveFolders
            // 
            this.btnBrowseArchiveFolders.Location = new System.Drawing.Point(419, 48);
            this.btnBrowseArchiveFolders.Name = "btnBrowseArchiveFolders";
            this.btnBrowseArchiveFolders.Size = new System.Drawing.Size(24, 65);
            this.btnBrowseArchiveFolders.TabIndex = 5;
            this.btnBrowseArchiveFolders.Text = "...";
            this.btnBrowseArchiveFolders.UseVisualStyleBackColor = true;
            this.btnBrowseArchiveFolders.Click += new System.EventHandler(this.btnBrowseArchiveFolders_Click);
            // 
            // Hoot_AuditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 554);
            this.Name = "Hoot_AuditorForm";
            this.Text = "Hoot_AuditorForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox tbXmlPath;
        private System.Windows.Forms.Button btnBrowseXmlFolder;
        private System.Windows.Forms.TextBox tbArchiveFolders;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseArchiveFolders;
    }
}