using Cauldron.Activator;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Cauldron.Core.Threading
{
    /// <summary>
    /// Provides a class that insures that an action is only called once.
    /// <para/>
    /// Only methods that has no parameters and has no return value are supported.
    /// </summary>
    public sealed class CallOnce
    {
        private static IDispatcher dispatcher;
        private static ConcurrentDictionary<object, CallOnce> instances = new ConcurrentDictionary<object, CallOnce>();
        private Action @delegate;
        private Func<Task> asyncDelegate;
        private object instance;
        private volatile bool isWaiting = false;
        private Task task;

        static CallOnce() => dispatcher = Factory.Create<IDispatcher>();

        private CallOnce(Action @delegate)
        {
            this.@delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
            this.task = Task.FromResult(0);
        }

        private CallOnce(Func<Task> asyncDelegate)
        {
            this.asyncDelegate = asyncDelegate ?? throw new ArgumentNullException(nameof(asyncDelegate));
            this.task = Task.FromResult(0);
        }

        /// <summary>
        /// Occures if the action has been invoked.
        /// </summary>
        public event EventHandler Invoked;

        /// <summary>
        /// Gets the current <see cref="Task"/>. The task can used to await for the execution of the encapsulated method.
        /// </summary>
        public Task CurrentTask => this.task;

        /// <summary>
        /// Gets a value that indicates that the call is still waiting for its execution.
        /// </summary>
        public bool IsWaiting => this.isWaiting;

        /// <summary>
        /// Creates a new instance of <see cref="CallOnce"/> that encapsulates a method that has no parameters
        /// and has no return value.
        /// </summary>
        /// <param name="delegate">The delegate to encapsulate.</param>
        /// <returns>A new instance of <see cref="CallOnce"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="delegate"/> is null.</exception>
        public static CallOnce Create(Action @delegate) => new CallOnce(@delegate);

        /// <summary>
        /// Creates a new instance of <see cref="CallOnce"/> that encapsulates a method that has no parameters
        /// and has no return value.
        /// </summary>
        /// <param name="delegate">The delegate to encapsulate.</param>
        /// <returns>A new instance of <see cref="CallOnce"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="delegate"/> is null.</exception>
        public static CallOnce Create(Func<Task> @delegate) => new CallOnce(@delegate);

        /// <summary>
        /// Creates a new instance of <see cref="CallOnce"/> that encapsulates a method that has no parameters
        /// and has no return value.
        /// </summary>
        /// <param name="instance">The instance that is used as key to identify the invoker.</param>
        /// <param name="delegate">The delegate to encapsulate.</param>
        /// <returns>A new instance of <see cref="CallOnce"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="delegate"/> is null.</exception>
        public static CallOnce Create(object instance, Action @delegate)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (instances.TryGetValue(instance, out CallOnce value))
                return value;

            var newCallOnce = new CallOnce(@delegate)
            {
                instance = instance
            };
            instances.TryAdd(instance, newCallOnce);

            return newCallOnce;
        }

        /// <summary>
        /// Creates a new instance of <see cref="CallOnce"/> that encapsulates a method that has no parameters
        /// and has no return value.
        /// </summary>
        /// <param name="instance">The instance that is used as key to identify the invoker.</param>
        /// <param name="delegate">The delegate to encapsulate.</param>
        /// <returns>A new instance of <see cref="CallOnce"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="delegate"/> is null.</exception>
        public static CallOnce Create(object instance, Func<Task> @delegate)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (instances.TryGetValue(instance, out CallOnce value))
                return value;

            var newCallOnce = new CallOnce(@delegate)
            {
                instance = instance
            };
            instances.TryAdd(instance, newCallOnce);

            return newCallOnce;
        }

        /// <summary>
        /// Invokes the encapsulated method.
        /// </summary>
        public void Invoke()
        {
            if (this.isWaiting)
                return;

            this.isWaiting = true;
            this.task = new Task(() => { });

            if (this.@delegate != null)
                dispatcher.RunAsync(DispatcherPriority.Low, () =>
                {
                    try
                    {
                        this.@delegate();
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        this.isWaiting = false;
                        this.Invoked?.Invoke(this, EventArgs.Empty);
                        this.RemoveFromDictionary();
                        this.task.Start();
                    }
                });
            else if (this.asyncDelegate != null)
                dispatcher.RunAsync(DispatcherPriority.Low, async () =>
                {
                    try
                    {
                        await this.asyncDelegate();
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        this.isWaiting = false;
                        this.Invoked?.Invoke(this, EventArgs.Empty);
                        this.RemoveFromDictionary();
                        this.task.Start();
                    }
                });
            else
                throw new NotImplementedException();
        }

        private void RemoveFromDictionary()
        {
            if (this.instance == null)
                return;

            instances.TryRemove(this.instance, out CallOnce value);
        }
    }
}