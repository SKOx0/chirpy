using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Zippy.Chirp.Manager;

namespace Zippy.Chirp.Engines {
    abstract class Engine {
        private static Engine[] _All;

        public static Engine[] All {
            get {
                if (_All == null) Initialize();
                return _All;
            }
        }

        protected static void Initialize() {
            _All = typeof(Engine).Assembly.GetTypes()
                          .Where(x => typeof(Engine).IsAssignableFrom(x) && !x.IsAbstract)
                          .Select(x => (Engine)Activator.CreateInstance(x)).ToArray();
        }

        public static bool IsHandled(string filename) {
            return _All.Any(x => x.IsEngineFor(filename));
        }

        public static void RunTransformations(DTE2 app, ProjectItem projectItem) {
            var item = new Item(app, projectItem);

            TaskList.Instance.Remove(item.FileName);
            using (new EnvironmentDirectory(item.FileName)) {
                try {
                    using (var manager = new VSProjectItemManager(app, projectItem)) {
                        var results = All.Where(x => x.IsEngineFor(item.FileName))
                            .SelectMany(x => x.Transform(item)).OrderBy(x => x.Priority);

                        if (results != null && results.Any()) {
                            foreach (var result in results) {
                                result.Process(app, projectItem, manager);
                            }
                        }
                    }

                } catch (Exception e) {
                    TaskList.Instance.Add(projectItem.ContainingProject, e, item.FileName);
                }
            }
        }

        public abstract bool IsEngineFor(string filename);
        public abstract IEnumerable<IResult> Transform(Item item);

        private static readonly string[] _DefaultExtensions = new[] { ".chirp.js", ".simple.js", ".whitespace.js", ".yui.js", ".gct.js", ".chirp.ascx", ".chirp.aspx", ".chirp.less", ".chirp.less.css", ".chirp.css", ".chirp.config" };
        internal static string GetBaseFileName(string fullFileName, params string[] extensions) {
            extensions = (extensions ?? new string[0]).Union(_DefaultExtensions).ToArray();

            var fileExt = extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.Length).FirstOrDefault()
                ?? System.IO.Path.GetExtension(fullFileName);

            if (!string.IsNullOrEmpty(fileExt)) {
                fullFileName = fullFileName.Substring(0, fullFileName.Length - fileExt.Length);
            }

            return fullFileName;
        }
    }

    abstract class Engine<T> : Engine where T : Engine<T> {
        public Engine() {
            if (_Instance == null) _Instance = (T)this;
        }

        private static T _Instance = null;
        public static T Instance {
            get {
                if (_Instance == null) Initialize();
                return _Instance;
            }
        }
    }

    abstract class BasicEngine<T> : Engine<T> where T : Engine<T> {
        public BasicEngine(string[] extentions, string[] ignoreExtensions) {
            Extensions = extentions;
            IgnoreExtensions = ignoreExtensions;
        }

        public sealed override IEnumerable<IResult> Transform(Item item) {

            IEnumerable<IResult> results = new IResult[0];

            if (!string.IsNullOrEmpty(item.Text)) {
                item.BaseFileName = GetBaseFileName(item.FileName, Extensions);
                results = BasicTransform(item);
            }

            CheckForConfigRefresh(item.App, item.ProjectItem);
            return results;
        }

        private void CheckForConfigRefresh(DTE2 app, ProjectItem projectItem) {
            string fullFileName = projectItem.get_FileNames(1);
            var dependentFiles = ConfigEngine.Instance.dependentFiles;

            if (dependentFiles.ContainsKey(fullFileName)) {
                foreach (string configFile in dependentFiles[fullFileName]) {
                    ProjectItem configItem = app.LocateProjectItemForFileName(configFile);
                    if (projectItem != null) {
                        ConfigEngine.Instance.Transform(new Item(app, configItem));
                    }
                }
            }

            foreach (ProjectItem projectItemInner in projectItem.ProjectItems) {
                CheckForConfigRefresh(app, projectItemInner);
            }
        }

        public abstract IEnumerable<IResult> BasicTransform(Item item);
        public string[] Extensions { get; private set; }
        public string[] IgnoreExtensions { get; private set; }

        public sealed override bool IsEngineFor(string filename) {
            return Extensions.Any(x => filename.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                && (IgnoreExtensions == null || !IgnoreExtensions.Any(x => filename.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
