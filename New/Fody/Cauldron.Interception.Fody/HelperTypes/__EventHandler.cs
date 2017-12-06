using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __EventHandler : HelperTypeBase
    {
        public __EventHandler(Builder builder) : base(builder, "System.EventHandler")
        {
            this.Invoke = this.type.GetMethod("Invoke", 2);
            this.EventArgs = new __EventArgs(builder);
        }

        public __EventArgs EventArgs { get; private set; }
        public Method Invoke { get; private set; }
    }
}