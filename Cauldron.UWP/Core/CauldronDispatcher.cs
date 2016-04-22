using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Cauldron.Core
{
    /// <summary>
    /// Wrapper class for the Application dispatcher object
    /// </summary>
    public sealed class CauldronDispatcher
    {
        private CoreDispatcher dispatcher;

        /// <summary>
        /// Initial
        /// </summary>
        public CauldronDispatcher()
        {
            this.dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        }

        /// <summary>
        /// Determines whether the calling thread has access to this <see cref="CoreDispatcher"/>
        /// </summary>
        public bool HasThreadAccess
        {
            get
            {
                return this.dispatcher.HasThreadAccess;
            }
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments,
        /// at the specified priority, on the thread that the System.Windows.Threading.Dispatcher
        /// was created on.
        /// </summary>
        /// <param name="action">The delegate to a method, which is pushed onto the <see cref="CoreDispatcher"/> event queue.</param>
        /// <returns>Returns a awaitable task</returns>
        public async Task RunAsync(Action action)
        {
            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
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
                    await this.dispatcher.RunAsync(CoreDispatcherPriority.Low, () => action());
                    break;

                case CauldronDispatcherPriority.High:
                    await this.dispatcher.RunAsync(CoreDispatcherPriority.High, () => action());
                    break;

                default:
                    await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
                    break;
            }
        }
    }
}