using System;

namespace Cauldron.UnitTest.Interceptors.Method
{
    public interface IMethod_Interceptor_Invoke
    {
        void OnEnter(string name);

        void OnException(Exception e);

        void OnExit();
    }
}