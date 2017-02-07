using Cauldron.Core.Interceptors;
using System;
using System.Reflection;

namespace Cauldron.Interception.Test.Interceptors
{
    public sealed class TestFieldInterceptorAttribute : Attribute, IFieldInterceptor
    {
        public void OnGet(FieldInfo fieldInfo, object fieldValue, Action<object> setValue)
        {
            throw new NotImplementedException();
        }
    }
}