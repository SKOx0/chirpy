using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace Zippy.Chirp.Xml
{
    public class FolderXml
    {
        public string Pattern { get; set; }
		public bool Recursive { get; set; }
        public bool? Minify { get; set; }
		public MinifyType MinifyWith { get; set; }
        public IList<FileXml> FileXmlList { get; set; }

        public FolderXml(XElement xElement) : this(xElement, string.Empty) { }
        public FolderXml(XElement xElement, string basePath)
        {
			this.Pattern = (string)xElement.Attribute("Pattern");
            this.Minify = ((string)xElement.Attribute("Minify")).TryToBool();
			this.Recursive = ((string)xElement.Attribute("Recursive")).ToBool(true);
			this.MinifyWith = ((string)xElement.Attribute("MinifyWith")).ToEnum(MinifyType.Unspecified);
			if (this.Pattern == null)
            {
                throw new Exception("Path attribute required on Folder element");
            }

            var path = basePath;

            FileXmlList = new List<FileXml>();
			var searchOption = (this.Recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var filePaths = Directory.GetFiles(path, this.Pattern, searchOption);

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
