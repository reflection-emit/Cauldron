using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("System.EventHandler`1")]
    public sealed class __EventHandler_1 : HelperTypeBase<__EventHandler_1>
    {
        [HelperTypeMethod("Invoke", 2)]
        public Method Invoke { get; private set; }
    }
}