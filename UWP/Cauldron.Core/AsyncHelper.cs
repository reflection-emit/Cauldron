using Cauldron.Core.Extensions;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides methods for Asyncronous operations
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new
          TaskFactory(CancellationToken.None,
                      TaskCreationOptions.None,
                      TaskContinuationOptions.None,
                      TaskScheduler.Default);

        /// <summary>
        /// Insures that an awaited method always returns a <see cref="Task"/>.
        /// </summary>
        /// <param name="task">The awaitable task</param>
        /// <returns>An awaitable task</returns>
        /// <example>
        /// <code>
        /// await AsyncHelper.NullGuard(instance.GetStuff()?.RemoveAllAsync());
        /// </code>
        /// </example>
        public static Task NullGuard(Task task)
        {
            if (task == null)
                return Task.FromResult(0);

            return task;
        }

        /// <summary>
        /// Insures that an awaited method always returns a <see cref="Task"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result produced by <see cref="Task{T}"/>.</typeparam>
        /// <param name="task">The awaitable task</param>
        /// <returns>An awaitable task</returns>
        /// <exception cref="NotSupportedException"><typeparamref name="TResult"/> is a value type</exception>
        /// <example>
        /// <code>
        /// var value = await AsyncHelper.NullGuard(instance.GetStuff()?.GetAllValuesAsync());
        /// </code>
        /// </example>
        public static Task<TResult> NullGuard<TResult>(Task<TResult> task)
        {
            if (task == null)
            {
                var type = typeof(TResult);

#if WINDOWS_UWP
                if (type.GetTypeInfo().IsValueType && !type.IsNullable())
#else
                if (type.IsValueType && !type.IsNullable())
#endif
                    throw new NotSupportedException("AsyncHelper.NullGuard does not support value types, because of high bug potential");

                return Task.FromResult(default(TResult));
            }

            return task;
        }

        internal static TResult RunSync<TResult>(Func<Task<TResult>> func) =>
            AsyncHelper._myTaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();

        internal static void RunSync(Func<Task> func) =>
            AsyncHelper._myTaskFactory
              .StartNew<Task>(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
    }
}