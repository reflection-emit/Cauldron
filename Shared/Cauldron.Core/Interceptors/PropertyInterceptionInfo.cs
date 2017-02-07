using System;
using System.Reflection;

namespace Cauldron.Core.Interceptors
{
    public sealed class PropertyInterceptionInfo
    {
        private Action<object> _setter;

        public PropertyInterceptionInfo(MethodBase method, string propertyName, Type propertyType, object instance, Action<object> setter)
        {
            this._setter = setter;
            this.Method = method;
            this.PropertyName = propertyName;
            this.PropertyType = propertyType;
            this.Instance = instance;
            this.DeclaringType = method.DeclaringType;
        }

        public Type DeclaringType { get; private set; }
        public object Instance { get; private set; }
        public MethodBase Method { get; private set; }
        public string PropertyName { get; private set; }
        public Type PropertyType { get; private set; }

        public void Set(object value) => this._setter(value);
    }
}