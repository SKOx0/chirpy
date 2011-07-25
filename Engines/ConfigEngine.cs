﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EnvDTE;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines
{
    public class ConfigEngine : ActionEngine
    {
        private const string RegularCssFile = ".css";
        private const string RegularJsFile = ".js";
        private const string RegularCoffeeScriptFile = ".coffee";
        private const string RegularLessFile = ".less";
        private Dictionary<string, List<string>> dependentFiles =
       new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

        public void RefreshAll()
        {
            var configs = this.dependentFiles.SelectMany(x => x.Value).Distinct(StringComparer.InvariantCultureIgnoreCase);
            foreach (var configFile in configs)
            {
                this.Refresh(configFile);
            }
        }

        public void Refresh(string configFile)
        {
            ProjectItem configItem = this.Chirp.App.Solution.FindProjectItem(configFile);

            if (configItem != null)
            {
                this.Chirp.EngineManager.Enqueue(configItem);
            }
        }

        public void CheckForConfigRefresh(ProjectItem projectItem)
        {
            string fullFileName = projectItem.get_FileNames(1);

            if (this.dependentFiles.ContainsKey(fullFileName))
            {
                foreach (string configFile in this.dependentFiles[fullFileName]
                    .Distinct(StringComparer.InvariantCultureIgnoreCase) // prevent the same config file from being fired multiple times
                    .ToArray())
                {
                    // ToArray to prevent "Collection Modified" exceptions 
                    this.Refresh(configFile);
                }
            }

            if (projectItem.ProjectItems != null)
            {
                foreach (ProjectItem projectItemInner in projectItem.ProjectItems.Cast<ProjectItem>().ToArray())
                {
                    // ToArray to prevent "Collection Modified" exceptions 
                    this.CheckForConfigRefresh(projectItemInner);
                }
            }
        }

        public override int Handles(string fullFileName)
        {
            return fullFileName.EndsWith(Settings.ChirpConfigFile, StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
        }

        public override void Run(string fullFileName, ProjectItem projectItem)
        {
            var fileGroups = this.LoadConfigFileGroups(fullFileName);
            string directory = Path.GetDirectoryName(fullFileName);
            var productionFileText = new StringBuilder();

            using (var manager = new Manager.VSProjectItemManager(this.Chirp != null ? this.Chirp.App : null, projectItem))
            {
                foreach (var fileGroup in fileGroups)
                {
                    productionFileText.Clear();

                    bool isJS = this.IsJsFile(fileGroup.GetName);

                    string fullPath = directory + @"\" + fileGroup.Name;
                    if (!string.IsNullOrEmpty(fileGroup.Path))
                    {
                        fullPath = fileGroup.Path;
                    }
                    string fullPathMin = Utilities.GetBaseFileName(fullPath) + (isJS ? ".min.js" : ".min.css");
                    if (TaskList.Instance != null) 
                    {
                        TaskList.Instance.Remove(fullPath);
                        TaskList.Instance.Remove(fullPathMin);
                    }

                    if (ImageSprite.IsImage(fileGroup.GetName))
                    {
                        var img = ImageSprite.Build(fileGroup, fullPath);
                        manager.AddFileByFileName(fullPath, img);
                        continue;
                    }

                    foreach (var file in fileGroup.Files)
                    {
                        var path = file.Path;
                        string code = System.IO.File.ReadAllText(path);
                        string customArg = file.CustomArgument;
                        
                        if ((this.IsLessFile(path) || this.IsCoffeeScriptFile(path) || file.Minify == true) && TaskList.Instance != null) 
                        {
                            TaskList.Instance.Remove(path);
                        }

                        if (fileGroup.Debug)
                        {
                            var debugCode = "\r\n/* Chirpy Minify: {Minify}, MinifyWith: {MinifyWith}, File: {FilePath} */\r\n{Code}"
                                .F(new
                                {
                                    Minify = file.Minify.GetValueOrDefault(),
                                    FilePath = path,
                                    Code = isJS ? UglifyEngine.Beautify(code) : code,
                                    MinifyWith = file.MinifyWith.ToString()
                                });
                            productionFileText.AppendLine(debugCode);
                        }

                        if (this.IsLessFile(path))
                        {
                            code = LessEngine.TransformToCss(path, code, projectItem);
                        } 
                        else if (this.IsCoffeeScriptFile(path))
                        {
                            code = CoffeeScriptEngine.TransformToJs(path, code, projectItem);
                        }

                        if (file.Minify == true) {
                            if (this.IsCssFile(path)) {
                                code = CssEngine.Minify(path, code, projectItem, file.MinifyWith);
                            } else if (this.IsJsFile(path)) {
                                code = JsEngine.Minify(path, code, projectItem, file.MinifyWith, customArg);
                            }
                        }
                        productionFileText.AppendLine(code);
                    }

                    string output = productionFileText.ToString();

                    if (fileGroup.Minify == FileGroupXml.MinifyActions.True) {
                        output = isJS ? JsEngine.Minify(fullPath, output, projectItem, fileGroup.MinifyWith)
                            : CssEngine.Minify(fullPath, output, projectItem, fileGroup.MinifyWith);
                    }

                    manager.AddFileByFileName(Utilities.GetBaseFileName(fullPath) + (isJS ? ".js" : ".css"), output);

                    if (fileGroup.Minify == FileGroupXml.MinifyActions.Both)
                    {
                        if (TaskList.Instance != null)
                        {
                            TaskList.Instance.Remove(fullPathMin);
                        }

                        output = isJS ? JsEngine.Minify(fullPath, output, projectItem, fileGroup.MinifyWith)
                            : CssEngine.Minify(fullPath, output, projectItem, fileGroup.MinifyWith);
                        manager.AddFileByFileName(fullPathMin, output);
                    }
                }

                if (projectItem != null)
                {
                    this.ReloadConfigFileDependencies(projectItem);
                }
            }
        }

        /// <summary>
        /// build a dictionary that has the files that could change as the key.
        /// for the value it is a LIST of config files that need updated if it does change.
        /// so, when a .less.css file changes, we look in the list and rebuild any of the configs associated with it.
        /// if a config file changes...this rebuild all of this....
        /// </summary>
        /// <param name="projectItem">project Item</param>
        internal void ReloadConfigFileDependencies(ProjectItem projectItem)
        {
            string configFileName = projectItem.get_FileNames(1);

            // remove all current dependencies for this config file...
            foreach (string key in this.dependentFiles.Keys.ToArray())
            {
                List<string> files = this.dependentFiles[key];
                if (files.Remove(configFileName) && files.Count == 0)
                {
                    this.dependentFiles.Remove(key);
                }
            }

            var fileGroups = this.LoadConfigFileGroups(configFileName);
            foreach (var fileGroup in fileGroups)
            {
                foreach (var file in fileGroup.Files)
                {
                    if (!this.dependentFiles.ContainsKey(file.Path))
                    {
                        this.dependentFiles.Add(file.Path, new List<string> { configFileName });
                    }
                    else
                    {
                        this.dependentFiles[file.Path].Add(configFileName);
                    }
                }
            }
        }

        private bool IsLessFile(string fileName)
        {
            return fileName.EndsWith(RegularLessFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsCoffeeScriptFile(string fileName)
        {
            return fileName.EndsWith(RegularCoffeeScriptFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsCssFile(string fileName)
        {
            return fileName.EndsWith(RegularCssFile, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsJsFile(string fileName)
        {
            return fileName.EndsWith(RegularJsFile, StringComparison.OrdinalIgnoreCase);
        }

        private IList<FileGroupXml> LoadConfigFileGroups(string configFileName)
        {
            try
            {
                XDocument doc = XDocument.Load(configFileName);

                string appRoot = string.Format("{0}\\", Path.GetDirectoryName(configFileName));

                IList<FileGroupXml> returnList = doc.Descendants("FileGroup")
                        .Concat(doc.Descendants(XName.Get("FileGroup", "urn:ChirpyConfig")))
                    .Select(n => new FileGroupXml(n, appRoot))
                    .ToList();

                return returnList;
            }
            catch (System.Xml.XmlException eError)
            {
                if (!eError.Message.Contains("Root element not found") && !eError.Message.Contains("Root element is missing"))
                {
                    throw eError;
                }

                return new List<FileGroupXml>(); //return empty list
            }
        }
    }
}
