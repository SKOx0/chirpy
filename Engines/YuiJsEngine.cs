using Yahoo.Yui.Compressor;

namespace Zippy.Chirp.Engines
{
    public class YuiJsEngine : JsEngine
    {
        #region "constructor"
        public YuiJsEngine()
        {
            Extensions = new[] { this.Settings.ChirpYUIJsFile };
            OutputExtension = this.Settings.OutputExtensionJS;
        }
        #endregion

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            Settings settings=Settings.Instance(fullFileName);
            return Minify(fullFileName, text, projectItem, settings.YuiJsSettings);
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem,Yui.JsSettings jsSettings)
        {
            string returnScript = string.Empty;
            var reporter = new EcmaScriptErrorReporter(fullFileName, projectItem);
            try
            {
                // http://chirpy.codeplex.com/workitem/133
                int lineBreakPosition = jsSettings.LineBreakPosition;
                if (lineBreakPosition == 0)
                {
                    lineBreakPosition = -1;
                }

                // http://chirpy.codeplex.com/workitem/54
                // http://chirpy.codeplex.com/workitem/60
                /*var compressor = new JavaScriptCompressor(text, true, System.Text.Encoding.UTF8, System.Globalization.CultureInfo.InvariantCulture, false, reporter);
                returnScript = compressor.Compress(jsSettings.IsObfuscateJavascript, jsSettings.PreserveAllSemiColons, jsSettings.DisableOptimizations, lineBreakPosition);*/
                var compressor = new JavaScriptCompressor();
                compressor.CompressionType = CompressionType.Standard;
                compressor.DisableOptimizations = jsSettings.DisableOptimizations;
                compressor.Encoding = System.Text.Encoding.UTF8;
                compressor.IgnoreEval = jsSettings.IgnoreEval;
                compressor.LineBreakPosition = lineBreakPosition;
                compressor.LoggingType = LoggingType.Info;
                compressor.ObfuscateJavascript = jsSettings.IsObfuscateJavascript;
                compressor.PreserveAllSemicolons = jsSettings.PreserveAllSemiColons;
                compressor.DisableOptimizations = jsSettings.DisableOptimizations;
                compressor.ErrorReporter = reporter;
                
                compressor.ThreadCulture = System.Globalization.CultureInfo.InvariantCulture;
                returnScript = compressor.Compress(text);

            }
            catch (System.Exception eError)
            {
                returnScript = string.Format("/* error = {0} */", eError.Message);
            }

            return returnScript;
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return Minify(fullFileName, text, projectItem,this.Settings.YuiJsSettings);
        }
    }
}