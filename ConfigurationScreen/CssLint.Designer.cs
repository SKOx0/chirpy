namespace Zippy.Chirp.ConfigurationScreen
{
    partial class CssLint
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkCSSLint = new System.Windows.Forms.CheckBox();
            this.gbSetting = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkStarHack = new System.Windows.Forms.CheckBox();
            this.chkUnderscoreHack = new System.Windows.Forms.CheckBox();
            this.chkIeFilters = new System.Windows.Forms.CheckBox();
            this.gbSetting.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkCSSLint
            // 
            this.chkCSSLint.AutoSize = true;
            this.chkCSSLint.Location = new System.Drawing.Point(3, 12);
            this.chkCSSLint.Name = "chkCSSLint";
            this.chkCSSLint.Size = new System.Drawing.Size(92, 17);
            this.chkCSSLint.TabIndex = 27;
            this.chkCSSLint.Text = "Run CSS Hint";
            this.chkCSSLint.UseVisualStyleBackColor = true;
            // 
            // gbSetting
            // 
            this.gbSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSetting.Controls.Add(this.panel1);
            this.gbSetting.Location = new System.Drawing.Point(3, 35);
            this.gbSetting.Name = "gbSetting";
            this.gbSetting.Size = new System.Drawing.Size(381, 252);
            this.gbSetting.TabIndex = 28;
            this.gbSetting.TabStop = false;
            this.gbSetting.Text = "Options";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.chkStarHack);
            this.panel1.Controls.Add(this.chkUnderscoreHack);
            this.panel1.Controls.Add(this.chkIeFilters);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 233);
            this.panel1.TabIndex = 29;
            // 
            // chkStarHack
            // 
            this.chkStarHack.AutoSize = true;
            this.chkStarHack.Location = new System.Drawing.Point(3, 3);
            this.chkStarHack.Name = "chkStarHack";
            this.chkStarHack.Size = new System.Drawing.Size(156, 17);
            this.chkStarHack.TabIndex = 0;
            this.chkStarHack.Text = "Allow IE6 star hack as valid";
            this.chkStarHack.UseVisualStyleBackColor = true;
            // 
            // chkUnderscoreHack
            // 
            this.chkUnderscoreHack.AutoSize = true;
            this.chkUnderscoreHack.Location = new System.Drawing.Point(3, 25);
            this.chkUnderscoreHack.Name = "chkUnderscoreHack";
            this.chkUnderscoreHack.Size = new System.Drawing.Size(348, 17);
            this.chkUnderscoreHack.TabIndex = 1;
            this.chkUnderscoreHack.Text = "Interpret leading underscores as IE6-7 targeting for known properties";
            this.chkUnderscoreHack.UseVisualStyleBackColor = true;
            // 
            // chkIeFilters
            // 
            this.chkIeFilters.AutoSize = true;
            this.chkIeFilters.Location = new System.Drawing.Point(3, 47);
            this.chkIeFilters.Name = "chkIeFilters";
            this.chkIeFilters.Size = new System.Drawing.Size(361, 17);
            this.chkIeFilters.TabIndex = 2;
            this.chkIeFilters.Text = "Indicate that IE 8 filters should be accepted and not throw syntax errors";
            this.chkIeFilters.UseVisualStyleBackColor = true;
            // 
            // CssLint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbSetting);
            this.Controls.Add(this.chkCSSLint);
            this.Name = "CssLint";
            this.Size = new System.Drawing.Size(387, 291);
            this.gbSetting.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkCSSLint;
        private System.Windows.Forms.GroupBox gbSetting;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkStarHack;
        private System.Windows.Forms.CheckBox chkUnderscoreHack;
        private System.Windows.Forms.CheckBox chkIeFilters;
    }
}
