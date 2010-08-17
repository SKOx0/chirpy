using System;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Zippy.Chirp.Engines;
using Zippy.Chirp.Manager;


namespace Zippy.Chirp {
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Chirp : IDTExtensibility2 {
        internal DTE2 app;
        Events2 events;
        DocumentEvents eventsOnDocs;
        ProjectItemsEvents eventsOnProjectItems;
        SolutionEvents eventsOnSolution;
        BuildEvents eventsOnBuild;
        CommandEvents eventsOnCommand;
        AddIn instance;
        TaskList tasks;
        EngineManager actions;

        internal static YuiJsEngine YuiJsEngine { get; set; }
        internal static YuiCssEngine YuiCssEngine { get; set; }

        internal static ClosureCompilerEngine ClosureCompilerEngine { get; set; }

        internal static MsCssEngine MsCssEngine { get; set; }
        internal static MsJsEngine MsJsEngine { get; set; }

        internal static LessEngine LessEngine { get; set; }
        internal static ConfigEngine ConfigEngine { get; set; }
        internal static T4Engine T4Engine { get; set; }
        internal static ViewEngine ViewEngine { get; set; }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom) {
            Settings.Load();
            Settings.Saved += LoadActions;

            this.instance = addInInst as AddIn;
            this.app = application as DTE2;
            this.events = app.Events as Events2;

            this.eventsOnDocs = this.events.get_DocumentEvents();
            this.eventsOnProjectItems = this.events.ProjectItemsEvents;
            this.eventsOnSolution = this.events.SolutionEvents;
            this.eventsOnBuild = this.events.BuildEvents;
            this.eventsOnCommand = this.events.CommandEvents;

            this.eventsOnCommand.BeforeExecute += new _dispCommandEvents_BeforeExecuteEventHandler(CommandEvents_BeforeExecute);
            //this.eventsOnBuild.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(BuildEvents_OnBuildBegin);
            //this.eventsOnBuild.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(BuildEvents_OnBuildDone);
            this.eventsOnSolution.Opened += new _dispSolutionEvents_OpenedEventHandler(SolutionEvents_Opened);
            this.eventsOnSolution.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(SolutionEvents_ProjectRemoved);
            this.eventsOnSolution.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler(eventsOnSolution_AfterClosing);
            this.eventsOnProjectItems.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(ProjectItemsEvents_ItemRenamed);
            this.eventsOnProjectItems.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(ProjectItemsEvents_ItemAdded);
            this.eventsOnProjectItems.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(ProjectItemsEvents_ItemRemoved);
            this.eventsOnDocs.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(DocumentEvents_DocumentSaved);

            tasks = new TaskList(app);
            actions = new EngineManager(this);
            LoadActions();
            TreatLessAsCss(false);
        }

        public void LoadActions() {
            actions.Clear();
            actions.Add(Chirp.YuiCssEngine = new YuiCssEngine());
            actions.Add(Chirp.YuiJsEngine = new YuiJsEngine());

            actions.Add(Chirp.ClosureCompilerEngine = new ClosureCompilerEngine());

            actions.Add(Chirp.LessEngine = new LessEngine());
            actions.Add(Chirp.MsJsEngine = new MsJsEngine());
            actions.Add(Chirp.MsCssEngine = new MsCssEngine());
            actions.Add(Chirp.ConfigEngine = new ConfigEngine());
            actions.Add(Chirp.ViewEngine = new ViewEngine());
            actions.Add(Chirp.T4Engine = new T4Engine());
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
            actions.Dispose();
        }

        void SolutionEvents_ProjectRemoved(Project Project) {
            tasks.Remove(Project);
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

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom) {
        }
        #endregion

        #region Event Handlers
        static string[] buildCommands = new[] { "Build.BuildSelection", "Build.BuildSolution", "ClassViewContextMenus.ClassViewProject.Build" };
        void CommandEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault) {
            EnvDTE.Command objCommand = default(EnvDTE.Command);
            string sCommandName = null;

            objCommand = app.Commands.Item(Guid, ID);
            if ((objCommand != null)) {
                sCommandName = objCommand.Name;

                if (Settings.T4RunAsBuild) {
                    if (buildCommands.Contains(sCommandName)) {
                        Engines.T4Engine.RunT4Template(app, Settings.T4RunAsBuildTemplate);
                    }
                }
            }
        }

        void SolutionEvents_Opened() {
            try {

                foreach (Project project in app.Solution.Projects) {
                    var configs = project.ProjectItems.ProcessFolderProjectItemsRecursively()
                        .Where(x => ConfigEngine.Handles(x.Name) > 0);

                    foreach (ProjectItem config in configs) {
                        ConfigEngine.ReloadConfigFileDependencies(config);
                    }
                }

            } catch (Exception e) {
                OutputWindowWriteText(e.ToString());
            }
        }

        void ProjectItemsEvents_ItemAdded(ProjectItem projectItem) {
            try {
                actions.Enqueue(projectItem);

            } catch (Exception e) {
                OutputWindowWriteText(e.ToString());
            }
        }

        void ProjectItemsEvents_ItemRenamed(ProjectItem projectItem, string oldFileName) {
            if (actions.IsTransformed(projectItem.get_FileNames(1))) {
                // Now a chirp file
                ProjectItemsEvents_ItemAdded(projectItem);
            } else if (actions.IsTransformed(oldFileName)) {
                try {
                    VSProjectItemManager.DeleteAllItems(projectItem.ProjectItems);
                    tasks.Remove(oldFileName);
                } catch (Exception e) {
                    OutputWindowWriteText("Exception was thrown when trying to rename file.\n" + e.ToString());
                }
            }
        }

        void ProjectItemsEvents_ItemRemoved(ProjectItem projectItem) {
            string fileName = projectItem.get_FileNames(1);

            if (Chirp.T4Engine.Handles(fileName) > 0) {
                Chirp.T4Engine.Run(fileName, projectItem);

            } else if (actions.IsTransformed(fileName)) {
                tasks.Remove(fileName);
            }
        }

        void DocumentEvents_DocumentSaved(Document document) {
            ProjectItemsEvents_ItemAdded(document.ProjectItem);
        }

        #endregion


        #region Less

        public void TreatLessAsCss(bool force) {
            using (var extKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0_Config\Languages\File Extensions")) {
                var lessKey = extKey.OpenSubKey(".less", true);
                var cssKey = extKey.OpenSubKey(".css");
                var cssGuid = cssKey == null ? null : cssKey.GetValue(string.Empty) as string;
                var lessGuid = lessKey == null ? null : lessKey.GetValue(string.Empty) as string;

                if (!string.IsNullOrEmpty(cssGuid) && (force || string.IsNullOrEmpty(lessGuid))) {
                    lessKey.SetValue(string.Empty, cssGuid);
                }
            }
        }
        #endregion


        #region "output login"

        OutputWindowPane lazyOutputWindowPane;
        public OutputWindowPane outputWindowPane {
            get {
                if (lazyOutputWindowPane == null) {
                    lazyOutputWindowPane = GetOutputWindowPane("Chirpy");
                }
                return lazyOutputWindowPane;
            }
        }

        OutputWindowPane GetOutputWindowPane(string name) {
            OutputWindow ow = app.ToolWindows.OutputWindow;
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