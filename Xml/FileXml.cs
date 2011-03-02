using System;
using System.Xml.Linq;

namespace Zippy.Chirp.Xml {
    public class FileXml {
        public string Path { get; set; }
        public bool? Minify { get; set; }
        public MinifyType MinifyWith { get; set; }

        public FileXml() { }
        public FileXml(XElement xElement) : this(xElement, string.Empty) { }
        public FileXml(XElement xElement, string basePath) {
            var path = (string)xElement.Attribute("Path");

            var minify = (string)xElement.Attribute("Minify");

            if (path == null) {
                throw new Exception("Path attribute required on File element");
            }

            Path = System.IO.Path.Combine(basePath, path);
            Minify = minify.TryToBool();

			this.MinifyWith = ((string)xElement.Attribute("MinifyWith")).ToEnum(MinifyType.Unspecified);			
		
        }
    }
}
