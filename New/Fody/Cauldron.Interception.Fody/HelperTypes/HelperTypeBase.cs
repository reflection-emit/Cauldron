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

        public HelperTypeBase(Builder builder, string typeFullname1, string typeFullname2)
        {
            this.builder = builder;
            this.type = builder.GetType(builder.TypeExists(typeFullname1) ? typeFullname1 : typeFullname2);
        }

        public BuilderType Type => this.type;

        public void Import() => this.type.Import();
    }
}