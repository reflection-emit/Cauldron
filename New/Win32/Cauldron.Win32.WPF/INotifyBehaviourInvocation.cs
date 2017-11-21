using System;

namespace Cauldron.XAML
{
    /// <summary>
    /// Notifies the client that a behaviour should be invoked
    /// </summary>
    public interface INotifyBehaviourInvocation
    {
        /// <summary>
        /// Occures if a behaviour should be invoked
        /// </summary>
        event EventHandler<BehaviourInvocationArgs> BehaviourInvoke;
    }
}