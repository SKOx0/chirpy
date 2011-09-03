
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
            
            chkIeFilters.Checked = this.Settings.CssLintOptions.ieFilters;
            chkStarHack.Checked = this.Settings.CssLintOptions.starHack;
            chkUnderscoreHack.Checked = this.Settings.CssLintOptions.underscoreHack;
        }

        public override void OnOK()
        {
            this.Settings.RunCSSLint = chkCSSLint.Checked;

            this.Settings.CssLintOptions.ieFilters = chkIeFilters.Checked;
            this.Settings.CssLintOptions.starHack = chkStarHack.Checked;
            this.Settings.CssLintOptions.underscoreHack = chkUnderscoreHack.Checked;

            this.Settings.Save();
        }
    }
}
