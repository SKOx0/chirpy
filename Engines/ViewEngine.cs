using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zippy.Chirp.Engines {
    class ViewEngine : BasicEngine<ViewEngine> {
        public ViewEngine() : base(new[] { Settings.ChirpViewFile, Settings.ChirpPartialViewFile }, null) { }
        static Regex rxScripts = new Regex(@"\<(style|script)([^>]*)\>(.*?)\</\1\>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public override IEnumerable<IResult> Transform(Item item) {
            var tags = rxScripts.Matches(item.Text).Cast<Match>().Reverse();

            var text = item.Text;

            foreach (var match in tags) {
                var tagName = match.Groups[1].Value;
                var attrs = match.Groups[2].Value;
                var code = match.Groups[3].Value;
                var subitem = new Item { FileName = item.FileName, BaseFileName = item.BaseFileName, Text = code };
                IEnumerable<IResult> subresult = null;

                if (tagName.Is("script")) {
                    subresult = YuiJsEngine.Instance.Transform(subitem);

                } else if (tagName.Is("style")) {
                    int i = attrs.IndexOf("text/less", StringComparison.InvariantCultureIgnoreCase);
                    if (i > -1) {
                        attrs = attrs.Substring(0, i) + "text/css" + attrs.Substring(i + "text/less".Length);
                        subresult = LessEngine.Instance.Transform(subitem);
                    } else {
                        subresult = YuiCssEngine.Instance.Transform(subitem);
                    }
                }

                if (subresult != null) {
                    code = subresult.OfType<FileResult>().Where(x => x.Minified).Select(x => x.Text).FirstOrDefault();
                    foreach (var err in subresult.OfType<ErrorResult>()) yield return err;
                }

                text = text.Substring(0, match.Index)
                    + '<' + tagName + attrs + '>' + code + "</" + tagName + '>'
                    + text.Substring(match.Index + match.Length);
            }

            var ext = item.FileName.EndsWith(Settings.ChirpViewFile, StringComparison.OrdinalIgnoreCase) ? ".aspx" : ".ascx";
            yield return new FileResult(item, ext, text, true);
        }

    }
}
