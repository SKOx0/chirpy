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
            this.propertyGridOptions = new System.Windows.Forms.PropertyGrid();
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
            // propertyGridOptions
            // 
            this.propertyGridOptions.Location = new System.Drawing.Point(3, 35);
            this.propertyGridOptions.Name = "propertyGridOptions";
            this.propertyGridOptions.Size = new System.Drawing.Size(378, 249);
            this.propertyGridOptions.TabIndex = 30;
            // 
            // CssLint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyGridOptions);
            this.Controls.Add(this.chkCSSLint);
            this.Name = "CssLint";
            this.Size = new System.Drawing.Size(387, 291);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkCSSLint;
        private System.Windows.Forms.PropertyGrid propertyGridOptions;
    }
}
