using Cauldron.Interception;
using System;

namespace Cauldron.Core.Interceptors
{
    /// <summary>
    /// Provides an interceptor that can invoke a method of the type instance.<br/>
    /// The method must have the following name 'On_____Set' where the blanks is the property's name.<br/>
    /// The method can be void and parameterless or void and with 2 arguments of type <see cref="object"/>.
    /// The first argument is the old value and the second is the new value.
    /// </summary>
    /// <example>
    /// The following example shows the usage of the interceptor.
    /// <para/>
    /// If the property is named Title.
    /// <code>
    /// [PropertyOnSet]
    /// public string Title { get; set; }
    /// </code>
    /// The associated method has to be named like following:
    /// <code>
    /// private void OnTitleSet()
    /// {
    /// }
    /// </code>
    /// or
    /// <code>
    /// private void OnTitleSet(object oldValue, object newValue)
    /// {
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyOnSetAttribute : Attribute, IPropertySetterInterceptor
    {
        /// <exclude/>
        [AssignMethod("On{Name}Set")]
        public Action onPropertySet;

        /// <exclude/>
        [AssignMethod("On{Name}Set")]
        public Action<object, object> onPropertySetParametered;

        /// <exclude/>
        public void OnException(Exception e)
        {
        }

        /// <exclude/>
        public void OnExit()
        {
        }

        /// <exclude/>
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            this.onPropertySet?.Invoke();
            this.onPropertySetParametered?.Invoke(oldValue, newValue);
            return false;
        }
    }
}