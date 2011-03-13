
using System;
using EnvDTE;

namespace Zippy.Chirp.Engines
{
    public class ClosureCompilerEngine : JsEngine
    {
        public ClosureCompilerEngine()
        {
            Extensions = new[] { Settings.ChirpSimpleJsFile, Settings.ChirpWhiteSpaceJsFile, Settings.ChirpGctJsFile, Settings.ChirpJsFile };
            OutputExtension = ".min.js";
        }

        public override string Transform(string fullFileName, string text, ProjectItem projectItem)
        {
            var mode = fullFileName.EndsWith(Settings.ChirpGctJsFile, StringComparison.OrdinalIgnoreCase) ? ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS
                  : fullFileName.EndsWith(Settings.ChirpSimpleJsFile, StringComparison.OrdinalIgnoreCase) ? ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS
                  : ClosureCompilerCompressMode.WHITESPACE_ONLY;

            return Minify(fullFileName, text, projectItem, mode);
        }

        public static string Minify(string fullFileName, string text, ProjectItem projectItem, ClosureCompilerCompressMode mode)
        {
            string returnedCode = null;

            returnedCode = GoogleClosureCompiler.Compress(
                fullFileName,
                text,
                mode,
                (category, msg, line, col) =>
                {
                    if (TaskList.Instance == null)
                    {
                        Console.WriteLine(string.Format("{0}({1},{2}){3}", fullFileName, line.ToString(), col.ToString(), msg));
                    }
                    else
                    {
                        TaskList.Instance.Add(projectItem.ContainingProject, category, fullFileName, line, col, msg);
                    }
                });

            return returnedCode;
        }
    }
}
