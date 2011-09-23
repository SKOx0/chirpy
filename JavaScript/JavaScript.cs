using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Zippy.Chirp.JavaScript {

    [ComVisible(true)]
    public class JavaScript {
        public JavaScript() {
            Data = new Dictionary<string, object>();
            Requirements = new List<string>();
        }

        public JavaScript(string code, object data, params string[] requirements)
            : this() {
            Code = code;
            if (data != null) {
                if (data is string) {
                    Requirements.Add(data as string);

                } else if (data is IDictionary<string, object>) {
                    Data = data as IDictionary<string, object>;

                } else {
                    foreach (var prop in data.GetType().GetProperties())
                        Set(prop.Name, prop.GetValue(data, null));
                }
            }
            Requirements.AddRange(requirements);
        }

        public static JavaScript Execute(string code, object data, params string[] requirements) {
            var js = new JavaScript(code, data, requirements);
            js.Execute();
            return js;
        }

        private System.Threading.AutoResetEvent _Event = new System.Threading.AutoResetEvent(false);
        public List<string> Requirements { get; set; }
        public string Code { get; set; }

        public IDictionary<string, object> Data { get; set; }

        public object Get(string key) {
            object value;
            if (Data.TryGetValue(key, out value))
                return value;
            return null;
        }

        public void Set(string key, object value) {
            lock (Data)
                if (Data.ContainsKey(key)) Data[key] = value;
                else Data.Add(key, value);
        }

        public void AddMessage(int line, int col, int type, string message) {
            Messages.Add(new Message { Line = line, Column = col, Content = message, Type = (Message.Types)type });
        }

        public string GetFullUri(string path, string @base = null) {
            if (path.Contains("://")) return path;
            if (!path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)) path += ".js";
            var uri = path.ToUri(@base.ToUri());
            return uri.ToString();
        }

        public string Download(string path) {
            var uri = GetFullUri(path).ToUri();
            string file;
            if (uri.IsFile) {
                file = uri.LocalPath;

            } else {
                file = null;
                Extensibility.Instance.Download(uri, ref file);
            }

            return System.IO.File.ReadAllText(file);
        }

        public class Message {
            public Types Type { get; set; }
            public int Line { get; set; }
            public int Column { get; set; }
            public string Content { get; set; }
            public enum Types {
                Information = 0, Warning = 1, Error = 2
            }
        }

        public List<Message> Messages = new List<Message>();

        System.Threading.Thread _Thread;
        public void Execute() {
            if (_Thread == null) {
                _Thread = new System.Threading.Thread(_Execute);
                _Thread.SetApartmentState(System.Threading.ApartmentState.STA);
            }

            _Thread.Start();
            _Thread.Join();
        }

        public void Finished() {
            _Event.Set();
        }

        private void _Execute() {
            var requirements = string.Empty;

            if (Requirements != null)
                foreach (var req in Requirements) {
                    if (req.IsNullOrEmpty()) continue;
                    var script = req;

                    if (req.Contains("://") || ((script.Contains(":\\") || script.StartsWith("\\\\")) && System.IO.File.Exists(script))) {
                        script = "require(\"" + script.Replace("\\", @"\\") + "\");";
                    }

                    requirements += script + "\r\n";
                }

            var html = @"<!DOCTYPE html><html>
                <head><meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"" /></head>
                <body><script>
                        var required = {}, bases = [], module = window;
                        window.onerror = function(msg, file, line){
                            external.AddMessage(line, 0, 2, file + ': ' + msg);
                            external.Finished();
                        };
  
                        window.console = {
                            log: function(msg){
                                external.AddMessage(0, 0, 0, msg + '');
                            },

                            error: function(msg){
                                external.AddMessage(0, 0, 2, msg + '');
                            },

                            warning: function(msg){
                                external.AddMessage(0, 0, 1, msg + '');
                            }
                        };
                        
                        function require(path){
                            var key = external.GetFullUri(path, bases.length == 0 ? null : bases[bases.length-1]);

                            if(!required[key]){
                                required[key]  = {};
                                var code = external.Download(key),
                                    func = new Function('exports', code);

                                bases.push(key);
                                func(required[key]);
                                bases.pop();
                            }
                            return required[key];
                        }

                        try {
                            " + requirements + @"

                            " + Code + @" 
                        } catch(x){
                            external.AddMessage(x.line || 0, x.col || 0, 2, x.message);
                        }
                        external.Finished();
            
                    </script>
                </body></html>";

            using (var ie = new System.Windows.Forms.WebBrowser()) {
                ie.ObjectForScripting = this;
                ie.DocumentText = html;
                ie.ScriptErrorsSuppressed = true;

                while (!_Event.WaitOne(100)) {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }
    }
}
