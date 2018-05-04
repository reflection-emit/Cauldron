using System;

namespace Cauldron.Activator
{
    /// <summary>
    /// Provides meta information for the <see cref="Factory.ObjectCreated"/> event.
    /// </summary>
    public sealed class FactoryObjectCreatedEventArgs : EventArgs
    {
        internal FactoryObjectCreatedEventArgs(object @object, IFactoryTypeInfo factoryTypeInfo)
        {
            this.FactoryTypeInfo = factoryTypeInfo;
            this.ObjectInstance = @object;
        }

        /// <summary>
        /// Gets the factory type initialization information.
        /// </summary>
        public IFactoryTypeInfo FactoryTypeInfo { get; private set; }

        /// <summary>
        /// Gets the instance of the newly created object
        /// </summary>
        public object ObjectInstance { get; private set; }
    }
}