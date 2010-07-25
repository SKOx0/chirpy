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

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object PropertiesObject)
        {
            PropertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject)
        {
            Settings.Load();
            txtChirpCssFile.Text = Settings.ChirpCssFile;
            txtMichaelAshCssFile.Text = Settings.ChirpMichaelAshCssFile;
            txtHybridCssFile.Text = Settings.ChirpHybridCssFile;
            txtMSAjaxCssFile.Text = Settings.ChirpMSAjaxCssFile;
           
 
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
            Settings.ChirpCssFile = txtChirpCssFile.Text;
            Settings.ChirpMichaelAshCssFile = txtMichaelAshCssFile.Text;
            Settings.ChirpHybridCssFile = txtHybridCssFile.Text;
            Settings.ChirpMSAjaxCssFile = txtMSAjaxCssFile.Text;
            Settings.Save();
        }
    }
}
