using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Activator.Factory")]
    public sealed class __Factory : HelperTypeBase<__Factory>
    {
        [HelperTypeMethod("OnObjectCreation", 2)]
        public Method OnObjectCreation { get; private set; }
    }
}