using System;
using System.Linq;
using System.Text.RegularExpressions;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines {
    public class CoffeeScriptEngine : TransformEngine {
        private static UglifyCS.CoffeeScript _coffee;
        private static Regex rxError = new Regex(@"Error\:\s*(.*?)\s+on\s+line\s+([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled); //"Error: unclosed { on line 1"

        public CoffeeScriptEngine() {
            Extensions = new[] { Settings.ChirpCoffeeScriptFile, Settings.ChirpGctCoffeeScriptFile, Settings.ChirpMSAjaxCoffeeScriptFile, Settings.ChirpSimpleCoffeeScriptFile, Settings.ChirpWhiteSpaceCoffeeScriptFile, Settings.ChirpYUICoffeeScriptFile };
            OutputExtension = ".js";
        }


        public static string TransformToJs(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            if (_coffee == null) lock (UglifyCS.Extensibility.Instance) if (_coffee == null) _coffee = new UglifyCS.CoffeeScript();

            string error = null;
            try {
                return _coffee.compile(text);
            } catch (Exception e) {
                Match match;
                if (TaskList.Instance != null && (match = rxError.Match(e.Message)).Success) {
                    TaskList.Instance.Add(projectItem.ContainingProject,
                        Microsoft.VisualStudio.Shell.TaskErrorCategory.Error,
                        fullFileName, match.Groups[2].Value.ToInt(1), 0, match.Groups[1].Value);
                } else error = e.Message;
                return null;
            }
        }

        public override void Dispose() {
            Utilities.Dispose(ref _coffee);
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            return TransformToJs(fullFileName, text, projectItem);
        }

        public override int Handles(string fullFileName) {

            // if (fullFileName.EndsWith(GetOutputExtension(fullFileName), StringComparison.InvariantCultureIgnoreCase)) return 0; --remove for handle less.css workitem=31,34
            var match = Extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault() ?? string.Empty;
            return match.Length;
        }

        private bool IsChirpCoffeeScriptFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpGctCoffeeScriptFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpGctCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpMSAjaxCoffeeScriptFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpMSAjaxCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpSimpleCoffeeScriptFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpSimpleCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpWhiteSpaceCoffeeScriptFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpWhiteSpaceCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpYUICoffeeScriptFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpYUICoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsUglifyScriptFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpUglifyJsFile, StringComparison.OrdinalIgnoreCase));
        }

        public override void Process(Manager.VSProjectItemManager manager, string fullFileName, EnvDTE.ProjectItem projectItem, string baseFileName, string outputText) {
            base.Process(manager, fullFileName, projectItem, baseFileName, outputText);

            var mode = GetMinifyType(fullFileName);
            string mini = JsEngine.Minify(fullFileName, outputText, projectItem, mode);
            manager.AddFileByFileName(baseFileName + ".min.js", mini);
        }

        public MinifyType GetMinifyType(string fullFileName) {
            MinifyType mode = MinifyType.gctAdvanced;

            if (IsChirpGctCoffeeScriptFile(fullFileName))
                mode = MinifyType.gctAdvanced;
            if (IsChirpMSAjaxCoffeeScriptFile(fullFileName))
                mode = MinifyType.msAjax;
            if (IsChirpSimpleCoffeeScriptFile(fullFileName))
                mode = MinifyType.gctSimple;
            if (IsChirpWhiteSpaceCoffeeScriptFile(fullFileName))
                mode = MinifyType.gctWhiteSpaceOnly;
            if (IsChirpYUICoffeeScriptFile(fullFileName))
                mode = MinifyType.yui;
            if (IsUglifyScriptFile(fullFileName))
                mode = MinifyType.uglify;
            return mode;
        }
    }
}

