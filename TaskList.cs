using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

// http://www.mztools.com/articles/2008/MZ2008022.aspx

namespace Zippy.Chirp {
    public class TaskList : IDisposable {
        static TaskList instance;
        ErrorListProvider listProvider;
        ServiceProvider serviceProvider;
        List<ErrorTask> tasks = new List<ErrorTask>();

        public TaskList(object application) {
            instance = this;

            serviceProvider = new ServiceProvider(application as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);

            listProvider = new ErrorListProvider(serviceProvider);
            listProvider.ProviderName = this.GetType().Assembly.FullName;
            listProvider.ProviderGuid = new Guid("F1415C4C-5D67-401F-A81C-71F0721BB6F0");
            listProvider.Show();
        }

        public TaskList() {
            instance = this;
        }

        public static TaskList Instance {
            get { return instance; }
        }

        public IEnumerable<ErrorTask> Errors {
            get { return tasks; }
        }

        Dictionary<ErrorTask, Project> taskProjects = new Dictionary<ErrorTask, Project>();

        public void Add(Project project, TaskErrorCategory category, string file, int line, int column, string description) {
            var task = new ErrorTask {
                ErrorCategory = category,
                Document = file,
                Line = Math.Max(line - 1, 0),
                Column = Math.Max(column - 1, 0),
                Text = description,
            };
            Add(project, task);
        }

        public void Add(Project project, Exception ex, string filename) {
            Add(project, TaskErrorCategory.Error, filename, 0, 0, ex.ToString());
        }

        private void Add(Project project, ErrorTask task) {
            IVsHierarchy hierarchy = null;
            if (project != null && serviceProvider != null) {
                var solution = serviceProvider.GetService(typeof(IVsSolution)) as IVsSolution;
                if (solution != null) {
                    solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);
                }
            }

            task.HierarchyItem = hierarchy;
            task.Navigate += new EventHandler(task_Navigate);
            if (listProvider != null) listProvider.Tasks.Add(task);
            tasks.Add(task);

            if (project != null) {
                lock (taskProjects) {
                    taskProjects.Add(task, project);
                }
            }
        }

        void task_Navigate(object sender, EventArgs e) {
            var task = sender as ErrorTask;

            task.Line++;
            var result = listProvider.Navigate(task, new Guid(EnvDTE.Constants.vsViewKindCode));
            task.Line--;
        }

        public void Remove(string file) {
            // var tasks = listProvider.Tasks.Cast<ErrorTask>().Where(x => file.Is(x.Document)).ToArray();
            foreach (var task in tasks.Where(x => x.Document.Is(file)).ToArray()) {
                Remove(task);
            }
        }

        public void Remove(Project project) {
            lock (taskProjects) {
                var tasks = taskProjects.Where(x => x.Value == project).Select(x => x.Key).ToArray();
                foreach (var task in tasks) {
                    Remove(task);
                }
            }
        }

        public void RemoveAll() {
            if (listProvider != null) listProvider.Tasks.Clear();
            tasks.Clear();
            taskProjects.Clear();
        }

        private void Remove(ErrorTask task) {
            if (listProvider != null) listProvider.Tasks.Remove(task);
            tasks.Remove(task);
            lock (taskProjects) {
                if (taskProjects.ContainsKey(task)) {
                    taskProjects.Remove(task);
                }
            }
        }

        public void Dispose() {
            if (listProvider != null) listProvider.Dispose();
            if (serviceProvider != null) serviceProvider.Dispose();
        }
    }
}
