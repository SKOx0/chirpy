using System;
using System.Diagnostics;
using System.Linq;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines
{
    public class TypeScriptEngine : TransformEngine
    {

        public TypeScriptEngine()
        {
            Extensions = new[] { this.Settings.ChirpTypeScriptFile };
            OutputExtension = ".js";
        }

        public override int Handles(string fullFileName)
        {
            this.Settings = Settings.Instance(fullFileName);
            var match = Extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                          .FirstOrDefault() ?? string.Empty;
            return match.Length;
        }

        public static string TransformToJs(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            string error = null;
            string output = string.Empty;

            string fullFileNameJs = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".js";

            try
            {

                string toCall = string.Format("--out \"{0}\" \"{1}\" ", fullFileNameJs, fullFileName);

                using (var process = System.Diagnostics.Process.Start("tsc", toCall))
                {
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = process.StartInfo.RedirectStandardError = true;

                    process.Start();

                    output = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();

                    if (!process.HasExited)
                    {
                        process.Kill();
                    }

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        if (TaskList.Instance != null)
                        {
                            TaskList.Instance.Add(
                                projectItem.ContainingProject,
                                new Exception(error), fullFileName);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        if (TaskList.Instance != null)
                        {
                            TaskList.Instance.Add(
                                projectItem.ContainingProject,
                                new Exception(error), fullFileName);
                        }
                    }
                }

                string jsCode = System.IO.File.ReadAllText(fullFileNameJs);
                System.IO.File.Delete(fullFileNameJs);
                return jsCode;
            }
            catch (Exception e)
            {
                if (TaskList.Instance != null)
                {
                    TaskList.Instance.Add(
                        projectItem.ContainingProject,
                        e, fullFileName);
                }

                return null;
            }

        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return TransformToJs(fullFileName, text, projectItem);
        }

        public override void Process(Manager.VSProjectItemManager manager, string fullFileName, EnvDTE.ProjectItem projectItem, string baseFileName, string outputText)
        {
            base.Process(manager, fullFileName, projectItem, baseFileName, outputText);

            string mini = JsEngine.Minify(fullFileName, outputText, projectItem, MinifyType.gctSimple, string.Empty);
            manager.AddFileByFileName(baseFileName + this.Settings.OutputExtensionJS, mini);
        }

        private bool IsChirpTypeScriptFile(string fileName)
        {
            return fileName.EndsWith(this.Settings.ChirpTypeScriptFile, StringComparison.OrdinalIgnoreCase);
        }
    }
}