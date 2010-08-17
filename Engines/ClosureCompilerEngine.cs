﻿
using System;
using EnvDTE;
namespace Zippy.Chirp.Engines {
    class ClosureCompilerEngine : JsEngine {
        public ClosureCompilerEngine() {
            Extensions = new[] { Settings.ChirpSimpleJsFile, Settings.ChirpWhiteSpaceJsFile, Settings.ChirpGctJsFile };
            OutputExtension = ".min.js";
        }

        public override string Transform(string fullFileName, string text, ProjectItem projectItem) {
            var mode = fullFileName.EndsWith(Settings.ChirpGctJsFile, StringComparison.OrdinalIgnoreCase) ? ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS
                  : fullFileName.EndsWith(Settings.ChirpSimpleJsFile, StringComparison.OrdinalIgnoreCase) ? ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS
                  : ClosureCompilerCompressMode.WHITESPACE_ONLY;

            return Minify(fullFileName, text, projectItem, mode);
        }

        public static string Minify(string fullFileName, string text, ProjectItem projectItem, ClosureCompilerCompressMode mode) {
            string returnedCode = null;

            try {
                returnedCode = GoogleClosureCompiler.Compress(text, mode, (category, msg, line, col) => {
                    TaskList.Instance.Add(projectItem.ContainingProject, category, fullFileName, line, col, msg);
                });
            } catch (System.Exception) { }

            return returnedCode;

        }
    }
}