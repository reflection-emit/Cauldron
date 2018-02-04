namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder
    {
        public BooleanExpressionCallCoder Call(Method method, params object[] parameters)
            => new BooleanExpressionCallCoder(this.coder, this.jumpTarget, this.coder.ToCodeBlock(), method, parameters);
    }
}