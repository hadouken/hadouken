// LICENSE: MIT
// Author: github.com/bryceg
// Modifications by github.com/vktr

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hadouken.Core.Http.WebSockets.Extensions {
    // Allows serial queuing of Task instances
    // The tasks are not called on the current synchronization context
    public sealed class TaskQueue {
        private readonly object _mLockObj = new object();
        private readonly int? _mMaxSize;
        private volatile bool _mDrained;
        private Task _mLastQueuedTask;
        private long _mSize;

        public TaskQueue()
            : this(TaskAsyncHelper.Empty) {}

        public TaskQueue(Task initialTask) {
            this._mLastQueuedTask = initialTask;
        }

        public long Size {
            get { return this._mSize; }
        }

        /// <summary>
        ///     Enqueue a new task on the end of the queue
        /// </summary>
        /// <returns>The enqueued Task or NULL if the max size of the queue was reached</returns>
        public Task Enqueue<T>(Func<T, Task> taskFunc, T state) {
            // Lock the object for as short amount of time as possible
            lock (this._mLockObj) {
                if (this._mDrained) {
                    return this._mLastQueuedTask;
                }

                if (this._mMaxSize != null) {
                    // Increment the size if the queue
                    if (Interlocked.Increment(ref this._mSize) > this._mMaxSize) {
                        Interlocked.Decrement(ref this._mSize);

                        // We failed to enqueue because the size limit was reached
                        return null;
                    }
                }

                var newTask = this._mLastQueuedTask.Then((next, nextState) => {
                    return next(nextState).Finally(s => {
                        var queue = (TaskQueue) s;
                        if (queue._mMaxSize != null) {
                            // Decrement the number of items left in the queue
                            Interlocked.Decrement(ref queue._mSize);
                        }
                    },
                        this);
                },
                    taskFunc, state);

                this._mLastQueuedTask = newTask;
                return newTask;
            }
        }

        /// <summary>
        ///     Triggers a drain fo the task queue and waits for the drain to complete
        /// </summary>
        public void Drain() {
            lock (this._mLockObj) {
                this._mDrained = true;

                this._mLastQueuedTask.Wait();

                this._mDrained = false;
            }
        }
    }
}