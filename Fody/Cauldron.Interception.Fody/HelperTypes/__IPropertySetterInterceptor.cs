using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.IPropertySetterInterceptor")]
    public sealed class __IPropertySetterInterceptor : HelperTypeBase<__IPropertySetterInterceptor>
    {
        [HelperTypeMethod("OnException", 1)]
        public Method OnException { get; private set; }

        [HelperTypeMethod("OnExit")]
        public Method OnExit { get; private set; }

        [HelperTypeMethod("OnSet", 3)]
        public Method OnSet { get; private set; }
    }
}