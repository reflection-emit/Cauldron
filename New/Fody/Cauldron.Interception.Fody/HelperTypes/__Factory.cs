using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __Factory : HelperTypeBase
    {
        public __Factory(Builder builder) : base(builder, "Cauldron.Activator.Factory")
        {
            this.OnObjectCreation = this.Type.GetMethod("OnObjectCreation", 2);
        }

        public Method OnObjectCreation { get; private set; }
    }
}