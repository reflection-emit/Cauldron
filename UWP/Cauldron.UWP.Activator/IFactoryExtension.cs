using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cauldron.Activator
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
        /// Returns true if a <see cref="Type"/> can modify arguments passed to <see cref="IFactoryExtension.ModifyArgument(ParameterInfo[], object[])"/> with this <see cref="IFactoryExtension"/> implementation
        /// </summary>
        /// <param name="method">The defined constructor defined by <see cref="ComponentConstructorAttribute"/></param>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        /// <returns>True if can be manipulated</returns>
        bool CanModifyArguments(MethodBase method, Type objectType);

        /// <summary>
        /// Modifies the arguments defined by <paramref name="arguments"/> and returns the modified array
        /// </summary>
        /// <param name="argumentTypes">The parameter info of the constructor</param>
        /// <param name="arguments">The arguments used to create an object</param>
        /// <returns>A modified array of arguments</returns>
        object[] ModifyArgument(ParameterInfo[] argumentTypes, object[] arguments);

        /// <summary>
        /// Occures when an object is created
        /// </summary>
        /// <param name="context">The object instance</param>
        /// <param name="objectType">The <see cref="Type"/> of the object created</param>
        void OnCreateObject(object context, Type objectType);

        /// <summary>
        /// Occures when <see cref="Factory"/> is initialized
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the object created</param>
        void OnInitialize(Type type);

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