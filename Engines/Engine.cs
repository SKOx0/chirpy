using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
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
                case MinifyType.gstWhiteSpaceOnly:
                    return ClosureCompilerEngine.Minify(fullFileName, outputText, projectItem,ClosureCompilerCompressMode.WHITESPACE_ONLY);
                case MinifyType.msAjax:
                    return MsJsEngine.Minify(fullFileName, outputText, projectItem);
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
                case MinifyType.yuiHybird:
                default:
                    return YuiCssEngine.Minify(outputText,mode);
            }
        }
    }

    /// <summary>
    /// Performs some action on a ProjectItem
    /// </summary>
    public abstract class ActionEngine {
        /// <summary>
        /// Determines whether this action hands the specified file.  Returns an int to specify the priority--0 being not handled.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public abstract int Handles(string fullFileName);
        public abstract void Run(string fullFileName, ProjectItem projectItem);
        internal DTE2 _app;
    }

    /// <summary>
    /// Transforms the contents of a file
    /// </summary>
    public abstract class TransformEngine : ActionEngine {
        public string OutputExtension { get; set; }
        public string[] Extensions { get; set; }

        public abstract string Transform(string fullFileName, string text, ProjectItem projectItem);

        public override int Handles(string fullFileName) {
            if (fullFileName.EndsWith(OutputExtension, StringComparison.InvariantCultureIgnoreCase)) return 0;
            var match = Extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault() ?? string.Empty;
            return match.Length;
        }

        public override void Run(string fullFileName, ProjectItem projectItem) {
            using (var manager = new VSProjectItemManager(_app, projectItem))
            using (new EnvironmentDirectory(fullFileName)) {
                string baseFileName = Utilities.GetBaseFileName(fullFileName, Extensions);
                string inputText = System.IO.File.ReadAllText(fullFileName);
                string outputText = Transform(fullFileName, inputText, projectItem);
                Process(manager, fullFileName, projectItem, baseFileName, outputText);
            }

            Chirp.ConfigEngine.CheckForConfigRefresh(projectItem);
        }

        public virtual void Process(VSProjectItemManager manager, string fullFileName, ProjectItem projectItem, string baseFileName, string outputText) {
            manager.AddFileByFileName(baseFileName + OutputExtension, outputText);
        }
    }

    public class EngineManager : IDisposable {
        private Queue<ProjectItem> _queue = new Queue<ProjectItem>();
        private System.Threading.Thread _process;
        private System.Threading.AutoResetEvent _are = new System.Threading.AutoResetEvent(false);

        private List<string> _queued = new List<string>();
        private List<ActionEngine> _allactions = new List<ActionEngine>();
        private TransformEngine[] _transformers = new TransformEngine[0];
        private ActionEngine[] _actions = new ActionEngine[0];
        private Chirp _Chirp;

        public bool IsHandled(string fullFileName) {
            return _actions.Any(x => x.Handles(fullFileName) > 0);
        }

        public bool IsTransformed(string fullFileName) {
            return _transformers.Any(x => x.Handles(fullFileName) > 0);
        }

        public EngineManager(Chirp chirp) {
            _Chirp = chirp;
            _process = new System.Threading.Thread(() => {
                while (true) {
                    string fullFileName = null;
                    ProjectItem projectItem = null;

                    try {
                        if (!_queue.Any()) _are.WaitOne();
                        if (!_queue.Any()) continue;
                        projectItem = _queue.Dequeue();

                        var parent = projectItem.GetParent();
                        if (parent != null && !parent.IsFolder() && IsTransformed(parent.get_FileNames(1))) return;

                        fullFileName = projectItem.get_FileNames(1);
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
                            action.Run(fullFileName, projectItem);
                        }

                    } catch (System.Threading.ThreadAbortException) {
                    } catch (Exception ex) {
                        if (projectItem != null)
                            TaskList.Instance.Add(projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error, fullFileName, 1, 1, ex.ToString());
                        else _Chirp.outputWindowPane.OutputString(ex.ToString() + Environment.NewLine);
                    } finally {
                        if (fullFileName != null)
                            _queued.Remove(fullFileName);
                    }
                }
            });
            _process.Start();
        }

        //Add a new ProjectItem processor
        public void Add(ActionEngine action) {
            action._app = _Chirp.app;
            _allactions.Add(action);
            _actions = _allactions.ToArray();
            _transformers = _allactions.OfType<TransformEngine>().ToArray();
        }

        //Remove all actions
        public void Clear() {
            _allactions.Clear();
            _actions = new ActionEngine[0];
            _transformers = new TransformEngine[0];
        }

        /// <summary>
        /// Schedule a ProjectItem to be processed
        /// </summary>
        /// <param name="projectItem"></param>
        public void Enqueue(ProjectItem projectItem) {
            string fullFileName = projectItem.get_FileNames(1);
            if (_queued.Contains(fullFileName, StringComparer.InvariantCultureIgnoreCase))
                return;
            _queued.Add(fullFileName);
            _queue.Enqueue(projectItem);
            _are.Set();
        }

        /// <summary>
        /// Abort the thread
        /// </summary>
        public void Dispose() {
            _process.Abort();
            _process = null;
            _allactions.Clear();
            _allactions = null;
            _actions = null;
            _transformers = null;
            _Chirp = null;
        }
    }
}
