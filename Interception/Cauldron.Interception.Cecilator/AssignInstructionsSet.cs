using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public abstract class AssignInstructionsSet<T> : InstructionsSet
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected readonly List<T> target = new List<T>();

        internal AssignInstructionsSet(InstructionsSet instructionsSet, T target, IEnumerable<Instruction> instructions) : base(instructionsSet, instructions)
        {
            this.target.Add(target);
        }

        internal AssignInstructionsSet(InstructionsSet instructionsSet, IEnumerable<T> targets, IEnumerable<Instruction> instructions) : base(instructionsSet, instructions)
        {
            this.target.AddRange(targets);
        }

        protected abstract TypeReference TargetType { get; }

        public ICode NewObj(Method constructor, params object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(false, this.processor, constructor.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.AddRange(inst.Instructions);
            }

            this.instructions.Add(processor.Create(OpCodes.Newobj, this.moduleDefinition.Import(constructor.methodReference)));
            this.StoreCall();
            return new InstructionsSet(this, this.instructions);
        }

        public ICode NewObj(AttributedField attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedMethod attribute) => this.NewObj(attribute.customAttribute);

        public ICode Set(object value)
        {
            var inst = this.AddParameter(this.processor.Body.Method.IsStatic, this.processor, this.TargetType, value);
            this.instructions.AddRange(inst.Instructions);
            this.StoreCall();
            return new InstructionsSet(this, this.instructions);
        }

        protected abstract override void StoreCall();
    }
}