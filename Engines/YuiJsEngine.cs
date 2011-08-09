using Yahoo.Yui.Compressor;

namespace Zippy.Chirp.Engines
{
    public class YuiJsEngine : JsEngine
    {
        #region "constructor"
        public YuiJsEngine()
        {
            Extensions = new[] { Settings.ChirpYUIJsFile };
            OutputExtension = Settings.OutputExtensionJS;
        }
        #endregion

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            var reporter = new EcmaScriptErrorReporter(fullFileName, projectItem);
            try
            {
                // http://chirpy.codeplex.com/workitem/54
                // http://chirpy.codeplex.com/workitem/60
                var compressor = new JavaScriptCompressor(text, true, System.Text.Encoding.UTF8, System.Globalization.CultureInfo.InvariantCulture, false, reporter);
                return compressor.Compress();
            }
            catch (System.Exception)
            {
                return "/* error */";
            }
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return Minify(fullFileName, text, projectItem);
        }
    }
}