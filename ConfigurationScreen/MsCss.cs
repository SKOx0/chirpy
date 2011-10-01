
namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class MsCss : BaseConfigurationControl
    {
        public MsCss()
        {
            InitializeComponent();
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            this.chkAllowEmbeddedAspNetBlocks.Checked = this.Settings.MsCssSettings.AllowEmbeddedAspNetBlocks;
            cboColorNames.SelectedItem = this.Settings.MsCssSettings.ColorNames;
            cboCommentMode.SelectedItem = this.Settings.MsCssSettings.CommentMode;
            this.chkExpandOutput.Checked = this.Settings.MsCssSettings.ExpandOutput;
            this.txtIndentSpaces.Value = this.Settings.MsCssSettings.IndentSpaces;
            this.chkMinifyExpressions.Checked = this.Settings.MsCssSettings.MinifyExpressions;
            this.txtSeverity.Value = this.Settings.MsCssSettings.Severity;
            this.chkTermSemicolons.Checked = this.Settings.MsCssSettings.TermSemicolons;
        }

        public override void OnOK()
        {
            this.Settings.MsCssSettings.AllowEmbeddedAspNetBlocks = this.chkAllowEmbeddedAspNetBlocks.Checked;
            this.Settings.MsCssSettings.ColorNames = (Microsoft.Ajax.Utilities.CssColor)cboColorNames.SelectedItem;
            this.Settings.MsCssSettings.CommentMode = (Microsoft.Ajax.Utilities.CssComment)cboCommentMode.SelectedItem;
            this.Settings.MsCssSettings.ExpandOutput = this.chkExpandOutput.Checked;
            this.Settings.MsCssSettings.IndentSpaces = (int)this.txtIndentSpaces.Value;
            this.Settings.MsCssSettings.MinifyExpressions = this.chkMinifyExpressions.Checked;
            this.Settings.MsCssSettings.Severity = (int)this.txtSeverity.Value;
            this.Settings.MsCssSettings.TermSemicolons = this.chkTermSemicolons.Checked;
            this.Settings.Save();
        }

        private void linkLabelModeInfo_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.asp.net/ajaxlibrary/AjaxMinDLL.ashx");
        }
    }
}
