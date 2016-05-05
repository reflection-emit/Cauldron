using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cauldron
{
    /// <summary>
    /// Represents an interface for the <see cref="Factory"/> extension
    /// </summary>
    public interface IFactoryExtension
    {
        /// <summary>
        /// Gets a value that indicates that this extension is able to resolve <see cref="AmbiguousMatchException"/>
        /// </summary>
        bool CanHandleAmbiguousMatch { get; }

        /// <summary>
        /// Returns true if a <see cref="Type"/> can be constructed with this <see cref="IFactoryExtension"/> implementation
        /// </summary>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <param name="objectTypeInfo">The <see cref="TypeInfo"/> of the object instance</param>
        /// <returns>True if can be constructed</returns>
        bool CanConstruct(Type objectType, TypeInfo objectTypeInfo);

        /// <summary>
        /// Creates an <see cref="object"/> described by <paramref name="objectType"/> or <paramref name="objectTypeInfo"/>.
        /// </summary>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <param name="objectTypeInfo">The <see cref="TypeInfo"/> of the object instance</param>
        /// <returns>The new instance of <paramref name="objectType"/> or <paramref name="objectTypeInfo"/></returns>
        object Construct(Type objectType, TypeInfo objectTypeInfo);

        /// <summary>
        /// Occures when an object is created
        /// </summary>
        /// <param name="context">The object instance</param>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <param name="objectTypeInfo">The <see cref="TypeInfo"/> of the object instance</param>
        void OnCreateObject(object context, Type objectType, TypeInfo objectTypeInfo);

        /// <summary>
        /// Occures when <see cref="Factory"/> is initialized
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> of the object instance</param>
        /// <param name="type">The <see cref="Type"/> of the object created</param>
        void OnInitialize(TypeInfo typeInfo, Type type);

        /// <summary>
        /// Occures if multiple Types with the same <paramref name="contractName"/> was found.
        /// <para/>
        /// Should return null if <paramref name="ambiguousTypes"/> collection does not contain the required <see cref="Type"/>
        /// </summary>
        /// <param name="ambiguousTypes">A collection of Types that with the same <paramref name="contractName"/></param>
        /// <param name="contractName">The contract name of the implementations</param>
        /// <returns>The selected <see cref="Type"/></returns>
        Type SelectAmbiguousMatch(IEnumerable<Type> ambiguousTypes, string contractName);
    }
}