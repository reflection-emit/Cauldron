using Cauldron.Activator;
using System;
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
        private Action @delegate;
        private Func<Task> asyncDelegate;
        private bool isWaiting = false;
        private Task task;

        static CallOnce()
        {
            dispatcher = Factory.Create<IDispatcher>();
        }

        private CallOnce(Action @delegate)
        {
            this.@delegate = @delegate;
            this.task = new Task(() => { });
        }

        private CallOnce(Func<Task> asyncDelegate)
        {
            this.asyncDelegate = asyncDelegate;
            this.task = new Task(() => { });
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
        public static CallOnce Create(Action @delegate) => new CallOnce(@delegate);

        /// <summary>
        /// Creates a new instance of <see cref="CallOnce"/> that encapsulates a method that has no parameters
        /// and has no return value.
        /// </summary>
        /// <param name="delegate">The delegate to encapsulate.</param>
        /// <returns>A new instance of <see cref="CallOnce"/></returns>
        public static CallOnce Create(Func<Task> @delegate) => new CallOnce(@delegate);

        /// <summary>
        /// Invokes the encapsulated method.
        /// </summary>
        public void Invoke()
        {
            if (this.isWaiting)
                return;

            this.isWaiting = true;

            if (this.@delegate != null)
                dispatcher.RunAsync(DispatcherPriority.Low, () =>
                {
                    this.@delegate();
                    this.isWaiting = false;
                    this.Invoked?.Invoke(this, EventArgs.Empty);
                    this.task.Start();
                });
            else if (this.asyncDelegate != null)
                dispatcher.RunAsync(DispatcherPriority.Low, async () =>
                {
                    await this.asyncDelegate();
                    this.isWaiting = false;
                    this.Invoked?.Invoke(this, EventArgs.Empty);
                    this.task.Start();
                });
            else
                throw new NotImplementedException();
        }
    }
}