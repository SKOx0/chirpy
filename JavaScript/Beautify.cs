
namespace Zippy.Chirp.JavaScript {
    public class Beautify : Environment {
        public class options {
            public options() {
                indent_size = 4;
                indent_char = " ";
                preserve_newlines = true;
                max_preserve_newlines = 0;
                indent_level = 0;
                space_after_anon_function = false;
                keep_array_indentation = false;
            }

            public int indent_size { get; set; }
            public string indent_char { get; set; }
            public bool preserve_newlines { get; set; }
            public int max_preserve_newlines { get; set; }
            public int indent_level { get; set; }
            public bool space_after_anon_function { get; set; }
            public bool keep_array_indentation { get; set; }
        }

        protected override void OnInit() {
            RunFile("beautify");
        }

        public string js_beautify(string code, options options = null) {
            Run("jscode = js_beautify(jscode, jsoptions)",
                jsoptions => options, jscode => code);
            return (string)this["jscode"];
        }
    }
}
