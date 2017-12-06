using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __ISyncRoot : HelperTypeBase
    {
        public const string TypeName = "Cauldron.Interception.ISyncRoot";

        public __ISyncRoot(Builder builder) : base(builder, TypeName)
        {
            this.SyncRoot = this.type.GetMethod("set_SyncRoot", 1);
        }

        public Method SyncRoot { get; private set; }
    }
}