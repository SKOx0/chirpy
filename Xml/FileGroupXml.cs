using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Zippy.Chirp.Xml {
    public class FileGroupXml {
        public string Name { get; set; }
        public string Path { get; set; }
        public IList<FileXml> Files { get; set; }
        public MinifyMode Minify { get; set; }

        public enum MinifyMode {
            True, False, Both
        }

        public FileGroupXml(XElement xElement) : this(xElement, string.Empty) { }
        public FileGroupXml(XElement xElement, string basePath) {
            var name = xElement.Attribute("Name");

            if (name == null) {
                throw new Exception("Name attribute required on FileGroup element");
            }

            Name = name.Value;
            Path = System.IO.Path.Combine(basePath, Name);

            var files = xElement.Descendants("File")
                .Select(n => new FileXml(n, basePath));

            var folderFiles = xElement.Descendants("Folder")
                .Select(n => new FolderXml(n, basePath))
                .SelectMany(n => n.FileXmlList);

            Files = files.Union(folderFiles).ToList();

            Minify = ((string)xElement.Attribute("Minify")).ToEnum(Files.Any(x => x.Minify != null) ? MinifyMode.False : MinifyMode.Both);
            if (Minify == MinifyMode.True) {
                foreach (var file in Files) {
                    file.Minify = true;
                }

            } else if (Minify == MinifyMode.Both) {
                foreach (var file in Files) {
                    file.Minify = null;
                }
            }
        }
    }
}
