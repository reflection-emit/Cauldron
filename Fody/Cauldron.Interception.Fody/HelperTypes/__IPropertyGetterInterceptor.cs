using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.IPropertyGetterInterceptor")]
    public sealed class __IPropertyGetterInterceptor : HelperTypeBase<__IPropertyGetterInterceptor>
    {
        [HelperTypeMethod("OnException", 1)]
        public Method OnException { get; private set; }

        [HelperTypeMethod("OnExit")]
        public Method OnExit { get; private set; }

        [HelperTypeMethod("OnGet", 2)]
        public Method OnGet { get; private set; }
    }
}