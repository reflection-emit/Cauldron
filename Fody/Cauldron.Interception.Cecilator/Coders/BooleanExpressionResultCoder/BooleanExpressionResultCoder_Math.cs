using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed partial class BooleanExpressionResultCoder
    {
        public BooleanExpressionResultCoder Negate()
        {
            this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Neg));
            return this;
        }
    }
}