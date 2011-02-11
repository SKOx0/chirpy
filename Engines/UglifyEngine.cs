
namespace Zippy.Chirp.Engines {
    public class UglifyEngine : JsEngine {
        private static UglifyCS.Uglify _uglify;
        private static UglifyCS.Beautify _beautify;

        public UglifyEngine() {
            Extensions = new[] { Settings.ChirpUglifyJsFile };
            OutputExtension = ".min.js";
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            if (_uglify == null) _uglify = new UglifyCS.Uglify();
            try {
                return _uglify.squeeze_it(text);
            } catch (System.Exception) {
                //Uglify.JS doesn't return meaningful error messages, so try minifying with YUI and grab their error messages
                return JsEngine.Minify(fullFileName, text, projectItem, Xml.MinifyType.yui);
            }
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
