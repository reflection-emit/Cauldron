using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator
{
    public class IfCode : InstructionsSet, IIfCode
    {
        private Instruction jumpTarget;

        internal IfCode(InstructionsSet instructionsSet, InstructionContainer instructions, Instruction jumpTarget) : base(instructionsSet, instructions)
        {
            this.jumpTarget = jumpTarget;
        }

        public IIfCode AndAnd => this;

        public ICode Else => new InstructionsSet(this, this.instructions);

        protected override Instruction JumpTarget => this.jumpTarget;

        public ICode EndIf()
        {
            this.instructions.Append(this.JumpTarget);
            return this;
        }

        public IIfCode Then(Action<ICode> action)
        {
            action(this);
            this.instructions.Append(this.JumpTarget);

            return new IfCode(this, this.instructions, this.jumpTarget);
        }
    }
}