using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Cauldron.Core
{
    /// <summary>
    /// Wrapper class to handle CoreDispatcher (WinRT) and DispatcherObject (Windows Desktop)
    /// </summary>
    public sealed class DispatcherEx
    {
        private static volatile DispatcherEx current;
        private static object syncRoot = new object();
        private CoreDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of <see cref="DispatcherEx"/>
        /// </summary>
        private DispatcherEx()
        {
            this.dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
        }

        /// <summary>
        /// Gets the current instance of <see cref="DispatcherEx"/>
        /// </summary>
        public static DispatcherEx Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                        {
                            current = new DispatcherEx();
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// Determines whether the calling thread has access to this Dispatcher
        /// </summary>
        public bool HasThreadAccess
        {
            get
            {
                return this.dispatcher.HasThreadAccess;
            }
        }

        /// <summary>
        /// Implicitly converts the <see cref="CoreDispatcher"/> to <see cref="DispatcherEx"/>
        /// </summary>
        /// <param name="dispatcher"></param>
        public static implicit operator DispatcherEx(CoreDispatcher dispatcher)
        {
            lock (syncRoot)
            {
                if (current == null)
                    current = new DispatcherEx();

                current.dispatcher = dispatcher;
            }

            return current;
        }

        /// <summary>
        /// Starts the dispatcher processing the input event queue for this instance of CoreWindow.
        /// <para/>
        /// Use this method only in unit tests!
        /// Use the Dispatcher.ProcessEvents (UWP) in game loops directly.
        /// </summary>
        public void ProcessEvents()
        {
            // TODO - Bugged! Does not work well
            this.dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
             this.dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent));
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments,
        /// with priority <see cref="CoreDispatcherPriority.Normal"/>, on the thread that the Dispatcher
        /// was created on.
        /// </summary>
        /// <param name="action">The delegate to a method, which is pushed onto the Dispatcher event queue.</param>
        /// <returns>Returns a awaitable task</returns>
        public async Task RunAsync(Action action) =>
                await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments,
        /// at the specified priority, on the thread that the Dispatcher
        /// was created on.
        /// </summary>
        /// <param name="priority">The priority, relative to the other pending operations in the Dispatcher event queue, the specified method is invoked.</param>
        /// <param name="action">The delegate to a method, which is pushed onto the Dispatcher event queue.</param>
        /// <returns>Returns a awaitable task</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public async Task RunAsync(CoreDispatcherPriority priority, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (priority == CoreDispatcherPriority.Idle)
                await this.dispatcher.RunIdleAsync(new IdleDispatchedHandler(x => action()));
            else
                await this.dispatcher.RunAsync(priority, () => action());
        }
    }
}