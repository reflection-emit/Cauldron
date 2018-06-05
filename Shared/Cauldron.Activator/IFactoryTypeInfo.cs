using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cauldron.Activator
{
    /// <summary>
    /// Holds data that is required by the <see cref="Factory"/> to create instances
    /// </summary>
    public interface IFactoryTypeInfo
    {
        /// <summary>
        /// Gets the child type of the collection. null if <see cref="IsEnumerable"/> is false.
        /// </summary>
        Type ChildType { get; }

        /// <summary>
        /// The contract name associated with the type
        /// </summary>
        string ContractName { get; }

        /// <summary>
        /// The contract type of the component. Can be null if contract name is not a type.
        /// </summary>
        Type ContractType { get; }

        /// <summary>
        /// The creation policy of the type
        /// </summary>
        FactoryCreationPolicy CreationPolicy { get; }

        /// <summary>
        /// Gets or sets the instance of a singleton
        /// </summary>
        object Instance { get; set; }

        /// <summary>
        /// Gets a value that indicates if the described type is an array or implements <see cref="IEnumerable{T}"/> or <see cref="IEnumerable"/>.
        /// </summary>
        bool IsEnumerable { get; }

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

        /// <summary>
        /// Returns an instance of the component
        /// </summary>
        /// <returns>The instance of the component</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        object CreateInstance();
    }
}