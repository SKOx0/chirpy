using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Yahoo.Yui.Compressor;
using Zippy.Chirp.Manager;
using Zippy.Chirp.Xml;


namespace Zippy.Chirp {
    public struct document {
        static document() {

        }
    }
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Chirp : IDTExtensibility2 {
        DTE2 app;
        Events2 events;
        DocumentEvents eventsOnDocs;
        ProjectItemsEvents eventsOnProjectItems;
        SolutionEvents eventsOnSolution;
        BuildEvents eventsOnBuild;
        CommandEvents eventsOnCommand;
        AddIn instance;
        TaskList tasks;

        Dictionary<string, List<string>> dependentFiles =
            new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

        const string regularCssFile = ".css";
        const string regularJsFile = ".js";
        const string regularLessFile = ".less";
        const string minifiedCssFile = ".min.css";
        const string minifiedJsFile = ".min.js";
        const string ControllerCSFile = ".cs";
        const string ControllerVBFile = ".vb";
        const string MVCViewFile = ".aspx";
        const string MVCPartialViewFile = ".ascx";
        const string MVCT4TemplateName = "T4MVC.tt";

        /// <summary>
        /// Implements the constructor for the Add-in object. Place your initialization code within this method.
        /// </summary>
        public Chirp() {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom) {
            Settings.Load();

            this.instance = addInInst as AddIn;
            this.app = application as DTE2;
            this.events = app.Events as Events2;

            object missing = System.Reflection.Missing.Value;
            this.eventsOnDocs = this.events.get_DocumentEvents(missing as Document);
            this.eventsOnProjectItems = this.events.ProjectItemsEvents;
            this.eventsOnSolution = this.events.SolutionEvents;
            this.eventsOnBuild = this.events.BuildEvents;
            this.eventsOnCommand = this.events.CommandEvents;

            this.eventsOnCommand.BeforeExecute += new _dispCommandEvents_BeforeExecuteEventHandler(CommandEvents_BeforeExecute);
            this.eventsOnBuild.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(BuildEvents_OnBuildBegin);
            //this.eventsOnBuild.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(BuildEvents_OnBuildDone);
            this.eventsOnSolution.Opened += new _dispSolutionEvents_OpenedEventHandler(SolutionEvents_Opened);
            this.eventsOnSolution.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(SolutionEvents_ProjectRemoved);
            this.eventsOnSolution.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler(eventsOnSolution_AfterClosing);
            this.eventsOnProjectItems.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(ProjectItemsEvents_ItemRenamed);
            this.eventsOnProjectItems.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(ProjectItemsEvents_ItemAdded);
            this.eventsOnProjectItems.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(ProjectItemsEvents_ItemRemoved);
            this.eventsOnDocs.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(DocumentEvents_DocumentSaved);

            //OutputWindowWriteText("---Chirpy log---");
        }

        //void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action) {
        //    try {

        //        if (Action == vsBuildAction.vsBuildActionBuild || Action == vsBuildAction.vsBuildActionRebuildAll) {
        //            tasks.RemoveAll();

        //            var projects = (this.app.ActiveSolutionProjects as IEnumerable).Cast<Project>();
        //            foreach (var proj in projects) {
        //                var items = this.ProcessAllProjectItemsRecursively(proj.ProjectItems);
        //                foreach (var item in items) {
        //                    ProjectItemsEvents_ItemAdded(item);
        //                }
        //            }
        //        }

        //    } catch (Exception ex) {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}

        void eventsOnSolution_AfterClosing() {
            tasks.RemoveAll();
        }

        void SolutionEvents_ProjectRemoved(Project Project) {
            tasks.Remove(Project);
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom) {
            tasks = new TaskList(this.app);
        }


        #region Unused IDTExtensibility2 methods
        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom) {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom) {
        }
        #endregion

