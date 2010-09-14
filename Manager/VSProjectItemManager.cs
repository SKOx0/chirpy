using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Zippy.Chirp.Manager {
    public class VSProjectItemManager : IDisposable {
        #region Members
        private DTE2 _dte;
        private ProjectItem _projectItem;
        private Dictionary<String, String> _filesAdded;	// FullFileName / Content
        private List<String> _filesCreated;
        private string _fullFileNamePrefix;
        #endregion

        #region Constructors
        internal VSProjectItemManager(DTE2 dte, ProjectItem projectItem)
            : this(dte, projectItem, null) {
        }
        internal VSProjectItemManager(DTE2 dte, ProjectItem projectItem, string fileNamePrefix) {
            if(dte == null) {
                throw new ArgumentNullException("dte");
            }
            if(projectItem == null) {
                throw new ArgumentNullException("projectItem");
            }

            _dte = dte;
            _projectItem = projectItem;

            _filesAdded = new Dictionary<String, String>();
            _filesCreated = new List<string>();

            if(String.IsNullOrEmpty(fileNamePrefix)) {
                fileNamePrefix = GetFileNamePrefix(projectItem.Name);
            }
            _fullFileNamePrefix = Path.GetDirectoryName(projectItem.get_FileNames(0)) + @"\" + fileNamePrefix;
        }
        #endregion

        #region Methods
        protected virtual String GetFileNamePrefix(string fileName) {
            string prefix = Path.GetFileNameWithoutExtension(fileName);

            if(prefix.Length < 1) {
                throw new Exception("Cannot get filename prefix");
            }

            return prefix;
        }
        public void AddFile(String fileExtension, String content) {
            string fullFileName = _fullFileNamePrefix + fileExtension;
            _filesAdded.Add(fullFileName, content);
        }

        public void AddFileByFileName(String fileName, String content) {
            _filesAdded.Add(fileName, content);
        }

        public void Process() {
            if(_filesAdded.Count < 1) {
                return;
            }

            // Remove unused items
            if(_projectItem.ProjectItems != null) {
                foreach(ProjectItem item in _projectItem.ProjectItems) {
                    if(!_filesAdded.ContainsKey(item.get_FileNames(0))) {
                        item.Delete();
                    }
                }
            }

            // Create Files
            foreach(var file in _filesAdded) {
                string fullFileName = file.Key;

                if(!File.Exists(fullFileName)) {
                    // File doesnt exists

                    _filesCreated.Add(fullFileName);
                    CheckoutFileIfRequired(fullFileName);
                    File.WriteAllText(fullFileName, file.Value);
                } else {
                    // File exists - find out if it is added to the projectItem
                    if(_projectItem.ProjectItems != null) {
                        if(ContainsItem(_projectItem.ProjectItems, fullFileName)) {
                            if(File.ReadAllText(fullFileName) != file.Value) {
                                // Content was different
                                CheckoutFileIfRequired(fullFileName);
                                File.WriteAllText(fullFileName, file.Value);
                            } else {
                                // File is already added to the projectItem
                                _filesCreated.Add(fullFileName);
                            }
                        } else {
                            // File exists but is not added to the projectItem
                            // For a security reason dont overwrite - instead let the user know
                            // MessageBox.Show("Was not able to create file: " + fullFileName + "\nA file with the same name already exists.");
                            TaskList.Instance.Add(_projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning, fullFileName, 1, 1, "Was not able to create file: " + fullFileName + "\nA file with the same name already exists.");
                        }
                    } else {
                        //visual studio 2010 (web site projet)
                        for(short i = 0; i <= _projectItem.FileCount; i++) {

                            fullFileName = _projectItem.FileNames[i];
                            // File is already added to the projectItem
                            _filesCreated.Add(fullFileName);

                            if(File.ReadAllText(fullFileName) != file.Value) {
                                // Content was different
                                CheckoutFileIfRequired(fullFileName);
                                //File.WriteAllText(fullFileName, file.Value);
                                File.WriteAllText(file.Key, file.Value);
                            } else {
                                // File exists but is not added to the projectItem
                                // For a security reason dont overwrite - instead let the user know
                                //MessageBox.Show("Was not able to create file: " + fullFileName + "\nA file with the same name already exists.");
                                TaskList.Instance.Add(_projectItem.ContainingProject, Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning, fullFileName, 1, 1, "Was not able to create file: " + fullFileName + "\nA file with the same name already exists.");
                            }
                        }
                    }
                }
            }

            // Add files created
            AddItems(_projectItem, _filesCreated);

            // Clear
            Clear();
        }
        public void Clear() {
            _filesAdded.Clear();
            _filesCreated.Clear();
        }

        private void CheckoutFileIfRequired(String fullFileName) {
            var sc = _dte.SourceControl;
            if(sc != null && sc.IsItemUnderSCC(fullFileName) && !sc.IsItemCheckedOut(fullFileName)) {
                _dte.SourceControl.CheckOutItem(fullFileName);
            }
        }

        // Static methods
        internal static bool ContainsItem(ProjectItems items, String fullFileNameOfItemContained) {
            if(items == null) {
                throw new ArgumentNullException("items");
            }
            if(String.IsNullOrEmpty(fullFileNameOfItemContained)) {
                throw new Exception("fullFileNameOfItemContained needs to contain a valid filename.");
            }

            foreach(ProjectItem item in items) {
                if(item.get_FileNames(0).Equals(fullFileNameOfItemContained)) {
                    return true;
                }
            }
            //valid is same folder
            string CurringItemDir = new FileInfo(((ProjectItem)items.Parent).get_FileNames(0)).DirectoryName;
            if(CurringItemDir != new FileInfo(fullFileNameOfItemContained).DirectoryName) {
                return true;
            }
            return false;
        }
        internal static ProjectItem GetItemByFullFileName(ProjectItems items, String fullFileNameOfItemToGet) {
            return GetItemByFullFileName(items, fullFileNameOfItemToGet, false);
        }
        internal static ProjectItem GetItemByFullFileName(ProjectItems items, String fullFileNameOfItemToGet, bool ignoreCase) {
            if(items == null) {
                throw new ArgumentNullException("items");
            }
            if(String.IsNullOrEmpty(fullFileNameOfItemToGet)) {
                throw new Exception("fullFileNameOfItemToGet needs to contain a valid filename.");
            }

            StringComparison strComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            foreach(ProjectItem item in items) {
                if(item.get_FileNames(0).Equals(fullFileNameOfItemToGet, strComparison)) {
                    return item;
                }
            }

            return null;
        }
        internal static void AddItem(ProjectItem projectItem, string fullFileNameToAdd) {
            if(projectItem == null) {
                throw new ArgumentNullException("projectItem");
            }
            if(String.IsNullOrEmpty(fullFileNameToAdd)) {
                throw new Exception("fullFileNameToAdd needs to contain a valid filename.");
            }

            // Er det et problem den tilføjer hvis den allerede er tilføjet ? 
            // Tjek om allerede tilføjet inden ? if (!ContainsItem(x)) .... ?
            if(projectItem.ProjectItems != null) {
                string CurringItemDir = projectItem.get_FileNames(0);
                CurringItemDir = new FileInfo(CurringItemDir).DirectoryName;
                if(CurringItemDir == new FileInfo(fullFileNameToAdd).DirectoryName)
                    projectItem.ProjectItems.AddFromFile(fullFileNameToAdd);

            }
        }
        internal static void AddItems(ProjectItem projectItem, IEnumerable<String> filesToAdd) {
            if(projectItem == null) {
                throw new ArgumentNullException("projectItem");
            }
            if(filesToAdd == null) {
                throw new ArgumentNullException("filesToAdd");
            }

            foreach(String fullFileNameToAdd in filesToAdd) {
                AddItem(projectItem, fullFileNameToAdd);
            }
        }
        internal static void DeleteAllItems(ProjectItems projectItems) {
            if(projectItems == null) {
                throw new ArgumentNullException("projectItems");
            }

            foreach(ProjectItem item in projectItems) {
                item.Delete();
            }
        }
        internal static void DeleteItems(ProjectItems projectItems, IEnumerable<String> itemsToKeep) {
            if(projectItems == null) {
                throw new ArgumentNullException("projectItem");
            }
            if(itemsToKeep == null) {
                throw new ArgumentNullException("itemsToKeep");
            }

            foreach(ProjectItem item in projectItems) {
                if(!itemsToKeep.Contains(item.get_FileNames(0))) {
                    item.Delete();
                }
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose() {
            Process();

            _dte = null;
            _projectItem = null;
            _filesAdded = null;
            _filesCreated = null;
            _fullFileNamePrefix = null;
        }
        #endregion
    }
}