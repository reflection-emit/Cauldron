using System;

namespace Couldron
{
    /// <summary>
    /// Notifies the client that a behaviour should be invoked
    /// </summary>
    public interface INotifyBehaviourInvokation
    {
        /// <summary>
        /// Occures if a behaviour should be invoked
        /// </summary>
        event EventHandler<BehaviourInvokationArgs> BehaviourInvoke;
    }

    /// <summary>
    /// Provides data for the <see cref="INotifyBehaviourInvokation.BehaviourInvoke"/> event.
    /// </summary>
    public sealed class BehaviourInvokationArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BehaviourInvokationArgs"/>
        /// </summary>
        /// <param name="behaviourName"></param>
        public BehaviourInvokationArgs(string behaviourName)
        {
            this.BehaviourName = behaviourName;
        }

        /// <summary>
        /// Gets the name of the behaviour to be invoked
        /// </summary>
        public string BehaviourName { get; private set; }
    }
}