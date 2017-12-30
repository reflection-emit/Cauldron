using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Activator.ComponentAttribute")]
    public class __ComponentAttribute : HelperTypeBase<__ComponentAttribute>
    {
        [HelperTypeMethod("get_ContractName")]
        public Method ContractName { get; private set; }

        [HelperTypeMethod("get_Policy")]
        public Method Policy { get; private set; }

        [HelperTypeMethod("get_Priority")]
        public Method Priority { get; private set; }
    }
}