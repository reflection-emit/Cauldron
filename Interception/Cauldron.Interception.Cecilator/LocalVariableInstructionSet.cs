using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class LocalVariableInstructionSet : AssignInstructionsSet<LocalVariable>, ILocalVariableCode
    {
        internal LocalVariableInstructionSet(InstructionsSet instructionsSet, LocalVariable target, InstructionContainer instructions) : base(instructionsSet, target, instructions)
        {
        }

        internal LocalVariableInstructionSet(InstructionsSet instructionsSet, IEnumerable<LocalVariable> targets, InstructionContainer instructions) : base(instructionsSet, targets, instructions)
        {
        }

        protected override TypeReference TargetType { get { return this.target.Last().variable.VariableType; } }

        protected override ILocalVariableCode CreateLocalVariableInstructionSet(LocalVariable localVariable)
        {
            var newList = new List<LocalVariable>();
            newList.AddRange(this.target);
            newList.Add(localVariable);
            return new LocalVariableInstructionSet(this, newList, this.instructions);
        }

        protected override void StoreCall()
        {
            var last = this.target.Last();

            switch (last.Index)
            {
                case 0: this.instructions.Append(processor.Create(OpCodes.Stloc_0)); break;
                case 1: this.instructions.Append(processor.Create(OpCodes.Stloc_1)); break;
                case 2: this.instructions.Append(processor.Create(OpCodes.Stloc_2)); break;
                case 3: this.instructions.Append(processor.Create(OpCodes.Stloc_3)); break;
                default:
                    this.instructions.Append(processor.Create(OpCodes.Stloc, last.variable));
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
        public override int GetHashCode() => this.target.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.GetType().FullName;

        #endregion Equitable stuff
    }
}