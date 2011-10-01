using System;
using Microsoft.Ajax.Utilities;

namespace Zippy.Chirp.Engines
{
    public class MsJsEngine : JsEngine
    {
        public MsJsEngine()
        {
            Extensions = new[] { this.Settings.ChirpMSAjaxJsFile };
            OutputExtension = this.Settings.OutputExtensionJS;
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            Settings settings = Settings.Instance(fullFileName);
            return Minify(fullFileName, text, projectItem, settings.MsJsSettings);
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem,CodeSettings codeSettings)
        {
            Minifier minifier = new Minifier();
            
             string mini = minifier.MinifyJavaScript(text,codeSettings);

            foreach (var err in minifier.Errors)
            {
                int line = 0;
                int column = 0;

                // todo : use regex
                int indexBegin = err.IndexOf("(");
                int indexEnd = err.IndexOf(",");
                int.TryParse(err.Substring(indexBegin + 1, (indexEnd - indexBegin) - 1), out line);

                indexBegin = indexEnd;
                indexEnd = err.IndexOf("-");
                int.TryParse(err.Substring(indexBegin + 1, (indexEnd - indexBegin) - 1), out column);

                if (TaskList.Instance == null)
                {
                    Console.WriteLine(string.Format("{0}({1},{2}){3}", fullFileName, line.ToString(), column.ToString(), err));
                }
                else
                {
                    TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, fullFileName, line, column, err);
                }
            }

            return mini;
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return Minify(fullFileName, text, projectItem, this.Settings.MsJsSettings);
        }
    }
}
