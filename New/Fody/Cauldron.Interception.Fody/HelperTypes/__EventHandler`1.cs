using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __EventHandler_1 : HelperTypeBase
    {
        public __EventHandler_1(Builder builder) : base(builder, "System.EventHandler`1")
        {
            this.Invoke = this.type.GetMethod("Invoke", 2);
        }

        public Method Invoke { get; private set; }
    }
}