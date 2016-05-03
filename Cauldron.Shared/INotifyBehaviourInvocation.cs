using System;

namespace Cauldron
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

    /// <summary>
    /// Provides data for the <see cref="INotifyBehaviourInvocation.BehaviourInvoke"/> event.
    /// </summary>
    public sealed class BehaviourInvocationArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BehaviourInvocationArgs"/>
        /// </summary>
        /// <param name="behaviourName"></param>
        public BehaviourInvocationArgs(string behaviourName)
        {
            this.BehaviourName = behaviourName;
        }

        /// <summary>
        /// Gets the name of the behaviour to be invoked
        /// </summary>
        public string BehaviourName { get; private set; }
    }
}