using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class Css : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public Css()
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
            this.txtChirpCssFile.Text = Settings.ChirpCssFile;
            this.txtMichaelAshCssFile.Text = Settings.ChirpMichaelAshCssFile;
            this.txtHybridCssFile.Text = Settings.ChirpHybridCssFile;
            this.txtMSAjaxCssFile.Text = Settings.ChirpMSAjaxCssFile;
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
            Settings.ChirpCssFile = this.txtChirpCssFile.Text;
            Settings.ChirpMichaelAshCssFile = this.txtMichaelAshCssFile.Text;
            Settings.ChirpHybridCssFile = this.txtHybridCssFile.Text;
            Settings.ChirpMSAjaxCssFile = this.txtMSAjaxCssFile.Text;
            Settings.Save();
        }
    }
}
