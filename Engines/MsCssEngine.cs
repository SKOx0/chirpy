
using Microsoft.Ajax.Utilities;
namespace Zippy.Chirp.Engines {
    class MsCssEngine : BasicEngine<MsCssEngine> {
        public MsCssEngine() : base(new[] { Settings.ChirpMSAjaxCssFile }, new[] { ".min.css" }) { }

        public override System.Collections.Generic.IEnumerable<IResult> BasicTransform(Item item) {
            Minifier minifier = new Minifier();
            string miniCss = minifier.MinifyStyleSheet(item.Text);

            yield return new FileResult(item, ".min.css", miniCss, true);

            foreach (var err in minifier.Errors) {
                yield return new ErrorResult(item.FileName, err, 1, 1);
            }
        }
    }
}
