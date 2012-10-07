using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class TypeScript : BaseConfigurationControl
    {
        public TypeScript()
        {
            this.InitializeComponent();
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            txtChirpTypeScriptFile.Text = this.Settings.ChirpTypeScriptFile;
            chkCompress.Checked = this.Settings.TypeScriptCompress;
        }

        public override void OnOK()
        {
            this.Settings.ChirpTypeScriptFile = txtChirpTypeScriptFile.Text;
            this.Settings.TypeScriptCompress = chkCompress.Checked;
            this.Settings.Save();
        }

        private void linkLabelModeInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.typescriptlang.org/");
        }
    }
}
