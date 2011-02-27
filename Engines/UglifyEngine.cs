
namespace Zippy.Chirp.Engines {
    public class JSHintEngine : ActionEngine {
        public override int Handles(string fullFileName) {
            if (Settings.RunJSHint && fullFileName.EndsWith(".js", System.StringComparison.OrdinalIgnoreCase)) return 1;
            return 0;
        }

        public override void Run(string fullFileName, EnvDTE.ProjectItem projectItem) {
            var code = System.IO.File.ReadAllText(fullFileName);
            var results = UglifyCS.JSHint.Hintify(code);

            foreach (var item in results) {
                TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning,
                    fullFileName, item.line, item.character, item.reason);
            }
        }
    }

    public class UglifyEngine : JsEngine {
        public UglifyEngine() {
            Extensions = new[] { Settings.ChirpUglifyJsFile };
            OutputExtension = ".min.js";
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            try {
                return UglifyCS.Uglify.UglifyScript(text);
            } catch (System.Exception) {
                //Uglify.JS doesn't return meaningful error messages, so try minifying with YUI and grab their error messages
                return JsEngine.Minify(fullFileName, text, projectItem, Xml.MinifyType.yui);
            }
        }

        public static string Beautify(string text) {
            return UglifyCS.Beautify.BeautifyScript(text);
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            return Minify(fullFileName, text, projectItem);
        }

    }
}
