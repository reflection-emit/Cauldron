using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.PropertyInterceptionInfo")]
    public sealed class __PropertyInterceptionInfo : HelperTypeBase<__PropertyInterceptionInfo>
    {
        [HelperTypeMethod(".ctor", 7)]
        public Method Ctor { get; private set; }
    }
}