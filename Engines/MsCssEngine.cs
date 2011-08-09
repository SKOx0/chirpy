using System;
using Microsoft.Ajax.Utilities;

namespace Zippy.Chirp.Engines
{
    public class MsCssEngine : CssEngine
    {
        public MsCssEngine()
        {
            Extensions = new[] { Settings.ChirpMSAjaxCssFile };
            OutputExtension = Settings.OutputExtensionCSS;
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            Minifier minifier = new Minifier();
            string miniCss = minifier.MinifyStyleSheet(text);

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

            return miniCss;
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return Minify(fullFileName, text, projectItem);
        }
    }
}
