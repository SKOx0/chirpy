using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Zippy.Chirp.Xml
{
    public class FileGroupXml
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public IList<FileXml> Files { get; set; }
        public MinifyType MinifyWith { get; set; }
        public bool? Minify { get; set; }

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


            var files = xElement.Descendants("File")
                .Select(n => new FileXml(n, basePath));
            if (files.Count() == 0)
                files = xElement.Descendants(XName.Get("File", "urn:ChirpyConfig"))
                .Select(n => new FileXml(n, basePath));

            var folderFiles = xElement.Descendants("Folder")
                .Select(n => new FolderXml(n, basePath))
                .SelectMany(n => n.FileXmlList);
            if (folderFiles.Count() == 0)
                folderFiles = xElement.Descendants(XName.Get("Folder", "urn:ChirpyConfig"))
                 .Select(n => new FolderXml(n, basePath))
                 .SelectMany(n => n.FileXmlList);

            Files = files.Union(folderFiles).ToList();

            var minify = (string)xElement.Attribute("Minify");
            if (minify != null)
                Minify = minify.ToBool(false);
            else if (Files.Any(x => x.Minify == true)) //if any file is specifically marked to minify, then don't minify all
                Minify = false;

            MinifyWith = ((string)xElement.Attribute("Minify")).ToEnum(MinifyType.None);
        }
    }
}
