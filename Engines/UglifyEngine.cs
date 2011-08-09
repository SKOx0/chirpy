using System;

namespace Zippy.Chirp.Engines
{
    public class CSSLintEngine : ActionEngine 
    {
        private static UglifyCS.CSSLint lint;

        public override int Handles(string fullFileName) 
        {
            if (Settings.RunCSSLint && fullFileName.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
                && !fullFileName.EndsWith(Settings.OutputExtensionCSS, StringComparison.OrdinalIgnoreCase)) 
            {
                return 1;
            }

            return 0;
        }

        public override void Run(string fullFileName, EnvDTE.ProjectItem projectItem) 
        {
            if (lint == null) 
            {
                lock (UglifyCS.Extensibility.Instance) 
                {
                    if (lint == null) 
                    {
                        lint = new UglifyCS.CSSLint();
                    }
                }
            }

            var code = System.IO.File.ReadAllText(fullFileName);
            var results = lint.CSSLINT(code);

            if (results != null && results.messages != null && results.messages.Length > 0) 
            {
                foreach (var item in results.messages) 
                {
                    TaskList.Instance.Add(projectItem.ContainingProject,
                        item.type == UglifyCS.CSSLint.Message.types.error ? Microsoft.VisualStudio.Shell.TaskErrorCategory.Error
                            : item.type == UglifyCS.CSSLint.Message.types.warning ? Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning
                            : Microsoft.VisualStudio.Shell.TaskErrorCategory.Message,
                        fullFileName, item.line, item.col, item.message);
                }
            }
        }

        public override void Dispose() 
        {
            Utilities.Dispose(ref lint);
        }
    }

    public class JSHintEngine : ActionEngine
    {
        private static UglifyCS.JSHint hint;

        public override int Handles(string fullFileName)
        {
            if (Settings.RunJSHint && fullFileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
                && !fullFileName.EndsWith(Settings.OutputExtensionJS, StringComparison.OrdinalIgnoreCase))
            { 
                return 1; 
            }

            return 0;
        }

        public override void Run(string fullFileName, EnvDTE.ProjectItem projectItem)
        {
            if (JSHintEngine.hint == null)
            {
                lock (UglifyCS.Extensibility.Instance)
                {
                    if (JSHintEngine.hint == null)
                    {
                        JSHintEngine.hint = new UglifyCS.JSHint();
                    }
                }
            }

            var code = System.IO.File.ReadAllText(fullFileName);
            var results = JSHintEngine.hint.JSHINT(code);

            if (results != null)
            {
                foreach (var item in results)
                {
                    TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning, fullFileName, item.line, item.character, item.reason);
                }
            }
        }

        public override void Dispose()
        {
            Utilities.Dispose(ref JSHintEngine.hint);
        }
    }

    public class UglifyEngine : JsEngine
    {
        private static UglifyCS.Uglify uglify = new UglifyCS.Uglify();
        private static UglifyCS.Beautify beautify = new UglifyCS.Beautify();

        public UglifyEngine()
        {
            Extensions = new[] { Settings.ChirpUglifyJsFile };
            OutputExtension = Settings.OutputExtensionJS;
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            if (UglifyEngine.uglify == null)
            {
                lock (UglifyCS.Extensibility.Instance)
                {
                    if (UglifyEngine.uglify == null)
                    {
                        UglifyEngine.uglify = new UglifyCS.Uglify();
                    }
                }
            }

            try
            {
                return UglifyEngine.uglify.squeeze_it(text);
            }
            catch (System.Exception)
            {
                // Uglify.JS doesn't return meaningful error messages, so try minifying with YUI and grab their error messages
                return JsEngine.Minify(fullFileName, text, projectItem, Xml.MinifyType.yui);
            }
        }

        public static string Beautify(string text)
        {
            if (UglifyEngine.beautify == null)
            {
                lock (UglifyCS.Extensibility.Instance)
                {
                    if (UglifyEngine.beautify == null)
                    {
                        UglifyEngine.beautify = new UglifyCS.Beautify();
                    }
                }
            }

            return UglifyEngine.beautify.js_beautify(text);
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return Minify(fullFileName, text, projectItem);
        }

        public override void Dispose()
        {
            Utilities.Dispose(ref UglifyEngine.uglify);
            Utilities.Dispose(ref UglifyEngine.beautify);
        }
    }
}
