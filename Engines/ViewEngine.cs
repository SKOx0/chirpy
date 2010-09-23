using System;
using System.Linq;
using System.Text.RegularExpressions;


namespace Zippy.Chirp.Engines {
   public class ViewEngine : TransformEngine {
        public ViewEngine() {
            Extensions = new[] { Settings.ChirpViewFile, Settings.ChirpPartialViewFile };
            OutputExtension = Settings.ChirpViewFile;
        }

        public override string GetOutputExtension(string fullFileName) {
            if(fullFileName.EndsWith(Settings.ChirpViewFile, System.StringComparison.InvariantCultureIgnoreCase))
                return ".aspx";
            else return ".ascx";
        }

        public override int Handles(string fullFileName) {
            if(fullFileName.EndsWith(Settings.ChirpViewFile, System.StringComparison.InvariantCultureIgnoreCase)) return 1;
            else if(fullFileName.EndsWith(Settings.ChirpPartialViewFile, System.StringComparison.InvariantCultureIgnoreCase)) return 1;
            else return 0;
        }

        static Regex rxScripts = new Regex(@"\<(style|script)([^>]*)\>(.*?)\</\1\>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            var tags = rxScripts.Matches(text).Cast<Match>().Reverse();

            foreach(var match in tags) {
                var tagName = match.Groups[1].Value;
                var attrs = match.Groups[2].Value;
                var code = match.Groups[3].Value;

                if(tagName.Is("script")) {
                    code = JsEngine.Minify(fullFileName, code, projectItem, Xml.MinifyType.None);

                } else if(tagName.Is("style")) {
                    int i = attrs.IndexOf("text/less", StringComparison.InvariantCultureIgnoreCase);
                    if(i > -1) {
                        attrs = attrs.Substring(0, i) + "text/css" + attrs.Substring(i + "text/less".Length);
                        code = LessEngine.TransformToCss(fullFileName, code, projectItem);
                    }
                    code = CssEngine.Minify(fullFileName, code, projectItem, Xml.MinifyType.None);
                }

                text = text.Substring(0, match.Index)
                    + '<' + tagName + attrs + '>' + code + "</" + tagName + '>'
                    + text.Substring(match.Index + match.Length);
            }

            return text;
        }
    }
}
