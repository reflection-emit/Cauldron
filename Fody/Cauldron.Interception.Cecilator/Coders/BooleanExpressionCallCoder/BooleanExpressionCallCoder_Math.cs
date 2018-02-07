using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder
    {
        public BooleanExpressionCoder Negate()
        {
            this.coder.CallInternal(this.instance, this.methodToCall, OpCodes.Call, this.parameters);
            this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Neg));
            return new BooleanExpressionCoder(coder);
        }
    }
}