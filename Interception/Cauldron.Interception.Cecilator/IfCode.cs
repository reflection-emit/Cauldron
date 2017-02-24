using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class IfCode : InstructionsSet, IIfCode
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Instruction jumpTarget;

        internal IfCode(InstructionsSet instructionsSet, InstructionContainer instructions, Instruction jumpTarget) : base(instructionsSet, instructions)
        {
            this.jumpTarget = jumpTarget;
        }

        public ICode Else
        {
            get { return this; }
        }

        public ICode EndIf()
        {
            throw new NotImplementedException();
        }

        public IIfCode Then(Action<ICode> action)
        {
            action(this);
            this.instructions.Append(this.jumpTarget);
            return this;
        }
    }
}