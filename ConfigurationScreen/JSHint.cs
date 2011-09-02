﻿
namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class JSHint : BaseConfigurationControl
    {
        public JSHint()
        {
            InitializeComponent();
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            chkJSHint.Checked = this.Settings.RunJSHint;
            propertyGridOptions.SelectedObject = this.Settings.JsHintOptions;
        }

        public override void OnOK()
        {
            this.Settings.RunJSHint = chkJSHint.Checked;
            this.Settings.Save();
        }
    }
}
