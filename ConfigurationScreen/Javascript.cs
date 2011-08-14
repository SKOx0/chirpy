﻿using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class Javascript : BaseConfigurationControl
    {
        public Javascript()
        {
            this.InitializeComponent();
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            this.txtChirpJsFile.Text = this.Settings.ChirpJsFile;
            this.txtChirpSimpleJsFile.Text = this.Settings.ChirpSimpleJsFile;
            this.txtChirpWhiteSpaceJsFile.Text = this.Settings.ChirpWhiteSpaceJsFile;
            this.txtChirpYUIJsFile.Text = this.Settings.ChirpYUIJsFile;
            this.txtMSAjaxJsFile.Text = this.Settings.ChirpMSAjaxJsFile;
            this.txtUglifyJsFile.Text = this.Settings.ChirpUglifyJsFile;
            this.txtOutputExtension.Text = this.Settings.OutputExtensionJS;
        }

        public override void OnOK()
        {
            this.Settings.ChirpJsFile = this.txtChirpJsFile.Text;
            this.Settings.ChirpSimpleJsFile = this.txtChirpSimpleJsFile.Text;
            this.Settings.ChirpWhiteSpaceJsFile = this.txtChirpWhiteSpaceJsFile.Text;
            this.Settings.ChirpYUIJsFile = this.txtChirpYUIJsFile.Text;
            this.Settings.ChirpMSAjaxJsFile = this.txtMSAjaxJsFile.Text;
            this.Settings.ChirpUglifyJsFile = this.txtUglifyJsFile.Text;
            this.Settings.OutputExtensionJS = this.txtOutputExtension.Text;
            this.Settings.Save();
        }
    }
}