        #region Event Handlers
        void CommandEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault) {
            EnvDTE.Command objCommand = default(EnvDTE.Command);
            string sCommandName = null;

            objCommand = app.Commands.Item(Guid, ID);
            if ((objCommand != null)) {
                sCommandName = objCommand.Name;

                if (Settings.T4RunAsBuild) {
                    System.Text.RegularExpressions.Regex buildCommand = new System.Text.RegularExpressions.Regex("Build.BuildSelection|Build.BuildSolution|ClassViewContextMenus.ClassViewProject.Build");
                    if (buildCommand.IsMatch(sCommandName)) {
                        RunT4Template(Settings.T4RunAsBuildTemplate);
                    }
                }
            }
        }


        void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action) {
            //code move to CommandEvents_BeforeExecute in this events show warning message
            //if (Settings.T4RunAsBuild)
            //{
            //    //run t4 template on build begin
            //    if (Scope == vsBuildScope.vsBuildScopeSolution || Scope == vsBuildScope.vsBuildScopeProject)
            //    {

            //        try
            //        {
            //            string[] T4List=Settings.T4RunAsBuildTemplate.Split(new char[] { ',' });
            //            foreach (string t4Template in T4List)
            //            {
            //                ProjectItem projectItem = app.Solution.FindProjectItem(t4Template.Trim());

            //                if (projectItem != null)
            //                {
            //                    if (!projectItem.IsOpen)
            //                        projectItem.Open();
            //                    projectItem.Save();
            //                }
            //            }
            //        }
            //        catch(Exception eError)
            //        {
            //            OutputWindowWriteText("Error run template :" + eError.ToString());
            //        }
            //    }
            //}
        }

        void SolutionEvents_Opened() {
            foreach (Project project in this.app.Solution.Projects) {
                foreach (ProjectItem projectItem in ProcessAllProjectItemsRecursively(project.ProjectItems)) {
                    if (IsConfigFile(projectItem.Name)) {
                        ReloadConfigFileDependencies(projectItem);
                    }
                }
            }
        }

        void ProjectItemsEvents_ItemAdded(ProjectItem projectItem) {
            string fileName = projectItem.Name;

            if (IsChirpLessFile(fileName)) {
                GenerateCssFromLess(projectItem);

            }
            else if (IsMichaelAshChirpCssFile(fileName))
            {
                GenerateMinCssFromCss(projectItem,Settings.ChirpMichaelAshCssFile, CssCompressionType.MichaelAshRegexEnhancements);
            }
            else if (IsChirpHybridCssFile(fileName))
            {
                GenerateMinCssFromCss(projectItem,Settings.ChirpHybridCssFile, CssCompressionType.Hybrid);
            }
            else if (IsChirpCssFile(fileName))
            {
                GenerateMinCssFromCss(projectItem,Settings.ChirpCssFile, CssCompressionType.StockYuiCompressor);
            } else if (IsChirpYUIJsFile(fileName)) {
                GenerateMinYUIJsFromJs(projectItem);

            } else if (IsChirpJsFile(fileName)) {
                GenerateMinYUIJsFromJs(projectItem);
                //GenerateMinJsFromJs(projectItem, ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS);

            } else if (IsChirpGctJsFile(fileName)) {
                GenerateMinJsFromJs(projectItem, ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS);

            } else if (IsChirpSimpleJsFile(fileName)) {
                GenerateMinJsFromJs(projectItem, ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS);

            } else if (IsChirpWhiteSpaceJsFile(fileName)) {
                GenerateMinJsFromJs(projectItem, ClosureCompilerCompressMode.WHITESPACE_ONLY);

            } else if (IsMVCStandardViewScriptOrContentFile(projectItem.get_FileNames(1))) {
                if (IsChirpView(fileName)) {
                    GenerateView(projectItem, false);

                } else if (IsChirpViewPartial(fileName)) {
                    GenerateView(projectItem, true);
                }

                if (Settings.SmartRunT4MVC) {
                    RunT4Template(MVCT4TemplateName);
                }
            }
        }

       

        void ProjectItemsEvents_ItemRenamed(ProjectItem projectItem, string oldFileName) {
            if (IsAnyChirpFile(projectItem.Name)) {
                // Now a chirp file
                ProjectItemsEvents_ItemAdded(projectItem);
            } else if (IsAnyChirpFile(oldFileName)) {
                try {
                    VSProjectItemManager.DeleteAllItems(projectItem.ProjectItems);
                } catch (Exception e) {
                    OutputWindowWriteText("Exception was thrown when trying to rename file.\n" + e.ToString());
                    //MessageBox.Show("Exception was thrown when trying to rename file.\n" + e.ToString());
                }
            } else if (Settings.SmartRunT4MVC && IsMVCStandardViewScriptOrContentFile(projectItem.get_FileNames(1))) {
                RunT4Template(MVCT4TemplateName);
            }
        }

        void ProjectItemsEvents_ItemRemoved(ProjectItem projectItem) {
            if (Settings.SmartRunT4MVC && IsMVCStandardViewScriptOrContentFile(projectItem.get_FileNames(1))) {
                RunT4Template(MVCT4TemplateName);
            }

            string fileName = projectItem.get_FileNames(1);
            if (IsAnyChirpFile(projectItem.Name)) {
                tasks.Remove(fileName);
            }
        }

        void DocumentEvents_DocumentSaved(Document document) {
            var projectItem = document.ProjectItem;
            string fileName = projectItem.Name;

            if (IsConfigFile(fileName)) {
                GenerateFilesFromConfig(projectItem);
                ReloadConfigFileDependencies(projectItem);
            } else {
                if (IsAnyChirpFile(fileName)) {
                    ProjectItemsEvents_ItemAdded(projectItem);
                }

                CheckForConfigRefreshRecursively(document.ProjectItem);
                if (Settings.SmartRunT4MVC) {
                    if (IsMVCStandardControllerFile(fileName)) {
                        RunT4Template(MVCT4TemplateName);
                    }
                }
            }
        }

        void CheckForConfigRefreshRecursively(ProjectItem projectItem) {
            CheckForConfigRefresh(projectItem);
            foreach (ProjectItem projectItemInner in projectItem.ProjectItems) {
                CheckForConfigRefreshRecursively(projectItemInner);
            }
        }

        void CheckForConfigRefresh(ProjectItem projectItem) {
            string fullFileName = projectItem.get_FileNames(1);

            if (dependentFiles.ContainsKey(fullFileName)) {
                foreach (string configFile in dependentFiles[fullFileName]) {
                    ProjectItem item = LocateProjectItemForFileName(configFile);
                    if (item != null) {
                        GenerateFilesFromConfig(item);
                    }
                }
            }
        }
        #endregion

        #region File Dependencies
        /// <summary>
        /// build a dictionary that has the files that could change as the key.
        /// for the value it is a LIST of config files that need updated if it does change.
        /// so, when a .less.css file changes, we look in the list and rebuild any of the configs associated with it.
        /// if a config file changes...this rebuild all of this....
        /// </summary>
        /// <param name="projectItem"></param>
        void ReloadConfigFileDependencies(ProjectItem projectItem) {
            string configFileName = projectItem.get_FileNames(1);

            //remove all current dependencies for this config file...
            foreach (string key in dependentFiles.Keys) {
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

        IEnumerable<ProjectItem> ProcessAllProjectItemsRecursively(ProjectItems projectItems) {
            foreach (ProjectItem projectItem in projectItems) {
                if (projectItem.Kind == Constants.vsProjectItemKindPhysicalFolder) {
                    foreach (ProjectItem folderProjectItem in ProcessAllProjectItemsRecursively(projectItem.ProjectItems))
                        yield return folderProjectItem;
                }

                yield return projectItem;
            }
        }

        ProjectItem LocateProjectItemForFileName(string fileName) {
            foreach (Project project in this.app.Solution.Projects) {
                foreach (ProjectItem projectItem in ProcessAllProjectItemsRecursively(project.ProjectItems)) {
                    if (projectItem.get_FileNames(1) == fileName)
                        return projectItem;
                }

            }
            return null;
        }
        #endregion

        #region File Generation

        private void RunT4Template(string t4TemplateList) {
            try {
                string[] T4List = t4TemplateList.Split(new char[] { ',' });
                foreach (string t4Template in T4List) {
                    ProjectItem projectItem = app.Solution.FindProjectItem(t4Template.Trim());

                    if (projectItem != null) {
                        if (!projectItem.IsOpen)
                            projectItem.Open();
                        projectItem.Save();
                    }
                }
            } catch (Exception eError) {
                OutputWindowWriteText("Error running template :" + eError.ToString());
            }
        }

        void GenerateMinYUIJsFromJs(ProjectItem projectItem) {
            try {
                string fileNamePrefix = GetYuiPrefix(projectItem.Name);
                string file = projectItem.get_FileNames(1);
                string js = File.ReadAllText(file);
                string miniJs = string.IsNullOrEmpty(js) ? string.Empty : TransformYUIJsToMiniJs(projectItem.ContainingProject, js, file);
                string miniJsSuffix = Settings.ChirpYUIJsFile.Replace(".js", miniJs);

                using (var manager = new VSProjectItemManager(app, projectItem, fileNamePrefix)) {
                    manager.AddFile(minifiedJsFile, miniJs);
                }
            } catch (Exception e) {
                OutputWindowWriteText("Chirpy Burped: Exception was thrown.\n" + e.ToString());
                //MessageBox.Show("Chirpy Burped: Exception was thrown.\n" + e.ToString());
            }
        }

        void GenerateView(ProjectItem projectItem, bool partial) {
            try {
                string fileNamePrefix = GetFileNamePrefix(projectItem.Name, partial ? Settings.ChirpPartialViewFile : Settings.ChirpViewFile);
                string file = projectItem.get_FileNames(1);
                string view = File.ReadAllText(file);
                string processed = string.IsNullOrEmpty(view) ? string.Empty : TransformView(projectItem.ContainingProject, view, file);

                using (var manager = new VSProjectItemManager(app, projectItem, fileNamePrefix)) {
                    manager.AddFile(partial ? MVCPartialViewFile : MVCViewFile, processed);
                }

            } catch (Exception e) {
                OutputWindowWriteText("Chirpy Burped: Exception was thrown.\n" + e.ToString());
            }
        }

        private string GetYuiPrefix(string fileName) {
            if (fileName.EndsWith(Settings.ChirpYUIJsFile)) {
                return GetFileNamePrefix(fileName, Settings.ChirpYUIJsFile);
            } else {
                return GetFileNamePrefix(fileName, Settings.ChirpJsFile);
            }
        }

        void GenerateMinJsFromJs(ProjectItem projectItem, ClosureCompilerCompressMode compressMode) {
            try {
                string fileNamePrefix = string.Empty;
                string fileName = projectItem.get_FileNames(1);

                tasks.Remove(fileName);

                switch (compressMode) {
                    case ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS:
                        fileNamePrefix = GetFileNamePrefix(projectItem.Name, Settings.ChirpSimpleJsFile);
                        break;
                    case ClosureCompilerCompressMode.WHITESPACE_ONLY:
                        fileNamePrefix = GetFileNamePrefix(projectItem.Name, Settings.ChirpWhiteSpaceJsFile);
                        break;
                    default:
                        fileNamePrefix = GetFileNamePrefix(projectItem.Name, Settings.ChirpGctJsFile);
                        break;
                }

                string miniJs = TransformFromClosureCompilerToMiniJs(projectItem.ContainingProject, fileName, compressMode);

                using (var manager = new VSProjectItemManager(app, projectItem, fileNamePrefix)) {
                    manager.AddFile(minifiedJsFile, miniJs);
                }
            } catch (Exception e) {
                OutputWindowWriteText("Chirpy Burped: Exception was thrown.\n" + e.ToString());
                //MessageBox.Show("Chirpy Burped: Exception was thrown.\n" + e.ToString());
            }
        }

        void GenerateMinCssFromCss(ProjectItem projectItem,string chirpCssFile, CssCompressionType cssCompressionType)
        {
            try {
                string fileNamePrefix = GetFileNamePrefix(projectItem.Name, chirpCssFile);
                string css = File.ReadAllText(projectItem.get_FileNames(1));
                string miniCss = string.IsNullOrEmpty(css) ? string.Empty : TransformCssToMiniCss(css, cssCompressionType);

                using (var manager = new VSProjectItemManager(app, projectItem, fileNamePrefix)) {
                    manager.AddFile(minifiedCssFile, miniCss);
                }
            } catch (Exception e) {
                OutputWindowWriteText("Chirpy Burped: Exception was thrown.\n" + e.ToString());
                //MessageBox.Show("Chirpy Burped: Exception was thrown.\n" + e.ToString());
            }
        }

        void GenerateCssFromLess(ProjectItem projectItem) {
            try {
                string fileNamePrefix = GetLessPrefix(projectItem.Name);
                string filePath = projectItem.get_FileNames(1);
                string fileText = File.ReadAllText(filePath);

                string css = TransformLessToCss(projectItem.ContainingProject, filePath, fileText);
                string miniCss = string.IsNullOrEmpty(css) ? string.Empty : TransformCssToMiniCss(css);

                using (var manager = new VSProjectItemManager(app, projectItem, fileNamePrefix)) {
                    manager.AddFile(regularCssFile, css);
                    manager.AddFile(minifiedCssFile, miniCss);
                }
            } catch (Exception e) {
                OutputWindowWriteText("Chirpy Burped: Exception was thrown.\n" + e.ToString());
            }
        }
        #endregion

        #region Config Files

        IList<FileGroupXml> LoadConfigFileGroups(string configFileName) {
            XDocument doc = XDocument.Load(configFileName);

            string appRoot = string.Format("{0}\\", Path.GetDirectoryName(configFileName));
            return doc.Descendants("FileGroup")
                .Select(n => new FileGroupXml(n, appRoot))
                .ToList();
        }

        void GenerateFilesFromConfig(ProjectItem projectItem) {
            try {
                var fileGroups = LoadConfigFileGroups(projectItem.get_FileNames(1));
                string directory = Path.GetDirectoryName(projectItem.get_FileNames(1));

                using (var manager = new VSProjectItemManager(this.app, projectItem)) {
                    foreach (var fileGroup in fileGroups) {
                        var allFileText = new StringBuilder();
                        foreach (var file in fileGroup.Files) {
                            string path = file.Path;
                            string text = File.ReadAllText(path);

                            if (IsLessFile(path)) {
                                text = TransformLessToCss(projectItem.ContainingProject, path, text);
                            }

                            if (file.Minify) {
                                if (IsCssFile(path) || IsLessFile(path)) {
                                    text = TransformCssToMiniCss(text);
                                } else if (IsJsFile(path)) {
                                    text = TransformJsToMiniJs(projectItem.ContainingProject, text, path);
                                }
                            }

                            allFileText.Append(text);
                            allFileText.Append(Environment.NewLine);
                        }

                        string fullPath = directory + @"\" + fileGroup.Name;
                        manager.AddFileByFileName(fullPath, allFileText.ToString());
                    }
                }
            } catch (Exception e) {
                OutputWindowWriteText("Chirpy Burped: Exception was thrown.\n" + e.ToString());
                //MessageBox.Show("Chirpy Burped: Exception was thrown.\n" + e.ToString());
            }
        }
        #endregion

        #region IsChirpFile
        bool IsChirpLessFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpLessFile, StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(Settings.ChirpLessCssFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsLessFile(string fileName) {
            return (fileName.EndsWith(regularLessFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsChirpCssFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpCssFile, StringComparison.OrdinalIgnoreCase));
        }
        private bool IsChirpHybridCssFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpHybridCssFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsMichaelAshChirpCssFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpMichaelAshCssFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsCssFile(string fileName) {
            return (fileName.EndsWith(regularCssFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsChirpYUIJsFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpYUIJsFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsChirpView(string filename) {
            return (filename.EndsWith(Settings.ChirpViewFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsChirpViewPartial(string filename) {
            return (filename.EndsWith(Settings.ChirpPartialViewFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsChirpGctJsFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpGctJsFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsChirpJsFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpJsFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsChirpWhiteSpaceJsFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpWhiteSpaceJsFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsChirpSimpleJsFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpSimpleJsFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsJsFile(string fileName) {
            return (fileName.EndsWith(regularJsFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsConfigFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpConfigFile, StringComparison.OrdinalIgnoreCase));
        }

        bool IsAnyChirpFile(string fileName) {
            return IsChirpLessFile(fileName) || IsChirpCssFile(fileName) || IsChirpHybridCssFile(fileName) || IsMichaelAshChirpCssFile(fileName) 
                || IsChirpJsFile(fileName) || IsChirpSimpleJsFile(fileName) || IsChirpWhiteSpaceJsFile(fileName) || IsChirpYUIJsFile(fileName) || IsChirpGctJsFile(fileName) 
                || IsChirpView(fileName) || IsChirpViewPartial(fileName);
        }

        bool IsMVCStandardControllerFile(string fileName) {
            return (fileName.EndsWith(ControllerCSFile, StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(ControllerVBFile, StringComparison.OrdinalIgnoreCase)) &&
                                        fileName.Contains("Controller");
        }

        bool IsMVCStandardViewScriptOrContentFile(string fileName) {
            return ((fileName.EndsWith(MVCViewFile, StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(MVCPartialViewFile, StringComparison.OrdinalIgnoreCase)) &&
                                        fileName.Contains("Views")) || fileName.Contains("Scripts") || fileName.Contains("Content");
        }

        string GetFileNamePrefix(string fileName, string suffix) {
            int PosSuffix = fileName.ToLower().IndexOf(suffix.ToLower());
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("Invalid file name: " + fileName);
            if (PosSuffix >= 0)
                return fileName.Substring(0, PosSuffix);
            else
                return string.Empty;
        }

        string GetLessPrefix(string fileName) {
            if (fileName.Contains(Settings.ChirpLessCssFile)) {
                return GetFileNamePrefix(fileName, Settings.ChirpLessCssFile);
            }

            return GetFileNamePrefix(fileName, Settings.ChirpLessFile);
        }
        #endregion

        //This is incompatible with the latest dotLess
        //#region Less Engine
        //ILessEngine lazyLessEngine;
        //ILessEngine lessEngine
        //{
        //    get
        //    {
        //        if (lazyLessEngine == null)
        //        {
        //            lazyLessEngine = new EngineFactory().GetEngine(new DotlessConfiguration
        //            {
        //                MinifyOutput = false
        //            });
        //        }
        //        return lazyLessEngine;
        //    }
        //}
        //#endregion

        #region Less

        public void TreatLessAsCss(bool force) {
            var extKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0_Config\Languages\File Extensions");
            var lessKey = extKey.OpenSubKey(".less", true);
            var cssKey = extKey.OpenSubKey(".css");
            var cssGuid = cssKey == null ? null : cssKey.GetValue(string.Empty) as string;
            var lessGuid = lessKey == null ? null : lessKey.GetValue(string.Empty) as string;

            if (!cssGuid.IsNullOrEmpty() && (force || string.IsNullOrEmpty(lessGuid))) {
                lessKey.SetValue(string.Empty, cssGuid);
            }
        }

        dotless.Core.Parser.Parser lazyLessParser;
        dotless.Core.Parser.Parser lessParser {
            get {
                if (lazyLessParser == null) {
                    lazyLessParser = new dotless.Core.Parser.Parser();
                }
                return lazyLessParser;
            }
        }
        #endregion


        #region Transformations
        static Regex rxLineNum = new Regex(@"line\s+([0-9]+)", RegexOptions.Compiled);
        static Regex rxColNum = new Regex(@"\s+(\-*)\^", RegexOptions.Compiled);

        string TransformLessToCss(Project project, string fullFileName, string text) {
            try {
                tasks.Remove(fullFileName);

                if (string.IsNullOrEmpty(text)) return string.Empty;

                //LessSourceObject lessFile = new LessSourceObject {
                //    Key = fullFileName,
                //    Content = text
                //};

                var current = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(fullFileName));

                var ret = lessParser.Parse(text, fullFileName).ToCSS();
                //var ret = this.lessEngine.TransformToCss(lessFile);

                Directory.SetCurrentDirectory(current);
                return ret;

            } catch (Exception e) {
                int line = 1, column = 1;
                var description = e.Message.Trim();
                Match match;
                if ((match = rxLineNum.Match(description)).Success) {
                    line = match.Groups[1].Value.ToInt(1);
                }
                if ((match = rxColNum.Match(description)).Success) {
                    column = match.Groups[1].Length + 1;
                }

                tasks.Add(project, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, fullFileName, line, column, description);

                this.app.ToolWindows.ErrorList.Parent.Activate();

                return HandleTransformException(e);
            }
        }

        Regex rxScripts = new Regex(@"\<(style|script)([^>]*)\>(.*?)\</\1\>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        string TransformView(Project project, string text, string fullFilename) {
            try {
                var tags = rxScripts.Matches(text).Cast<Match>().Reverse();

                foreach (var match in tags) {
                    var tagName = match.Groups[1].Value;
                    var attrs = match.Groups[2].Value;
                    var code = match.Groups[3].Value;

                    if (tagName.Is("script")) {
                        code = TransformYUIJsToMiniJs(project, code, fullFilename);

                    } else if (tagName.Is("style")) {
                        int i = attrs.IndexOf("text/less", StringComparison.InvariantCultureIgnoreCase);
                        if (i > -1) {
                            attrs = attrs.Substring(0, i) + "text/css" + attrs.Substring(i + "text/less".Length);
                            code = TransformLessToCss(project, fullFilename, code);
                        }

                        code = TransformCssToMiniCss(code);

                    } else {
                        throw new NotImplementedException();
                    }

                    text = text.Substring(0, match.Index)
                        + '<' + tagName + attrs + '>' + code + "</" + tagName + '>'
                        + text.Substring(match.Index + match.Length);
                }

                return text;

            } catch (Exception e) {
                this.app.ToolWindows.ErrorList.Parent.Activate();
                return HandleTransformException(e);
            }
        }

        string TransformCssToMiniCss(string text)
        {
            return TransformCssToMiniCss(text, CssCompressionType.StockYuiCompressor);
        }

        string TransformCssToMiniCss(string text, CssCompressionType cssCompressionType)
        {
            try {
                return CssCompressor.Compress(text,0,cssCompressionType);
            } catch (Exception e) {
                return HandleTransformException(e);
            }
        }

        string TransformYUIJsToMiniJs(Project project, string text, string fullFileName) {
            try {
                tasks.Remove(fullFileName);

                var errors = new EcmaScriptErrorReporter(tasks, project, fullFileName);
                var compressor = new JavaScriptCompressor(text, true, System.Text.Encoding.Default, System.Globalization.CultureInfo.CurrentCulture, false, errors);
                return compressor.Compress();

            } catch (Exception e) {
                this.app.ToolWindows.ErrorList.Parent.Activate();
                return HandleTransformException(e);
            }
        }

        string TransformFromClosureCompilerToMiniJs(Project project, string fileName, ClosureCompilerCompressMode compressMode) {
            try {
                string returnedCode = GoogleClosureCompiler.Compress(fileName, compressMode, (category, msg, line, col) => {
                    tasks.Add(project, category, fileName, line, col, msg);
                });

                return returnedCode;
            } catch (Exception e) {
                this.app.ToolWindows.ErrorList.Parent.Activate();
                return HandleTransformException(e);
            }
        }

        string TransformJsToMiniJs(Project project, string text, string fullFileName) {
            return TransformYUIJsToMiniJs(project, text, fullFileName);
        }
        #endregion

        #region "output login"
        private string HandleTransformException(Exception e) {
            string msg = "Error Generating Ouput: " + Environment.NewLine + e.ToString();
            string commentedMessage = "/*" + Environment.NewLine + msg + Environment.NewLine + " */";
            OutputWindowWriteText(msg);

            return string.Empty;
        }

        OutputWindowPane lazyOutputWindowPane;
        OutputWindowPane outputWindowPane {
            get {
                if (lazyOutputWindowPane == null) {
                    lazyOutputWindowPane = GetOutputWindowPane("Chirpy");
                }
                return lazyOutputWindowPane;
            }
        }

        OutputWindowPane GetOutputWindowPane(string name) {
            OutputWindow ow = this.app.ToolWindows.OutputWindow;
            OutputWindowPane owP;

            owP = ow.OutputWindowPanes.Cast<OutputWindowPane>().FirstOrDefault(x => x.Name == name);
            if (owP == null) {
                owP = ow.OutputWindowPanes.Add(name);
            }

            owP.Activate();
            return owP;
        }

        private void OutputWindowWriteText(string messageText) {
            try {
                outputWindowPane.OutputString(messageText + System.Environment.NewLine);
            } catch (Exception eError) {
                MessageBox.Show(eError.ToString());
            }
        }
        #endregion
    }
}