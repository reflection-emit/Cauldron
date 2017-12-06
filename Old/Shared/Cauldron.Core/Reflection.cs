using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides useful reflection methods
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Returns the child type of a <see cref="IEnumerable{T}"/> or an array. The <see cref="Type"/> will be hardcoded in the dll by the Fody weaver. Use with caution!
        /// <para/>
        /// EXPERIMENTAL - Only tested for the case in the example
        /// </summary>
        /// <param name="type">The type to get the child from</param>
        /// <returns>The type of the child object</returns>
        /// <example>
        /// Take example of a collection implemented like following:
        /// <code>
        /// public class AClassThatInheritsFromDictionary : KeyedCollection&lt;int, string&gt;
        /// {
        ///     ...
        /// }
        /// </code>
        /// The following code will ...
        /// <code>
        /// var child = Reflection.ChildTypeOf(typeof(AClassThatInheritsFromDictionary));
        /// </code>
        /// ... be replace by
        /// <code>
        /// var child = typeof(string);
        /// </code>
        /// </example>
        public static Type ChildTypeOf(Type type) =>  /* NOTE: This will be implemented by Cauldron.Interception.Fody */  null;

        /// <summary>
        /// Returns field information of the field defined by <paramref name="fieldName"/>
        /// <para/>
        /// EXPERIMENTAL - Only tested for the case in the example
        /// </summary>
        /// <param name="fieldName">The name of the field</param>
        /// <returns>An instance of <see cref="FieldInfo"/> that contains the information about the field</returns>
        /// <example>
        /// nameof can be used for fields that resides on the same class.
        /// <code>
        /// var fieldInfo = Reflection.GetFieldInfo(nameof(myField));
        /// </code>
        /// For static fields from another classes:
        /// <code>
        /// var fieldInfo = Reflection.GetFieldInfo("TestNamespace.AnyClass.myField");
        /// </code>
        /// What gets compiled:
        /// <code>
        /// var fieldInfo = fieldof(myField);
        /// var fieldInfo = fieldof(TestNamespace.AnyClass.myField);
        /// </code>
        /// </example>
        public static FieldInfo GetFieldInfo(string fieldName) => /* NOTE: This will be implemented by Cauldron.Interception.Fody */ null;

        /// <summary>
        /// Returns method information of the current method represented by an instance of <see cref="MethodBase"/>
        /// </summary>
        /// <returns>An instance of <see cref="MethodBase"/> that contains information about the method.</returns>
        public static MethodBase GetMethodBase() => /* NOTE: This will be implemented by Cauldron.Interception.Fody */ null;
    }
}