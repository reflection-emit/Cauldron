using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class LocalVariableInstructionSet : AssignInstructionsSet<LocalVariable>, ILocalVariableCode
    {
        internal LocalVariableInstructionSet(InstructionsSet instructionsSet, LocalVariable target, InstructionContainer instructions, AssignInstructionType instructionType) : base(instructionsSet, target, instructions, instructionType)
        {
        }

        internal LocalVariableInstructionSet(InstructionsSet instructionsSet, IEnumerable<LocalVariable> targets, InstructionContainer instructions, AssignInstructionType instructionType) : base(instructionsSet, targets, instructions, instructionType)
        {
        }

        protected override TypeReference TargetType { get { return this.target.Last().variable.VariableType; } }

        protected override ILocalVariableCode CreateLocalVariableInstructionSet(LocalVariable localVariable, AssignInstructionType instructionType)
        {
            var newList = new List<LocalVariable>();
            newList.AddRange(this.target);
            newList.Add(localVariable);
            return new LocalVariableInstructionSet(this, newList, this.instructions, instructionType);
        }

        protected override void StoreCall()
        {
            if (this.instructionType == AssignInstructionType.Load)
                return;

            var last = this.target.Last();

            if (this.instructions.Count > 0 && this.instructions.Last().OpCode == OpCodes.Ldnull && last.Type.IsNullable)
            {
                this.instructions.RemoveLast();
                this.instructions.Append(processor.Create(OpCodes.Ldloca, last.variable));
                this.instructions.Append(processor.Create(OpCodes.Initobj, last.variable.VariableType));
            }
            else
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

        public static implicit operator string(LocalVariableInstructionSet value) => value.ToString();

        public static bool operator !=(LocalVariableInstructionSet a, LocalVariableInstructionSet b) => !(a == b);

        public static bool operator ==(LocalVariableInstructionSet a, LocalVariableInstructionSet b)
        {
            if (object.Equals(a, null) && object.Equals(b, null))
                return true;

            if (object.Equals(a, null))
                return false;

            return a.Equals(b);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is LocalVariableInstructionSet)
                return this.Equals(obj as LocalVariableInstructionSet);

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(FieldInstructionsSet other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return object.Equals(other, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.target.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.GetType().FullName;

        #endregion Equitable stuff
    }
}