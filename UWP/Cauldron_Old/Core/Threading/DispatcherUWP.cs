using Cauldron.Activator;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Cauldron.Core.Threading
{
    /// <summary>
    /// Wrapper class for handling <see cref="CoreDispatcher"/>
    /// </summary>
    [Component(typeof(IDispatcher), FactoryCreationPolicy.Singleton)]
    public sealed class DispatcherUWP : IDispatcher
    {
        private CoreDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of <see cref="DispatcherUWP"/>
        /// </summary>
        [ComponentConstructor]
        public DispatcherUWP()
        {
            this.dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
        }

        /// <summary>
        /// Determines whether the calling thread has access to this Dispatcher
        /// </summary>
        public bool HasThreadAccess => this.dispatcher.HasThreadAccess;

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, with
        /// priority <see cref="CoreDispatcherPriority.Normal"/>, on the thread that the Dispatcher
        /// was created on.
        /// </summary>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the Dispatcher event queue.
        /// </param>
        /// <returns>Returns a awaitable task</returns>
        public async Task RunAsync(Action action) =>
                await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, at the
        /// specified priority, on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="priority">
        /// The priority, relative to the other pending operations in the Dispatcher event queue, the
        /// specified method is invoked.
        /// </param>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the Dispatcher event queue.
        /// </param>
        /// <returns>Returns a awaitable task</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public async Task RunAsync(DispatcherPriority priority, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (priority == DispatcherPriority.Idle)
                await this.dispatcher.RunIdleAsync(new IdleDispatchedHandler(x => action()));
            else
                await this.dispatcher.RunAsync((CoreDispatcherPriority)priority, () => action());
        }
    }
}