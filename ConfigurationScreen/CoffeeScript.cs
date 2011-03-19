using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class CoffeeScript : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public CoffeeScript()
        {
            InitializeComponent();
        }

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object propertiesObject)
        {
            propertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject)
        {
            Settings.Load();
            txtChirpJsFile.Text = Settings.ChirpCoffeeScriptFile;
            txtChirpSimpleJsFile.Text = Settings.ChirpSimpleCoffeeScriptFile;
            txtChirpWhiteSpaceJsFile.Text = Settings.ChirpWhiteSpaceCoffeeScriptFile;
            txtChirpYUIJsFile.Text = Settings.ChirpYUICoffeeScriptFile;
            txtMSAjaxJsFile.Text = Settings.ChirpMSAjaxCoffeeScriptFile;
            txtPath.Text = Settings.CoffeeScriptBatFilePath;
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
            Settings.ChirpCoffeeScriptFile = txtChirpJsFile.Text;
            Settings.ChirpSimpleCoffeeScriptFile = txtChirpSimpleJsFile.Text;
            Settings.ChirpWhiteSpaceCoffeeScriptFile = txtChirpWhiteSpaceJsFile.Text;
            Settings.ChirpYUICoffeeScriptFile = txtChirpYUIJsFile.Text;
            Settings.ChirpMSAjaxCoffeeScriptFile = txtMSAjaxJsFile.Text;
            Settings.CoffeeScriptBatFilePath = txtPath.Text;
            Settings.Save();
        }

        private void llDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/alisey/CoffeeScript-Compiler-for-Windows");
        }
    }
}
