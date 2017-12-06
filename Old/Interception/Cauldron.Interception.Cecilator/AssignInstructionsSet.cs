using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public enum AssignInstructionType
    {
        Load,
        Store
    }

    public abstract class AssignInstructionsSet<T> : InstructionsSet
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly AssignInstructionType instructionType;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<T> target = new List<T>();

        internal AssignInstructionsSet(InstructionsSet instructionsSet, T target, InstructionContainer instructions, AssignInstructionType instructionType) : base(instructionsSet, instructions)
        {
            this.target.Add(target);
            this.instructionType = instructionType;
        }

        internal AssignInstructionsSet(InstructionsSet instructionsSet, IEnumerable<T> targets, InstructionContainer instructions, AssignInstructionType instructionType) : base(instructionsSet, instructions)
        {
            this.target.AddRange(targets);
            this.instructionType = instructionType;
        }

        protected abstract TypeReference TargetType { get; }

        public ICode Set(object value)
        {
            if (this.instructionType == AssignInstructionType.Load)
                this.LogWarning("This could provoke error 0x8013184B. Use Assign() instead of Load if you want to set a new value to a field, property or variable.");

            var inst = this.AddParameter(this.processor, this.TargetType, value);
            this.instructions.Append(inst.Instructions);

            this.StoreCall();
            return new InstructionsSet(this, this.instructions);
        }

        protected abstract override void StoreCall();
    }
}