using System;
using Yahoo.Yui.Compressor;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines {
    public class YuiCssEngine : CssEngine {
        public YuiCssEngine() {
            Extensions = new[] { Settings.ChirpHybridCssFile, Settings.ChirpMichaelAshCssFile, Settings.ChirpCssFile };
            OutputExtension = ".min.css";
        }

        public static string Minify(string text, MinifyType mode) {
            if (string.IsNullOrEmpty(text)) return text;
            var cssmode = mode == MinifyType.yuiHybird ? CssCompressionType.Hybrid
               : mode == MinifyType.yuiMARE ? CssCompressionType.MichaelAshRegexEnhancements
               : CssCompressionType.StockYuiCompressor;

            return CssCompressor.Compress(text, 0, cssmode);
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem) {
            var mode = fullFileName.EndsWith(Settings.ChirpHybridCssFile, StringComparison.InvariantCultureIgnoreCase) ? MinifyType.yuiHybird
                : fullFileName.EndsWith(Settings.ChirpMichaelAshCssFile, StringComparison.InvariantCultureIgnoreCase) ? MinifyType.yuiMARE
                : MinifyType.yui;

            return Minify(text, mode);
        }
    }
}
