using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __ICollection_1 : HelperTypeBase
    {
        public __ICollection_1(Builder builder) : base(builder, "System.Collections.Generic.ICollection`1")
        {
            this.Add = this.type.GetMethod("Add", 1);
        }

        public Method Add { get; private set; }
    }
}