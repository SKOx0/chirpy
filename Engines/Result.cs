
using EnvDTE;
using EnvDTE80;
using Zippy.Chirp.Manager;
namespace Zippy.Chirp.Engines
{
    interface IResult
    {
        void Process(DTE2 app, ProjectItem item, VSProjectItemManager manager);
        int Priority { get; }
    }

    class ErrorResult : IResult
    {
        public int Priority { get { return int.MaxValue; } }
        public string Description { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public ErrorResult(string description, int line, int column)
        {
            Line = line;
            Column = column;
            Description = description;
        }

        public void Process(DTE2 app, ProjectItem item, VSProjectItemManager manager)
        {
            app.ToolWindows.ErrorList.Parent.Activate();
            TaskList.Instance.Add(item.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error,
                              item.get_FileNames(1), Line, Column, Description);
        }
    }

    class FileResult : IResult
    {
        public int Priority { get { return 0; } }
        public FileResult(Item item, string ext, string code, bool minified) : this(item.BaseFileName + ext, code, minified) { }
        public FileResult(string file, string code, bool minified)
        {
            Text = code;
            FileName = file;
            Minified = minified;
        }
        public string Text { get; set; }
        public string FileName { get; set; }
        public bool Minified { get; set; }

        public void Process(DTE2 app, ProjectItem item, VSProjectItemManager manager)
        {
            manager.AddFileByFileName(FileName, Text);
        }
    }
}
