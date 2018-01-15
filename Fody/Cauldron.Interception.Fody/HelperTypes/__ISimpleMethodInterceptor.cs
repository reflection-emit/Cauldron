using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.ISimpleMethodInterceptor")]
    public sealed class __ISimpleMethodInterceptor : HelperTypeBase<__ISimpleMethodInterceptor>
    {
        [HelperTypeMethod("OnEnter", 4)]
        public Method OnEnter { get; private set; }
    }
}