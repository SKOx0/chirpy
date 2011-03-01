﻿
namespace Zippy.Chirp.Engines {
    public class JSHintEngine : ActionEngine {
        private static UglifyCS.JSHint _hint;

        public override int Handles(string fullFileName) {
            if (Settings.RunJSHint && fullFileName.EndsWith(".js", System.StringComparison.OrdinalIgnoreCase)) return 1;
            return 0;
        }

        public override void Run(string fullFileName, EnvDTE.ProjectItem projectItem) {
            if (_hint == null) lock (UglifyCS.Extensibility.Instance) if (_hint == null) _hint = new UglifyCS.JSHint();
            var code = System.IO.File.ReadAllText(fullFileName);
            var results = _hint.JSHINT(code);

            if (results != null)
                foreach (var item in results) {
                    TaskList.Instance.Add(projectItem.ContainingProject,
                        Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning,
                        fullFileName, item.line, item.character, item.reason);
                }
        }

        public override void Dispose() {
            Utilities.Dispose(ref _hint);
        }
    }

    public class UglifyEngine : JsEngine {
        private static UglifyCS.Uglify _uglify = new UglifyCS.Uglify();
        private static UglifyCS.Beautify _beautify = new UglifyCS.Beautify();

        public UglifyEngine() {
            Extensions = new[] { Settings.ChirpUglifyJsFile };
            OutputExtension = ".min.js";
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            if (_uglify == null) lock (UglifyCS.Extensibility.Instance) if (_uglify == null) _uglify = new UglifyCS.Uglify();
            try {
                return _uglify.squeeze_it(text);
            } catch (System.Exception) {
                //Uglify.JS doesn't return meaningful error messages, so try minifying with YUI and grab their error messages
                return JsEngine.Minify(fullFileName, text, projectItem, Xml.MinifyType.yui);
            }
        }

        public static string Beautify(string text) {
            if (_beautify == null) lock (UglifyCS.Extensibility.Instance) if (_beautify == null) _beautify = new UglifyCS.Beautify();
            return _beautify.js_beautify(text);
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            return Minify(fullFileName, text, projectItem);
        }

        public override void Dispose() {
            Utilities.Dispose(ref _uglify);
            Utilities.Dispose(ref _beautify);
        }
    }
}
