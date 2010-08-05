using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EnvDTE;
using Yahoo.Yui.Compressor;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines {
    class ConfigEngine : Engine<ConfigEngine> {
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

        public override bool IsEngineFor(string filename) {
            return filename.EndsWith(Settings.ChirpConfigFile, System.StringComparison.OrdinalIgnoreCase);
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

        IList<FileGroupXml> LoadConfigFileGroups(string configFileName)
        {
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

        public override IEnumerable<IResult> Transform(Item item) {
            var fileGroups = LoadConfigFileGroups(item.FileName);
            string directory = Path.GetDirectoryName(item.FileName);

            foreach (var fileGroup in fileGroups) {
                var allFileText = new StringBuilder();
                bool isjs = false;

                foreach (var file in fileGroup.Files) {
                    IEnumerable<IResult> subresult = null;
                    var path = file.Path;
                    var subitem = new Item(path);
                    using (new EnvironmentDirectory(path)) {
                        isjs = IsJsFile(path);
                        TaskList.Instance.Remove(path);

                        if (IsLessFile(path))
                        {

                            subresult = LessEngine.Instance.BasicTransform(subitem,file.MinifyWith);


                        }
                        else if (file.Minify == true)
                        {
                            if (IsCssFile(path))
                            {
                                switch (file.MinifyWith)
                                {
                                    case MinifyType.yui:
                                        subresult = YuiCssEngine.Instance.BasicTransform(subitem);
                                        break;
                                    case MinifyType.yuiMARE:
                                        subresult = YuiCssEngine.Instance.BasicTransform(subitem, CssCompressionType.MichaelAshRegexEnhancements);
                                        break;
                                    case MinifyType.yuiHybird:
                                        subresult = YuiCssEngine.Instance.BasicTransform(subitem, CssCompressionType.Hybrid);
                                        break;
                                    case MinifyType.msAjax:
                                        subresult = MsCssEngine.Instance.BasicTransform(subitem);
                                        break;
                                    default:
                                        subresult = YuiCssEngine.Instance.BasicTransform(subitem);
                                        break;
                                }

                            }
                            else if (IsJsFile(path))
                            {
                                switch (file.MinifyWith)
                                {
                                    case MinifyType.yui:
                                        subresult = YuiJsEngine.Instance.BasicTransform(subitem);
                                        break;
                                    case MinifyType.yuiMARE:
                                        subresult = YuiJsEngine.Instance.BasicTransform(subitem);
                                        break;
                                    case MinifyType.yuiHybird:
                                        subresult = YuiJsEngine.Instance.BasicTransform(subitem);
                                        break;
                                    case MinifyType.gctAdvanced:
                                        subresult = ClosureCompilerEngine.Instance.BasicTransform(subitem, ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS);
                                        break;
                                    case MinifyType.gctSimple:
                                        subresult = ClosureCompilerEngine.Instance.BasicTransform(subitem, ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS);
                                        break;
                                    case MinifyType.gstWhiteSpaceOnly:
                                        subresult = ClosureCompilerEngine.Instance.BasicTransform(subitem, ClosureCompilerCompressMode.WHITESPACE_ONLY);
                                        break;
                                    case MinifyType.msAjax:
                                        subresult = MsJsEngine.Instance.BasicTransform(subitem);
                                        break;
                                    default:
                                        subresult = YuiJsEngine.Instance.BasicTransform(subitem);
                                        break;
                                }

                            }
                        }

                        if (subresult != null) {
                            subresult = subresult.ToArray(); //compile the results, otherwise weirdness happens
                            subitem.Text = subresult.OfType<FileResult>().Where(x => x.Minified == file.Minify).Select(x => x.Text).FirstOrDefault();
                            foreach (var err in subresult.OfType<ErrorResult>()) yield return err;
                        }
                    }

                    allFileText.Append(subitem.Text);
                    allFileText.Append(Environment.NewLine);
                }

                string fullPath = directory + @"\" + fileGroup.Name;
                if (!string.IsNullOrEmpty(fileGroup.Path))
                    fullPath = fileGroup.Path;

                string output = allFileText.ToString();

                yield return new FileResult(fullPath, output, true);

                if (fileGroup.Minify == FileGroupXml.MinifyMode.Both) {
                    var subitem = new Item(item, output);
                    subitem.BaseFileName = Engine.GetBaseFileName(fullPath);
                    var subresults = isjs ? YuiJsEngine.Instance.BasicTransform(subitem) : YuiCssEngine.Instance.BasicTransform(subitem);
                    foreach (var subresult in subresults) yield return subresult;
                }
            }

            ReloadConfigFileDependencies(item.ProjectItem);
        }

    }
}
