using System;

namespace Cauldron.Interception
{
    public enum InterceptionRuleOptions : byte
    {
        /// <summary>
        /// The interceptor will not be weaved if the member is decorated with a ceratain attribute.
        /// <para/>
        /// Requires an attribute type.
        /// </summary>
        DoNotInterceptIfDecorated = 0,

        /// <summary>
        /// The interceptor will only be weaved if the declaring type is implementing a certain interface.
        /// <para/>
        /// Requires an interface type.
        /// </summary>
        IsImplementingInterface = 1,

        /// <summary>
        /// The interceptor will only be weaved if the declaring type is inheriting from a certain class.
        /// <para/>
        /// Requires a class type.
        /// </summary>
        IsInheritingBaseClass = 2,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class InterceptionRuleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InterceptionRuleAttribute"/>.
        /// </summary>
        /// <param name="rule">
        /// Any of the values of <see cref="InterceptionRuleOptions"/>.
        /// </param>
        /// <param name="type">
        /// The type of attribute, interface or class associated to the <paramref name="rule"/>.
        /// </param>
        public InterceptionRuleAttribute(InterceptionRuleOptions rule, Type type)
        {
        }
    }
}