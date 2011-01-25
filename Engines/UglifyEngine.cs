
namespace Zippy.Chirp.Engines {
    public class UglifyEngine : JsEngine {
        private static UglifyCS.Uglify _uglify;
        private static UglifyCS.Beautify _beautify;

        public UglifyEngine() {
            Extensions = new[] { Settings.ChirpUglifyJsFile };
            OutputExtension = ".min.js";
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            return //(_uglify ?? (_uglify = new UglifyCS.Uglify())).squeeze_it(text);
                UglifyCS.Uglify.UglifyScript(text);
        }

        public static string Beautify(string text) {
            return (_beautify ?? (_beautify = new UglifyCS.Beautify())).js_beautify(text);
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            return Minify(fullFileName, text, projectItem);
        }

        public override void Dispose() {
            try {
                if (_uglify != null) {
                    var temp = _uglify;
                    _uglify = null;
                    temp.Dispose();
                }
            } catch { }

            try {
                if (_beautify != null) {
                    var temp = _beautify;
                    _beautify = null;
                    temp.Dispose();
                }
            } catch { }
        }
    }
}
