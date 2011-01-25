using System;
using EnvDTE;

namespace Zippy.Chirp {
    class EcmaScriptErrorReporter : EcmaScript.NET.ErrorReporter {
        private ProjectItem _ProjectItem;
        private string _fullFileName;
        public EcmaScriptErrorReporter(string fullFileName, ProjectItem projectItem) {
            _fullFileName = fullFileName;
            _ProjectItem = projectItem;
        }

        public void Error(string message, string sourceName, int line, string lineSource, int lineOffset) {
            if (TaskList.Instance == null)
                Console.WriteLine(string.Format("{0}({1},{2}){3}", _fullFileName, line.ToString(), lineOffset.ToString(), message));
            else
                TaskList.Instance.Add(_ProjectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error,
                _fullFileName, line, lineOffset, message);
        }

        public void Warning(string message, string sourceName, int line, string lineSource, int lineOffset) {
            // _Result.Warnings.Add(new Result.Error { Description = message, Line = line, Column = lineOffset });
        }

        public EcmaScript.NET.EcmaScriptRuntimeException RuntimeError(string message, string sourceName, int line, string lineSource, int lineOffset) {
            return new EcmaScript.NET.EcmaScriptRuntimeException(message, sourceName, line, lineSource, lineOffset);
        }

    }
}
