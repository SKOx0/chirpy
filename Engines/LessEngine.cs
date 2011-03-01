using System;
using System.Linq;
using System.Text.RegularExpressions;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines {
    public class LessEngine : TransformEngine {
        public LessEngine() {
            Extensions = new[] { Settings.ChirpLessFile, Settings.ChirpMichaelAshLessFile, Settings.ChirpHybridLessFile, Settings.ChirpMSAjaxLessFile };
            OutputExtension = ".css";
        }

        static Regex rxLineNum = new Regex(@"line\s+([0-9]+)", RegexOptions.Compiled);
        static Regex rxColNum = new Regex(@"\s+(\-*)\^", RegexOptions.Compiled);

        private bool IsChirpLessFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpLessFile, StringComparison.OrdinalIgnoreCase));
        }
        private bool IsChirpHybridLessFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpHybridLessFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpMichaelAshLessFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpMichaelAshLessFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpMSAjaxLessFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpMSAjaxLessFile, StringComparison.OrdinalIgnoreCase));
        }

        public static string TransformToCss(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            string css = null;

            using (new EnvironmentDirectory(fullFileName))
                try {
                    css = dotless.Core.Less.Parse(text, new dotless.Core.configuration.DotlessConfiguration { MinifyOutput = false });
                } catch (Exception e) {
                    int line = 1, column = 1;
                    var description = e.ToString().Trim();
                    Match match;
                    if ((match = rxLineNum.Match(description)).Success) {
                        line = match.Groups[1].Value.ToInt(1);
                    }

                    if ((match = rxColNum.Match(description)).Success) {
                        column = match.Groups[1].Length + 1;
                    }
                    if (TaskList.Instance == null)
                        Console.WriteLine(string.Format("{0}({1},{2}){3}", fullFileName, line.ToString(), column.ToString(), description));
                    else
                        TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, fullFileName, line, column, description);
                }

            return css;
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            return TransformToCss(fullFileName, text, projectItem);
        }

        public override int Handles(string fullFileName) {

            // if (fullFileName.EndsWith(GetOutputExtension(fullFileName), StringComparison.InvariantCultureIgnoreCase)) return 0; --remove for handle less.css workitem=31,34
            var match = Extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault() ?? string.Empty;
            return match.Length;
        }

        public override void Process(Manager.VSProjectItemManager manager, string fullFileName, EnvDTE.ProjectItem projectItem, string baseFileName, string outputText) {
            base.Process(manager, fullFileName, projectItem, baseFileName, outputText);

            var mode = GetMinifyType(fullFileName);
            string mini = CssEngine.Minify(fullFileName, outputText, projectItem, mode);
            manager.AddFileByFileName(baseFileName + ".min.css", mini);
        }

        public MinifyType GetMinifyType(string fullFileName) {
            MinifyType mode = MinifyType.yui;
            if (IsChirpMichaelAshLessFile(fullFileName) || IsChirpHybridLessFile(fullFileName) || IsChirpLessFile(fullFileName)) {
                mode = IsChirpMichaelAshLessFile(fullFileName) ? MinifyType.yuiMARE
               : IsChirpHybridLessFile(fullFileName) ? MinifyType.yuiHybrid
               : MinifyType.yui;

            }
            if (IsChirpMSAjaxLessFile(fullFileName)) {
                mode = MinifyType.msAjax;
            }

            return mode;
        }
    }
}
