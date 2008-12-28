namespace VGMToolbox.forms
{
    partial class Xsf_2sfRipperForm
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
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tb2sf_Source = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btn2sf_BrowseContainerRom = new System.Windows.Forms.Button();
            this.tb2sf_ContainerPath = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.tb2sf_FilePrefix = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 635);
            this.pnlLabels.Size = new System.Drawing.Size(756, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(756, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 558);
            this.tbOutput.Size = new System.Drawing.Size(756, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 538);
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
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.tb2sf_Source);
            this.groupBox7.Controls.Add(this.label15);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox7.Location = new System.Drawing.Point(0, 23);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(756, 61);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Source ROM";
            // 
            // tb2sf_Source
            // 
            this.tb2sf_Source.AllowDrop = true;
            this.tb2sf_Source.Location = new System.Drawing.Point(6, 19);
            this.tb2sf_Source.Name = "tb2sf_Source";
            this.tb2sf_Source.Size = new System.Drawing.Size(259, 20);
            this.tb2sf_Source.TabIndex = 0;
            this.tb2sf_Source.DragDrop += new System.Windows.Forms.DragEventHandler(this.tb2sf_Source_DragDrop);
            this.tb2sf_Source.DragEnter += new System.Windows.Forms.DragEventHandler(this.doDragEnter);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 42);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(108, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "Drag ROM to rip here";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.btn2sf_BrowseContainerRom);
            this.groupBox8.Controls.Add(this.tb2sf_ContainerPath);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox8.Location = new System.Drawing.Point(0, 84);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(756, 61);
            this.groupBox8.TabIndex = 9;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Container ROM";
            // 
            // btn2sf_BrowseContainerRom
            // 
            this.btn2sf_BrowseContainerRom.Location = new System.Drawing.Point(272, 19);
            this.btn2sf_BrowseContainerRom.Name = "btn2sf_BrowseContainerRom";
            this.btn2sf_BrowseContainerRom.Size = new System.Drawing.Size(28, 20);
            this.btn2sf_BrowseContainerRom.TabIndex = 4;
            this.btn2sf_BrowseContainerRom.Text = "...";
            this.btn2sf_BrowseContainerRom.UseVisualStyleBackColor = true;
            this.btn2sf_BrowseContainerRom.Click += new System.EventHandler(this.btn2sf_BrowseContainerRom_Click);
            // 
            // tb2sf_ContainerPath
            // 
            this.tb2sf_ContainerPath.Location = new System.Drawing.Point(6, 19);
            this.tb2sf_ContainerPath.Name = "tb2sf_ContainerPath";
            this.tb2sf_ContainerPath.Size = new System.Drawing.Size(259, 20);
            this.tb2sf_ContainerPath.TabIndex = 2;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 42);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(178, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "Container ROM Path (Yoshi\'s Island)";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.tb2sf_FilePrefix);
            this.groupBox9.Controls.Add(this.label16);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox9.Location = new System.Drawing.Point(0, 145);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(756, 49);
            this.groupBox9.TabIndex = 10;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Options";
            // 
            // tb2sf_FilePrefix
            // 
            this.tb2sf_FilePrefix.Location = new System.Drawing.Point(6, 19);
            this.tb2sf_FilePrefix.Name = "tb2sf_FilePrefix";
            this.tb2sf_FilePrefix.Size = new System.Drawing.Size(100, 20);
            this.tb2sf_FilePrefix.TabIndex = 5;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(112, 22);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(102, 13);
            this.label16.TabIndex = 6;
            this.label16.Text = "Set Name File Prefix";
            // 
            // Xsf_2sfRipperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 676);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Name = "Xsf_2sfRipperForm";
            this.Text = "Xsf_2sfRipperForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.groupBox7, 0);
            this.Controls.SetChildIndex(this.groupBox8, 0);
            this.Controls.SetChildIndex(this.groupBox9, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox tb2sf_Source;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button btn2sf_BrowseContainerRom;
        private System.Windows.Forms.TextBox tb2sf_ContainerPath;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox tb2sf_FilePrefix;
        private System.Windows.Forms.Label label16;
    }
}