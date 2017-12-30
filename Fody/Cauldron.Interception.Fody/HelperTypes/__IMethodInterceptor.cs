using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.IMethodInterceptor")]
    public sealed class __IMethodInterceptor : HelperTypeBase<__IMethodInterceptor>
    {
        [HelperTypeMethod("OnEnter", 4)]
        public Method OnEnter { get; private set; }

        [HelperTypeMethod("OnException", 1)]
        public Method OnException { get; private set; }

        [HelperTypeMethod("OnExit")]
        public Method OnExit { get; private set; }
    }
}