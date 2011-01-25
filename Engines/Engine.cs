using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Zippy.Chirp.Manager;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines {
    public abstract class JsEngine : TransformEngine {
        public static string Minify(string fullFileName, string outputText, ProjectItem projectItem, MinifyType mode) {
            switch (mode) {
                case MinifyType.gctAdvanced:
                    return ClosureCompilerEngine.Minify(fullFileName, outputText, projectItem, ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS);
                case MinifyType.gctSimple:
                    return ClosureCompilerEngine.Minify(fullFileName, outputText, projectItem, ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS);
                case MinifyType.gctWhiteSpaceOnly:
                    return ClosureCompilerEngine.Minify(fullFileName, outputText, projectItem, ClosureCompilerCompressMode.WHITESPACE_ONLY);
                case MinifyType.msAjax:
                    return MsJsEngine.Minify(fullFileName, outputText, projectItem);
                case MinifyType.uglify:
                    return UglifyEngine.Minify(fullFileName, outputText, projectItem);
                case MinifyType.jsBeautifier:
                    return UglifyEngine.Beautify(outputText);
                default:
                    return YuiJsEngine.Minify(fullFileName, outputText, projectItem);
            }
        }
    }

    public abstract class CssEngine : TransformEngine {
        public static string Minify(string fullFileName, string outputText, ProjectItem projectItem, MinifyType mode) {
            switch (mode) {
                case MinifyType.msAjax:
                    return MsCssEngine.Minify(fullFileName, outputText, projectItem);
                case MinifyType.yui:
                case MinifyType.yuiMARE:
                case MinifyType.yuiHybrid:
                default:
                    return YuiCssEngine.Minify(outputText, mode);
            }
        }
    }

    /// <summary>
    /// Performs some action on a ProjectItem
    /// </summary>
    public abstract class ActionEngine : IDisposable {
        /// <summary>
        /// Determines whether this action hands the specified file.  Returns an int to specify the priority--0 being not handled.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public abstract int Handles(string fullFileName);
        public abstract void Run(string fullFileName, ProjectItem projectItem);
        internal Chirp _Chirp;

        public virtual void Dispose() { }
    }

    /// <summary>
    /// Transforms the contents of a file
    /// </summary>
    public abstract class TransformEngine : ActionEngine {
        public string OutputExtension { get; set; }
        public string[] Extensions { get; set; }
        public virtual string GetOutputExtension(string fullFileName) {
            return OutputExtension;
        }
        public abstract string Transform(string fullFileName, string text, ProjectItem projectItem);

        public override int Handles(string fullFileName) {
            if (fullFileName.EndsWith(GetOutputExtension(fullFileName), StringComparison.InvariantCultureIgnoreCase)) return 0;
            var match = Extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault() ?? string.Empty;
            return match.Length;
        }

        public override void Run(string fullFileName, ProjectItem projectItem) {
            using (var manager = new VSProjectItemManager(_Chirp.app, projectItem)) {
                string baseFileName = Utilities.GetBaseFileName(fullFileName, Extensions);
                string inputText = System.IO.File.ReadAllText(fullFileName);
                string outputText = Transform(fullFileName, inputText, projectItem);
                Process(manager, fullFileName, projectItem, baseFileName, outputText);
            }
        }

        public virtual void Process(VSProjectItemManager manager, string fullFileName, ProjectItem projectItem, string baseFileName, string outputText) {
            if (manager != null)
                manager.AddFileByFileName(baseFileName + GetOutputExtension(fullFileName), outputText);
        }
    }

    public class EngineManager : Zippy.Chirp.Threading.ServiceQueue<ProjectItem> {
        private List<ActionEngine> _allactions = new List<ActionEngine>();
        private TransformEngine[] _transformers = new TransformEngine[0];
        private ActionEngine[] _actions = new ActionEngine[0];
        private Chirp _Chirp;

        public EngineManager(Chirp chirp) {
            _Chirp = chirp;
        }

        public bool IsHandled(string fullFileName) {
            return _actions.Any(x => x.Handles(fullFileName) > 0);
        }

        public bool IsTransformed(string fullFileName) {
            return _transformers.Any(x => x.Handles(fullFileName) > 0);
        }

        protected override void Process(ProjectItem projectItem) {
            var fullFileName = projectItem.get_FileNames(1);
            TaskList.Instance.Remove(fullFileName);

            var actions = _allactions.Select(x => new { action = x, priority = x.Handles(fullFileName) })
                .OrderByDescending(x => x.priority).Where(x => x.priority > 0).Select(x => x.action);

            bool transformed = false;
            foreach (var action in actions) {
                if (action is TransformEngine) {
                    if (transformed) continue;
                    transformed = true;
                }
                _Chirp.outputWindowPane.OutputString(action.GetType().Name + " -- " + fullFileName + "\r\n");
                try {
                    action.Run(fullFileName, projectItem);
                } catch (System.Exception eError) {
                    System.Windows.Forms.MessageBox.Show(string.Format("Error: {0}. See output window for details.", eError.Message));
                    _Chirp.outputWindowPane.OutputString(string.Format("Error: {0}\r\n", eError));
                }
                if (TaskList.Instance.HasErrors(fullFileName))
                    break;
            }

            _Chirp.ConfigEngine.CheckForConfigRefresh(projectItem);

        }

        protected override void Error(ProjectItem projectItem, Exception ex) {
            if (ex is COMException || ex is System.Threading.ThreadAbortException) return;
            if (projectItem != null)
                TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, projectItem.get_FileNames(1), 1, 1, ex.ToString());
            else _Chirp.outputWindowPane.OutputString(ex.ToString() + Environment.NewLine);
        }

        //Add a new ProjectItem processor
        public void Add(ActionEngine action) {
            action._Chirp = _Chirp;
            _allactions.Add(action);
            _actions = _allactions.ToArray();
            _transformers = _allactions.OfType<TransformEngine>().ToArray();
        }

        //Remove all actions
        public void Clear() {
            foreach (var action in _allactions)
                try {
                    action.Dispose();
                } catch (Exception) { }
            _allactions.Clear();
            _actions = new ActionEngine[0];
            _transformers = new TransformEngine[0];
        }

        public override void Enqueue(ProjectItem projectItem) {

            var parent = projectItem.GetParent();
            if (parent != null && !parent.IsFolder() && IsTransformed(parent.get_FileNames(1))) {
                return;
            }

            var file = projectItem.get_FileNames(1);
            if (!Any(i => i.get_FileNames(1).Is(file))) {
                base.Enqueue(projectItem);
            }
        }
    }
}
