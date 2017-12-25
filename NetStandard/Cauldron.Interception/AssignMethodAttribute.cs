using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Indicates the weaver to find the matching method and assign it to the <see cref="Action"/> or <see cref="Func{TResult}"/><br/>
    /// This attribute can only be applied to public non-static fields of type <see cref="Action"/> and <see cref="Func{TResult}"/>; with or without arguments.<br/>
    /// If the field's type is an <see cref="Action"/>, the weaver will search for a void method without arguments.<br/>
    /// If the field's type is an <see cref="Action{T}"/>, the weaver will search for a void method with 1 argument.<br/>
    /// If the field's type in a <see cref="Func{TResult}"/>, the weaver will search for a method with a return type that matches TResult and no argmuents.<br/>
    /// If the field's type in a <see cref="Func{T, TResult}"/>, the weaver will search for a method with a return type that matches TResult and 1 argmuent.
    /// <para/>
    /// This interceptor extension is available for <see cref="IPropertyGetterInterceptor"/>, <see cref="IPropertySetterInterceptor"/>,
    /// <see cref="IPropertyInterceptor"/>, <see cref="IMethodInterceptor"/> and <see cref="IConstructorInterceptor"/>.
    /// <para/>
    /// Since properties are only methods, the <see cref="AssignMethodAttribute"/> can also search for the property's getter and setter.
    /// The getter requires a 'get_' prefix and the setter a 'set_' prefix e.g. if the property is named 'DispatchDate' then the setter search pattern will be
    /// 'set_DispatchDate' and the setter's search pattern will be 'get_DispatchDate'.
    /// </summary>
    /// <example>
    /// The following sample implementation will execute a method if a property setter is invoked.
    /// The method to execute is described by the constructor parameter 'On{Name}Set'.
    /// '{Name}' is a placeholder and will be replaced by the property's name.
    /// <code>
    /// [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class PropertyOnSetAttribute : Attribute, IPropertySetterInterceptor
    /// {
    ///     [AssignMethod("On{Name}Set")]
    ///     public Action onPropertySet;
    ///
    ///     public void OnException(Exception e)
    ///     {
    ///     }
    ///
    ///     public void OnExit()
    ///     {
    ///     }
    ///
    ///     public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
    ///     {
    ///         this.onPropertySet?.Invoke();
    ///         return false;
    ///     }
    /// }
    /// </code>
    /// The following is a sample usage of the interceptor above.
    /// <code>
    /// [PropertyOnSet]
    /// public DateTime DispatchDate { get; set; }
    ///
    /// private void OnDispatchDateSet()
    /// {
    ///    // Your code that is executed if a value is assigned to the DispatchDate property.
    /// }
    /// </code>
    /// <br/>
    /// The following sample interceptor implementation accepts a custom method name.
    /// <code>
    /// [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    /// public sealed class PropertyOnSetAttribute : Attribute, IPropertySetterInterceptor
    /// {
    ///     [AssignMethod("{CtorArgument:0}")]
    ///     public Action&gt;string, object, object&lt; onPropertySet;
    ///
    ///     public PropertyOnSetAttribute(string onSetPropertyMethod)
    ///     {
    ///     }
    ///
    ///     public void OnException(Exception e)
    ///     {
    ///     }
    ///
    ///     public void OnExit()
    ///     {
    ///     }
    ///
    ///     public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
    ///     {
    ///         this.onPropertySet?.Invoke(propertyInterceptionInfo.PropertyName, oldValue, newValue);
    ///         return false;
    ///     }
    /// }
    /// </code>
    /// The '{CtorArgument:0}' placeholder tells the weaver that the name (or part of the name) of the method to assign to the
    /// 'onPropertySet' field is described by the the first constructor parameter of the 'PropertyOnSetAttribute'.
    /// <br/>
    /// The following is a sample usage of the interceptor above.
    /// <code>
    /// [PropertyOnSet(nameof(AnyMethodToExecute))]
    /// public DateTime DispatchDate { get; set; }
    ///
    /// private void AnyMethodToExecute(string propertyName, object oldValue, object newValue)
    /// {
    ///    // Your code that is executed if a value is assigned to the DispatchDate property.
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AssignMethodAttribute : Attribute, IInterceptor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AssignMethodAttribute"/>.
        /// </summary>
        /// <param name="methodName">
        /// The name of the method to find and assign to the field.
        /// <para/>
        /// <paramref name="optional"/>
        /// If false throws an error if the method decribed by <paramref name="methodName"/> is not found; otherwise no error is thrown.
        /// </param>
        /// The <paramref name="methodName"/> can contain the following placeholders:
        /// <br/>
        /// {Name} - This will be replaced by the name of the property or method. A field requires a additional suffix 'Property'.
        /// <br/>
        /// {ReturnType} - This will be replaced by the return type of the property, method or field.
        /// <br/>
        /// {CtorArgument:index} - This will be replaced by the string passed to the interceptor. The index is a 32-bit unsigned integer 0-based index of the constructor arguments.
        /// For usage examples see <see cref="AssignMethodAttribute"/>.
        /// </param>
        public AssignMethodAttribute(string methodName, bool optional = false) => this.MethodName = methodName;

        /// <summary>
        /// Gets the name of the method to find.
        /// </summary>
        public string MethodName { get; private set; }
    }
}