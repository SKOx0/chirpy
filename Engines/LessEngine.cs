using System;
using System.Linq;
using System.Text.RegularExpressions;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines
{
    public class LessEngine : TransformEngine
    {
        public LessEngine()
        {
            Extensions = new[] { Settings.ChirpLessFile, Settings.ChirpMichaelAshLessFile, Settings.ChirpHybridLessFile, Settings.ChirpMSAjaxLessFile };
            OutputExtension = ".css";
        }

        static Regex rxLineNum = new Regex(@"line\s+([0-9]+)", RegexOptions.Compiled);
        static Regex rxColNum = new Regex(@"\s+(\-*)\^", RegexOptions.Compiled);

        static dotless.Core.Parser.Parser lazyLessParser;

        static dotless.Core.Parser.Parser lessParser
        {
            get
            {
                if (lazyLessParser == null)
                {
                    lazyLessParser = new dotless.Core.Parser.Parser();
                }

                return lazyLessParser;
            }
        }

        private bool IsChirpLessFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpLessFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpHybridLessFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpHybridLessFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpMichaelAshLessFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpMichaelAshLessFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsChirpMSAjaxLessFile(string fileName)
        {
            return fileName.EndsWith(Settings.ChirpMSAjaxLessFile, StringComparison.OrdinalIgnoreCase);
        }

        public static string TransformToCss(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            string css = null;

            lock (lessParser)
                using (new EnvironmentDirectory(fullFileName))
                    try
                    {
                        // The built in static method doesn't throw error messages
                        css = lessParser.Parse(text, fullFileName).ToCSS();
                    }
                    catch (Exception e)
                    {
                        int line = 1, column = 1;
                        var description = e.Message.Trim();
                        Match match;
                        if ((match = rxLineNum.Match(description)).Success)
                        {
                            line = match.Groups[1].Value.ToInt(1);
                        }

                        if ((match = rxColNum.Match(description)).Success)
                        {
                            column = match.Groups[1].Length + 1;
                        }

                        if (TaskList.Instance == null)
                            Console.WriteLine(string.Format("{0}({1},{2}){3}", fullFileName, line.ToString(), column.ToString(), description));
                        else
                            TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, fullFileName, line, column, description);
                    }

            return css;
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            return TransformToCss(fullFileName, text, projectItem);
        }

        public override int Handles(string fullFileName)
        {
            var match = Extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                          .FirstOrDefault() ?? string.Empty;
            return match.Length;
        }

        public override void Process(Manager.VSProjectItemManager manager, string fullFileName, EnvDTE.ProjectItem projectItem, string baseFileName, string outputText)
        {
            base.Process(manager, fullFileName, projectItem, baseFileName, outputText);

            var mode = this.GetMinifyType(fullFileName);
            string mini = CssEngine.Minify(fullFileName, outputText, projectItem, mode);
            manager.AddFileByFileName(baseFileName + ".min.css", mini);
        }

        public MinifyType GetMinifyType(string fullFileName)
        {
            MinifyType mode = MinifyType.yui;
            if (this.IsChirpMichaelAshLessFile(fullFileName) || this.IsChirpHybridLessFile(fullFileName) || this.IsChirpLessFile(fullFileName))
            {
                mode = this.IsChirpMichaelAshLessFile(fullFileName) ? MinifyType.yuiMARE
               : this.IsChirpHybridLessFile(fullFileName) ? MinifyType.yuiHybrid
               : MinifyType.yui;
            }

            if (this.IsChirpMSAjaxLessFile(fullFileName))
            {
                mode = MinifyType.msAjax;
            }

            return mode;
        }
    }
}
