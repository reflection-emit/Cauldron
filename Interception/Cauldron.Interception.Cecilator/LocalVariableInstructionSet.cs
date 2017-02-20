using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class LocalVariableInstructionSet : InstructionsSet, ILocalVariableCode
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<LocalVariable> localVariables = new List<LocalVariable>();

        internal LocalVariableInstructionSet(InstructionsSet instructionsSet, LocalVariable localVariable, IEnumerable<Instruction> instructions) : base(instructionsSet, instructions)
        {
            this.localVariables.Add(localVariable);
        }

        internal LocalVariableInstructionSet(InstructionsSet instructionsSet, IEnumerable<LocalVariable> localVariables, IEnumerable<Instruction> instructions) : base(instructionsSet, instructions)
        {
            this.localVariables.AddRange(localVariables);
        }

        public ICode NewObj(Method constructor, params object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(false, this.processor, constructor.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.AddRange(inst.Instructions);
            }

            this.instructions.Add(processor.Create(OpCodes.Newobj, this.moduleDefinition.Import(constructor.methodReference)));
            this.StoreToLocal();
            return new InstructionsSet(this, this.instructions);
        }

        public ICode NewObj(AttributedField attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedMethod attribute) => this.NewObj(attribute.customAttribute);

        public ICode Set(object value)
        {
            var inst = this.AddParameter(this.processor.Body.Method.IsStatic, this.processor, localVariables.Last().variable.VariableType, value);
            this.instructions.AddRange(inst.Instructions);
            this.StoreToLocal();
            return new InstructionsSet(this, this.instructions);
        }

        protected override ILocalVariableCode CreateLocalVariableInstructionSet(LocalVariable localVariable)
        {
            var newList = new List<LocalVariable>();
            newList.AddRange(this.localVariables);
            newList.Add(localVariable);
            return new LocalVariableInstructionSet(this, newList, this.instructions);
        }

        protected override void StoreCall() => this.StoreToLocal();

        private void StoreToLocal()
        {
            var last = this.localVariables.Last();

            switch (last.Index)
            {
                case 0: this.instructions.Add(processor.Create(OpCodes.Stloc_0)); break;
                case 1: this.instructions.Add(processor.Create(OpCodes.Stloc_1)); break;
                case 2: this.instructions.Add(processor.Create(OpCodes.Stloc_2)); break;
                case 3: this.instructions.Add(processor.Create(OpCodes.Stloc_3)); break;
                default:
                    this.instructions.Add(processor.Create(OpCodes.Stloc, last.variable));
                    break;
            }
        }

        #region Equitable stuff

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            return object.Equals(obj, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.localVariables.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.GetType().FullName;

        #endregion Equitable stuff
    }
}