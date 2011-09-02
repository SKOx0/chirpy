
namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class CssLint : BaseConfigurationControl
    {
        public CssLint()
        {
            InitializeComponent();
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            chkCSSLint.Checked = this.Settings.RunCSSLint;
            propertyGridOptions.SelectedObject = this.Settings.CssLintOptions;
        }

        public override void OnOK()
        {
            this.Settings.RunCSSLint = chkCSSLint.Checked;
            this.Settings.Save();
        }
    }
}
