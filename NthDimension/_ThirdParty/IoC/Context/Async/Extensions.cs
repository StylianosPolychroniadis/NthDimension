using System;
using System.Threading.Tasks;

namespace NthDimension.Context.Async
{
    #region Task
    public static class TaskExtensions
    {
        public static Task<T> AsTask<T>(this T value)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetResult(value);
            return taskCompletionSource.Task;
        }

        /// <summary>Creates a Task that has completed in the Faulted state with the specified exception.</summary>
        /// <typeparam name="TResult">Specifies the type of payload for the new Task.</typeparam>
        /// <param name="factory">The target TaskFactory.</param>
        /// <param name="exception">The exception with which the Task should fault.</param>
        /// <returns>The completed Task.</returns>
        public static Task<TResult> FromException<TResult>(this TaskFactory factory, Exception exception)
        {
            var tcs = new TaskCompletionSource<TResult>(factory.CreationOptions);
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
    #endregion

    #region AsyncEventHub
    public static class AsyncEventHubExtensions
    {
        private static readonly AsyncEventHub AsyncEventHub = new AsyncEventHub();

        public static Task<Task[]> Publish<TEvent>(this object sender, Task<TEvent> eventData)
        {
            return AsyncEventHub.Publish(sender, eventData);
        }

        public static Task Subscribe<TEvent>(this object sender, Func<Task<TEvent>, Task> eventHandlerTaskFactory)
        {
            return AsyncEventHub.Subscribe(sender, eventHandlerTaskFactory);
        }

        public static Task Unsubscribe<TEvent>(this object sender)
        {
            return AsyncEventHub.Unsubscribe<TEvent>(sender);
        }
    }
    #endregion AsyncEventHub
}
