
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zippy.Chirp.Threading
{
    public class ServiceQueue<T> : IDisposable
    {
        private ServicePool p_Pool;
        private long p_Running = 0;
        private long p_Finished = 0;

        private System.Collections.Generic.Queue<T> p_Queue = new System.Collections.Generic.Queue<T>();
        private System.Threading.AutoResetEvent p_Notifier = new System.Threading.AutoResetEvent(false);
        private Action<T> p_Action = t => { };
        private Action<T, Exception> p_OnError = (t, e) => { };
        private bool p_IsDisposed = false;

        public bool IsDisposed
        {
            get { return this.p_IsDisposed; }
        }

        protected virtual void Process(T item)
        {
            this.p_Action(item);
        }

        protected virtual void Error(T item, Exception ex)
        {
            this.p_OnError(item, ex);
        }

        public ServiceQueue(Action<T> action = null, Action<T, Exception> onError = null, int? numThreads = null, string name = "Worker", System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal)
        {
            this.p_Action = action;
            this.p_OnError = onError;
            this.p_Pool = new ServicePool(this.ProcessQueue, numThreads ?? System.Environment.ProcessorCount, name, priority);
        }

        public virtual void Enqueue(T obj)
        {
            lock (this.p_Queue)
                this.p_Queue.Enqueue(obj);
            this.p_Notifier.Set();
        }

        public bool Any(Func<T, bool> predicate)
        {
            lock (this.p_Queue) return this.p_Queue.Any(predicate);
        }

        public virtual void Enqueue(IEnumerable<T> objs)
        {
            lock (this.p_Queue)
                foreach (var x in objs)
                    this.p_Queue.Enqueue(x);

            this.p_Notifier.Set();
        }

        public long Finished
        {
            get
            {
                return this.p_Finished;
            }
        }

        public long Total
        {
            get
            {
                return this.Finished + this.Length;
            }
        }

        public long Length
        {
            get
            {
                if (this.p_IsDisposed) return 0;
                return this.p_Queue.Count + this.p_Running;
            }
        }

        public long Running
        {
            get { return this.p_Running; }
        }

        public bool IsActive
        {
            get { return this.Length > 0; }
        }

        private void ProcessQueue()
        {
            while (!this.p_IsDisposed)
            {
                this.p_Notifier.WaitOne(300);

                T obj = default(T);
                bool exec = false;
                if (!this.p_IsDisposed && this.p_Queue != null && this.p_Queue.Count > 0)
                {
                    lock (this.p_Queue)
                    {
                        // Now that we have an exclusive lock on the queue, it could be empty
                        if (this.p_Queue.Count > 0)
                        {
                            System.Threading.Interlocked.Increment(ref this.p_Running);

                            // Ensure that CurrentlyExecuting + QueueLength <> 0
                            obj = this.p_Queue.Dequeue();
                            exec = true;
                        }
                    }
                }

                if (exec)
                {
                    try
                    {
                        this.Process(obj);
                    }
                    catch (Exception ex)
                    {
                        if (!this.p_IsDisposed)
                        {
                            if (this.p_OnError != null)
                            {
                                this.p_OnError(obj, ex);
                            }
                        }
                    }
                    finally
                    {
                        System.Threading.Interlocked.Decrement(ref this.p_Running);
                        System.Threading.Interlocked.Increment(ref this.p_Finished);
                        if (obj is IDisposable)
                        {
                            ((IDisposable)obj).Dispose();
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            this.p_IsDisposed = true;
            if (this.p_Pool != null)
            {
                this.p_Pool.Dispose();
            }

            this.p_Queue = null;
            this.p_Pool = null;
        }

        public void Wait(int waitInterval = 100, Action whileWaiting = null)
        {
            var mre = new System.Threading.ManualResetEvent(false);
            while (this.IsActive)
            {
                mre.WaitOne(100);
                if (whileWaiting != null)
                    whileWaiting();
            }
        }
    }
}