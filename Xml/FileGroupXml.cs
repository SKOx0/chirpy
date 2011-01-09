using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web;

namespace Zippy.Chirp.Xml
{
    public class FileGroupXml
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public IList<FileXml> Files { get; set; }
        public MinifyType MinifyWith { get; set; }
        public bool Minify { get; set; }
		public bool Debug { get; set; }

        public FileGroupXml(XElement xElement) : this(xElement, string.Empty) { }
        public FileGroupXml(XElement xElement, string basePath)
        {
            var name = xElement.Attribute("Name");

            if (name == null && xElement.Attribute("Path") == null)
            {
                throw new Exception("Name or path attribute required on FileGroup element");
            }

            if (name != null)
                Name = name.Value;

            if (xElement.Attribute("Path") != null)
                Path = System.IO.Path.Combine(basePath, xElement.Attribute("Path").Value);
            else
                Path = System.IO.Path.Combine(basePath, Name);

			var fileDescriptors = xElement.XPathSelectElements(@"*[name() = 'File' or name() = 'Folder']");
			var files = new List<FileXml>();
			foreach (var fileDescriptor in fileDescriptors)
			{
				if (fileDescriptor.Name.LocalName == "File")
					files.Add(new FileXml(fileDescriptor,basePath));
				if (fileDescriptor.Name.LocalName == "Folder")
					files.AddRange(new FolderXml(fileDescriptor, basePath).FileXmlList);
			}
			this.Files = files;
            var minify = (string)xElement.Attribute("Minify");
            this.Minify = minify.ToBool(true);

			MinifyWith = ((string)xElement.Attribute("Minify")).ToEnum(MinifyType.yui);

			var debug = (string)xElement.Attribute("Debug");
			this.Debug = debug.ToBool(false);
        }
		public string GetName()
		{
			return this.Path ?? this.Name;
		}
    }
}
