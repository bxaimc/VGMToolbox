namespace VGMToolbox.forms
{
    partial class Auditing_DatafileCreatorForm
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
            this.grpDatCreator_Header = new System.Windows.Forms.GroupBox();
            this.lblDatCreator_HeaderUrl = new System.Windows.Forms.Label();
            this.tbDatCreator_Url = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderHomepage = new System.Windows.Forms.Label();
            this.tbDatCreator_Homepage = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderEmail = new System.Windows.Forms.Label();
            this.tbDatCreator_Email = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderDate = new System.Windows.Forms.Label();
            this.tbDatCreator_Date = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderCategory = new System.Windows.Forms.Label();
            this.tbDatCreator_Category = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderComment = new System.Windows.Forms.Label();
            this.tbDatCreator_Comment = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderAuthor = new System.Windows.Forms.Label();
            this.tbDatCreator_Author = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderVersion = new System.Windows.Forms.Label();
            this.tbDatCreator_Version = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderDescription = new System.Windows.Forms.Label();
            this.tbDatCreator_Description = new System.Windows.Forms.TextBox();
            this.lblDatCreator_HeaderName = new System.Windows.Forms.Label();
            this.tbDatCreator_Name = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDatCreator_BrowseDestination = new System.Windows.Forms.Button();
            this.btnDatCreator_BrowseSource = new System.Windows.Forms.Button();
            this.lblDatCreator_DestinationFolder = new System.Windows.Forms.Label();
            this.tbDatCreator_OutputDat = new System.Windows.Forms.TextBox();
            this.lblDatCreator_SourceFolder = new System.Windows.Forms.Label();
            this.tbDatCreator_SourceFolder = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpDatCreator_Header.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 430);
            this.pnlLabels.Size = new System.Drawing.Size(756, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(756, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 353);
            this.tbOutput.Size = new System.Drawing.Size(756, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 333);
            this.pnlButtons.Size = new System.Drawing.Size(756, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(696, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(636, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnDatCreator_BuildDat_Click);
            // 
            // grpDatCreator_Header
            // 
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderUrl);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Url);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderHomepage);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Homepage);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderEmail);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Email);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderDate);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Date);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderCategory);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Category);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderComment);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Comment);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderAuthor);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Author);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderVersion);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Version);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderDescription);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Description);
            this.grpDatCreator_Header.Controls.Add(this.lblDatCreator_HeaderName);
            this.grpDatCreator_Header.Controls.Add(this.tbDatCreator_Name);
            this.grpDatCreator_Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpDatCreator_Header.Location = new System.Drawing.Point(0, 23);
            this.grpDatCreator_Header.Name = "grpDatCreator_Header";
            this.grpDatCreator_Header.Size = new System.Drawing.Size(756, 140);
            this.grpDatCreator_Header.TabIndex = 5;
            this.grpDatCreator_Header.TabStop = false;
            this.grpDatCreator_Header.Text = "Header Information";
            // 
            // lblDatCreator_HeaderUrl
            // 
            this.lblDatCreator_HeaderUrl.AutoSize = true;
            this.lblDatCreator_HeaderUrl.Location = new System.Drawing.Point(233, 114);
            this.lblDatCreator_HeaderUrl.Name = "lblDatCreator_HeaderUrl";
            this.lblDatCreator_HeaderUrl.Size = new System.Drawing.Size(29, 13);
            this.lblDatCreator_HeaderUrl.TabIndex = 39;
            this.lblDatCreator_HeaderUrl.Text = "URL";
            // 
            // tbDatCreator_Url
            // 
            this.tbDatCreator_Url.Location = new System.Drawing.Point(295, 110);
            this.tbDatCreator_Url.Name = "tbDatCreator_Url";
            this.tbDatCreator_Url.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Url.TabIndex = 38;
            // 
            // lblDatCreator_HeaderHomepage
            // 
            this.lblDatCreator_HeaderHomepage.AutoSize = true;
            this.lblDatCreator_HeaderHomepage.Location = new System.Drawing.Point(233, 90);
            this.lblDatCreator_HeaderHomepage.Name = "lblDatCreator_HeaderHomepage";
            this.lblDatCreator_HeaderHomepage.Size = new System.Drawing.Size(59, 13);
            this.lblDatCreator_HeaderHomepage.TabIndex = 37;
            this.lblDatCreator_HeaderHomepage.Text = "Homepage";
            // 
            // tbDatCreator_Homepage
            // 
            this.tbDatCreator_Homepage.Location = new System.Drawing.Point(295, 86);
            this.tbDatCreator_Homepage.Name = "tbDatCreator_Homepage";
            this.tbDatCreator_Homepage.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Homepage.TabIndex = 36;
            // 
            // lblDatCreator_HeaderEmail
            // 
            this.lblDatCreator_HeaderEmail.AutoSize = true;
            this.lblDatCreator_HeaderEmail.Location = new System.Drawing.Point(233, 66);
            this.lblDatCreator_HeaderEmail.Name = "lblDatCreator_HeaderEmail";
            this.lblDatCreator_HeaderEmail.Size = new System.Drawing.Size(33, 13);
            this.lblDatCreator_HeaderEmail.TabIndex = 35;
            this.lblDatCreator_HeaderEmail.Text = "EMail";
            // 
            // tbDatCreator_Email
            // 
            this.tbDatCreator_Email.Location = new System.Drawing.Point(295, 62);
            this.tbDatCreator_Email.Name = "tbDatCreator_Email";
            this.tbDatCreator_Email.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Email.TabIndex = 34;
            // 
            // lblDatCreator_HeaderDate
            // 
            this.lblDatCreator_HeaderDate.AutoSize = true;
            this.lblDatCreator_HeaderDate.Location = new System.Drawing.Point(233, 42);
            this.lblDatCreator_HeaderDate.Name = "lblDatCreator_HeaderDate";
            this.lblDatCreator_HeaderDate.Size = new System.Drawing.Size(30, 13);
            this.lblDatCreator_HeaderDate.TabIndex = 33;
            this.lblDatCreator_HeaderDate.Text = "Date";
            // 
            // tbDatCreator_Date
            // 
            this.tbDatCreator_Date.Location = new System.Drawing.Point(295, 38);
            this.tbDatCreator_Date.Name = "tbDatCreator_Date";
            this.tbDatCreator_Date.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Date.TabIndex = 32;
            // 
            // lblDatCreator_HeaderCategory
            // 
            this.lblDatCreator_HeaderCategory.AutoSize = true;
            this.lblDatCreator_HeaderCategory.Location = new System.Drawing.Point(233, 19);
            this.lblDatCreator_HeaderCategory.Name = "lblDatCreator_HeaderCategory";
            this.lblDatCreator_HeaderCategory.Size = new System.Drawing.Size(49, 13);
            this.lblDatCreator_HeaderCategory.TabIndex = 31;
            this.lblDatCreator_HeaderCategory.Text = "Category";
            // 
            // tbDatCreator_Category
            // 
            this.tbDatCreator_Category.Location = new System.Drawing.Point(295, 15);
            this.tbDatCreator_Category.Name = "tbDatCreator_Category";
            this.tbDatCreator_Category.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Category.TabIndex = 30;
            // 
            // lblDatCreator_HeaderComment
            // 
            this.lblDatCreator_HeaderComment.AutoSize = true;
            this.lblDatCreator_HeaderComment.Location = new System.Drawing.Point(4, 114);
            this.lblDatCreator_HeaderComment.Name = "lblDatCreator_HeaderComment";
            this.lblDatCreator_HeaderComment.Size = new System.Drawing.Size(51, 13);
            this.lblDatCreator_HeaderComment.TabIndex = 29;
            this.lblDatCreator_HeaderComment.Text = "Comment";
            // 
            // tbDatCreator_Comment
            // 
            this.tbDatCreator_Comment.Location = new System.Drawing.Point(66, 111);
            this.tbDatCreator_Comment.Name = "tbDatCreator_Comment";
            this.tbDatCreator_Comment.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Comment.TabIndex = 28;
            // 
            // lblDatCreator_HeaderAuthor
            // 
            this.lblDatCreator_HeaderAuthor.AutoSize = true;
            this.lblDatCreator_HeaderAuthor.Location = new System.Drawing.Point(4, 90);
            this.lblDatCreator_HeaderAuthor.Name = "lblDatCreator_HeaderAuthor";
            this.lblDatCreator_HeaderAuthor.Size = new System.Drawing.Size(38, 13);
            this.lblDatCreator_HeaderAuthor.TabIndex = 27;
            this.lblDatCreator_HeaderAuthor.Text = "Author";
            // 
            // tbDatCreator_Author
            // 
            this.tbDatCreator_Author.Location = new System.Drawing.Point(66, 86);
            this.tbDatCreator_Author.Name = "tbDatCreator_Author";
            this.tbDatCreator_Author.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Author.TabIndex = 26;
            // 
            // lblDatCreator_HeaderVersion
            // 
            this.lblDatCreator_HeaderVersion.AutoSize = true;
            this.lblDatCreator_HeaderVersion.Location = new System.Drawing.Point(4, 66);
            this.lblDatCreator_HeaderVersion.Name = "lblDatCreator_HeaderVersion";
            this.lblDatCreator_HeaderVersion.Size = new System.Drawing.Size(42, 13);
            this.lblDatCreator_HeaderVersion.TabIndex = 25;
            this.lblDatCreator_HeaderVersion.Text = "Version";
            // 
            // tbDatCreator_Version
            // 
            this.tbDatCreator_Version.Location = new System.Drawing.Point(66, 62);
            this.tbDatCreator_Version.Name = "tbDatCreator_Version";
            this.tbDatCreator_Version.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Version.TabIndex = 24;
            // 
            // lblDatCreator_HeaderDescription
            // 
            this.lblDatCreator_HeaderDescription.AutoSize = true;
            this.lblDatCreator_HeaderDescription.Location = new System.Drawing.Point(4, 42);
            this.lblDatCreator_HeaderDescription.Name = "lblDatCreator_HeaderDescription";
            this.lblDatCreator_HeaderDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDatCreator_HeaderDescription.TabIndex = 23;
            this.lblDatCreator_HeaderDescription.Text = "Description";
            // 
            // tbDatCreator_Description
            // 
            this.tbDatCreator_Description.Location = new System.Drawing.Point(66, 38);
            this.tbDatCreator_Description.Name = "tbDatCreator_Description";
            this.tbDatCreator_Description.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Description.TabIndex = 22;
            // 
            // lblDatCreator_HeaderName
            // 
            this.lblDatCreator_HeaderName.AutoSize = true;
            this.lblDatCreator_HeaderName.Location = new System.Drawing.Point(4, 19);
            this.lblDatCreator_HeaderName.Name = "lblDatCreator_HeaderName";
            this.lblDatCreator_HeaderName.Size = new System.Drawing.Size(35, 13);
            this.lblDatCreator_HeaderName.TabIndex = 21;
            this.lblDatCreator_HeaderName.Text = "Name";
            // 
            // tbDatCreator_Name
            // 
            this.tbDatCreator_Name.Location = new System.Drawing.Point(66, 15);
            this.tbDatCreator_Name.Name = "tbDatCreator_Name";
            this.tbDatCreator_Name.Size = new System.Drawing.Size(163, 20);
            this.tbDatCreator_Name.TabIndex = 20;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDatCreator_BrowseDestination);
            this.groupBox1.Controls.Add(this.btnDatCreator_BrowseSource);
            this.groupBox1.Controls.Add(this.lblDatCreator_DestinationFolder);
            this.groupBox1.Controls.Add(this.tbDatCreator_OutputDat);
            this.groupBox1.Controls.Add(this.lblDatCreator_SourceFolder);
            this.groupBox1.Controls.Add(this.tbDatCreator_SourceFolder);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 94);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source/Destination";
            // 
            // btnDatCreator_BrowseDestination
            // 
            this.btnDatCreator_BrowseDestination.Location = new System.Drawing.Point(268, 70);
            this.btnDatCreator_BrowseDestination.Name = "btnDatCreator_BrowseDestination";
            this.btnDatCreator_BrowseDestination.Size = new System.Drawing.Size(28, 20);
            this.btnDatCreator_BrowseDestination.TabIndex = 14;
            this.btnDatCreator_BrowseDestination.Text = "...";
            this.btnDatCreator_BrowseDestination.UseVisualStyleBackColor = true;
            this.btnDatCreator_BrowseDestination.Click += new System.EventHandler(this.btnDatCreator_BrowseDestination_Click);
            // 
            // btnDatCreator_BrowseSource
            // 
            this.btnDatCreator_BrowseSource.Location = new System.Drawing.Point(268, 30);
            this.btnDatCreator_BrowseSource.Name = "btnDatCreator_BrowseSource";
            this.btnDatCreator_BrowseSource.Size = new System.Drawing.Size(28, 20);
            this.btnDatCreator_BrowseSource.TabIndex = 13;
            this.btnDatCreator_BrowseSource.Text = "...";
            this.btnDatCreator_BrowseSource.UseVisualStyleBackColor = true;
            this.btnDatCreator_BrowseSource.Click += new System.EventHandler(this.btnDatCreator_BrowseSource_Click);
            // 
            // lblDatCreator_DestinationFolder
            // 
            this.lblDatCreator_DestinationFolder.AutoSize = true;
            this.lblDatCreator_DestinationFolder.Location = new System.Drawing.Point(3, 53);
            this.lblDatCreator_DestinationFolder.Name = "lblDatCreator_DestinationFolder";
            this.lblDatCreator_DestinationFolder.Size = new System.Drawing.Size(43, 13);
            this.lblDatCreator_DestinationFolder.TabIndex = 12;
            this.lblDatCreator_DestinationFolder.Text = "Datafile";
            // 
            // tbDatCreator_OutputDat
            // 
            this.tbDatCreator_OutputDat.Location = new System.Drawing.Point(3, 70);
            this.tbDatCreator_OutputDat.Name = "tbDatCreator_OutputDat";
            this.tbDatCreator_OutputDat.Size = new System.Drawing.Size(259, 20);
            this.tbDatCreator_OutputDat.TabIndex = 11;
            // 
            // lblDatCreator_SourceFolder
            // 
            this.lblDatCreator_SourceFolder.AutoSize = true;
            this.lblDatCreator_SourceFolder.Location = new System.Drawing.Point(3, 13);
            this.lblDatCreator_SourceFolder.Name = "lblDatCreator_SourceFolder";
            this.lblDatCreator_SourceFolder.Size = new System.Drawing.Size(73, 13);
            this.lblDatCreator_SourceFolder.TabIndex = 10;
            this.lblDatCreator_SourceFolder.Text = "Source Folder";
            // 
            // tbDatCreator_SourceFolder
            // 
            this.tbDatCreator_SourceFolder.Location = new System.Drawing.Point(3, 30);
            this.tbDatCreator_SourceFolder.Name = "tbDatCreator_SourceFolder";
            this.tbDatCreator_SourceFolder.Size = new System.Drawing.Size(259, 20);
            this.tbDatCreator_SourceFolder.TabIndex = 9;
            // 
            // Auditing_DatafileCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 471);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpDatCreator_Header);
            this.Name = "Auditing_DatafileCreatorForm";
            this.Text = "Auditing_DatafileCreatorForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpDatCreator_Header, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.grpDatCreator_Header.ResumeLayout(false);
            this.grpDatCreator_Header.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDatCreator_Header;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDatCreator_BrowseDestination;
        private System.Windows.Forms.Button btnDatCreator_BrowseSource;
        private System.Windows.Forms.Label lblDatCreator_DestinationFolder;
        private System.Windows.Forms.TextBox tbDatCreator_OutputDat;
        private System.Windows.Forms.Label lblDatCreator_SourceFolder;
        private System.Windows.Forms.TextBox tbDatCreator_SourceFolder;
        private System.Windows.Forms.Label lblDatCreator_HeaderUrl;
        private System.Windows.Forms.TextBox tbDatCreator_Url;
        private System.Windows.Forms.Label lblDatCreator_HeaderHomepage;
        private System.Windows.Forms.TextBox tbDatCreator_Homepage;
        private System.Windows.Forms.Label lblDatCreator_HeaderEmail;
        private System.Windows.Forms.TextBox tbDatCreator_Email;
        private System.Windows.Forms.Label lblDatCreator_HeaderDate;
        private System.Windows.Forms.TextBox tbDatCreator_Date;
        private System.Windows.Forms.Label lblDatCreator_HeaderCategory;
        private System.Windows.Forms.TextBox tbDatCreator_Category;
        private System.Windows.Forms.Label lblDatCreator_HeaderComment;
        private System.Windows.Forms.TextBox tbDatCreator_Comment;
        private System.Windows.Forms.Label lblDatCreator_HeaderAuthor;
        private System.Windows.Forms.TextBox tbDatCreator_Author;
        private System.Windows.Forms.Label lblDatCreator_HeaderVersion;
        private System.Windows.Forms.TextBox tbDatCreator_Version;
        private System.Windows.Forms.Label lblDatCreator_HeaderDescription;
        private System.Windows.Forms.TextBox tbDatCreator_Description;
        private System.Windows.Forms.Label lblDatCreator_HeaderName;
        private System.Windows.Forms.TextBox tbDatCreator_Name;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}