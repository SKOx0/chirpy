using EnvDTE;

namespace Zippy.Chirp
{
    class EcmaScriptErrorReporter : EcmaScript.NET.ErrorReporter
    {
        TaskList tasks;
        Project project;
        string file;

        public EcmaScriptErrorReporter(TaskList tasks, Project project, string file)
        {
            this.tasks = tasks;
            this.project = project;
            this.file = file;
        }

        public void Error(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            tasks.Add(project, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, file, line, lineOffset, message);
        }

        public EcmaScript.NET.EcmaScriptRuntimeException RuntimeError(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            return new EcmaScript.NET.EcmaScriptRuntimeException(message, sourceName, line, lineSource, lineOffset);
        }

        public void Warning(string message, string sourceName, int line, string lineSource, int lineOffset)
        {
            //tasks.Add(project, Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning, file, line, lineOffset, message);
        }
    }
}
