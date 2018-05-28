using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.ExtensionsReflection")]
    public sealed class __ExtensionsReflection : HelperTypeBase<__ExtensionsReflection>
    {
        [HelperTypeMethod("CreateInstance", 2)]
        public Method CreateInstance { get; private set; }
    }
}