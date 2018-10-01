using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class ExtensionAsync
    {
        private static readonly TaskFactory _myTaskFactory = new
          TaskFactory(CancellationToken.None,
                      TaskCreationOptions.None,
                      TaskContinuationOptions.None,
                      TaskScheduler.Default);

        /// <summary>
        /// Runs the <see cref="Task"/> synchronously on the default <see cref="TaskScheduler"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by this <see cref="Task"/></typeparam>
        /// <param name="task">The task instance</param>
        /// <returns>The value returned by the function</returns>
        public static TResult RunSync<TResult>(this Task<TResult> task) => RunSync(() => task);

        /// <summary>
        /// Runs the <see cref="Task"/> synchronously on the default <see cref="TaskScheduler"/>.
        /// </summary>
        /// <param name="task">The task instance</param>
        public static void RunSync(this Task task) => RunSync(() => task);

        internal static TResult RunSync<TResult>(Func<Task<TResult>> func) =>
            _myTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        internal static void RunSync(Func<Task> func) =>
            _myTaskFactory
                .StartNew<Task>(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
    }
}