using Cauldron.Interception;
using System;

namespace Cauldron.Core.Interceptors
{
    /// <summary>
    /// Provides an interceptor that can invoke a method of the type instance.
    /// The method must have the following name 'On_____Set' where the blanks is the property's name.
    /// The method has to be void and parameterless.
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
    /// </example>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyOnSetAttribute : Attribute, IPropertySetterInterceptor
    {
        /// <exclude/>
        [AssignMethod("On{Name}Set")]
        public Action onPropertySet;

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
            return false;
        }
    }
}