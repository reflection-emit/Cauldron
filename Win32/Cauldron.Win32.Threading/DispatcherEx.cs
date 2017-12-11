using Cauldron.Activator;
using System;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Cauldron.Core.Threading
{
    /// <summary>
    /// Wrapper class for handling <see cref="Dispatcher.CurrentDispatcher"/>
    /// </summary>
    [Component(typeof(IDispatcher), FactoryCreationPolicy.Singleton)]
    public class DispatcherEx : IDispatcher
    {
        private Dispatcher dispatcher = null;

        /// <summary>
        /// Initializes a new instance of <see cref="Dispatcher"/>
        /// </summary>
        [ComponentConstructor]
        public DispatcherEx()
        {
        }

        /// <summary>
        /// Determines whether the calling thread has access to this <see cref="Dispatcher"/>
        /// </summary>
        public bool HasThreadAccess
        {
            get
            {
                if (this.dispatcher != null)
                    return dispatcher.CheckAccess();

                return true;
            }
        }

        /// <summary>
        /// Starts the dispatcher processing the input event queue for this instance of Dispatcher.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void ProcessEvents()
        {
            // http://stackoverflow.com/questions/1106881/using-the-wpf-dispatcher-in-unit-tests

            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, with
        /// priority <see cref="DispatcherPriority.Normal"/>, on the thread that the <see
        /// cref="Dispatcher"/> was created on.
        /// </summary>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the <see cref="Dispatcher"/> event queue.
        /// </param>
        /// <returns>Returns a awaitable task</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public async Task RunAsync(Action action)
        {
            if (this.dispatcher == null)
                return;

            await this.dispatcher.InvokeAsync(action, System.Windows.Threading.DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, at the
        /// specified priority, on the thread that the <see cref="Dispatcher"/> was created on.
        /// </summary>
        /// <param name="priority">
        /// The priority, relative to the other pending operations in the <see cref="Dispatcher"/>
        /// event queue, the specified method is invoked.
        /// </param>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the <see cref="Dispatcher"/> event queue.
        /// </param>
        /// <returns>Returns a awaitable task</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public async Task RunAsync(DispatcherPriority priority, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (this.dispatcher == null)
                return;

            switch (priority)
            {
                case DispatcherPriority.Idle:
                    await this.dispatcher.InvokeAsync(action, System.Windows.Threading.DispatcherPriority.ContextIdle);
                    break;

                case DispatcherPriority.Low:
                    await this.dispatcher.InvokeAsync(action, System.Windows.Threading.DispatcherPriority.Background);
                    break;

                case DispatcherPriority.High:
                    await this.dispatcher.InvokeAsync(action, System.Windows.Threading.DispatcherPriority.Send);
                    break;

                default:
                    await this.dispatcher.InvokeAsync(action, System.Windows.Threading.DispatcherPriority.Normal);
                    break;
            }
        }

        /// <summary>
        /// Invoked if the dispatcher is created.
        /// </summary>
        /// <returns>A new instance of <see cref="Dispatcher"/></returns>
        protected virtual Dispatcher OnCreateDispatcher() => Dispatcher.CurrentDispatcher;

        private static object ExitFrame(object frame)
        {
            (frame as DispatcherFrame).Continue = false;
            return null;
        }
    }
}