using EnvDTE;
using EnvDTE80;

namespace Zippy.Chirp.Engines {
    class Item {
        public Item(DTE2 app, ProjectItem projectItem)
            : this(projectItem.get_FileNames(1)) {
            ProjectItem = projectItem;
            App = app;
        }

        public Item(string fullFileName) {
            FileName = fullFileName;
            Text = System.IO.File.ReadAllText(FileName);
        }

        public Item(Item item, string code) {
            Text = code;
            FileName = item.FileName;
            BaseFileName = item.BaseFileName;
            ProjectItem = item.ProjectItem;
            App = item.App;
        }

        public string Text { get; set; }
        public string FileName { get; set; }
        public string BaseFileName { get; set; }
        public DTE2 App { get; set; }

        private ProjectItem projectItem;
        public ProjectItem ProjectItem {
            get {
                if (projectItem == null && App != null)
                    projectItem = App.LocateProjectItemForFileName(FileName);
                return projectItem;
            }
            set { projectItem = value; }
        }
    }
}
