
using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
namespace Zippy.Chirp.Engines {
    class T4Engine : Engine<T4Engine> {
        const string ControllerCSFile = ".cs";
        const string ControllerVBFile = ".vb";
        const string MVCViewFile = ".aspx";
        const string MVCPartialViewFile = ".ascx";
        const string MVCT4TemplateName = "T4MVC.tt";

        bool IsMVCStandardControllerFile(string fileName) {
            return (fileName.EndsWith(ControllerCSFile, StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(ControllerVBFile, StringComparison.OrdinalIgnoreCase)) &&
                                        fileName.Contains("Controller");
        }

        bool IsMVCStandardViewScriptOrContentFile(string fileName) {
            return ((fileName.EndsWith(MVCViewFile, StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(MVCPartialViewFile, StringComparison.OrdinalIgnoreCase)) &&
                                        fileName.Contains("Views")) || fileName.Contains("Scripts") || fileName.Contains("Content");
        }

        public override bool IsEngineFor(string filename) {
            return Settings.SmartRunT4MVC
                && (IsMVCStandardViewScriptOrContentFile(filename) || IsMVCStandardControllerFile(filename));
        }

        public override IEnumerable<IResult> Transform(Item item) {
            yield return new T4Result();
        }

        private class T4Result : IResult {
            public int Priority { get { return 1; } }
            public void Process(DTE2 app, ProjectItem item, Manager.VSProjectItemManager manager) {
                RunT4Template(app, MVCT4TemplateName);
            }
        }

        public static void RunT4Template(DTE2 app, string t4TemplateList) {
            string[] T4List = t4TemplateList.Split(new char[] { ',' });
            foreach (string t4Template in T4List) {
                ProjectItem projectItem = app.Solution.FindProjectItem(t4Template.Trim());

                if (projectItem != null) {
                    if (!projectItem.IsOpen)
                        projectItem.Open();
                    projectItem.Save();
                }
            }
        }
    }
}
