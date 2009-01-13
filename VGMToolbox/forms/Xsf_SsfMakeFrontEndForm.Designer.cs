namespace VGMToolbox.forms
{
    partial class Xsf_SsfMakeFrontEndForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbMatchSeqBank = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbEffect = new System.Windows.Forms.TextBox();
            this.tbMixerNumber = new System.Windows.Forms.TextBox();
            this.tbMixerBank = new System.Windows.Forms.TextBox();
            this.tbVolume = new System.Windows.Forms.TextBox();
            this.tbSequenceTrack = new System.Windows.Forms.TextBox();
            this.tbSequenceBank = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbDsp = new System.Windows.Forms.TextBox();
            this.tbSequence = new System.Windows.Forms.TextBox();
            this.tbToneData = new System.Windows.Forms.TextBox();
            this.tbDriver = new System.Windows.Forms.TextBox();
            this.pnlLabels.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLabels
            // 
            this.pnlLabels.Location = new System.Drawing.Point(0, 472);
            this.pnlLabels.Size = new System.Drawing.Size(739, 19);
            // 
            // pnlTitle
            // 
            this.pnlTitle.Size = new System.Drawing.Size(739, 20);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(0, 395);
            this.tbOutput.Size = new System.Drawing.Size(739, 77);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Location = new System.Drawing.Point(0, 375);
            this.pnlButtons.Size = new System.Drawing.Size(739, 20);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(679, 0);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoTask
            // 
            this.btnDoTask.Location = new System.Drawing.Point(619, 0);
            this.btnDoTask.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(739, 352);
            this.panel1.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbMatchSeqBank);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tbEffect);
            this.groupBox2.Controls.Add(this.tbMixerNumber);
            this.groupBox2.Controls.Add(this.tbMixerBank);
            this.groupBox2.Controls.Add(this.tbVolume);
            this.groupBox2.Controls.Add(this.tbSequenceTrack);
            this.groupBox2.Controls.Add(this.tbSequenceBank);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(739, 98);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // cbMatchSeqBank
            // 
            this.cbMatchSeqBank.AutoSize = true;
            this.cbMatchSeqBank.Checked = true;
            this.cbMatchSeqBank.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMatchSeqBank.Location = new System.Drawing.Point(305, 21);
            this.cbMatchSeqBank.Name = "cbMatchSeqBank";
            this.cbMatchSeqBank.Size = new System.Drawing.Size(136, 17);
            this.cbMatchSeqBank.TabIndex = 12;
            this.cbMatchSeqBank.Text = "Match Sequence Bank";
            this.cbMatchSeqBank.UseVisualStyleBackColor = true;
            this.cbMatchSeqBank.CheckedChanged += new System.EventHandler(this.cbMatchSeqBank_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(168, 74);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Effect";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(168, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Mixer Number";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(168, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Mixer Bank";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Volume";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Sequence Track";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Sequence Bank";
            // 
            // tbEffect
            // 
            this.tbEffect.Location = new System.Drawing.Point(248, 71);
            this.tbEffect.Name = "tbEffect";
            this.tbEffect.Size = new System.Drawing.Size(51, 20);
            this.tbEffect.TabIndex = 5;
            this.tbEffect.Text = "0x00";
            // 
            // tbMixerNumber
            // 
            this.tbMixerNumber.Location = new System.Drawing.Point(248, 45);
            this.tbMixerNumber.Name = "tbMixerNumber";
            this.tbMixerNumber.Size = new System.Drawing.Size(51, 20);
            this.tbMixerNumber.TabIndex = 4;
            this.tbMixerNumber.Text = "0x00";
            // 
            // tbMixerBank
            // 
            this.tbMixerBank.Location = new System.Drawing.Point(248, 19);
            this.tbMixerBank.Name = "tbMixerBank";
            this.tbMixerBank.ReadOnly = true;
            this.tbMixerBank.Size = new System.Drawing.Size(51, 20);
            this.tbMixerBank.TabIndex = 3;
            this.tbMixerBank.Text = "0x00";
            // 
            // tbVolume
            // 
            this.tbVolume.Location = new System.Drawing.Point(101, 71);
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(51, 20);
            this.tbVolume.TabIndex = 2;
            this.tbVolume.Text = "0x7F";
            // 
            // tbSequenceTrack
            // 
            this.tbSequenceTrack.Location = new System.Drawing.Point(101, 45);
            this.tbSequenceTrack.Name = "tbSequenceTrack";
            this.tbSequenceTrack.Size = new System.Drawing.Size(51, 20);
            this.tbSequenceTrack.TabIndex = 1;
            this.tbSequenceTrack.Text = "0x00";
            // 
            // tbSequenceBank
            // 
            this.tbSequenceBank.Location = new System.Drawing.Point(101, 19);
            this.tbSequenceBank.Name = "tbSequenceBank";
            this.tbSequenceBank.Size = new System.Drawing.Size(51, 20);
            this.tbSequenceBank.TabIndex = 0;
            this.tbSequenceBank.Text = "0x00";
            this.tbSequenceBank.TextChanged += new System.EventHandler(this.tbSequenceBank_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbDsp);
            this.groupBox1.Controls.Add(this.tbSequence);
            this.groupBox1.Controls.Add(this.tbToneData);
            this.groupBox1.Controls.Add(this.tbDriver);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(739, 123);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Files";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "DSP Program";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Sequence Data";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Tone Data";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Driver";
            // 
            // tbDsp
            // 
            this.tbDsp.Location = new System.Drawing.Point(101, 97);
            this.tbDsp.Name = "tbDsp";
            this.tbDsp.Size = new System.Drawing.Size(247, 20);
            this.tbDsp.TabIndex = 3;
            // 
            // tbSequence
            // 
            this.tbSequence.Location = new System.Drawing.Point(101, 71);
            this.tbSequence.Name = "tbSequence";
            this.tbSequence.Size = new System.Drawing.Size(247, 20);
            this.tbSequence.TabIndex = 2;
            // 
            // tbToneData
            // 
            this.tbToneData.Location = new System.Drawing.Point(101, 45);
            this.tbToneData.Name = "tbToneData";
            this.tbToneData.Size = new System.Drawing.Size(247, 20);
            this.tbToneData.TabIndex = 1;
            // 
            // tbDriver
            // 
            this.tbDriver.Location = new System.Drawing.Point(101, 19);
            this.tbDriver.Name = "tbDriver";
            this.tbDriver.Size = new System.Drawing.Size(247, 20);
            this.tbDriver.TabIndex = 0;
            // 
            // Xsf_SsfMakeFrontEndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 513);
            this.Controls.Add(this.panel1);
            this.Name = "Xsf_SsfMakeFrontEndForm";
            this.Text = "Xsf_SsfMakeFrontEndForm";
            this.Controls.SetChildIndex(this.pnlLabels, 0);
            this.Controls.SetChildIndex(this.tbOutput, 0);
            this.Controls.SetChildIndex(this.pnlTitle, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.pnlLabels.ResumeLayout(false);
            this.pnlLabels.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbToneData;
        private System.Windows.Forms.TextBox tbDriver;
        private System.Windows.Forms.TextBox tbSequence;
        private System.Windows.Forms.TextBox tbDsp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbSequenceBank;
        private System.Windows.Forms.TextBox tbSequenceTrack;
        private System.Windows.Forms.TextBox tbVolume;
        private System.Windows.Forms.TextBox tbMixerBank;
        private System.Windows.Forms.TextBox tbEffect;
        private System.Windows.Forms.TextBox tbMixerNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbMatchSeqBank;

    }
}