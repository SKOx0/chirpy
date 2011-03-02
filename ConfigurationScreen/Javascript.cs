using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen {
    public partial class Javascript : UserControl, EnvDTE.IDTToolsOptionsPage {
        public Javascript() {
            InitializeComponent();
        }

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object PropertiesObject) {
            PropertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject) {
            Settings.Load();
            txtChirpJsFile.Text = Settings.ChirpJsFile;
            txtChirpSimpleJsFile.Text = Settings.ChirpSimpleJsFile;
            txtChirpWhiteSpaceJsFile.Text = Settings.ChirpWhiteSpaceJsFile;
            txtChirpYUIJsFile.Text = Settings.ChirpYUIJsFile;
            txtMSAjaxJsFile.Text = Settings.ChirpMSAjaxJsFile;
            txtUglifyJsFile.Text = Settings.ChirpUglifyJsFile;
            chkJSHint.Checked = Settings.RunJSHint;

        }

        void EnvDTE.IDTToolsOptionsPage.OnCancel() {
            throw new NotImplementedException();
        }

        void EnvDTE.IDTToolsOptionsPage.OnHelp() {
            System.Diagnostics.Process.Start("http://chirpy.codeplex.com/");
        }

        void EnvDTE.IDTToolsOptionsPage.OnOK() {
            Settings.ChirpJsFile = txtChirpJsFile.Text;
            Settings.ChirpSimpleJsFile = txtChirpSimpleJsFile.Text;
            Settings.ChirpWhiteSpaceJsFile = txtChirpWhiteSpaceJsFile.Text;
            Settings.ChirpYUIJsFile = txtChirpYUIJsFile.Text;
            Settings.ChirpMSAjaxJsFile = txtMSAjaxJsFile.Text;
            Settings.ChirpUglifyJsFile = txtUglifyJsFile.Text;
            Settings.RunJSHint = chkJSHint.Checked;
            Settings.Save();
        }

    }
}
