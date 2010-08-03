using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yahoo.Yui.Compressor;
using Microsoft.Ajax.Utilities;
using Zippy.Chirp.Xml;

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
            MinifyType mode = MinifyType.yui;
            if (IsChirpMichaelAshLessFile(item.FileName) || IsChirpHybridLessFile(item.FileName) || IsChirpLessFile(item.FileName))
            {
                mode = IsChirpMichaelAshLessFile(item.FileName) ? MinifyType.yuiMARE
               : IsChirpHybridLessFile(item.FileName) ? MinifyType.yuiHybird
               : MinifyType.yui;

            }
            if (IsChirpMSAjaxLessFile(item.FileName))
            {
                mode = MinifyType.msAjax;
            }
         return    BasicTransform(item, mode);
        }

        public IEnumerable<IResult> BasicTransform(Item item,MinifyType mode)
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
                switch (mode)
                {
                    case MinifyType.yui:
                        yield return new FileResult(item, ".min.css", YuiCssEngine.Instance.Compress(css,CssCompressionType.StockYuiCompressor), true);
                        break;
                    case MinifyType.yuiMARE:
                        yield return new FileResult(item, ".min.css", YuiCssEngine.Instance.Compress(css, CssCompressionType.MichaelAshRegexEnhancements), true);
                        break;
                    case MinifyType.yuiHybird:
                        yield return new FileResult(item, ".min.css", YuiCssEngine.Instance.Compress(css, CssCompressionType.Hybrid), true);
                        break;
                    case MinifyType.msAjax:
                        Minifier minifier = new Minifier();
                        string miniCss = minifier.MinifyStyleSheet(css);
                        yield return new FileResult(item, ".min.css", miniCss, true);
                        break;
                    default:
                        yield return new FileResult(item, ".min.css", YuiCssEngine.Instance.Compress(css, CssCompressionType.StockYuiCompressor), true);
                        break;
                }
            }
        }
    }
}
