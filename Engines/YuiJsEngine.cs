using System.Collections.Generic;
using System.Linq;
using Yahoo.Yui.Compressor;

namespace Zippy.Chirp.Engines {

    class YuiJsEngine : BasicEngine<YuiJsEngine> {
        public YuiJsEngine() : base(new[] { Settings.ChirpYUIJsFile, Settings.ChirpJsFile }, new[] { ".min.js" }) { }

        public override IEnumerable<IResult> BasicTransform(Item item) {
            var reporter = new EcmaScriptErrorReporter();

            string text = null;
            try {
                var compressor = new JavaScriptCompressor(item.Text, true, System.Text.Encoding.Default, System.Globalization.CultureInfo.CurrentCulture, false, reporter);
                text = compressor.Compress();
            } catch (System.Exception) { }

            if (reporter.Errors.Any()) foreach (var err in reporter.Errors) yield return err;
            else yield return new FileResult(item, ".min.js", text, true);
        }
    }

}
