using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Zippy.Chirp {
    public static class Utilities {
        public static bool Is(this string a, string b) {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int ToInt(this string input, int defaultValue) {
            int result;
            if (int.TryParse(input, out result))
                return result;
            else return defaultValue;
        }

        public static bool Contains(this string input, string other, StringComparison comparison) {
            return (input ?? string.Empty).IndexOf(other, comparison) > -1;
        }

        public static ProjectItem LocateProjectItemForFileName(this DTE2 app, string fileName) {
            ProjectItem item = null;
            foreach (Project project in app.Solution.Projects) {
                item = project.ProjectItems.ProcessFolderProjectItemsRecursively()
                    .FirstOrDefault(x => x.get_FileNames(1).Is(fileName));
                if (item != null) break;
            }
            return item;
        }

        public static bool IsFolder(this ProjectItem item) {
            return item.Kind == Constants.vsProjectItemKindPhysicalFolder;
        }

        public static IEnumerable<ProjectItem> ProcessFolderProjectItemsRecursively(this  ProjectItems projectItems) {
            foreach (ProjectItem projectItem in projectItems) {
                if (projectItem.IsFolder()) {
                    foreach (ProjectItem folderProjectItem in ProcessFolderProjectItemsRecursively(projectItem.ProjectItems))
                        yield return folderProjectItem;
                }

                yield return projectItem;
            }
        }

        public static IEnumerable<ProjectItem> GetAll(this  ProjectItems projectItems) {
            foreach (ProjectItem projectItem in projectItems) {
                foreach (ProjectItem subItem in GetAll(projectItem.ProjectItems))
                    yield return subItem;

                yield return projectItem;
            }
        }

        public static ProjectItem GetParent(this ProjectItem projectItem) {
            var all = projectItem.ContainingProject.ProjectItems.GetAll();
            var parents = all.Where(x => x.ProjectItems.Cast<ProjectItem>().Contains(projectItem)).ToArray();
            return parents.FirstOrDefault();
        }

        public static bool IsAnyChirpFile(this ProjectItem projectItem) {
            return Engines.Engine.IsHandled(projectItem.get_FileNames(1));
        }

        public static T ToEnum<T>(this string input, T defaultValue) where T : struct, IConvertible {
            var values = System.Enum.GetValues(typeof(T)).Cast<T>().Where(x => x.ToString().Is(input));
            if (!values.Any()) return defaultValue;
            else return values.First();
        }
    }
}
