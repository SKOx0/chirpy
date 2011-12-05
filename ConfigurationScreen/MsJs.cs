
namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class MsJs : BaseConfigurationControl
    {
        public MsJs()
        {
            InitializeComponent();
            this.cboEvalTreatment.DataSource = System.Enum.GetNames(typeof(Microsoft.Ajax.Utilities.EvalTreatment));
            this.cboLocalRenaming.DataSource = System.Enum.GetNames(typeof(Microsoft.Ajax.Utilities.LocalRenaming));
            this.cboOutputMode.DataSource = System.Enum.GetNames(typeof(Microsoft.Ajax.Utilities.OutputMode));
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            
            this.chkAllowEmbeddedAspNetBlocks.Checked = this.Settings.MsJsSettings.AllowEmbeddedAspNetBlocks;
            this.chkEvalLiteralExpressions.Checked = this.Settings.MsJsSettings.EvalLiteralExpressions;
            this.chkIgnoreConditionalCompilation.Checked = this.Settings.MsJsSettings.IgnoreConditionalCompilation;
            this.chkInlineSafeStrings.Checked = this.Settings.MsJsSettings.InlineSafeStrings;
            this.chkManualRenamesProperties.Checked = this.Settings.MsJsSettings.ManualRenamesProperties;
            this.chkMinifyCode.Checked = this.Settings.MsJsSettings.MinifyCode;
            this.chkPreserveFunctionNames.Checked = this.Settings.MsJsSettings.PreserveFunctionNames;
            this.chkRemoveFunctionExpressionNames.Checked = this.Settings.MsJsSettings.RemoveFunctionExpressionNames;
            this.chkCollapseToLiteral.Checked = this.Settings.MsJsSettings.CollapseToLiteral;
            this.chkCombineDuplicateLiterals.Checked = this.Settings.MsJsSettings.CombineDuplicateLiterals;
            this.cboEvalTreatment.SelectedItem = this.Settings.MsJsSettings.EvalTreatment;
            this.TxtIndentSize.Value = this.Settings.MsJsSettings.IndentSize;
            this.cboLocalRenaming.SelectedItem = this.Settings.MsJsSettings.LocalRenaming;
            //this.txtLineBreakThreshold.Value = this.Settings.MsJsSettings.LineBreakThreshold;
            this.chkMacSafariQuirks.Checked = this.Settings.MsJsSettings.MacSafariQuirks;
            this.cboOutputMode.SelectedItem = this.Settings.MsJsSettings.OutputMode;
            this.chkRemoveUnneededCode.Checked = this.Settings.MsJsSettings.RemoveUnneededCode;
            this.chkStripDebugStatements.Checked = this.Settings.MsJsSettings.StripDebugStatements;
            //this.chkStrictMode.Checked = this.Settings.MsJsSettings.StrictMode;
            //this.chkTermSemicolons.Checked = this.Settings.MsJsSettings.TermSemicolons;
        }

        public override void OnOK()
        {
            this.Settings.MsJsSettings.AllowEmbeddedAspNetBlocks = this.chkAllowEmbeddedAspNetBlocks.Checked;
            this.Settings.MsJsSettings.EvalLiteralExpressions = this.chkEvalLiteralExpressions.Checked;
            this.Settings.MsJsSettings.IgnoreConditionalCompilation = this.chkIgnoreConditionalCompilation.Checked;
            this.Settings.MsJsSettings.InlineSafeStrings = this.chkInlineSafeStrings.Checked;
            this.Settings.MsJsSettings.ManualRenamesProperties = this.chkManualRenamesProperties.Checked;
            this.Settings.MsJsSettings.MinifyCode = this.chkMinifyCode.Checked;
            this.Settings.MsJsSettings.PreserveFunctionNames = this.chkPreserveFunctionNames.Checked;
            this.Settings.MsJsSettings.RemoveFunctionExpressionNames = this.chkRemoveFunctionExpressionNames.Checked;
            this.Settings.MsJsSettings.CollapseToLiteral = this.chkCollapseToLiteral.Checked;
            this.Settings.MsJsSettings.CombineDuplicateLiterals = this.chkCombineDuplicateLiterals.Checked;
            this.Settings.MsJsSettings.EvalTreatment = (Microsoft.Ajax.Utilities.EvalTreatment)this.cboEvalTreatment.SelectedItem;
            this.Settings.MsJsSettings.IndentSize = (int)TxtIndentSize.Value;
            this.Settings.MsJsSettings.LocalRenaming = (Microsoft.Ajax.Utilities.LocalRenaming)this.cboLocalRenaming.SelectedItem;
            //this.Settings.MsJsSettings.LineBreakThreshold=(int)this.txtLineBreakThreshold.Value;
            this.Settings.MsJsSettings.MacSafariQuirks = this.chkMacSafariQuirks.Checked;
            this.Settings.MsJsSettings.OutputMode = (Microsoft.Ajax.Utilities.OutputMode)this.cboOutputMode.SelectedItem;
            this.Settings.MsJsSettings.RemoveUnneededCode = this.chkRemoveUnneededCode.Checked;
            this.Settings.MsJsSettings.StripDebugStatements = this.chkStripDebugStatements.Checked;
            //this.Settings.MsJsSettings.StrictMode = this.chkStrictMode.Checked;
            //this.Settings.MsJsSettings.TermSemicolons = this.chkTermSemicolons.Checked;
            this.Settings.Save();
        }

        private void linkLabelModeInfo_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.asp.net/ajaxlibrary/AjaxMinDLL.ashx");
        }
    }
}
