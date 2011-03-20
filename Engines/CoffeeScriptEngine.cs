using System;
using System.Text.RegularExpressions;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines
{
    public class CoffeeScriptEngine : TransformEngine
    {
        private static UglifyCS.CoffeeScript coffee;
        private static Regex regexError = new Regex(@"Error\:\s*(.*?)\s+on\s+line\s+([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled); // "Error: unclosed { on line 1"

        public CoffeeScriptEngine()
        {
            Extensions = new[] { Settings.ChirpCoffeeScriptFile, Settings.ChirpGctCoffeeScriptFile, Settings.ChirpMSAjaxCoffeeScriptFile, Settings.ChirpSimpleCoffeeScriptFile, Settings.ChirpWhiteSpaceCoffeeScriptFile, Settings.ChirpYUICoffeeScriptFile };
            OutputExtension = ".js";
        }

        public static string TransformToJs(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            if (coffee == null)
            {
                lock (UglifyCS.Extensibility.Instance)
                {
                    if (coffee == null)
                    {
                        coffee = new UglifyCS.CoffeeScript();
                    }
                }
            }

            string error = null;
            try
            {
                return coffee.compile(text);
            }
            catch (Exception e)
            {
                Match match;
                if (TaskList.Instance != null && (match = regexError.Match(e.Message)).Success)
                {
                    TaskList.Instance.Add(
                        projectItem.ContainingProject,
                        Microsoft.VisualStudio.Shell.TaskErrorCategory.Error,
                        fullFileName,
                        match.Groups[2].Value.ToInt(1),
                        0,
                        match.Groups[1].Value);
                }
                else
                {
                    error = e.Message;
                }

                return null;
            }
        }

        public override void Dispose()
        {
            Utilities.Dispose(ref coffee);
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return TransformToJs(fullFileName, text, projectItem);
        }

        public override void Process(Manager.VSProjectItemManager manager, string fullFileName, EnvDTE.ProjectItem projectItem, string baseFileName, string outputText)
        {
            base.Process(manager, fullFileName, projectItem, baseFileName, outputText);

            var mode = this.GetMinifyType(fullFileName);
            string mini = JsEngine.Minify(fullFileName, outputText, projectItem, mode);
            manager.AddFileByFileName(baseFileName + ".min.js", mini);
        }

        public MinifyType GetMinifyType(string fullFileName)
        {
            MinifyType mode = MinifyType.gctAdvanced;

            if (this.IsChirpGctCoffeeScriptFile(fullFileName))
            {
                mode = MinifyType.gctAdvanced;
            }

            if (this.IsChirpMSAjaxCoffeeScriptFile(fullFileName))
            {
                mode = MinifyType.msAjax;
            }

            if (this.IsChirpSimpleCoffeeScriptFile(fullFileName))
            {
                mode = MinifyType.gctSimple;
            }

            if (this.IsChirpWhiteSpaceCoffeeScriptFile(fullFileName))
            {
                mode = MinifyType.gctWhiteSpaceOnly;
            }

            if (this.IsChirpYUICoffeeScriptFile(fullFileName))
            {
                mode = MinifyType.yui;
            }

            if (this.IsChirpUglifyCoffeeScriptFile(fullFileName))
            {
                mode = MinifyType.uglify;
            }

            return mode;
        }

        private bool IsChirpCoffeeScriptFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpCoffeeScriptFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpGctCoffeeScriptFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpGctCoffeeScriptFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpMSAjaxCoffeeScriptFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpMSAjaxCoffeeScriptFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpSimpleCoffeeScriptFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpSimpleCoffeeScriptFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpWhiteSpaceCoffeeScriptFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpWhiteSpaceCoffeeScriptFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpYUICoffeeScriptFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpYUICoffeeScriptFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpUglifyCoffeeScriptFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpUglifyCoffeeScriptFile, StringComparison.OrdinalIgnoreCase);
        }
    }
}

