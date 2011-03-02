using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Zippy.Chirp {
    public static class GoogleClosureOfflineCompiler {
        public static string Compress(string fileName, ClosureCompilerCompressMode compressMode, Action<Microsoft.VisualStudio.Shell.TaskErrorCategory, string, int, int> onError) {
            return Compress(fileName, compressMode, onError, null);
        }

        public static string Compress(string fileName, ClosureCompilerCompressMode compressMode, Action<Microsoft.VisualStudio.Shell.TaskErrorCategory, string, int, int> onError,
            IList<string> ReferencePathsOrUrls) {

            if (!File.Exists(fileName)) throw new FileNotFoundException("File does not exist: " + fileName);

            string toCall = string.Format("-jar \"{0}\" {1}",
                                       GetJarPath(),
                                       GetArguments(fileName, compressMode, ReferencePathsOrUrls));


            string error = string.Empty;
            string output = string.Empty;
            try {
                using (var process = Process.Start(Settings.GoogleClosureJavaPath, toCall)) {
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = process.StartInfo.RedirectStandardError = true;

                    process.Start();

                    output = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();

                    if (!process.HasExited) {
                        process.Kill();
                    }
                }
            } catch (Exception e) {
                onError(Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, "Java error : " + e.ToString(), 0, 0);
            }


            if (!string.IsNullOrEmpty(error)) {
                onError(Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, "Google Closure Compiler error : " + error,
                        0, 0);
                return string.Empty;
            }

            return output;
        }

        private static string GetJarPath() {
            //need to find a more elegant way to do this..
            //System.Windows.Forms.MessageBox.Show("Assembly.GetExecutingAssembly().Location" + );

            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                          "compiler.jar");

        }

        private static string GetArguments(string FileName, ClosureCompilerCompressMode compressMode, IList<string> ReferencePathsOrUrls) {
            return
                string.Format("--js \"{0}\" --compilation_level {1} --warning_level QUIET",
                              FileName,
                              compressMode);

        }

    }
}
