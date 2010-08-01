using Microsoft.Ajax.Utilities;

namespace Zippy.Chirp.Engines {
    class MsJsEngine : BasicEngine<MsJsEngine> {
        public MsJsEngine() : base(new[] { Settings.ChirpMSAjaxJsFile }, new[] { ".min.js" }) { }

        public override System.Collections.Generic.IEnumerable<IResult> BasicTransform(Item item) {
            Minifier minifier = new Minifier();
            string mini = minifier.MinifyJavaScript(item.Text);
            yield return new FileResult(item, ".min.js", mini, true);

            foreach (var err in minifier.Errors) {
                int line=0;
                int column=0;
                //todo : use regex
                int IndexBegin = err.IndexOf("(");
                int IndexEnd = err.IndexOf(",");
                int.TryParse(err.Substring(IndexBegin+1, (IndexEnd - IndexBegin)-1), out line);

                IndexBegin = IndexEnd;
                IndexEnd = err.IndexOf("-");
                int.TryParse(err.Substring(IndexBegin + 1, (IndexEnd - IndexBegin) - 1), out column);

                yield return new ErrorResult(item.FileName, err, line, column);
            }
        }
    }
}
