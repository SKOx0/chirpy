using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class Less : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public Less()
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
            txtChirpLessFile.Text = Settings.ChirpLessFile;
            txtMichaelAshLessFile.Text = Settings.ChirpMichaelAshLessFile;
            txtHybridLessFile.Text = Settings.ChirpHybridLessFile;
            txtMSAjaxLessFile.Text = Settings.ChirpMSAjaxLessFile;
           
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
            Settings.ChirpLessFile = txtChirpLessFile.Text;
            Settings.ChirpMichaelAshLessFile = txtMichaelAshLessFile.Text;
            Settings.ChirpHybridLessFile = txtHybridLessFile.Text;
            Settings.ChirpMSAjaxLessFile = txtMSAjaxLessFile.Text;
           
            Settings.Save();
        }
    }
}
