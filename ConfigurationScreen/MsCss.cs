
namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class MsCss : BaseConfigurationControl
    {
        public MsCss()
        {
            InitializeComponent();
            this.cboColorNames.DataSource = System.Enum.GetNames(typeof(Microsoft.Ajax.Utilities.CssColor));
            this.cboCommentMode.DataSource = System.Enum.GetNames(typeof(Microsoft.Ajax.Utilities.CssComment));
            this.cboOutputMode.DataSource = System.Enum.GetNames(typeof(Microsoft.Ajax.Utilities.OutputMode));

        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            this.chkAllowEmbeddedAspNetBlocks.Checked = this.Settings.MsCssSettings.AllowEmbeddedAspNetBlocks;
            this.cboColorNames.SelectedItem = this.Settings.MsCssSettings.ColorNames;
            this.cboCommentMode.SelectedItem = this.Settings.MsCssSettings.CommentMode;
            this.cboOutputMode.SelectedItem = this.Settings.MsCssSettings.OutputMode;
            this.chkMinifyExpressions.Checked = this.Settings.MsCssSettings.MinifyExpressions;
            this.txtLineBreakThreshold.Value = this.Settings.MsCssSettings.LineBreakThreshold;
            this.chkTermSemicolons.Checked = this.Settings.MsCssSettings.TermSemicolons;
        }

        public override void OnOK()
        {
            this.Settings.MsCssSettings.AllowEmbeddedAspNetBlocks = this.chkAllowEmbeddedAspNetBlocks.Checked;
            this.Settings.MsCssSettings.ColorNames = (Microsoft.Ajax.Utilities.CssColor)cboColorNames.SelectedItem;
            this.Settings.MsCssSettings.CommentMode = (Microsoft.Ajax.Utilities.CssComment)cboCommentMode.SelectedItem;
            this.cboOutputMode.SelectedItem = (Microsoft.Ajax.Utilities.OutputMode)this.Settings.MsCssSettings.OutputMode;
            this.Settings.MsCssSettings.MinifyExpressions = this.chkMinifyExpressions.Checked;
            this.Settings.MsCssSettings.LineBreakThreshold = (int)this.txtLineBreakThreshold.Value;
            this.Settings.MsCssSettings.TermSemicolons = this.chkTermSemicolons.Checked;
            this.Settings.Save();
        }
        


        private void linkLabelModeInfo_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.asp.net/ajaxlibrary/AjaxMinDLL.ashx");
        }
    }
}
