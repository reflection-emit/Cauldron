using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.ISyncRoot")]
    public sealed class __ISyncRoot : HelperTypeBase<__ISyncRoot>
    {
        [HelperTypeMethod("set_SyncRoot", 1)]
        public Method SyncRoot { get; private set; }
    }
}