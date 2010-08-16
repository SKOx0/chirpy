using Microsoft.Ajax.Utilities;

namespace Zippy.Chirp.Engines {

    class MsJsEngine : JsEngine {
        public MsJsEngine() {
            Extensions = new[] { Settings.ChirpMSAjaxJsFile };
            OutputExtension = ".min.js";
        }

        public static string Minify(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            Minifier minifier = new Minifier();
            string mini = minifier.MinifyJavaScript(text);

            foreach (var err in minifier.Errors) {
                int line = 0;
                int column = 0;
                //todo : use regex
                int IndexBegin = err.IndexOf("(");
                int IndexEnd = err.IndexOf(",");
                int.TryParse(err.Substring(IndexBegin + 1, (IndexEnd - IndexBegin) - 1), out line);

                IndexBegin = IndexEnd;
                IndexEnd = err.IndexOf("-");
                int.TryParse(err.Substring(IndexBegin + 1, (IndexEnd - IndexBegin) - 1), out column);

                TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, fullFileName, line, column, err);
            }

            return mini;
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            return Minify(fullFileName, text, projectItem);
        }
    }
}
