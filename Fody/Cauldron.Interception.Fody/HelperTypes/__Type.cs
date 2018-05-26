using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Type")]
    public sealed class __Type : HelperTypeBase<__Type>
    {
        [HelperTypeMethod("get_FullName")]
        public Method FullName { get; private set; }
    }
}