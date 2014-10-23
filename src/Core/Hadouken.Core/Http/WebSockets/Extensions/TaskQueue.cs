// LICENSE: MIT
// Author: github.com/bryceg
// Modifications by github.com/vktr

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hadouken.Core.Http.WebSockets.Extensions
{
    // Allows serial queuing of Task instances
    // The tasks are not called on the current synchronization context
    public sealed class TaskQueue
    {
        private readonly object mLockObj = new object();
        private Task mLastQueuedTask;
        private volatile bool mDrained;
        private readonly int? mMaxSize;
        private long mSize;

        public long Size { get { return mSize; } }

        public TaskQueue()
            : this(TaskAsyncHelper.Empty)
        {
        }

        public TaskQueue(Task initialTask)
        {
            mLastQueuedTask = initialTask;
        }

        /// <summary>
        /// Enqueue a new task on the end of the queue
        /// </summary>
        /// <returns>The enqueued Task or NULL if the max size of the queue was reached</returns>
        public Task Enqueue<T>(Func<T, Task> taskFunc, T state)
        {
            // Lock the object for as short amount of time as possible
            lock (mLockObj)
            {
                if (mDrained)
                {
                    return mLastQueuedTask;
                }

                if (mMaxSize != null)
                {
                    // Increment the size if the queue
                    if (Interlocked.Increment(ref mSize) > mMaxSize)
                    {
                        Interlocked.Decrement(ref mSize);

                        // We failed to enqueue because the size limit was reached
                        return null;
                    }
                }

                var newTask = mLastQueuedTask.Then((next, nextState) =>
                {
                    return next(nextState).Finally(s =>
                    {
                        var queue = (TaskQueue)s;
                        if (queue.mMaxSize != null)
                        {
                            // Decrement the number of items left in the queue
                            Interlocked.Decrement(ref queue.mSize);
                        }
                    },
                    this);
                },
                taskFunc, state);

                mLastQueuedTask = newTask;
                return newTask;
            }
        }

        /// <summary>
        /// Triggers a drain fo the task queue and waits for the drain to complete
        /// </summary>
        public void Drain()
        {
            lock (mLockObj)
            {
                mDrained = true;

                mLastQueuedTask.Wait();

                mDrained = false;
            }
        }
    }
}
