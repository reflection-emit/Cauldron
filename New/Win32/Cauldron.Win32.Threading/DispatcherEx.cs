using System;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Cauldron.Core.Threading
{
    /// <summary>
    /// Wrapper class for handling CoreDispatcher (WinRT) and DispatcherObject (Windows Desktop)
    /// </summary>
    public sealed class DispatcherEx
    {
        private static volatile DispatcherEx current;
        private static object syncRoot = new object();
        private Dispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of <see cref="DispatcherEx"/>
        /// </summary>
        public DispatcherEx()
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
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
        /// Implicitly converts a <see cref="Dispatcher"/> to a <see cref="DispatcherEx"/>
        /// </summary>
        /// <param name="dispatcher"></param>
        public static implicit operator DispatcherEx(Dispatcher dispatcher)
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
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void ProcessEvents()
        {
            // http://stackoverflow.com/questions/1106881/using-the-wpf-dispatcher-in-unit-tests

            var frame = new DispatcherFrame();
            this.dispatcher.Invoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Executes the specified delegate synchronously with the specified arguments, with priority
        /// <see cref="CoreDispatcherPriority.Normal"/>, on the thread that the <see
        /// cref="Dispatcher"/> was created on.
        /// </summary>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the <see cref="Dispatcher"/> event queue.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public void Run(Action action) => this.dispatcher.Invoke(action, System.Windows.Threading.DispatcherPriority.Normal);

        /// <summary>
        /// Executes the specified delegate synchronously with the specified arguments, at the
        /// specified priority, on the thread that the <see cref="Dispatcher"/> was created on.
        /// </summary>
        /// <param name="priority">
        /// The priority, relative to the other pending operations in the <see cref="Dispatcher"/>
        /// event queue, the specified method is invoked.
        /// </param>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the <see cref="Dispatcher"/> event queue.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public void Run(CoreDispatcherPriority priority, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            switch (priority)
            {
                case CoreDispatcherPriority.Low:
                    this.dispatcher.Invoke(action, System.Windows.Threading.DispatcherPriority.Background);
                    break;

                case CoreDispatcherPriority.High:
                    this.dispatcher.Invoke(action, System.Windows.Threading.DispatcherPriority.Send);
                    break;

                default:
                    this.dispatcher.Invoke(action, System.Windows.Threading.DispatcherPriority.Normal);
                    break;
            }
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, with
        /// priority <see cref="CoreDispatcherPriority.Normal"/>, on the thread that the <see
        /// cref="Dispatcher"/> was created on.
        /// </summary>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the <see cref="Dispatcher"/> event queue.
        /// </param>
        /// <returns>Returns a awaitable task</returns>
        public async Task RunAsync(Action action) => await this.dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Normal);

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
        public async Task RunAsync(CoreDispatcherPriority priority, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            switch (priority)
            {
                case CoreDispatcherPriority.Low:
                    await this.dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Background);
                    break;

                case CoreDispatcherPriority.High:
                    await this.dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Send);
                    break;

                default:
                    await this.dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Normal);
                    break;
            }
        }

        private static object ExitFrame(object frame)
        {
            (frame as DispatcherFrame).Continue = false;
            return null;
        }
    }
}