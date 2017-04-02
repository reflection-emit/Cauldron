using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Indicates the weaver to find the matching method and assign it to the <see cref="Action"/> or <see cref="Func{TResult}"/>
    /// <br/>
    /// This attribute can only be applied to fields of type <see cref="Action"/> and <see cref="Func{TResult}"/>.
    /// <br/>
    /// If the field's type is an <see cref="Action"/>, the weaver will look for a void method with arguments matching the action's generic arguments.
    /// If the field's type in a <see cref="Func{TResult}"/>, the weaver will look for a method with a return type that matches TResult and arguments matching the func's generic arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AssignMethodAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AssignMethodAttribute"/>.
        /// </summary>
        /// <param name="methodName">The name of the method to find and assign to the field.</param>
        public AssignMethodAttribute(string methodName)
        {
            this.MethodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AssignMethodAttribute"/>.
        /// The default method name is the intercepted method's, field's or property's name plus the prefix "On".
        /// For example if the intercepted method's name is GetHashCode, the name of the method to find is OnGetHashCode.
        /// </summary>
        public AssignMethodAttribute()
        {
        }

        /// <summary>
        /// Gets the name of the method to find.
        /// </summary>
        public string MethodName { get; private set; }
    }
}