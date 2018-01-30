namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionFieldInstancedCoder
    {
        public BooleanExpressionFieldInstancedCoder Invert()
        {
            this.invert = true;
            return this;
        }
    }
}