using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class Less : BaseConfigurationControl
    {
        public Less()
        {
            this.InitializeComponent();
        }


        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            txtChirpLessFile.Text = this.Settings.ChirpLessFile;
            txtMichaelAshLessFile.Text = this.Settings.ChirpMichaelAshLessFile;
            txtHybridLessFile.Text = this.Settings.ChirpHybridLessFile;
            txtMSAjaxLessFile.Text = this.Settings.ChirpMSAjaxLessFile;
        }

        public override void OnOK()
        {
            this.Settings.ChirpLessFile = txtChirpLessFile.Text;
            this.Settings.ChirpMichaelAshLessFile = txtMichaelAshLessFile.Text;
            this.Settings.ChirpHybridLessFile = txtHybridLessFile.Text;
            this.Settings.ChirpMSAjaxLessFile = txtMSAjaxLessFile.Text;

            this.Settings.Save();
        }
    }
}
