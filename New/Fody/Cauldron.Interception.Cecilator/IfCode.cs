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

        public IIfCode And
        {
            get { return this; }
        }

        public ICode Else
        {
            get { return new InstructionsSet(this, this.instructions); }
        }

        protected override Instruction JumpTarget
        {
            get
            {
                return this.jumpTarget;
            }
        }

        public ICode EndIf()
        {
            throw new NotImplementedException();
        }

        public IIfCode Then(Action<ICode> action)
        {
            action(this);
            this.instructions.Append(this.JumpTarget);

            return new IfCode(this, this.instructions, this.jumpTarget);
        }
    }
}