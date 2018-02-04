namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder : ICasting<BooleanExpressionCallCoder>
    {
        public BooleanExpressionCallCoder As(BuilderType type)
        {
            this.castToType = type;
            return this;
        }
    }
}