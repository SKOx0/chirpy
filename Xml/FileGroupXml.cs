using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Zippy.Chirp.Xml {
    public class FileGroupXml {
        public string Name { get; set; }
        public string Path { get; set; }
        public IList<FileXml> Files { get; set; }
        public MinifyType MinifyWith { get; set; }
        public MinifyOptions Minify { get; set; }
        public bool Debug { get; set; }

        public enum MinifyOptions {
            True, False, Both
        }

        public FileGroupXml(XElement xElement) : this(xElement, string.Empty) { }
        public FileGroupXml(XElement xElement, string basePath) {
            var name = xElement.Attribute("Name");

            if (name == null && xElement.Attribute("Path") == null) {
                throw new Exception("Name or path attribute required on FileGroup element");
            }

            if (name != null)
                this.Name = name.Value;

            if (xElement.Attribute("Path") != null)
                this.Path = System.IO.Path.Combine(basePath, xElement.Attribute("Path").Value);
            else
                this.Path = System.IO.Path.Combine(basePath, this.Name);

            var minify = (string)xElement.Attribute("Minify");
            var debug = (string)xElement.Attribute("Debug");

            this.Minify = minify.ToEnum(MinifyOptions.True);
            this.MinifyWith = ((string)xElement.Attribute("MinifyWith")).ToEnum(MinifyType.Unspecified);
            this.Debug = debug.ToBool(false);

            var fileDescriptors = xElement.Elements();
            var files = new List<FileXml>();
            foreach (var fileDescriptor in fileDescriptors) {
                if (fileDescriptor.Name.LocalName == "File") {
                    var file = new FileXml(fileDescriptor, basePath);
                    if (file.Minify == null)
                        file.Minify = this.Minify != MinifyOptions.False ? true : false; //this.Minify;
                    if (file.MinifyWith == MinifyType.Unspecified)
                        file.MinifyWith = this.MinifyWith;
                    files.Add(file);
                } else if (fileDescriptor.Name.LocalName == "Folder") {
                    var folder = new FolderXml(this, fileDescriptor);
                    if (folder.Minify == null) {
                        folder.Minify = this.Minify != MinifyOptions.False ? true : false; //this.Minify;
                        foreach (var f in folder.FileXmlList) {
                            f.Minify = this.Minify != MinifyOptions.False ? true : false; //this.Minify;
                        }
                    }
                    if (folder.MinifyWith == MinifyType.Unspecified) {
                        folder.MinifyWith = this.MinifyWith;
                        foreach (var f in folder.FileXmlList) {
                            f.MinifyWith = this.MinifyWith;
                        }
                    }
                    files.AddRange(folder.FileXmlList);
                }
            }
            this.Files = files;

        }
        public string GetName() {
            return this.Path ?? this.Name;
        }
    }
}
