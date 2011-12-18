
using Microsoft.Ajax.Utilities;
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

            this.cboColorNames.DataSource = System.Enum.GetNames(typeof(CssColor));
            this.cboCommentMode.DataSource = System.Enum.GetNames(typeof(CssComment));
            this.cboOutputMode.DataSource = System.Enum.GetNames(typeof(OutputMode));

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
            this.Settings.MsCssSettings.ColorNames = (CssColor)System.Enum.Parse(typeof(CssColor), cboColorNames.SelectedItem.ToString());
            this.Settings.MsCssSettings.CommentMode = (CssComment)System.Enum.Parse(typeof(CssComment), cboCommentMode.SelectedItem.ToString());

            this.Settings.MsCssSettings.OutputMode = (OutputMode)System.Enum.Parse(typeof(OutputMode), this.cboOutputMode.SelectedItem.ToString());
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
