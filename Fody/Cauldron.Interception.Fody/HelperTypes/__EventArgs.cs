using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __EventArgs : HelperTypeBase
    {
        public __EventArgs(Builder builder) : base(builder, "System.EventArgs")
        {
            this.Empty = this.type.GetField("Empty");
            this.Ctor = this.type.GetMethod(".ctor");
        }

        public Method Ctor { get; private set; }
        public Field Empty { get; private set; }
    }
}