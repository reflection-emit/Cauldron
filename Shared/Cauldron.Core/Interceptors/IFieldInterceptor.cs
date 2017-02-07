using System;
using System.Reflection;

namespace Cauldron.Core.Interceptors
{
    public interface IFieldInterceptor
    {
        void OnGet(FieldInfo fieldInfo, object fieldValue, Action<object> setValue);
    }
}