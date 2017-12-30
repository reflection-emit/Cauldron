using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Collections.IList")]
    public sealed class __IList : HelperTypeBase<__IList>
    {
        [HelperTypeMethod("Clear")]
        public Method Clear { get; private set; }
    }
}