using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;

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

        public static bool ToBool(this string input, bool defaultValue) {
            bool result;
            if (bool.TryParse(input, out result))
                return result;
            else return defaultValue;
        }

        public static bool Contains(this string input, string other, StringComparison comparison) {
            return (input ?? string.Empty).IndexOf(other, comparison) > -1;
        }

        public static ProjectItem LocateProjectItemForFileName(this DTE2 app, string fileName)
        {
            foreach (Project project in app.Solution.Projects)
            {
                foreach (ProjectItem projectItem in project.ProjectItems.ProcessFolderProjectItemsRecursively())
                {
                    if (projectItem.get_FileNames(1) == fileName)
                        return projectItem;
                }

            }
            return null;
        }

        public static bool IsFolder(this ProjectItem item) {
            return item.Kind == Constants.vsProjectItemKindPhysicalFolder;
        }

        public static bool IsSolutionFolder(this ProjectItem item)
        {
            return item.SubProject != null;
        }

        public static IEnumerable<ProjectItem> ProcessFolderProjectItemsRecursively(this ProjectItems projectItems)
        {
            foreach (ProjectItem projectItem in projectItems)
            {
                if (projectItem.IsFolder())
                {
                    foreach (ProjectItem folderProjectItem in ProcessFolderProjectItemsRecursively(projectItem.ProjectItems))
                    {
                        yield return folderProjectItem;
                    }
                }
                else if (projectItem.IsSolutionFolder())
                {
                    foreach(ProjectItem solutionProjectItem in ProcessFolderProjectItemsRecursively(projectItem.SubProject.ProjectItems))
                    {
                        yield return solutionProjectItem;
                    }
                }
                else
                {
                    yield return projectItem;
                }
            }
        }

        public static IEnumerable<ProjectItem> GetAll(this  ProjectItems projectItems) {
            foreach (ProjectItem projectItem in projectItems) {
                foreach (ProjectItem subItem in GetAll(projectItem.ProjectItems))
                {
                    yield return subItem;
                }

                yield return projectItem;
            }
        }

        public static ProjectItem GetParent(this ProjectItem projectItem) {
            return projectItem.Collection.Parent as ProjectItem;
            //var all = projectItem.ContainingProject.ProjectItems.GetAll();
            //var parents = all.Where(x => x.ProjectItems.Cast<ProjectItem>().Contains(projectItem)).ToArray();
            //return parents.FirstOrDefault();
        }

        public static T ToEnum<T>(this string input, T defaultValue) where T : struct, IConvertible {
            var values = System.Enum.GetValues(typeof(T)).Cast<T>().Where(x => x.ToString().Is(input));
            if (!values.Any()) return defaultValue;
            else return values.First();
        }

        public static string GetBaseFileName(string fullFileName, params string[] extensions) {
            extensions = extensions == null ? Settings.AllExtensions : extensions.Union(Settings.AllExtensions).ToArray();

            var fileExt = extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.Length).FirstOrDefault()
                ?? System.IO.Path.GetExtension(fullFileName);

            if (!string.IsNullOrEmpty(fileExt)) {
                fullFileName = fullFileName.Substring(0, fullFileName.Length - fileExt.Length);
            }

            return fullFileName;
        }

    }
}
