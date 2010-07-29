
using Microsoft.Ajax.Utilities;
namespace Zippy.Chirp.Engines
{
    class MsJsEngine : BasicEngine<MsJsEngine>
    {
        public MsJsEngine() : base(new[] { Settings.ChirpMSAjaxJsFile }, new[] { ".min.js" }) { }

        public override System.Collections.Generic.IEnumerable<IResult> Transform(Item item)
        {
            Minifier minifier = new Minifier();
            string mini = minifier.MinifyJavaScript(item.Text);
            yield return new FileResult(item, ".min.js", mini, true);

            foreach (var err in minifier.Errors)
            {
                yield return new ErrorResult(err, 1, 1);
            }
        }
    }
}
