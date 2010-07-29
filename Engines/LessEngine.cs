using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Zippy.Chirp.Engines
{
    class LessEngine : BasicEngine<LessEngine>
    {
        public LessEngine() : base(new[] { Settings.ChirpLessFile, Settings.ChirpLessCssFile }, new[] { ".min.css", ".css" }) { }

        static Regex rxLineNum = new Regex(@"line\s+([0-9]+)", RegexOptions.Compiled);
        static Regex rxColNum = new Regex(@"\s+(\-*)\^", RegexOptions.Compiled);

        dotless.Core.Parser.Parser lazyLessParser;
        dotless.Core.Parser.Parser lessParser
        {
            get
            {
                if (lazyLessParser == null)
                {
                    lazyLessParser = new dotless.Core.Parser.Parser();
                }
                return lazyLessParser;
            }
        }

        public override IEnumerable<IResult> Transform(Item item)
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

                err = new ErrorResult(description, line, column);
            }

            if (err != null)
            {
                yield return err;
            }
            else if (css != null)
            {
                yield return new FileResult(item, ".css", css, false);
                yield return new FileResult(item, ".min.css", YuiCssEngine.Instance.Compress(css, Yahoo.Yui.Compressor.CssCompressionType.StockYuiCompressor), true);
            }
        }
    }
}
