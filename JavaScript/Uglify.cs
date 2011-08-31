
namespace Zippy.Chirp.JavaScript {

    public class Uglify : Environment {
        public class options {
            public options() {
                ast = false;
                mangle = true;
                mangle_toplevel = false;
                squeeze = true;
                make_seqs = true;
                dead_code = true;
                beautify = false;
                verbose = false;
                show_copyright = false;
                max_line_length = 32 * 1024;
                extra = false;
                @unsafe = false;
                beautify_options = new beautify_options {
                    indent_level = 4, indent_start = 0, quote_keys = false,
                    space_colon = false
                };
            }
            public bool ast { get; set; }
            public bool mangle { get; set; }
            public bool mangle_toplevel { get; set; }
            public bool squeeze { get; set; }
            public bool make_seqs { get; set; }
            public bool dead_code { get; set; }
            public bool beautify { get; set; }
            public bool verbose { get; set; }
            public bool show_copyright { get; set; }
            public int max_line_length { get; set; }
            public bool extra { get; set; }
            public bool @unsafe { get; set; }
            public beautify_options beautify_options { get; set; }
        }

        public class beautify_options {
            public int indent_level { get; set; }
            public int indent_start { get; set; }
            public bool quote_keys { get; set; }
            public bool space_colon { get; set; }
        }

        protected override void OnInit() {
            RunFile("uglify-js");
        }

        public string squeeze_it(string code, options options = null) {
            options = options ?? new options();

            Run("jscode = uglify(jscode, jsoptions)", jscode => code, jsoptions => options);
            return (string)this["jscode"];
        }

    }

}
