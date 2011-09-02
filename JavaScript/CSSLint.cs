using System;
using System.Linq;
using Jurassic.Library;
using System.ComponentModel;

namespace Zippy.Chirp.JavaScript {
    public class CSSLint : Environment {
        public class options
        {
            /// <summary>
            /// to allow IE6 star hack as valid
            /// </summary>
            [Description("Allow IE6 star hack as valid"), Category("Options")]
            public bool starHack { get; set; }

            /// <summary>
            /// to interpret leading underscores as IE6-7 targeting for known properties
            /// </summary>
            [Description("Interpret leading underscores as IE6-7 targeting for known properties"), Category("Options")]
            public bool underscoreHack { get; set; }

            /// <summary>
            /// to indicate that IE &lt; 8 filters should be accepted and not throw syntax errors
            /// </summary>
            [Description("Indicate that IE &lt; 8 filters should be accepted and not throw syntax errors"), Category("Options")]
            public bool ieFilters { get; set; }
        }

        public class Message {
            public int col { get; set; }
            public string evidence { get; set; }
            public int line { get; set; }
            public string message { get; set; }
            public types type { get; set; }
            //public rule rule { get; set; }

            public enum types {
                error, info, warning
            }
        }

        public class rule {
            public string browsers { get; set; }
            public string desc { get; set; }
            public string id { get; set; }
            public string name { get; set; }
        }

        public class result {
            public Message[] messages { get; set; }
        }

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
            RunFile("csslint");
        }

        public result CSSLINT(string source, options options = null) {
            this["text"] = source;
            this["options"] = options;
            Run(@"var result = CSSLint.verify(text, options);");

            var result = (ObjectInstance)this["result"];

            return new result {
                messages = ((ArrayInstance)result.GetPropertyValue("messages"))
                   .ElementValues.OfType<ObjectInstance>().Select(x => new Message {
                       col = get(x, "col", 0),
                       line = get(x, "line", 0),
                       evidence = get(x, "evidence", string.Empty),
                       message = get(x, "message", string.Empty),
                       type = (Message.types)System.Enum.Parse(typeof(Message.types), get(x, "type", string.Empty))
                   }).ToArray()
            };
        }
    }
}
