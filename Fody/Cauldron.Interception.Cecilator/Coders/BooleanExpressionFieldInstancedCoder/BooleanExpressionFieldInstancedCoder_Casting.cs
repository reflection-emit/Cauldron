namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionFieldInstancedCoder : ICasting<BooleanExpressionFieldInstancedCoder>
    {
        public BooleanExpressionFieldInstancedCoder As(BuilderType type)
        {
            this.castToType = type;
            return this;
        }
    }
}