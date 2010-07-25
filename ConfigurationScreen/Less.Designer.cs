namespace Zippy.Chirp.ConfigurationScreen
{
    partial class Less
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
            this.label5 = new System.Windows.Forms.Label();
            this.txtChirpLessFile = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "LESS :";
            // 
            // txtChirpLessFile
            // 
            this.txtChirpLessFile.Location = new System.Drawing.Point(203, 17);
            this.txtChirpLessFile.Name = "txtChirpLessFile";
            this.txtChirpLessFile.Size = new System.Drawing.Size(128, 20);
            this.txtChirpLessFile.TabIndex = 11;
            this.txtChirpLessFile.Text = ".chirp.less";
            // 
            // Less
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtChirpLessFile);
            this.Name = "Less";
            this.Size = new System.Drawing.Size(367, 141);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtChirpLessFile;
    }
}
