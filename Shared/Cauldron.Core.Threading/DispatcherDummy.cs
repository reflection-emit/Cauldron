using Cauldron.Activator;
using System;
using System.Threading.Tasks;

namespace Cauldron.Threading
{
    /// <summary>
    /// A dummy dispatcher that is used for Unit-tests
    /// </summary>
    [Component(typeof(IDispatcher), FactoryCreationPolicy.Singleton)]
    public sealed class DispatcherDummy : IDispatcher
    {
        /// <summary>
        /// Is always true
        /// </summary>
        public bool HasThreadAccess => true;

        /// <summary>
        /// Dummy run. Invokes the action.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <returns>A task</returns>
        public Task RunAsync(Action action)
        {
            action();
            return Task.FromResult(0);
        }

        /// <summary>
        /// Dummy run. Invokes the action.
        /// </summary>
        /// <param name="priority">
        /// Not used in this dummy implementation.
        /// </param>
        /// <param name="action">The action to invoke</param>
        /// <returns>A task</returns>
        public Task RunAsync(DispatcherPriority priority, Action action)
        {
            action();
            return Task.FromResult(0);
        }
    }
}