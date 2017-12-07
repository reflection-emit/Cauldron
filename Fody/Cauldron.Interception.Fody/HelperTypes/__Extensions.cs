using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __Extensions : HelperTypeBase
    {
        public __Extensions(Builder builder) : base(builder, "Cauldron.Interception.Extensions")
        {
            this.TryDisposeInternal = this.type.GetMethod("TryDisposeInternal", 1);
            this.RelativeUri = this.type.GetMethod("RelativeUri", 1);
        }

        public Method RelativeUri { get; private set; }
        public Method TryDisposeInternal { get; private set; }
    }
}