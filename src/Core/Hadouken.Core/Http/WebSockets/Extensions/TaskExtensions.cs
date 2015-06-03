// LICENSE: MIT
// Author: github.com/bryceg
// Modifications by github.com/vktr

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Hadouken.Core.Http.WebSockets.Extensions {
    public static class TaskAsyncHelper {
        private static readonly Task mEmptyTask = MakeTask<object>(null);
        private static readonly Task<bool> mTrueTask = MakeTask(true);
        private static readonly Task<bool> mFalseTask = MakeTask(false);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        public static Task Empty {
            get { return mEmptyTask; }
        }

        private static Task<T> MakeTask<T>(T value) {
            return FromResult(value);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        public static Task Then<T1, T2>(this Task task, Func<T1, T2, Task> successor, T1 arg1, T2 arg2) {
            switch (task.Status) {
                case TaskStatus.Faulted:
                case TaskStatus.Canceled:
                    return task;

                case TaskStatus.RanToCompletion:
                    return FromMethod(successor, arg1, arg2);

                default:
                    return GenericDelegates<object, Task, T1, T2>.ThenWithArgs(task, successor, arg1, arg2)
                        .FastUnwrap();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions are flowed to the caller")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        public static Task Finally(this Task task, Action<object> next, object state) {
            try {
                switch (task.Status) {
                    case TaskStatus.Faulted:
                    case TaskStatus.Canceled:
                        next(state);
                        return task;
                    case TaskStatus.RanToCompletion:
                        return FromMethod(next, state);

                    default:
                        return RunTaskSynchronously(task, next, state, false);
                }
            }
            catch (Exception ex) {
                return FromError(ex);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        public static Task FastUnwrap(this Task<Task> task) {
            var innerTask = (task.Status == TaskStatus.RanToCompletion) ? task.Result : null;
            return innerTask ?? task.Unwrap();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions are set in a tcs")]
        public static Task FromMethod<T1>(Action<T1> func, T1 arg) {
            try {
                func(arg);
                return Empty;
            }
            catch (Exception ex) {
                return FromError(ex);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions are set in a tcs")]
        public static Task FromMethod<T1, T2>(Func<T1, T2, Task> func, T1 arg1, T2 arg2) {
            try {
                return func(arg1, arg2);
            }
            catch (Exception ex) {
                return FromError(ex);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        public static Task<T> FromResult<T>(T value) {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        internal static Task FromError(Exception e) {
            return FromError<object>(e);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        internal static Task<T> FromError<T>(Exception e) {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetUnwrappedException(e);
            return tcs.Task;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        internal static void SetUnwrappedException<T>(this TaskCompletionSource<T> tcs, Exception e) {
            var aggregateException = e as AggregateException;
            if (aggregateException != null) {
                tcs.SetException(aggregateException.InnerExceptions);
            }
            else {
                tcs.SetException(e);
            }
        }

        internal static Task ContinueWithPreservedCulture(this Task task, Action<Task> continuationAction,
            TaskContinuationOptions continuationOptions) {
            var preservedCulture = Thread.CurrentThread.CurrentCulture;
            var preservedUiCulture = Thread.CurrentThread.CurrentUICulture;
            return task.ContinueWith(t => {
                var replacedCulture = Thread.CurrentThread.CurrentCulture;
                var replacedUiCulture = Thread.CurrentThread.CurrentUICulture;
                try {
                    Thread.CurrentThread.CurrentCulture = preservedCulture;
                    Thread.CurrentThread.CurrentUICulture = preservedUiCulture;
                    continuationAction(t);
                }
                finally {
                    Thread.CurrentThread.CurrentCulture = replacedCulture;
                    Thread.CurrentThread.CurrentUICulture = replacedUiCulture;
                }
            }, continuationOptions);
#endif
        }

        internal static Task ContinueWithPreservedCulture(this Task task, Action<Task> continuationAction) {
            return task.ContinueWithPreservedCulture(continuationAction, TaskContinuationOptions.None);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions are set in a tcs")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This is a shared file")]
        private static Task RunTaskSynchronously(Task task, Action<object> next, object state, bool onlyOnSuccess = true) {
            var tcs = new TaskCompletionSource<object>();
            task.ContinueWithPreservedCulture(t => {
                try {
                    if (t.IsFaulted) {
                        if (!onlyOnSuccess) {
                            next(state);
                        }

                        tcs.SetUnwrappedException(t.Exception);
                    }
                    else if (t.IsCanceled) {
                        if (!onlyOnSuccess) {
                            next(state);
                        }

                        tcs.SetCanceled();
                    }
                    else {
                        next(state);
                        tcs.SetResult(null);
                    }
                }
                catch (Exception ex) {
                    tcs.SetUnwrappedException(ex);
                }
            },
                TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        private static class TaskRunners<T, TResult> {
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
                Justification = "Exceptions are set in a tcs")]
            internal static Task<TResult> RunTask(Task task, Func<TResult> successor) {
                var tcs = new TaskCompletionSource<TResult>();
                task.ContinueWithPreservedCulture(t => {
                    if (t.IsFaulted) {
                        tcs.SetUnwrappedException(t.Exception);
                    }
                    else if (t.IsCanceled) {
                        tcs.SetCanceled();
                    }
                    else {
                        try {
                            tcs.SetResult(successor());
                        }
                        catch (Exception ex) {
                            tcs.SetUnwrappedException(ex);
                        }
                    }
                });

                return tcs.Task;
            }
        }

        private static class GenericDelegates<T, TResult, T1, T2> {
            internal static Task<Task> ThenWithArgs(Task task, Func<T1, T2, Task> successor, T1 arg1, T2 arg2) {
                return TaskRunners<object, Task>.RunTask(task, () => successor(arg1, arg2));
            }
        }
    }
}