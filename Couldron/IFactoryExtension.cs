using System;
using System.Reflection;

namespace Couldron
{
    /// <summary>
    /// Represents an interface for the <see cref="Factory"/> extension
    /// </summary>
    public interface IFactoryExtension
    {
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
    }
}