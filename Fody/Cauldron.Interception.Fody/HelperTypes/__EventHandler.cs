using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.EventHandler")]
    public sealed class __EventHandler : HelperTypeBase<__EventHandler>
    {
        public __EventArgs EventArgs => new __EventArgs();

        [HelperTypeMethod("Invoke", 2)]
        public Method Invoke { get; private set; }
    }
}