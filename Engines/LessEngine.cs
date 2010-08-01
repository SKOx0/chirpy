using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yahoo.Yui.Compressor;
using Microsoft.Ajax.Utilities;

namespace Zippy.Chirp.Engines {
    class LessEngine : BasicEngine<LessEngine> {
        public LessEngine() : base(new[] { Settings.ChirpLessFile, Settings.ChirpMichaelAshLessFile,Settings.ChirpHybridLessFile,Settings.ChirpMSAjaxLessFile }, new[] { ".min.css", ".css" }) { }

        Regex rxLineNum = new Regex(@"line\s+([0-9]+)", RegexOptions.Compiled);
        Regex rxColNum = new Regex(@"\s+(\-*)\^", RegexOptions.Compiled);

        dotless.Core.Parser.Parser lazyLessParser;
        dotless.Core.Parser.Parser lessParser {
            get {
                if (lazyLessParser == null) {
                    lazyLessParser = new dotless.Core.Parser.Parser();
                }
                return lazyLessParser;
            }
        }

        bool IsChirpLessFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpLessFile, StringComparison.OrdinalIgnoreCase));
        }
        private bool IsChirpHybridLessFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpHybridLessFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpMichaelAshLessFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpMichaelAshLessFile, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsChirpMSAjaxLessFile(string fileName)
        {
            return (fileName.EndsWith(Settings.ChirpMSAjaxLessFile, StringComparison.OrdinalIgnoreCase));
        }


        public override IEnumerable<IResult> BasicTransform(Item item)
        {
            string css = null;
            ErrorResult err = null;
            try
            {
                css = lessParser.Parse(item.Text, item.FileName).ToCSS();
            }
            catch (Exception e)
            {
                int line = 1, column = 1;
                var description = e.Message.Trim();
                Match match;
                if ((match = rxLineNum.Match(description)).Success)
                {
                    line = match.Groups[1].Value.ToInt(1);
                }

                if ((match = rxColNum.Match(description)).Success)
                {
                    column = match.Groups[1].Length + 1;
                }

                err = new ErrorResult(item.FileName, description, line, column);
            }

            if (err != null)
            {
                yield return err;
            }
            else if (css != null)
            {
                yield return new FileResult(item, ".css", css, false);

                //Yui compressor
                if (IsChirpMichaelAshLessFile(item.FileName) || IsChirpHybridLessFile(item.FileName) || IsChirpLessFile(item.FileName))
                {
                    var mode = IsChirpMichaelAshLessFile(item.FileName) ? CssCompressionType.MichaelAshRegexEnhancements
                   : IsChirpHybridLessFile(item.FileName) ? CssCompressionType.Hybrid
                   : CssCompressionType.StockYuiCompressor;
                    yield return new FileResult(item, ".min.css", YuiCssEngine.Instance.Compress(css, mode), true);
                }
                if (IsChirpMSAjaxLessFile(item.FileName))
                {
                    Minifier minifier = new Minifier();
                    string miniCss = minifier.MinifyStyleSheet(css);

                    yield return new FileResult(item, ".min.css", miniCss, true);

                }
            }
        }
    }
}
