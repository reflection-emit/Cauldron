using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __IList : HelperTypeBase
    {
        public __IList(Builder builder) : base(builder, "System.Collections.IList")
        {
            this.Clear = this.type.GetMethod("Clear");
        }

        public Method Clear { get; private set; }
    }
}