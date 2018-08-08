using Cauldron.Activator;
using Cauldron.XAML.Threading;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Security.Permissions;

namespace Cauldron.XAML
{
    using Cauldron.XAML.Threading;

    /// <exclude />
    [Component(typeof(IDispatcher), FactoryCreationPolicy.Singleton, 10)]
    public sealed class DispatcherWPF
    {
        private static volatile DispatcherWPF current;
        private static object syncRoot = new object();
        private Dispatcher dispatcher;

        /// <exclude />
        [ComponentConstructor]
        public DispatcherWPF()
        {
            this.dispatcher = (Application.Current as Application).Dispatcher;
        }

        /// <summary>
        /// Gets the current instance of <see cref="DispatcherWPF"/>
        /// </summary>
        public static DispatcherWPF Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new DispatcherWPF();
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
        /// Implicitly converts a <see cref="Dispatcher"/> to a <see cref="DispatcherWPF"/>
        /// </summary>
        /// <param name="dispatcher"></param>
        public static implicit operator DispatcherWPF(Dispatcher dispatcher)
        {
            lock (syncRoot)
            {
                if (current == null)
                    current = new DispatcherWPF();

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
            this.dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Executes the specified delegate synchronously with the specified arguments, with priority
        /// <see cref="DispatcherPriority.Normal"/>, on the thread that the <see
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
        public void Run(DispatcherPriority priority, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            switch (priority)
            {
                case DispatcherPriority.Low:
                    this.dispatcher.Invoke(action, System.Windows.Threading.DispatcherPriority.Background);
                    break;

                case DispatcherPriority.High:
                    this.dispatcher.Invoke(action, System.Windows.Threading.DispatcherPriority.Send);
                    break;

                default:
                    this.dispatcher.Invoke(action, System.Windows.Threading.DispatcherPriority.Normal);
                    break;
            }
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
        public async Task RunAsync(DispatcherPriority priority, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            switch (priority)
            {
                case DispatcherPriority.Low:
                    await this.dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Background);
                    break;

                case DispatcherPriority.High:
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