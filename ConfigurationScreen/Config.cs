using System;
using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen 
{
    public partial class Config : UserControl, EnvDTE.IDTToolsOptionsPage
    {
        public Config() 
        {
            InitializeComponent();
        }

        void EnvDTE.IDTToolsOptionsPage.GetProperties(ref object propertiesObject)
        {
            propertiesObject = null;
        }

        void EnvDTE.IDTToolsOptionsPage.OnAfterCreated(EnvDTE.DTE dteObject) 
        {
            Settings.Load();
            this.txtChirpConfigFile.Text = Settings.ChirpConfigFile;

            this.cmbCss.Items.Clear();
            this.cmbCss.Items.Add(Xml.MinifyType.msAjax.Description());
            this.cmbCss.Items.Add(Xml.MinifyType.yui.Description());
            this.cmbCss.Items.Add(Xml.MinifyType.yuiHybrid.Description());
            this.cmbCss.Items.Add(Xml.MinifyType.yuiMARE.Description());
            this.cmbCss.Text = Settings.DefaultCssMinifier.Description();

            this.cmbJavaScript.Items.Clear();
            this.cmbJavaScript.Items.Add(Xml.MinifyType.msAjax.Description());
            this.cmbJavaScript.Items.Add(Xml.MinifyType.yui.Description());
            this.cmbJavaScript.Items.Add(Xml.MinifyType.gctAdvanced.Description());
            this.cmbJavaScript.Items.Add(Xml.MinifyType.gctSimple.Description());
            this.cmbJavaScript.Items.Add(Xml.MinifyType.gctWhiteSpaceOnly.Description());
            this.cmbJavaScript.Items.Add(Xml.MinifyType.uglify.Description());
            this.cmbJavaScript.Text = Settings.DefaultJavaScriptMinifier.Description();
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
            Settings.ChirpConfigFile = this.txtChirpConfigFile.Text;
            Settings.DefaultCssMinifier = this.cmbCss.Text.ToEnum(Xml.MinifyType.Unspecified);
            Settings.DefaultJavaScriptMinifier = this.cmbJavaScript.Text.ToEnum(Xml.MinifyType.Unspecified);
            Settings.Save();
        }
    }
}
