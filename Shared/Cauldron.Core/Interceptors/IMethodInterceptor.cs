using System;

namespace Cauldron.Core.Interceptors
{
    public interface IMethodInterceptor
    {
        void OnEnter();

        void OnException(Exception e);

        void OnExit();
    }
}