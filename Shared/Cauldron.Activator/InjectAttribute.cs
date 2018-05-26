using System;
using System.Collections;

namespace Cauldron.Activator
{
    /// <summary>
    /// Specifies that the property or field contains a type that can be supplied by the <see cref="Factory"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class InjectAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InjectAttribute"/>
        /// </summary>
        public InjectAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InjectAttribute"/>
        /// </summary>
        /// <param name="arguments">
        /// The The arguments that can be used to initialize the instance.
        /// <para/>
        /// The arguments also supports referencing to a property or field. The property or field name has to be prefixed with [property] or [field]. Passing [this] will weave a "this" reference to the parameters.
        /// <example>
        /// An example on passing a property's value to an injection.
        /// <code>
        /// public string Name { get; set; }
        ///
        /// [Inject("[this]", "[property] Name")]
        /// public IShop Shop { get; }
        /// </code>
        /// An example on passing a field's value to an injection.
        /// <code>
        /// private string name;
        ///
        /// [Inject("[this]", "[field] name")]
        /// public IShop Shop { get; }
        /// </code>
        /// </example>
        /// </param>
        public InjectAttribute(params object[] arguments)
        {
        }

        /// <summary>
        /// Gets or sets the contract name of the component to inject.
        /// </summary>
        public string ContractName { get; set; }

        /// <summary>
        /// Gets or sets the contract type of the component to inject.
        /// </summary>
        public Type ContractType { get; set; }

        /// <summary>
        /// Gets or sets a value that tells the <see cref="Factory"/> not to inject many components even though the contract type is a subclass of <see cref="IEnumerable"/>.
        /// </summary>
        public bool ForceDontCreateMany { get; set; } = false;

        /// <summary>
        /// Gets or sets a value that indicates if the implementation with the highest priority is injected even though there are multiple implementations available.
        /// Equivalent to <see cref="Factory.CreateFirst(string, object[])"/>.
        /// </summary>
        public bool InjectFirst { get; set; } = false;

        /// <summary>
        /// Gets or sets a value that indicates if the injected values should be ordered by its priority. This has only an effect if multiple components are injected.
        /// Equivalent to <see cref="Factory.CreateManyOrdered(string, object[])"/>.
        /// </summary>
        public bool IsOrdered { get; set; } = false;

        /// <summary>
        /// Gets or sets a value that tells the weaver to implement a double check lock pattern.
        /// </summary>
        public bool MakeThreadSafe { get; set; } = false;
    }
}