using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Cauldron.Core
{
    /// <summary>
    /// Wrapper class for the Application dispatcher object
    /// </summary>
    public sealed class CauldronDispatcher
    {
        private Dispatcher dispatcher;

        /// <summary>
        /// Initial
        /// </summary>
        public CauldronDispatcher()
        {
            this.dispatcher = Application.Current == null ? Dispatcher.CurrentDispatcher : Application.Current.Dispatcher;
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
        /// Executes the specified delegate asynchronously with the specified arguments,
        /// at the specified priority, on the thread that the System.Windows.Threading.Dispatcher
        /// was created on.
        /// </summary>
        /// <param name="action">The delegate to a method, which is pushed onto the <see cref="Dispatcher"/> event queue.</param>
        /// <returns>Returns a awaitable task</returns>
        public async Task RunAsync(Action action)
        {
            await this.dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments,
        /// at the specified priority, on the thread that the System.Windows.Threading.Dispatcher
        /// was created on.
        /// </summary>
        /// <param name="priority">The priority, relative to the other pending operations in the <see cref="Dispatcher"/> event queue, the specified method is invoked.</param>
        /// <param name="action">The delegate to a method, which is pushed onto the <see cref="Dispatcher"/> event queue.</param>
        /// <returns>Returns a awaitable task</returns>
        public async Task RunAsync(CauldronDispatcherPriority priority, Action action)
        {
            switch (priority)
            {
                case CauldronDispatcherPriority.Low:
                    await this.dispatcher.BeginInvoke(action, DispatcherPriority.Background);
                    break;

                case CauldronDispatcherPriority.High:
                    await this.dispatcher.BeginInvoke(action, DispatcherPriority.Send);
                    break;

                default:
                    await this.dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
                    break;
            }
        }
    }
}