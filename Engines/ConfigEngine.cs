using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EnvDTE;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines {
    public class ConfigEngine : ActionEngine {
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

            IList<FileGroupXml> ReturnList = (
                    doc.Descendants("FileGroup")
                    .Concat(doc.Descendants(XName.Get("FileGroup", "urn:ChirpyConfig")))
                )
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

            using (var manager = new Manager.VSProjectItemManager(_Chirp != null ? _Chirp.app : null, projectItem)) {
                foreach (var fileGroup in fileGroups) {
                    var allFileText = new StringBuilder();
                    bool isJS = IsJsFile(fileGroup.GetName());

                    string fullPath = directory + @"\" + fileGroup.Name;
                    if (!string.IsNullOrEmpty(fileGroup.Path))
                        fullPath = fileGroup.Path;

                    if (ImageSprite.IsImage(fileGroup.GetName())) {
                        var img = ImageSprite.Build(fileGroup, fullPath);
                        manager.AddFileByFileName(fullPath, img);
                        continue;
                    }

                    bool minifySeperatly = fileGroup.Files.Any(f => {
                        //var minify = f.Minify ?? fileGroup.Minify ;
                        //return minify != fileGroup.Minify || f.MinifyWith != fileGroup.MinifyWith;
                        return f.Minify != (fileGroup.Minify != FileGroupXml.MinifyOptions.False ? true : false)
                            || f.MinifyWith != fileGroup.MinifyWith;
                    }) || fileGroup.Debug;

                    foreach (var file in fileGroup.Files) {
                        var path = file.Path;
                        string code = System.IO.File.ReadAllText(path);

                        if (IsLessFile(path)) {
                            code = LessEngine.TransformToCss(path, code, projectItem);
                        }
                        if (minifySeperatly && file.Minify == true) {
                            if (TaskList.Instance != null)
                                TaskList.Instance.Remove(path);
                            if (IsCssFile(path)) {
                                code = CssEngine.Minify(path, code, projectItem, file.MinifyWith);
                            } else if (IsJsFile(path)) {
                                code = JsEngine.Minify(path, code, projectItem, file.MinifyWith);
                            }
                        }
                        if (fileGroup.Debug) {
                            code = "\r\n/* Chirpy Minify: {Minify}, MinifyWith: {MinifyWith}, File: {FilePath} */\r\n{Code}"
                                .F(new {
                                    Minify = file.Minify.GetValueOrDefault(),
                                    FilePath = path,
                                    Code = code,
                                    MinifyWith = file.MinifyWith.ToString()
                                });
                        }
                        allFileText.AppendLine(code);
                    }

                    string output = allFileText.ToString();
                    string mini = null;

                    if (fileGroup.Minify == FileGroupXml.MinifyOptions.Both) {
                        manager.AddFileByFileName(Utilities.GetBaseFileName(fullPath) + (isJS ? ".js" : ".css"), isJS ? UglifyEngine.Beautify(output) : output);
                    }

                    if (!minifySeperatly && fileGroup.Minify != FileGroupXml.MinifyOptions.False) {
                        if (TaskList.Instance != null) TaskList.Instance.Remove(fullPath);

                        mini = isJS ? JsEngine.Minify(fullPath, output, projectItem, fileGroup.MinifyWith)
                            : CssEngine.Minify(fullPath, output, projectItem, fileGroup.MinifyWith);

                        output = mini;
                    }

                    if (fileGroup.Minify == FileGroupXml.MinifyOptions.Both) {
                        manager.AddFileByFileName(Utilities.GetBaseFileName(fullPath) + (isJS ? ".min.js" : ".min.css"), output);
                    } else {
                        manager.AddFileByFileName(fullPath, output);
                    }
                }

                if (projectItem != null) ReloadConfigFileDependencies(projectItem);
            }
        }
        public void RefreshAll() {
            var configs = dependentFiles.SelectMany(x => x.Value).Distinct(StringComparer.InvariantCultureIgnoreCase);
            foreach (var configFile in configs) {
                Refresh(configFile);
            }
        }

        public void Refresh(string configFile) {
            ProjectItem configItem = _Chirp.app.Solution.FindProjectItem(configFile);

            if (configItem != null) {
                _Chirp.EngineManager.Enqueue(configItem);
            }
        }

        public void CheckForConfigRefresh(ProjectItem projectItem) {

            string fullFileName = projectItem.get_FileNames(1);

            if (dependentFiles.ContainsKey(fullFileName)) {
                foreach (string configFile in dependentFiles[fullFileName]
                    .Distinct(StringComparer.InvariantCultureIgnoreCase) //prevent the same config file from being fired multiple times
                    .ToArray()) { //ToArray to prevent "Collection Modified" exceptions 
                    Refresh(configFile);
                }
            }

            if (projectItem.ProjectItems != null) {
                foreach (ProjectItem projectItemInner in projectItem.ProjectItems.Cast<ProjectItem>().ToArray()) { //ToArray to prevent "Collection Modified" exceptions 
                    CheckForConfigRefresh(projectItemInner);
                }
            }
        }
    }
}
