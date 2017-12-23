using System;

namespace Cauldron.Interception
{
    /// <summary>
    /// Indicates the weaver to find the matching method and assign it to the <see cref="Action"/> or <see cref="Func{TResult}"/><br/>
    /// This attribute can only be applied to public non-static fields of type <see cref="Action"/> and <see cref="Func{TResult}"/>.<br/>
    /// If the field's type is an <see cref="Action"/>, the weaver will search for a void method without arguments.
    /// If the field's type in a <see cref="Func{TResult}"/>, the weaver will search
    /// for a method with a return type that matches TResult and no argmuents.
    /// <para/>
    /// This interceptor extension is available for <see cref="IPropertyGetterInterceptor"/>, <see cref="IPropertySetterInterceptor"/>,
    /// <see cref="IPropertyInterceptor"/>, <see cref="IMethodInterceptor"/> and <see cref="IConstructorInterceptor"/>.
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
        /// The <paramref name="methodName"/> can contain the following placeholders:
        /// <br/>
        /// {Name} - This will be replaced by the name of the property or method. A field requires a additional suffix 'Property'.
        /// <br/>
        /// {ReturnType} - This will be replaced by the return type of the property, method or field.
        /// </param>
        public AssignMethodAttribute(string methodName) => this.MethodName = methodName;

        /// <summary>
        /// Gets the name of the method to find.
        /// </summary>
        public string MethodName { get; private set; }
    }
}