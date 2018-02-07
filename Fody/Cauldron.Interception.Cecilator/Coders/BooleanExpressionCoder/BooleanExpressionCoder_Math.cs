using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCoder
    {
        public BooleanExpressionCoder Negate()
        {
            this.coder.instructions.Emit(OpCodes.Neg);
            return this;
        }
    }
}