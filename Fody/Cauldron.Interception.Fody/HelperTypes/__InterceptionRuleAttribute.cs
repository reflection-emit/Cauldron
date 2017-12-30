using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.InterceptionRuleAttribute")]
    public sealed class __InterceptionRuleAttribute : HelperTypeBase<__InterceptionRuleAttribute>
    {
        [HelperTypeMethod(".ctor", 2)]
        public Method Ctor { get; private set; }
    }
}