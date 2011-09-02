using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Zippy.Chirp.JavaScript {
    public class JSHint : Environment {
        public class options
        {
            [Description("Prohibit the use of bitwise operators"), Category("Options")]
            public bool bitwise { get; set; }

            [Description("Allow assignments inside if/for/while/do"), Category("Options")]
            public bool boss { get; set; }

            [Description("Require curly braces around all blocks"), Category("Options")]
            public bool curly { get; set; }

            [Description("Allow debugger statements"), Category("Options")]
            public bool debug { get; set; }

            [BrowsableAttribute(false)]
            public bool devel { get; set; }

            [Description("Require that you use === and !== for all comparisons"), Category("Options")]
            public bool eqeqeq { get; set; }

            [Description("Allow the use of eval"), Category("Options")]
            public bool evil { get; set; }

            [Description("Disallow the use of for in without hasOwnProperty"), Category("Options")]
            public bool forin { get; set; }

            [Description("Require immediate invocations to be wrapped in parens"), Category("Options")]
            public bool immed { get; set; }

            [Description("Not check line breaks"), Category("Options")]
            public bool laxbreak { get; set; }

            [Description("Maximum number of errors before stops processing your source"), Category("Options")]
            public int? maxerr { get; set; }

            [Description("Require that you capitalize all constructor functions"), Category("Options")]
            public bool newcapp { get; set; }

            [Description("Prohibit the use of arguments.caller and arguments.callee"), Category("Options")]
            public bool noarg { get; set; }

            [Description("Prohibit the use of empty blocks"), Category("Options")]
            public bool noempty { get; set; }

            [Description("Prohibit the use of constructors for side-effects"), Category("Options")]
            public bool nonew { get; set; }

            [Description("Disallow the use of initial or trailing underbars in names"), Category("Options")]
            public bool nomen { get; set; }

            [Description("Allow only one var statement per function"), Category("Options")]
            public bool novar { get; set; }

            [Description("Stop on the first error it encounter"), Category("Options")]
            public bool passfail { get; set; }

            [Description("Prohibit the use of increment and decrement operators"), Category("Options")]
            public bool plusplus { get; set; }

            [Description("Disallow . and [^...] in regular expressions"), Category("Options")]
            public bool regex { get; set; }

            [Description("Require all non-global variables be declared before they are used"), Category("Options")]
            public bool undef { get; set; }

            [Description("Tolerate all forms of subscript notation"), Category("Options")]
            public bool sub { get; set; }

            [Description("Require you to use \"use strict\"; pragma"), Category("Options")]
            public bool strict { get; set; }

            [Description("Check your code against strict whitespace rules"), Category("Options")]
            public bool white { get; set; }
        }

        public class result {
            public int line { get; set; }
            public int character { get; set; }
            public string reason { get; set; }
            public string evidence { get; set; }
            public string raw { get; set; }
        }

        //private static object get(Dictionary<string, object> dic, string name) {
        //    object value;
        //    if (dic == null) return null;
        //    else if (dic.TryGetValue(name, out value)) return value;
        //    else return null;
        //}

        private static T get<T>(Jurassic.Library.ObjectInstance dic, string name, T defaultValue) {
            var value = dic.GetPropertyValue(name);
            T ret = defaultValue;
            try {
                if (defaultValue is string) ret = (T)(object)Convert.ToString(value);
                else ret = (T)Convert.ChangeType(value, typeof(T));
            } catch { }
            return ret;
        }

        protected override void OnInit() {
            RunFile("jshint");
        }

        public result[] JSHINT(string source, options options = null) {
            this["jscode"] = source;
            this["options"] = options;
            Run(@"var result = JSHINT(jscode, options), errors = JSHINT.errors;");

            if (!(bool)this["result"]) {
                var errors = ((Jurassic.Library.ArrayInstance)this["errors"])
                    .ElementValues
                    .OfType<Jurassic.Library.ObjectInstance>();
                var results = new List<result>();
                foreach (var result in errors) {
                    if (result == null) continue;
                    results.Add(new result {
                        character = get(result, "character", 0),
                        line = get(result, "line", 0),
                        reason = get(result, "reason", string.Empty),
                        evidence = get(result, "evidence", string.Empty),
                        raw = get(result, "raw", string.Empty),
                    });
                }
                return results.ToArray();
            } else return null;
        }
    }
}
