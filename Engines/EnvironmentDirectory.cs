using System;

namespace Zippy.Chirp.Engines {
    class EnvironmentDirectory : IDisposable {
        private string _CurrentDirectory;
        public EnvironmentDirectory(string file) {
            _CurrentDirectory = System.Environment.CurrentDirectory;
            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(file);
        }
        public void Dispose() {
            System.Environment.CurrentDirectory = _CurrentDirectory;
        }
    }
}
