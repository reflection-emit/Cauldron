using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __PropertyInterceptionInfo : HelperTypeBase
    {
        public __PropertyInterceptionInfo(Builder builder) : base(builder, "Cauldron.Interception.PropertyInterceptionInfo")
        {
            this.Ctor = this.type.GetMethod(".ctor", 7);
        }

        public Method Ctor { get; private set; }
    }
}