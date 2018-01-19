using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.Array")]
    public sealed class __Array : HelperTypeBase<__Array>
    {
        [HelperTypeMethod("get_Length")]
        public Method Length { get; private set; }
    }
}