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
    /// <br/>
    /// These rules only decides whether an interceptor is weaved or not.
    /// <br/>
    /// An interceptor can have multiple rules.
    /// </summary>
    /// <example>
    /// The following example shows the usage of the <see cref="InterceptionRuleOptions.DoNotInterceptIfDecorated"/> rule option.
    /// <code>
    /// [InterceptionRule(InterceptionRuleOptions.DoNotInterceptIfDecorated, typeof(BroadcastChangeDoNotInterceptAttribute))]
    /// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class BroadcastChangeAttribute : Attribute, IPropertyInterceptor
    /// {
    ///     ...
    /// }
    /// </code>
    /// The attribute used to suppress the weaving of an interceptor can be any custom attribute.
    /// <code>
    /// [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class BroadcastChangeDoNotInterceptAttribute : Attribute
    /// {
    /// }
    /// </code>
    /// If the 'BroadcastChange' interceptor is applied to a class and you wish that one of its properties is not intercepted you can decorate that property with our defined 'BroadcastChangeDoNotIntercept' attribute.
    /// <code>
    /// [BroadcastChange]
    /// public class Human
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     public string Name { get; set; }
    ///     [BroadcastChangeDoNotIntercept]
    ///     public Dna DnaInformation { get; set; }
    /// }
    /// </code>
    /// The weaver will not weave the 'BroadcastChange' interceptor to the 'DnaInformation' property.
    /// <br/>
    /// The following will also end up not being weaved.
    /// <code>
    /// public class Human
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     public string Name { get; set; }
    ///
    ///     [BroadcastChange]
    ///     [BroadcastChangeDoNotIntercept]
    ///     public Dna DnaInformation { get; set; }
    /// }
    /// </code>
    /// The following is also possible:
    /// <code>
    /// [InterceptionRule(InterceptionRuleOptions.DoNotInterceptIfDecorated, typeof(BroadcastChangeDoNotInterceptAttribute))]
    /// [InterceptionRule(InterceptionRuleOptions.DoNotInterceptIfDecorated, typeof(GuidAttribute))]
    /// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class BroadcastChangeAttribute : Attribute, IPropertyInterceptor
    /// {
    ///     ...
    /// }
    /// </code>
    /// The weaver will not weave the 'BroadcastChange' interceptor to the 'DnaInformation' and the 'Name' property.
    /// <code>
    /// [BroadcastChange]
    /// public class Human
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     [Guid("BB8809B1-1B45-4F47-9C53-F8763A562C50")]
    ///     public string Name { get; set; }
    ///     [BroadcastChangeDoNotIntercept]
    ///     public Dna DnaInformation { get; set; }
    /// }
    /// </code>
    /// The following example shows the usage of the <see cref="InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass"/> rule option.
    /// <code>
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(INotifyPropertyChanged))]
    /// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class PropertyChangedAttribute : Attribute, IPropertyInterceptor
    /// {
    ///     ...
    /// }
    /// </code>
    /// This sample interceptor will be weaved if the class it is decorating is implementing the INotifyPropertyChanged interface.
    /// <code>
    /// [PropertyChanged]
    /// public class BreadViewModel : INotifyPropertyChanged
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// The weaver will also weave the following...
    /// <code>
    /// public class BreadViewModel : INotifyPropertyChanged
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     [PropertyChanged]
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// ... While the following is not weaved, because of the lacking interface.
    /// <code>
    /// public class BreadViewModel
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     [PropertyChanged]
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// The following example shows a combination of rules.
    /// <code>
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(INotifyDataErrorInfo))]
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(IDisposable))]
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(INotifyPropertyChanged), Mode.Required)]
    /// [InterceptionRule(InterceptionRuleOptions.DoNotInterceptIfDecorated, typeof(DoNotInterceptAttribute))]
    /// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class PropertyChangedAttribute : Attribute, IPropertyInterceptor
    /// {
    ///     ...
    /// }
    /// </code>
    /// The PropertyChanged interceptor in the following implementation example will be weaved in all properties except for the 'Name' property.
    /// <code>
    /// [PropertyChanged]
    /// public class BreadViewModel : INotifyPropertyChanged
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     [DoNotIntercept]
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// While the rules are specifying 3 different interfaces to be implemented, only the 'INotifyPropertyChanged' has the mode 'Required'.
    /// This means that the class has to implement 'INotifyPropertyChanged', which renders the other specified interface rules useless.
    /// <br/>
    /// The following will also end up not being weaved.
    /// <code>
    /// [PropertyChanged]
    /// public class BreadViewModel : INotifyDataErrorInfo
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     [DoNotIntercept]
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// If we remove the mode of the third interface rule...
    /// <code>
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(INotifyDataErrorInfo))]
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(IDisposable))]
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(INotifyPropertyChanged))]
    /// [InterceptionRule(InterceptionRuleOptions.DoNotInterceptIfDecorated, typeof(DoNotInterceptAttribute))]
    /// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class PropertyChangedAttribute : Attribute, IPropertyInterceptor
    /// {
    ///     ...
    /// }
    /// </code>
    /// ... And decorate the 'BreadViewModel' class with it; The weaver will now weave the 'PropertyChanged' interceptor.
    /// The rules are now specifying that the decorated class has to implement at least one of the 3 defined interfaces.
    /// <code>
    /// [PropertyChanged]
    /// public class BreadViewModel : INotifyDataErrorInfo
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     [DoNotIntercept]
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// Adding the mode 'Required' to all interface rules will specify that the decorated class has to implement all 3 interfaces or else the interceptor is not weaved.
    /// <code>
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(INotifyDataErrorInfo), Mode.Required)]
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(IDisposable), Mode.Required)]
    /// [InterceptionRule(InterceptionRuleOptions.IsImplementingInterfaceOrInheritBaseClass, typeof(INotifyPropertyChanged), Mode.Required)]
    /// [InterceptionRule(InterceptionRuleOptions.DoNotInterceptIfDecorated, typeof(DoNotInterceptAttribute))]
    /// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class PropertyChangedAttribute : Attribute, IPropertyInterceptor
    /// {
    ///     ...
    /// }
    /// </code>
    /// <code>
    /// [PropertyChanged]
    /// public class BreadViewModel : INotifyDataErrorInfo, IDisposable, INotifyPropertyChanged
    /// {
    ///     public double Height { get; set; }
    ///     public double Weight { get; set; }
    ///     [DoNotIntercept]
    ///     public string Name { get; set; }
    ///
    ///     ...
    /// }
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