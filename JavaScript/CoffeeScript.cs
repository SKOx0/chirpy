
using System.Text.RegularExpressions;
namespace Zippy.Chirp.JavaScript {
    public class CoffeeScript : Environment {
        public class options {
            public bool bare { get; set; }
        }

        protected override void OnInit() {
            Run(Zippy.Chirp.Properties.Resources.browser);
            RunFile("coffee-script");
        }

        private static Regex rxDetectOptions = new Regex(@"/\*\s*CoffeeScript\:\s*(.*?)\*/", RegexOptions.Compiled);
        public string compile(string source, options options = null) {
            var js = "jscode = CoffeeScript.compile(jscode, options);";
            var moptions = rxDetectOptions.Match(source);
            if (moptions.Success) {
                js = "var options = { " + moptions.Groups[1].Value + "}; " + js;
                source = source.Remove(moptions.Index, moptions.Length);
            } else {
                this["options"] = options;
            }

            this["jscode"] = source;
            Run(js);
            return (string)this["jscode"];
        }
    }
}
