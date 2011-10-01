using System.Windows.Forms;

namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class YuiCss : BaseConfigurationControl
    {
        public YuiCss()
        {
            InitializeComponent();
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            txtColumnWidth.Value = Settings.YuiCssSettings.ColumnWidth;
            chkRemoveComments.Checked = Settings.YuiCssSettings.RemoveComments;
        }

        public override void OnOK()
        {
            Settings.YuiCssSettings.ColumnWidth = (int)txtColumnWidth.Value;
            Settings.YuiCssSettings.RemoveComments = chkRemoveComments.Checked;
            this.Settings.Save();
        }

        private void linkLabelModeInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://developer.yahoo.com/yui/compressor/");
        }
    }
}
