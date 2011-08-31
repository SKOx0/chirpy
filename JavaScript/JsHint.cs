using System;
using System.Collections.Generic;
using System.Linq;

namespace Zippy.Chirp.JavaScript {
    public class JSHint : Environment {
        public class options {
            public bool bitwise { get; set; }
            public bool boss { get; set; }
            public bool curly { get; set; }
            public bool debug { get; set; }
            public bool devel { get; set; }
            public bool eqeqeq { get; set; }
            public bool evil { get; set; }
            public bool forin { get; set; }
            public bool immed { get; set; }
            public bool laxbreak { get; set; }
            public int? maxerr { get; set; }
            public bool newcapp { get; set; }
            public bool noarg { get; set; }
            public bool noempty { get; set; }
            public bool nonew { get; set; }
            public bool nomen { get; set; }
            public bool novar { get; set; }
            public bool passfail { get; set; }
            public bool plusplus { get; set; }
            public bool regex { get; set; }
            public bool undef { get; set; }
            public bool sub { get; set; }
            public bool strict { get; set; }
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
