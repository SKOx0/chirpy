using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EnvDTE;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines {
    class ConfigEngine : ActionEngine {
        const string regularCssFile = ".css";
        const string regularJsFile = ".js";
        const string regularLessFile = ".less";

        bool IsLessFile(string fileName) {
            return (fileName.EndsWith(regularLessFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsCssFile(string fileName) {
            return (fileName.EndsWith(regularCssFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsJsFile(string fileName) {
            return (fileName.EndsWith(regularJsFile, StringComparison.OrdinalIgnoreCase));
        }

        internal Dictionary<string, List<string>> dependentFiles =
            new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// build a dictionary that has the files that could change as the key.
        /// for the value it is a LIST of config files that need updated if it does change.
        /// so, when a .less.css file changes, we look in the list and rebuild any of the configs associated with it.
        /// if a config file changes...this rebuild all of this....
        /// </summary>
        /// <param name="projectItem"></param>
        internal void ReloadConfigFileDependencies(ProjectItem projectItem) {
            string configFileName = projectItem.get_FileNames(1);

            //remove all current dependencies for this config file...
            foreach (string key in dependentFiles.Keys.ToArray()) {
                List<string> files = dependentFiles[key];
                if (files.Remove(configFileName) && files.Count == 0)
                    dependentFiles.Remove(key);
            }

            var fileGroups = LoadConfigFileGroups(configFileName);
            foreach (var fileGroup in fileGroups) {
                foreach (var file in fileGroup.Files) {
                    if (!dependentFiles.ContainsKey(file.Path)) {
                        dependentFiles.Add(file.Path, new List<string> { configFileName });
                    } else {
                        dependentFiles[file.Path].Add(configFileName);
                    }
                }
            }
        }

        IList<FileGroupXml> LoadConfigFileGroups(string configFileName) {
            XDocument doc = XDocument.Load(configFileName);

            string appRoot = string.Format("{0}\\", Path.GetDirectoryName(configFileName));

            IList<FileGroupXml> ReturnList = doc.Descendants("FileGroup")
                .Select(n => new FileGroupXml(n, appRoot))
                .ToList();

            if (ReturnList.Count == 0)
                ReturnList = doc.Descendants(XName.Get("FileGroup", "urn:ChirpyConfig"))
                     .Select(n => new FileGroupXml(n, appRoot))
                .ToList();

            return ReturnList;
        }

        public override int Handles(string fullFileName) {
            return fullFileName.EndsWith(Settings.ChirpConfigFile, StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
        }

        public override void Run(string fullFileName, ProjectItem projectItem) {
            var fileGroups = LoadConfigFileGroups(fullFileName);
            string directory = Path.GetDirectoryName(fullFileName);

            using (var manager = new Manager.VSProjectItemManager(_app, projectItem)) {
                foreach (var fileGroup in fileGroups) {
                    var allFileText = new StringBuilder();
                    bool isjs = false;

                    foreach (var file in fileGroup.Files) {
                        var path = file.Path;
                        string code = System.IO.File.ReadAllText(path);
                        isjs = IsJsFile(path);
                        TaskList.Instance.Remove(path);

                        using (new EnvironmentDirectory(path)) {
                            if (IsLessFile(path)) {
                                code = LessEngine.TransformToCss(path, code, projectItem);
                            }

                            if (file.Minify == true) {
                                if (IsCssFile(path)) {
                                    code = CssEngine.Minify(path, code, projectItem, file.MinifyWith);

                                } else if (IsJsFile(path)) {
                                    code = JsEngine.Minify(path, code, projectItem, file.MinifyWith);
                                    isjs = true;
                                }
                            }
                        }

                        allFileText.AppendLine(code);
                    }

                    string fullPath = directory + @"\" + fileGroup.Name;
                    if (!string.IsNullOrEmpty(fileGroup.Path))
                        fullPath = fileGroup.Path;

                    string output = allFileText.ToString();
                    string mini = null;
                    if (fileGroup.Minify == true || fileGroup.Minify == null) {

                        mini = isjs ? JsEngine.Minify(fullPath, output, projectItem, fileGroup.MinifyWith)
                            : CssEngine.Minify(fullPath, output, projectItem, fileGroup.MinifyWith);

                        if (fileGroup.Minify == true)
                            output = mini;
                    }

                    manager.AddFileByFileName(fullPath, output);
                    if (fileGroup.Minify == null) {
                        manager.AddFileByFileName(Utilities.GetBaseFileName(fullPath) + ".min." + (isjs ? "js" : "css"), mini);
                    }
                }

                ReloadConfigFileDependencies(projectItem);
            }
        }

        public void RefreshAll() {
            var configs = dependentFiles.SelectMany(x => x.Value).Distinct(StringComparer.InvariantCultureIgnoreCase);
            foreach (var configFile in configs) {
                Refresh(configFile);
            }
        }

        public void Refresh(string configFile) {
            ProjectItem configItem = _app.LocateProjectItemForFileName(configFile);
            if (configItem != null) {
                Chirp.ConfigEngine.Run(configFile, configItem);
            }
        }

        public void CheckForConfigRefresh(ProjectItem projectItem) {
            string fullFileName = projectItem.get_FileNames(1);
            var dependentFiles = Chirp.ConfigEngine.dependentFiles;

            if (dependentFiles.ContainsKey(fullFileName)) {
                foreach (string configFile in dependentFiles[fullFileName].ToArray()) { //ToArray to prevent "Collection Modified" exceptions 
                    Refresh(configFile);
                }
            }

            foreach (ProjectItem projectItemInner in projectItem.ProjectItems.Cast<ProjectItem>().ToArray()) { //ToArray to prevent "Collection Modified" exceptions 
                CheckForConfigRefresh(projectItemInner);
            }
        }
    }
}
