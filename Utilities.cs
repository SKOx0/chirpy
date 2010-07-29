using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Zippy.Chirp
{
    public static class Utilities
    {
        public static bool Is(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int ToInt(this string input, int defaultValue)
        {
            int result;
            if (int.TryParse(input, out result))
                return result;
            else return defaultValue;
        }

        public static bool Contains(this string input, string other, StringComparison comparison)
        {
            return (input ?? string.Empty).IndexOf(other, comparison) > -1;
        }

        public static ProjectItem GetParent(this ProjectItem item)
        {
            var all = item.ContainingProject.ProjectItems.ProcessAllProjectItemsRecursively();
            return all.FirstOrDefault(x => !x.IsFolder() && x.ProjectItems.Cast<ProjectItem>().Contains(item));
        }

        public static ProjectItem LocateProjectItemForFileName(this DTE2 app, string fileName)
        {
            ProjectItem item = null;
            foreach (Project project in app.Solution.Projects)
            {
                item = project.ProjectItems.ProcessAllProjectItemsRecursively()
                    .FirstOrDefault(x => x.get_FileNames(1).Is(fileName));
                if (item != null) break;
            }
            return item;
        }

        public static bool IsFolder(this ProjectItem item)
        {
            return item.Kind == Constants.vsProjectItemKindPhysicalFolder;
        }

        public static IEnumerable<ProjectItem> ProcessAllProjectItemsRecursively(this  ProjectItems projectItems)
        {
            foreach (ProjectItem projectItem in projectItems)
            {
                if (projectItem.IsFolder())
                {
                    foreach (ProjectItem folderProjectItem in ProcessAllProjectItemsRecursively(projectItem.ProjectItems))
                        yield return folderProjectItem;
                }

                yield return projectItem;
            }
        }
    }
}
