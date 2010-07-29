using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Zippy.Chirp.Manager;

namespace Zippy.Chirp.Engines
{
    abstract class Engine
    {
        private static Engine[] _All;

        public static Engine[] All
        {
            get
            {
                if (_All == null)
                    _All = typeof(Engine).Assembly.GetTypes()
                     .Where(x => typeof(Engine).IsAssignableFrom(x) && !x.IsAbstract)
                     .Select(x => (Engine)Activator.CreateInstance(x)).ToArray();
                return _All;
            }
        }

        public static bool IsHandled(string filename)
        {
            return _All.Any(x => x.IsEngineFor(filename));
        }

        public static void RunTransformations(DTE2 app, ProjectItem item)
        {
            using (var manager = new VSProjectItemManager(app, item))
            {
                string fullFileName = item.get_FileNames(1);
                var results = All.Where(x => x.IsEngineFor(fullFileName))
                    .SelectMany(x => x.Transform(app, item)).OrderBy(x => x.Priority);

                if (results != null && results.Any())
                {
                    foreach (var result in results)
                    {
                        result.Process(app, item, manager);
                    }
                }
            }

        }

        public abstract bool IsEngineFor(string filename);
        public abstract IEnumerable<IResult> Transform(DTE2 app, ProjectItem item);

    }

    abstract class Engine<T> : Engine where T : Engine<T>
    {
        public Engine()
        {
            if (_Instance == null)
                _Instance = (T)this;
        }

        private static T _Instance;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                    Console.WriteLine("Engines Found: {0}", All.Length);
                return _Instance;
            }
        }
    }

    abstract class BasicEngine<T> : Engine<T> where T : Engine<T>
    {
        public BasicEngine(string[] extentions, string[] ignoreExtensions)
        {
            Extensions = extentions;
            IgnoreExtensions = ignoreExtensions;
        }

        public override IEnumerable<IResult> Transform(DTE2 app, ProjectItem item)
        {
            string currentDir = Environment.CurrentDirectory;
            string fullFileName = item.get_FileNames(1);

            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(fullFileName);
            TaskList.Instance.Remove(fullFileName);

            try
            {
                string text = System.IO.File.ReadAllText(fullFileName);
                IEnumerable<IResult> results = new IResult[0];

                if (!string.IsNullOrEmpty(text))
                {

                    results = Transform(fullFileName, text);
                }
                CheckForConfigRefresh(app, item);
                return results;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Environment.CurrentDirectory = currentDir;
            }
        }

        private void CheckForConfigRefresh(DTE2 app, ProjectItem projectItem)
        {
            string fullFileName = projectItem.get_FileNames(1);
            var dependentFiles = ConfigEngine.Instance.dependentFiles;

            if (dependentFiles.ContainsKey(fullFileName))
            {
                foreach (string configFile in dependentFiles[fullFileName])
                {
                    ProjectItem item = app.LocateProjectItemForFileName(configFile);
                    if (item != null)
                    {
                        ConfigEngine.Instance.Transform(app, projectItem);
                    }
                }
            }

            foreach (ProjectItem projectItemInner in projectItem.ProjectItems)
            {
                CheckForConfigRefresh(app, projectItemInner);
            }
        }

        public abstract IEnumerable<IResult> Transform(Item item);
        public string[] Extensions { get; private set; }
        public string[] IgnoreExtensions { get; private set; }

        public override bool IsEngineFor(string filename)
        {
            return Extensions.Any(x => filename.EndsWith(x, StringComparison.InvariantCultureIgnoreCase))
                && (IgnoreExtensions == null || !IgnoreExtensions.Any(x => filename.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)));
        }

        public IEnumerable<IResult> Transform(string fullFileName, string text)
        {
            return Transform(new Item { FileName = fullFileName, Text = text, BaseFileName = GetBaseFileName(fullFileName) });
        }

        private static readonly string[] _DefaultExtensions = new[] { ".chirp.js", ".simple.js", ".whitespace.js", ".yui.js", ".gct.js", ".chirp.ascx", ".chirp.aspx", ".chirp.less", ".chirp.less.css", ".chirp.css", ".chirp.config" };
        protected string GetBaseFileName(string basefile)
        {
            var fileExt = Extensions.Union(_DefaultExtensions).Where(x => basefile.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.Length).FirstOrDefault()
                ?? System.IO.Path.GetExtension(basefile);

            if (!string.IsNullOrEmpty(fileExt))
            {
                basefile = basefile.Substring(0, basefile.Length - fileExt.Length);
            }

            return basefile;
        }
    }
}
