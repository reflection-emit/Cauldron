using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public abstract class HelperTypeBase
    {
        protected readonly Builder builder;
        protected readonly BuilderType type;

        public HelperTypeBase(Builder builder, string typeFullname)
        {
            this.builder = builder;
            this.type = builder.GetType(typeFullname);
        }

        public BuilderType Type => this.type;
    }
}