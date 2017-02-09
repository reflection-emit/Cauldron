using System;
using System.Diagnostics;
using System.Reflection;

namespace Cauldron.Interception
{
    /// <summary>
    /// Contains information about the intercepted property
    /// </summary>
    public sealed class PropertyInterceptionInfo
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<object> _setter;

        /// <summary>
        /// Initializes a new instance of <see cref="PropertyInterceptionInfo"/>
        /// </summary>
        /// <param name="getterMethod">Provides information about the method</param>
        /// <param name="setterMethod">Provides information about the method</param>
        /// <param name="propertyName">The name of the intercepted property</param>
        /// <param name="propertyType">The return tyoe of the property</param>
        /// <param name="instance">The instance of the declaring type</param>
        /// <param name="setter">A delegate to set the property backing field</param>
        public PropertyInterceptionInfo(MethodBase getterMethod, MethodBase setterMethod, string propertyName, Type propertyType, object instance, Action<object> setter)
        {
            this._setter = setter;
            this.GetMethod = getterMethod;
            this.SetMethod = setterMethod;
            this.PropertyName = propertyName;
            this.PropertyType = propertyType;
            this.Instance = instance;
            this.DeclaringType = getterMethod.DeclaringType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the object where the property resides in
        /// </summary>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// Gets an object that provides information about the getter method
        /// </summary>
        public MethodBase GetMethod { get; private set; }

        /// <summary>
        /// Gets the instance of the declaring type
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the type of the property
        /// </summary>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets an object that provides information about the setter method
        /// </summary>
        public MethodBase SetMethod { get; private set; }

        /// <summary>
        /// Sets the value of the property's backing field
        /// </summary>
        /// <param name="value">The new value of the property</param>
        public void SetValue(object value) => this._setter(value);

        /// <summary>
        /// Converts the <see cref="PropertyInterceptionInfo"/> instance to a <see cref="PropertyInfo"/>
        /// </summary>
        /// <returns>A new instance of <see cref="PropertyInfo"/></returns>

#if NETCORE
        public PropertyInfo ToPropertyInfo() => this.DeclaringType.GetTypeInfo().GetProperty(this.PropertyName, BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
#else

        public PropertyInfo ToPropertyInfo() => this.DeclaringType.GetProperty(this.PropertyName, BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);

#endif
    }
}