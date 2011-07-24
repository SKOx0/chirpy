using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Zippy.Chirp
{
    public static class Utilities
    {
        private const char ENUM_SEPERATOR_CHARACTER = ',';
        private static Dictionary<Enum, string> descriptions = new Dictionary<Enum, string>();
        
        public static void Dispose<T>(ref T obj) where T : class, IDisposable
        {
            try
            {
                if (obj != null) 
                {
                    obj.Dispose(); 
                }
            }
            catch
            { 
            }

            obj = null;
        }

        public static void Clear(this System.Text.StringBuilder str) {
            if (str.Length == 0) return;
            str.Remove(0, str.Length);
        }

        /// <summary>
        /// Looks for the <see cref="System.ComponentModel.DescriptionAttribute">DescriptionAttribute</see> on an
        /// enum, and returns the value.
        /// </summary>
        /// <param name="e">enum type</param>
        /// <returns>Description attribute</returns>    
        public static string Description(this Enum e)
        {
            lock (Utilities.descriptions)
            {
                if (!Utilities.descriptions.ContainsKey(e))
                {
                    var entries = e.ToString().Split(ENUM_SEPERATOR_CHARACTER);
                    string[] desc = new string[entries.Length];
                    Type type = e.GetType();
                    for (int i = 0; i <= entries.Length - 1; i++)
                    {
                        var fieldinfo = type.GetField(entries[i].Trim());
                        if (fieldinfo != null)
                        {
                            var attr = fieldinfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                                .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
                            desc[i] = attr != null ? attr.Description : entries[i].Trim().Replace("_", " ");
                        }
                        else
                        {
                            desc[i] = entries[i].Trim().Replace("_", " ");
                        }
                    }

                    Utilities.descriptions.Add(e, string.Join(ENUM_SEPERATOR_CHARACTER.ToString(), desc));
                }

                return Utilities.descriptions[e];
            }
        }

        public static ProjectItem LocateProjectItemForFileName(this DTE2 app, string fileName)
        {
            foreach (Project project in app.Solution.Projects)
            {
                foreach (ProjectItem projectItem in project.ProjectItems.ProcessFolderProjectItemsRecursively())
                {
                    if (projectItem.get_FileNames(1) == fileName)
                    {
                        return projectItem; 
                    }
                }
            }

            return null;
        }

        public static bool IsFolder(this ProjectItem item)
        {
            return item.Kind == Constants.vsProjectItemKindPhysicalFolder;
        }

        public static bool IsSolutionFolder(this ProjectItem item)
        {
            return item.SubProject != null;
        }

        public static IEnumerable<ProjectItem> ProcessFolderProjectItemsRecursively(this ProjectItems projectItems)
        {
            if (projectItems != null)
            {
                foreach (ProjectItem projectItem in projectItems)
                {
                    if (projectItem.IsFolder() && projectItem.ProjectItems != null)
                    {
                        foreach (ProjectItem folderProjectItem in ProcessFolderProjectItemsRecursively(projectItem.ProjectItems))
                        {
                            yield return folderProjectItem;
                        }
                    }
                    else if (projectItem.IsSolutionFolder())
                    {
                        foreach (ProjectItem solutionProjectItem in ProcessFolderProjectItemsRecursively(projectItem.SubProject.ProjectItems))
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
        }

        public static IEnumerable<ProjectItem> GetAll(this ProjectItems projectItems)
        {
            foreach (ProjectItem projectItem in projectItems)
            {
                foreach (ProjectItem subItem in GetAll(projectItem.ProjectItems))
                {
                    yield return subItem;
                }

                yield return projectItem;
            }
        }

        public static ProjectItem GetParent(this ProjectItem projectItem)
        {
            if (projectItem.Collection == null)
            {
                return null;
            }
            else
            {
                return projectItem.Collection.Parent as ProjectItem;
            }
        }

        /// <summary>
        /// Returns "C:\fakepath\test" when given "C:\fakepath\test.js"
        /// </summary>
        /// <param name="fullFileName">full file name</param>
        /// <param name="extensions">file extension</param>
        /// <returns>Base filename "C:\fakepath\test"</returns>
        public static string GetBaseFileName(string fullFileName, params string[] extensions)
        {
            if (Settings.AllExtensions == null)
            {
                Settings.Load(); 
            }

            extensions = extensions == null ? Settings.AllExtensions : extensions.Union(Settings.AllExtensions).ToArray();

            var fileExt = extensions.Where(x => fullFileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.Length).FirstOrDefault()
                ?? System.IO.Path.GetExtension(fullFileName);

            if (!string.IsNullOrEmpty(fileExt))
            {
                fullFileName = fullFileName.Substring(0, fullFileName.Length - fileExt.Length);
            }

            return fullFileName;
        }
    }
}
