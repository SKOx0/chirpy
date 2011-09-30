using System;
using Yahoo.Yui.Compressor;
using Zippy.Chirp.Xml;

namespace Zippy.Chirp.Engines
{
    public class YuiCssEngine : CssEngine
    {
        public YuiCssEngine()
        {
            Extensions = new[] { this.Settings.ChirpHybridCssFile, this.Settings.ChirpMichaelAshCssFile, this.Settings.ChirpCssFile };
            OutputExtension = this.Settings.OutputExtensionCSS;
        }

        public static string Minify(string text, MinifyType mode)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text; 
            }

            var cssmode = mode == MinifyType.yuiHybrid ? CssCompressionType.Hybrid
               : mode == MinifyType.yuiMARE ? CssCompressionType.MichaelAshRegexEnhancements
               : CssCompressionType.StockYuiCompressor;

            return CssCompressor.Compress(text, 0, cssmode,true);
        }

        public override string Transform(string fullFileName, string text, EnvDTE.ProjectItem projectItem)
        {
            this.Settings = Settings.Instance(fullFileName);
            var mode = fullFileName.EndsWith(this.Settings.ChirpHybridCssFile, StringComparison.InvariantCultureIgnoreCase) ? MinifyType.yuiHybrid
                : fullFileName.EndsWith(this.Settings.ChirpMichaelAshCssFile, StringComparison.InvariantCultureIgnoreCase) ? MinifyType.yuiMARE
                : MinifyType.yui;

            return Minify(text, mode);
        }
    }
}
