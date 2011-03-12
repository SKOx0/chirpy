﻿using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class Javascript : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public Javascript()
        {
            this.InitializeComponent();
        }

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object PropertiesObject)
        {
            PropertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject)
        {
            Settings.Load();
            this.txtChirpJsFile.Text = Settings.ChirpJsFile;
            this.txtChirpSimpleJsFile.Text = Settings.ChirpSimpleJsFile;
            this.txtChirpWhiteSpaceJsFile.Text = Settings.ChirpWhiteSpaceJsFile;
            this.txtChirpYUIJsFile.Text = Settings.ChirpYUIJsFile;
            this.txtMSAjaxJsFile.Text = Settings.ChirpMSAjaxJsFile;
            this.txtUglifyJsFile.Text = Settings.ChirpUglifyJsFile;
            this.chkJSHint.Checked = Settings.RunJSHint;
        }

        void EnvDTE.IDTToolsOptionsPage.OnCancel()
        {
            throw new NotImplementedException();
        }

        void EnvDTE.IDTToolsOptionsPage.OnHelp()
        {
            System.Diagnostics.Process.Start("http://chirpy.codeplex.com/");
        }

        void EnvDTE.IDTToolsOptionsPage.OnOK()
        {
            Settings.ChirpJsFile = this.txtChirpJsFile.Text;
            Settings.ChirpSimpleJsFile = this.txtChirpSimpleJsFile.Text;
            Settings.ChirpWhiteSpaceJsFile = this.txtChirpWhiteSpaceJsFile.Text;
            Settings.ChirpYUIJsFile = this.txtChirpYUIJsFile.Text;
            Settings.ChirpMSAjaxJsFile = this.txtMSAjaxJsFile.Text;
            Settings.ChirpUglifyJsFile = this.txtUglifyJsFile.Text;
            Settings.RunJSHint = this.chkJSHint.Checked;
            Settings.Save();
        }
    }
}
