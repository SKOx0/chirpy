﻿
using Microsoft.Ajax.Utilities;
namespace Zippy.Chirp.ConfigurationScreen
{
    public partial class MsJs : BaseConfigurationControl
    {
        public MsJs()
        {
            InitializeComponent();
        }

        public override void OnAfterCreated(EnvDTE.DTE dteObject)
        {
            this.cboEvalTreatment.DataSource = System.Enum.GetNames(typeof(EvalTreatment));
            this.cboLocalRenaming.DataSource = System.Enum.GetNames(typeof(LocalRenaming));
            this.cboOutputMode.DataSource = System.Enum.GetNames(typeof(OutputMode));

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
            this.txtLineBreakThreshold.Value = this.Settings.MsJsSettings.LineBreakThreshold;
            this.chkMacSafariQuirks.Checked = this.Settings.MsJsSettings.MacSafariQuirks;
            this.cboOutputMode.SelectedItem = this.Settings.MsJsSettings.OutputMode;
            this.chkRemoveUnneededCode.Checked = this.Settings.MsJsSettings.RemoveUnneededCode;
            this.chkStripDebugStatements.Checked = this.Settings.MsJsSettings.StripDebugStatements;
            this.chkStrictMode.Checked = this.Settings.MsJsSettings.StrictMode;
            this.chkTermSemicolons.Checked = this.Settings.MsJsSettings.TermSemicolons;
            this.chkPreserveImportantComments.Checked = this.Settings.MsJsSettings.PreserveImportantComments;

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
            this.Settings.MsJsSettings.EvalTreatment = (EvalTreatment)System.Enum.Parse(typeof(EvalTreatment), this.cboEvalTreatment.SelectedItem.ToString());
            this.Settings.MsJsSettings.IndentSize = (int)TxtIndentSize.Value;
            this.Settings.MsJsSettings.LocalRenaming = (LocalRenaming)System.Enum.Parse(typeof(LocalRenaming), this.cboLocalRenaming.SelectedItem.ToString());
            this.Settings.MsJsSettings.LineBreakThreshold = (int)this.txtLineBreakThreshold.Value;
            this.Settings.MsJsSettings.MacSafariQuirks = this.chkMacSafariQuirks.Checked;
            this.Settings.MsJsSettings.OutputMode = (OutputMode)System.Enum.Parse(typeof(OutputMode), this.cboOutputMode.SelectedItem.ToString());
            this.Settings.MsJsSettings.RemoveUnneededCode = this.chkRemoveUnneededCode.Checked;
            this.Settings.MsJsSettings.StripDebugStatements = this.chkStripDebugStatements.Checked;
            this.Settings.MsJsSettings.StrictMode = this.chkStrictMode.Checked;
            this.Settings.MsJsSettings.TermSemicolons = this.chkTermSemicolons.Checked;
            this.Settings.MsJsSettings.PreserveImportantComments = this.chkPreserveImportantComments.Checked;

            this.Settings.Save();
        }

        private void linkLabelModeInfo_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.asp.net/ajaxlibrary/AjaxMinDLL.ashx");
        }
    }
}
