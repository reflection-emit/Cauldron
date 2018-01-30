using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCoder
    {
        public BooleanExpressionCoder Negate()
        {
            this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Neg));
            return this;
        }
    }
}