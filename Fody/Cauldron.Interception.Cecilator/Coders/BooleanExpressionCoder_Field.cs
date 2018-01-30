namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCoder
    {
        public BooleanExpressionFieldInstancedCoder Load(Field value) => new BooleanExpressionFieldInstancedCoder(this, value);
    }
}