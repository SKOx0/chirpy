using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Zippy.Chirp.Xml
{
    public class FolderXml
    {
        public string Pattern { get; set; }
		public bool Deep { get; set; }
        public bool? Minify { get; set; }
		public MinifyType MinifyWith { get; set; }
        public IList<FileXml> FileXmlList { get; set; }

        public FolderXml(XElement xElement) : this(xElement, string.Empty) { }
        public FolderXml(XElement xElement, string basePath)
        {
			this.Pattern = (string)xElement.Attribute("Pattern");
            if (this.Pattern == null)
            {
                throw new Exception("Pattern attribute required on Folder element");
            }
			this.Minify = ((string)xElement.Attribute("Minify")).TryToBool();
			this.Deep = ((string)xElement.Attribute("Deep")).ToBool(true);
			this.MinifyWith = ((string)xElement.Attribute("MinifyWith")).ToEnum(MinifyType.Unspecified);
			this.FileXmlList = new List<FileXml>();

			var folderSeperators = new char[]{'/','\\'};
			if (this.Pattern.StartsWith(".."))
			{
				var combinedPath = Path.Combine(basePath, this.Pattern);
				var baseDirectory = Path.GetDirectoryName(combinedPath);
				var resolvedPath = new Uri(baseDirectory).AbsolutePath;
				basePath = resolvedPath;
				//basePath is rooted so remove directories from pattern
				this.Pattern = Regex.Replace(this.Pattern, @"^.+[\\/]", "");
			}
			if (folderSeperators.Contains(this.Pattern.First())
					&& !Regex.IsMatch(this.Pattern,@"^\\{2}")
				)
			{
				// If the pattern starts with a folder separator and not two backslashes (network path)
				// root basePath. 
				// Makes it forgiving of patterns in the form of /folder/*.js or \folder/*.js  or /*.js etcetera.
				var rootFolder = Path.GetDirectoryName(this.Pattern);
				rootFolder = rootFolder.Substring(1, rootFolder.Length - 1);
				basePath = Path.Combine(basePath, rootFolder);
				this.Pattern = Regex.Replace(this.Pattern, @"^.+[\\/]", "");
			}
			else if (this.Pattern.Any(e => folderSeperators.Contains(e)))
			{
				try
				{
					var uri = new Uri(this.Pattern);
					basePath = Path.GetDirectoryName(uri.LocalPath);
					this.Pattern = Regex.Replace(this.Pattern, @"^.+[\\/]", "");
				}
				catch { }
			}
			
			var searchOption = (this.Deep) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var filePaths = Directory.GetFiles(basePath, this.Pattern, searchOption);

            foreach (string filePath in filePaths)
            {
                FileXmlList.Add(new FileXml
                {
                    Minify = this.Minify,
                    Path = filePath,
					MinifyWith = this.MinifyWith
                });
            }
        }
    }
}
