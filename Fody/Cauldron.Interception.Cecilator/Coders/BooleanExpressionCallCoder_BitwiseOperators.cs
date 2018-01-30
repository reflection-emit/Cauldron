using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCallCoder
    {
        public BooleanExpressionCoder Invert()
        {
            this.coder.CallInternal(this.instance, this.calledMethod, OpCodes.Call, this.parameters);
            this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Ldc_I4_0));
            this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Ceq));
            return new BooleanExpressionCoder(coder);
        }
    }
}