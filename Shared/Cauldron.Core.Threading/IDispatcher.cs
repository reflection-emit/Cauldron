using System;
using System.Threading.Tasks;

namespace Cauldron.Core.Threading
{
    /// <summary>
    /// Represents a dispatcher. Implementations of this interface are responsible for processing the window messages and dispatching the events to the client.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Determines whether the calling thread has access to this <see cref="IDispatcher"/>
        /// </summary>
        bool HasThreadAccess { get; }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, with
        /// priority <see cref="DispatcherPriority.Normal"/>, on the thread that the <see
        /// cref="IDispatcher"/> was created on.
        /// </summary>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the <see cref="IDispatcher"/> event queue.
        /// </param>
        /// <returns>Returns a awaitable task</returns>
        Task RunAsync(Action action);

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, at the
        /// specified priority, on the thread that the <see cref="IDispatcher"/> was created on.
        /// </summary>
        /// <param name="priority">
        /// The priority, relative to the other pending operations in the <see cref="IDispatcher"/>
        /// event queue, the specified method is invoked.
        /// </param>
        /// <param name="action">
        /// The delegate to a method, which is pushed onto the <see cref="IDispatcher"/> event queue.
        /// </param>
        /// <returns>Returns a awaitable task</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        Task RunAsync(DispatcherPriority priority, Action action);
    }
}