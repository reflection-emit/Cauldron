using Mono.Cecil;
using Mono.Cecil.Cil;
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

        public ICode NewObj(Method constructor, params object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(this.processor, constructor.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.Append(inst.Instructions);
            }

            this.instructions.Append(processor.Create(OpCodes.Newobj, this.moduleDefinition.Import(constructor.methodReference)));
            this.StoreCall();
            return new InstructionsSet(this, this.instructions);
        }

        public ICode NewObj(AttributedField attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedMethod attribute) => this.NewObj(attribute.customAttribute);

        public ICode Set(object value)
        {
            var inst = this.AddParameter(this.processor, this.TargetType, value);
            this.instructions.Append(inst.Instructions);
            this.StoreCall();
            return new InstructionsSet(this, this.instructions);
        }

        protected abstract override void StoreCall();
    }
}