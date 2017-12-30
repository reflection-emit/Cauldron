using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.IConstructorInterceptor")]
    public sealed class __IConstructorInterceptor : HelperTypeBase<__IConstructorInterceptor>
    {
        [HelperTypeMethod("OnBeforeInitialization", 3)]
        public Method OnBeforeInitialization { get; private set; }

        [HelperTypeMethod("OnEnter", 4)]
        public Method OnEnter { get; private set; }

        [HelperTypeMethod("OnException", 1)]
        public Method OnException { get; private set; }

        [HelperTypeMethod("OnExit")]
        public Method OnExit { get; private set; }
    }
}