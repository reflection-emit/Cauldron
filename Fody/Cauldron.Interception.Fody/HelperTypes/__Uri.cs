using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Uri")]
    public sealed class __Uri : HelperTypeBase<__Uri>
    {
        [HelperTypeMethod(".ctor", "System.String")]
        public Method Ctor { get; private set; }
    }
}