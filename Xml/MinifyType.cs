using System.ComponentModel;

namespace Zippy.Chirp.Xml 
{
    public enum MinifyType
    {
        Unspecified,
        [Description("YUI")]
        yui,
        [Description("YUI w/ Michael Ash Regex Enhancement")]
        yuiMARE,
        [Description("YUI Hybrid")]
        yuiHybrid,
        [Description("Google Closure Tools - Advanced")]
        gctAdvanced,
        [Description("Google Closure Tools - Simple")]
        gctSimple,
        [Description("Google Closure Tools - Whitespace Only")]
        gctWhiteSpaceOnly,
        [Description("Microsoft Ajax Toolkit")]
        msAjax,
        [Description("Uglify.js")]
        uglify,
        jsBeautifier,
    }
}
