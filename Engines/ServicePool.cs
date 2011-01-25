using System;

namespace Zippy.Chirp.Threading {
    public class ServicePool : IDisposable {
        private System.Threading.Thread[] p_Threads;
        private System.Threading.ThreadStart p_Delegate;
        private long p_Runnning;

        public ServicePool(System.Threading.ThreadStart callback, int numThreads) : this(callback, numThreads, "ServicePool", System.Threading.ThreadPriority.Normal) { }

        public ServicePool(System.Threading.ThreadStart callback, int numThreads, string name, System.Threading.ThreadPriority priority) {
            p_Delegate = callback;
            p_Threads = new System.Threading.Thread[numThreads];
            p_Runnning = numThreads;
            for(int i = 0; i <= p_Threads.Length - 1; i++) {
                var th = new System.Threading.Thread(Execute);
                th.Name = string.Concat(name, " #", i + 1);
                th.Priority = priority;
                th.Start();
                p_Threads[i] = th;
            }
        }

        private void Execute() {
            p_Delegate();
            System.Threading.Interlocked.Decrement(ref p_Runnning);
        }

        public bool IsActive {
            get { return p_Runnning > 0; }
        }

        private bool _IsDisposed;
        public void Dispose() {
            if(_IsDisposed) return;

            _IsDisposed = true;
            foreach(System.Threading.Thread th in p_Threads) {
                if(th != null) {
                    th.Abort();
                }
            }

            p_Threads = null;
        }
    }
}