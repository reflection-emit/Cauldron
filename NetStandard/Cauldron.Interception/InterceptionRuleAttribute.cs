using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Defines the interceptor weaving rules
    /// </summary>
    public enum InterceptionRuleOptions : byte
    {
        /// <summary>
        /// The interceptor will not be weaved if the member is decorated with a ceratain attribute.
        /// <para/>
        /// Requires an attribute type.
        /// </summary>
        DoNotInterceptIfDecorated = 0,

        /// <summary>
        /// The interceptor will only be weaved if the declaring type is implementing a certain interface or inheriting from a certain class.
        /// <para/>
        /// Requires an interface or class type and <see cref="Mode"/>.
        /// </summary>
        IsImplementingInterfaceOrInheritBaseClass = 1,
    }

    /// <summary>
    /// Defines weaving modes for the rule.
    /// </summary>
    public enum Mode : byte
    {
        /// <summary>
        /// This rule is not required to match for the interceptor to be weaved, if other rules matches.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// This rule is required to match for the interceptor to be weaved, regardless of other rules matching.
        /// </summary>
        Required = 1
    }

    /// <summary>
    /// Adds weaving rules to the interceptor.
    /// These rules only decides whether an interceptor is weaved or not.
    /// </summary>
    /// <example>
    ///
    /// <code>
    ///
    /// </code>
    /// </example>
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

        /// <summary>
        /// Initializes a new instance of <see cref="InterceptionRuleAttribute"/>.
        /// </summary>
        /// <param name="rule">
        /// Any of the values of <see cref="InterceptionRuleOptions"/>.
        /// </param>
        /// <param name="type">
        /// The type of attribute, interface or class associated to the <paramref name="rule"/>.
        /// </param>
        /// <param name="mode">
        /// Any of the values of <see cref="Mode"/>.
        /// </param>
        public InterceptionRuleAttribute(InterceptionRuleOptions rule, Type type, Mode mode)
        {
        }
    }
}