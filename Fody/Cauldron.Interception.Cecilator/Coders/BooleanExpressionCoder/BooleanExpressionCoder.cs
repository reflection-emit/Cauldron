using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class BooleanExpressionCoder : BooleanExpressionContextCoder
    {
        internal BooleanExpressionCoder(Coder coder) : base(coder)
        {
        }

        internal BooleanExpressionCoder(Coder coder, Instruction jumpTarget) : base(coder, jumpTarget)
        {
        }

        internal BooleanExpressionCoder(BooleanExpressionCallCoder coder) : base(coder.coder, coder.jumpTarget)
        {
        }

        internal BooleanExpressionCoder(BooleanExpressionFieldInstancedCoder coder) : base(coder.coder, coder.jumpTarget)
        {
        }
    }
}