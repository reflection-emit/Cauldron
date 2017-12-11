using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __Uri : HelperTypeBase
    {
        public __Uri(Builder builder) : base(builder, "System.Uri")
        {
            this.Ctor = this.type.GetMethod(".ctor", true, "System.String");
        }

        public Method Ctor { get; private set; }
    }
}