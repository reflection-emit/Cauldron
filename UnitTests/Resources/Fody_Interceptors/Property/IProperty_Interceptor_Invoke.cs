using System;

namespace Cauldron.UnitTest.Interceptors.Property
{
    public interface IProperty_Interceptor_Invoke
    {
        void OnException(Exception e);

        void OnExit();

        void OnGet(string name, object value);

        void OnSet(string name, object oldValue, object newValue);
    }
}