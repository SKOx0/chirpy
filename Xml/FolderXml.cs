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
        public string Path { get; set; }
        public bool Minify { get; set; }
        public IList<FileXml> FileXmlList { get; set; }

        public FolderXml(XElement xElement) : this(xElement, string.Empty) { }
        public FolderXml(XElement xElement, string basePath)
        {
            var path = xElement.Attribute("Path");
            var type = xElement.Attribute("Type");
            var minify = xElement.Attribute("Minify");

            if (path == null)
            {
                throw new Exception("Path attribute required on Folder element");
            }

            if (type == null)
            {
                throw new Exception("Type attribute required on Folder element"); 
            }

            string relPath = System.IO.Path.GetDirectoryName(path.Value) ?? "";
            Path = System.IO.Path.Combine(basePath, relPath);

            Minify = (minify == null) ? true : bool.Parse(minify.Value);
            FileXmlList = new List<FileXml>();

            foreach (string filePath in Directory.GetFiles(Path, type.Value ?? ".js"))
            {
                FileXmlList.Add(new FileXml
                {
                    Minify = this.Minify,
                    Path = filePath 
                });
            }
        }
    }
}
