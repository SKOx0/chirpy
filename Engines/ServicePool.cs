using System;

namespace Zippy.Chirp.Threading
{
    public class ServicePool : IDisposable
    {
        private System.Threading.Thread[] p_Threads;
        private System.Threading.ThreadStart p_Delegate;
        private long p_Runnning;

        public ServicePool(System.Threading.ThreadStart callback, int numThreads) : this(callback, numThreads, "ServicePool", System.Threading.ThreadPriority.Normal) { }

        public ServicePool(System.Threading.ThreadStart callback, int numThreads, string name, System.Threading.ThreadPriority priority)
        {
            this.p_Delegate = callback;
            this.p_Threads = new System.Threading.Thread[numThreads];
            this.p_Runnning = numThreads;
            for (int i = 0; i <= this.p_Threads.Length - 1; i++)
            {
                var th = new System.Threading.Thread(this.Execute);
                th.Name = string.Concat(name, " #", i + 1);
                th.Priority = priority;
                th.Start();
                this.p_Threads[i] = th;
            }
        }

        private void Execute()
        {
            this.p_Delegate();
            System.Threading.Interlocked.Decrement(ref this.p_Runnning);
        }

        public bool IsActive
        {
            get { return this.p_Runnning > 0; }
        }

        private bool isDisposed;

        public void Dispose()
        {
            if(this.isDisposed) return;

            this.isDisposed = true;
            foreach (System.Threading.Thread th in this.p_Threads)
            {
                if (th != null)
                {
                    th.Abort();
                }
            }

            this.p_Threads = null;
        }
    }
}