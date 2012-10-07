namespace Zippy.Chirp.ConfigurationScreen
{
    partial class TypeScript
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkCompress = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtChirpTypeScriptFile = new System.Windows.Forms.TextBox();
            this.linkLabelModeInfo = new System.Windows.Forms.LinkLabel();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkCompress);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtChirpTypeScriptFile);
            this.groupBox3.Location = new System.Drawing.Point(3, 34);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(381, 66);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "TypeScript";
            // 
            // chkCompress
            // 
            this.chkCompress.AutoSize = true;
            this.chkCompress.Location = new System.Drawing.Point(9, 40);
            this.chkCompress.Name = "chkCompress";
            this.chkCompress.Size = new System.Drawing.Size(72, 17);
            this.chkCompress.TabIndex = 20;
            this.chkCompress.Text = "Compress";
            this.chkCompress.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Type script :";
            // 
            // txtChirpTypeScriptFile
            // 
            this.txtChirpTypeScriptFile.Location = new System.Drawing.Point(184, 13);
            this.txtChirpTypeScriptFile.Name = "txtChirpTypeScriptFile";
            this.txtChirpTypeScriptFile.Size = new System.Drawing.Size(128, 20);
            this.txtChirpTypeScriptFile.TabIndex = 11;
            this.txtChirpTypeScriptFile.Text = ".chirp.ts";
            // 
            // linkLabelModeInfo
            // 
            this.linkLabelModeInfo.AutoSize = true;
            this.linkLabelModeInfo.Location = new System.Drawing.Point(287, 12);
            this.linkLabelModeInfo.Name = "linkLabelModeInfo";
            this.linkLabelModeInfo.Size = new System.Drawing.Size(85, 13);
            this.linkLabelModeInfo.TabIndex = 33;
            this.linkLabelModeInfo.TabStop = true;
            this.linkLabelModeInfo.Text = "More information";
            this.linkLabelModeInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelModeInfo_LinkClicked);
            // 
            // TypeScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabelModeInfo);
            this.Controls.Add(this.groupBox3);
            this.Name = "TypeScript";
            this.Size = new System.Drawing.Size(387, 108);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtChirpTypeScriptFile;
        private System.Windows.Forms.CheckBox chkCompress;
        private System.Windows.Forms.LinkLabel linkLabelModeInfo;
    }
}
