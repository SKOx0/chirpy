
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zippy.Chirp.Threading {
    public class ServiceQueue<T> : IDisposable {

        private ServicePool p_Pool;
        private long p_Running = 0;
        private long p_Finished = 0;

        private System.Collections.Generic.Queue<T> p_Queue = new System.Collections.Generic.Queue<T>();
        private System.Threading.AutoResetEvent p_Notifier = new System.Threading.AutoResetEvent(false);
        private Action<T> p_Action = t => { };
        private Action<T, Exception> p_OnError = (t, e) => { };
        private bool p_IsDisposed = false;

        public bool IsDisposed {
            get { return p_IsDisposed; }
        }

        protected virtual void Process(T item) {
            p_Action(item);
        }

        protected virtual void Error(T item, Exception ex) {
            p_OnError(item, ex);
        }

        public ServiceQueue(Action<T> action = null, Action<T, Exception> onError = null, int? numThreads = null, string name = "Worker", System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.Normal) {
            p_Action = action;
            p_OnError = onError;
            p_Pool = new ServicePool(ProcessQueue, numThreads ?? System.Environment.ProcessorCount, name, priority);
        }

        public virtual void Enqueue(T obj) {
            lock(p_Queue)
                p_Queue.Enqueue(obj);
            p_Notifier.Set();
        }

        public bool Any(Func<T, bool> predicate) {
            lock(p_Queue) return p_Queue.Any(predicate);
        }

        public virtual void Enqueue(IEnumerable<T> objs) {
            lock(p_Queue)
                foreach(var x in objs)
                    p_Queue.Enqueue(x);

            p_Notifier.Set();
        }

        public long Finished {
            get {
                return p_Finished;
            }
        }

        public long Total {
            get {
                return Finished + Length;
            }
        }

        public long Length {
            get {
                if(p_IsDisposed) return 0;
                return p_Queue.Count + p_Running;
            }
        }

        public long Running {
            get { return p_Running; }
        }

        public bool IsActive {
            get { return Length > 0; }
        }

        private void ProcessQueue() {
            while(!p_IsDisposed) {
                p_Notifier.WaitOne(300);

                T obj = default(T);
                bool exec = false;
                if(!p_IsDisposed && p_Queue != null && p_Queue.Count > 0) {
                    lock(p_Queue) {
                        // Now that we have an exclusive lock on the queue, it could be empty
                        if(p_Queue.Count > 0) {
                            System.Threading.Interlocked.Increment(ref p_Running);
                            // Ensure that CurrentlyExecuting + QueueLength <> 0
                            obj = p_Queue.Dequeue();
                            exec = true;
                        }
                    }
                }

                if(exec) {
                    try {
                        Process(obj);
                    } catch(Exception ex) {
                        if(!p_IsDisposed) {
                            if(p_OnError != null) {
                                p_OnError(obj, ex);
                            }
                        }
                    } finally {
                        System.Threading.Interlocked.Decrement(ref p_Running);
                        System.Threading.Interlocked.Increment(ref p_Finished);
                        if(obj is IDisposable) ((IDisposable)obj).Dispose();
                    }
                }
            }
        }

        public void Dispose() {
            p_IsDisposed = true;
            if(p_Pool != null) {
                p_Pool.Dispose();
            }
            p_Queue = null;
            p_Pool = null;
        }

        public void Wait(int waitInterval = 100, Action whileWaiting = null) {
            var mre = new System.Threading.ManualResetEvent(false);
            while(IsActive) {
                mre.WaitOne(100);
                if(whileWaiting != null)
                    whileWaiting();
            }
        }
    }
}