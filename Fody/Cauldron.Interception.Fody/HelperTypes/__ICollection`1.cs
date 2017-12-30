using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Collections.Generic.ICollection`1")]
    public sealed class __ICollection_1 : HelperTypeBase<__ICollection_1>
    {
        [HelperTypeMethod("Add", 1)]
        public Method Add { get; private set; }
    }
}