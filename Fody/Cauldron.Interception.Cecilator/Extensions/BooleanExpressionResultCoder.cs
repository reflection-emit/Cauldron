using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public sealed class BooleanExpressionResultCoder
    {
        internal readonly Coder coder;

        internal readonly Instruction jumpTarget;

        internal BooleanExpressionResultCoder(Coder coder)
        {
            this.coder = coder;
            this.jumpTarget = coder.processor.Create(OpCodes.Nop);
        }

        public override int GetHashCode() => this.coder.GetHashCode();
    }
}