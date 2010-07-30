using System.Collections.Generic;
using System.Linq;

namespace Zippy.Chirp.Engines {
    class ClosureCompilerEngine : BasicEngine<ClosureCompilerEngine> {
        public ClosureCompilerEngine() : base(new[] { Settings.ChirpGctJsFile, Settings.ChirpWhiteSpaceJsFile, Settings.ChirpSimpleJsFile }, new[] { ".min.js" }) { }

        public override IEnumerable<IResult> BasicTransform(Item item) {
            var mode = item.FileName.EndsWith(Settings.ChirpGctJsFile, System.StringComparison.OrdinalIgnoreCase) ? ClosureCompilerCompressMode.ADVANCED_OPTIMIZATIONS
                : item.FileName.EndsWith(Settings.ChirpSimpleJsFile, System.StringComparison.OrdinalIgnoreCase) ? ClosureCompilerCompressMode.SIMPLE_OPTIMIZATIONS
                : ClosureCompilerCompressMode.WHITESPACE_ONLY;

            var reporter = new EcmaScriptErrorReporter();

            string returnedCode = null;

            try {
                returnedCode = GoogleClosureCompiler.Compress(item.FileName, mode, (category, msg, line, col) => {
                    reporter.Error(msg, string.Empty, line, string.Empty, col);
                });
            } catch (System.Exception) { }

            if (reporter.Errors.Any()) foreach (var err in reporter.Errors) yield return err;
            else yield return new FileResult(item, ".min.js", returnedCode, true);
        }
    }
}
