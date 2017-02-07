using System.Reflection;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides useful reflection methods
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Returns field information of the field defined by <paramref name="fieldName"/>
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
        /// </example>
        public static FieldInfo GetFieldInfo(string fieldName) => /* NOTE: This will be implemented by Cauldron.Interception.Fody */ null;

        /// <summary>
        /// Returns method information of the current method represented by an instance of <see cref="MethodBase"/>
        /// </summary>
        /// <returns>An instance of <see cref="MethodBase"/> that contains information about the method.</returns>
        public static MethodBase GetMethodBase() => /* NOTE: This will be implemented by Cauldron.Interception.Fody */ null;
    }
}