namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed partial class BooleanExpressionResultCoder
    {
        public BooleanExpressionCoder AndAnd() => new BooleanExpressionCoder(this.coder, this.jumpTarget);
    }
}