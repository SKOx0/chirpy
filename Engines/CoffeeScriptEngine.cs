using System;
using System.Diagnostics;
using System.Linq;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines
{
    public class CoffeeScriptEngine : TransformEngine
    {
        public CoffeeScriptEngine()
        {
            Extensions = new[] { Settings.ChirpCoffeeScriptFile, Settings.ChirpGctCoffeeScriptFile, Settings.ChirpMSAjaxCoffeeScriptFile, Settings.ChirpSimpleCoffeeScriptFile, Settings.ChirpWhiteSpaceCoffeeScriptFile, Settings.ChirpYUICoffeeScriptFile };
            OutputExtension = ".js";
        }

        public static string TransformToJs(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            string toCall = string.Format("\"{0}\"", fullFileName);
            string error = string.Empty;
            string output = string.Empty;

            try
            {
                var startInfo = new ProcessStartInfo(Settings.CoffeeScriptBatFilePath + @"\coffee.bat", toCall)
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = System.Diagnostics.Process.Start(startInfo);

                output = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            if (!string.IsNullOrEmpty(error))
            {
                if (TaskList.Instance == null)
                    Console.WriteLine(string.Format("Error compiling {0}.", fullFileName));
                else
                    TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, fullFileName, 0, 0, error);
            }

            return output;
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return TransformToJs(fullFileName, text, projectItem);
        }

        public override int Handles(string fullFileName)
        {

            // if (fullFileName.EndsWith(GetOutputExtension(fullFileName), StringComparison.InvariantCultureIgnoreCase)) return 0; --remove for handle less.css workitem=31,34
            var match = Extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault() ?? string.Empty;
            return match.Length;
        }

        private bool IsChirpCoffeeScriptFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpGctCoffeeScriptFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpGctCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpMSAjaxCoffeeScriptFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpMSAjaxCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpSimpleCoffeeScriptFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpSimpleCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpWhiteSpaceCoffeeScriptFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpWhiteSpaceCoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpYUICoffeeScriptFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpYUICoffeeScriptFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsUglifyScriptFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpUglifyJsFile, StringComparison.OrdinalIgnoreCase));
        }

        public override void Process(Manager.VSProjectItemManager manager, string fullFileName, EnvDTE.ProjectItem projectItem, string baseFileName, string outputText)
        {
            base.Process(manager, fullFileName, projectItem, baseFileName, outputText);

            var mode = GetMinifyType(fullFileName);
            string mini = JsEngine.Minify(fullFileName, outputText, projectItem, mode);
            manager.AddFileByFileName(baseFileName + ".min.js", mini);
        }

        public MinifyType GetMinifyType(string fullFileName)
        {
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

