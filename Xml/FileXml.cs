using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Zippy.Chirp.Xml
{
    public class FileXml
    {
        public string Path { get; set; }
        public bool Minify { get; set; }

        public FileXml() { }
        public FileXml(XElement xElement) : this(xElement, string.Empty) { }
        public FileXml(XElement xElement, string basePath)
        {
            var path = xElement.Attribute("Path");
            var type = xElement.Attribute("Type");
            var minify = xElement.Attribute("Minify");

            if (path == null)
            {
                throw new Exception("Path attribute required on File element");
            }

            Path = System.IO.Path.Combine(basePath, path.Value);
            Minify = (minify == null) ? true : bool.Parse(minify.Value);
        }
    }
}
