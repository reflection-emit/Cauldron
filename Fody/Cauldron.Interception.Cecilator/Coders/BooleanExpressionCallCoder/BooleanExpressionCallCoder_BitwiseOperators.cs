namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder
    {
        public BooleanExpressionCallCoder Invert()
        {
            this.invert = true;
            return this;
        }
    }
}