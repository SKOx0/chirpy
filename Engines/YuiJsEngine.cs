using Yahoo.Yui.Compressor;

namespace Zippy.Chirp.Engines {

    public class YuiJsEngine : JsEngine {
        public YuiJsEngine() {
            Extensions = new[] { Settings.ChirpYUIJsFile };
            OutputExtension = ".min.js";
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            return Minify(fullFileName, text, projectItem);
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            var reporter = new EcmaScriptErrorReporter(fullFileName, projectItem);
            try {
                //http://chirpy.codeplex.com/workitem/54
                var compressor = new JavaScriptCompressor(text, true, System.Text.Encoding.Default, System.Globalization.CultureInfo.InvariantCulture, false, reporter);
                return compressor.Compress();
            } catch (System.Exception) {
                return "/* error */";
            }
        }
    }
}
