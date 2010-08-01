using System;
using System.Collections.Generic;
using Yahoo.Yui.Compressor;

namespace Zippy.Chirp.Engines {
    class YuiCssEngine : BasicEngine<YuiCssEngine> {
        public YuiCssEngine()
            : base(new[] { Settings.ChirpMichaelAshCssFile, Settings.ChirpHybridCssFile, Settings.ChirpCssFile } /*list of extensions it handles*/,
                new[] { ".min.css" } /*list of extensions to ignore (prevent infinite recursion)*/) { }


        bool IsChirpCssFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpCssFile, StringComparison.OrdinalIgnoreCase));
        }
        private bool IsChirpHybridCssFile(string fileName) {
            return (fileName.EndsWith(Settings.ChirpHybridCssFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpMichaelAshCssFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpMichaelAshCssFile, StringComparison.OrdinalIgnoreCase));
        }

        public string Compress(string text, CssCompressionType mode) {
            return CssCompressor.Compress(text, 0, mode);
        }

        public override IEnumerable<IResult> BasicTransform(Item item) {
            var mode = IsChirpMichaelAshCssFile(item.FileName) ? CssCompressionType.MichaelAshRegexEnhancements
                : IsChirpHybridCssFile(item.FileName) ? CssCompressionType.Hybrid
                : CssCompressionType.StockYuiCompressor;

            yield return new FileResult(item, ".min.css", Compress(item.Text, mode), true);
        }
    }

}
