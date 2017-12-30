using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.Extensions")]
    public sealed class __Extensions : HelperTypeBase<__Extensions>
    {
        [HelperTypeMethod("TryDisposeInternal", 1)]
        public Method TryDisposeInternal { get; private set; }
    }
}