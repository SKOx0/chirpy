using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class CoffeeScript : BaseConfigurationControl
    {
        public CoffeeScript()
        {
            InitializeComponent();
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            txtChirpJsFile.Text = this.Settings.ChirpCoffeeScriptFile;
            txtChirpSimpleJsFile.Text = this.Settings.ChirpSimpleCoffeeScriptFile;
            txtChirpWhiteSpaceJsFile.Text = this.Settings.ChirpWhiteSpaceCoffeeScriptFile;
            txtChirpYUIJsFile.Text = this.Settings.ChirpYUICoffeeScriptFile;
            txtMSAjaxJsFile.Text = this.Settings.ChirpMSAjaxCoffeeScriptFile;

            propertyGridOptions.SelectedObject = this.Settings.CoffeeScriptOptions;
        }

        public override void OnOK()
        {
            this.Settings.ChirpCoffeeScriptFile = txtChirpJsFile.Text;
            this.Settings.ChirpSimpleCoffeeScriptFile = txtChirpSimpleJsFile.Text;
            this.Settings.ChirpWhiteSpaceCoffeeScriptFile = txtChirpWhiteSpaceJsFile.Text;
            this.Settings.ChirpYUICoffeeScriptFile = txtChirpYUIJsFile.Text;
            this.Settings.ChirpMSAjaxCoffeeScriptFile = txtMSAjaxJsFile.Text;
            this.Settings.Save();
        }

    }
}
