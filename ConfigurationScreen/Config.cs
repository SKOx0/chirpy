
using System;
using System.Windows.Forms;
namespace Zippy.Chirp.ConfigurationScreen {
    public partial class Config : UserControl, EnvDTE.IDTToolsOptionsPage {
        public Config() {
            InitializeComponent();
        }

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object PropertiesObject) {
            PropertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE DTEObject) {
            Settings.Load();
            txtChirpConfigFile.Text = Settings.ChirpConfigFile;

            cmbCss.Items.Clear();
            cmbCss.Items.Add(Xml.MinifyType.msAjax.Description());
            cmbCss.Items.Add(Xml.MinifyType.yui.Description());
            cmbCss.Items.Add(Xml.MinifyType.yuiHybrid.Description());
            cmbCss.Items.Add(Xml.MinifyType.yuiMARE.Description());
            cmbCss.Text = Settings.DefaultCssMinifier.Description();

            cmbJavaScript.Items.Clear();
            cmbJavaScript.Items.Add(Xml.MinifyType.msAjax.Description());
            cmbJavaScript.Items.Add(Xml.MinifyType.yui.Description());
            cmbJavaScript.Items.Add(Xml.MinifyType.gctAdvanced.Description());
            cmbJavaScript.Items.Add(Xml.MinifyType.gctSimple.Description());
            cmbJavaScript.Items.Add(Xml.MinifyType.gctWhiteSpaceOnly.Description());
            cmbJavaScript.Items.Add(Xml.MinifyType.uglify.Description());
            cmbJavaScript.Text = Settings.DefaultJavaScriptMinifier.Description();
        }

        void EnvDTE.IDTToolsOptionsPage.OnCancel() {
            throw new NotImplementedException();
        }

        void EnvDTE.IDTToolsOptionsPage.OnHelp() {
            System.Diagnostics.Process.Start("http://chirpy.codeplex.com/");
        }

        void EnvDTE.IDTToolsOptionsPage.OnOK() {
            Settings.ChirpConfigFile = txtChirpConfigFile.Text;
            Settings.DefaultCssMinifier = cmbCss.Text.ToEnum(Xml.MinifyType.Unspecified);
            Settings.DefaultJavaScriptMinifier = cmbJavaScript.Text.ToEnum(Xml.MinifyType.Unspecified);
            Settings.Save();
        }

        private void Config_Load(object sender, EventArgs e) {

        }
    }
}
