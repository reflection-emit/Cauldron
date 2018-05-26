using System;
using System.ComponentModel;

namespace Cauldron.Activator
{
    /// <summary>
    /// Holds data that is required by the <see cref="Factory"/> to create instances
    /// </summary>
    public interface IFactoryTypeInfo
    {
        /// <summary>
        /// The contract name associated with the type
        /// </summary>
        string ContractName { get; }

        /// <summary>
        /// The creation policy of the type
        /// </summary>
        FactoryCreationPolicy CreationPolicy { get; }

        /// <summary>
        /// Gets or sets the instance of a singleton
        /// </summary>
        object Instance { get; set; }

        /// <summary>
        /// Gets the priority of the component
        /// </summary>
        uint Priority { get; }

        /// <summary>
        /// The type to be constructed
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Returns an instance of the component
        /// </summary>
        /// <param name="arguments">The arguments required to create a type instance</param>
        /// <returns>The instance of the component</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        object CreateInstance(params object[] arguments);
    }
}