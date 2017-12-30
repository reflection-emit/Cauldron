using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.IPropertyInterceptorInitialize")]
    public sealed class __IPropertyInterceptorInitialize : HelperTypeBase<__IPropertyInterceptorInitialize>
    {
        [HelperTypeMethod("OnInitialize", 2)]
        public Method OnInitialize { get; private set; }
    }
}