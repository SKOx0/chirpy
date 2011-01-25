using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Zippy.Chirp.Xml {
    public class FolderXml {
        public string Path { get; set; }
        public string Pattern { get; set; }
        public bool Recursive { get; set; }
        public bool? Minify { get; set; }
        public MinifyType MinifyWith { get; set; }
        public IList<FileXml> FileXmlList { get; set; }

        public FolderXml(FileGroupXml group, XElement xElement) {
            Path = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(group.Path),
                ((string)xElement.Attribute("Path") ?? string.Empty).Replace('/', '\\').TrimStart('\\'));

            Pattern = (string)xElement.Attribute("Pattern") ?? ("*" + System.IO.Path.GetExtension(group.Name));
            Minify = ((string)xElement.Attribute("Minify")).TryToBool();
            Recursive = ((string)xElement.Attribute("Recursive")).ToBool(true);
            MinifyWith = ((string)xElement.Attribute("MinifyWith")).ToEnum(MinifyType.Unspecified);

            FileXmlList = new List<FileXml>();
            var searchOption = (this.Recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var filePaths = Directory.GetFiles(Path, this.Pattern, searchOption);

            foreach (string filePath in filePaths) {
                FileXmlList.Add(new FileXml {
                    Minify = this.Minify,
                    Path = filePath,
                    MinifyWith = this.MinifyWith
                });
            }
        }
    }
}
