using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public sealed class BooleanExpressionResultCoder : ContextCoder
    {
        internal bool isBrTrue = false;

        internal BooleanExpressionResultCoder(Coder coder, Instruction jumpTarget, bool isBrTrue) : base(coder, jumpTarget) => this.isBrTrue = isBrTrue;

        internal BooleanExpressionResultCoder(Coder coder, Instruction jumpTarget) : base(coder, jumpTarget)
        {
        }

        public CodeBlock ToCodeBlock() => new InstructionsCodeBlock(this.coder);

        internal void RemoveJump()
        {
            this.coder.instructions.RemoveLast();

            if (this.isBrTrue)
                this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Neg));
        }
    }
}