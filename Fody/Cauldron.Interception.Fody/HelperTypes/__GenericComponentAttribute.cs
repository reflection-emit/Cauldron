using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Activator.GenericComponentAttribute")]
    public class __GenericComponentAttribute : HelperTypeBase<__GenericComponentAttribute>
    {
        [HelperTypeMethod("get_ContractName")]
        public Method ContractName { get; private set; }

        [HelperTypeMethod("get_Policy")]
        public Method Policy { get; private set; }

        [HelperTypeMethod("get_Priority")]
        public Method Priority { get; private set; }
    }
}