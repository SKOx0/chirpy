
namespace Zippy.Chirp.JavaScript {
    public class CoffeeScript : Environment {
        public class options {
            public bool nowrap { get; set; }
        }

        protected override void OnInit() {
            Run(Zippy.Chirp.Properties.Resources.browser);
            RunFile("coffee-script");
        }

        public string compile(string source, options options = null) {
            this["jscode"] = source;
            this["options"] = options;
            Run("jscode = CoffeeScript.compile(jscode, options)");
            return (string)this["jscode"];
        }
    }
}
