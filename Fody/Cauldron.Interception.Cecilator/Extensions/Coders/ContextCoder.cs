using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class ContextCoder
    {
        internal readonly Coder coder;
        internal readonly Instruction jumpTarget;

        internal ContextCoder(Coder coder)
        {
            this.coder = coder;
            this.jumpTarget = coder.processor.Create(OpCodes.Nop);
        }

        internal ContextCoder(Coder coder, Instruction jumpTarget)
        {
            this.coder = coder;
            this.jumpTarget = jumpTarget;
        }

        public void InstructionDebug() => this.coder.InstructionDebug();
    }
}