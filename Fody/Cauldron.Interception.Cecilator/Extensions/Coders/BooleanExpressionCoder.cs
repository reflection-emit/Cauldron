using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class BooleanExpressionCoder : ContextCoder
    {
        internal BooleanExpressionCoder(Coder coder) : base(coder)
        {
        }

        internal BooleanExpressionCoder(Coder coder, Instruction jumpTarget) : base(coder, jumpTarget)
        {
        }
    }
}