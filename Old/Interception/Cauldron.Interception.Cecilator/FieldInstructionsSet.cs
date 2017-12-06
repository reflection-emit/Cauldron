using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class FieldInstructionsSet : AssignInstructionsSet<Field>, IFieldCode
    {
        internal FieldInstructionsSet(InstructionsSet instructionsSet, IEnumerable<Field> targets, InstructionContainer instructions, AssignInstructionType instructionType) : base(instructionsSet, targets, instructions, instructionType)
        {
        }

        internal FieldInstructionsSet(InstructionsSet instructionsSet, Field target, InstructionContainer instructions, AssignInstructionType instructionType) : base(instructionsSet, target, instructions, instructionType)
        {
        }

        protected override TypeReference TargetType { get { return this.target.Last().fieldRef.FieldType; } }

        protected override IFieldCode CreateFieldInstructionSet(Field field, AssignInstructionType instructionType)
        {
            var newList = new List<Field>();
            newList.AddRange(this.target);
            newList.Add(field);
            return new FieldInstructionsSet(this, newList, this.instructions, instructionType);
        }

        protected override void StoreCall()
        {
            if (this.instructionType == AssignInstructionType.Load)
                return;

            var field = this.target.Last();

            if (this.instructions.Last().OpCode == OpCodes.Ldnull && field.FieldType.IsNullable)
            {
                this.instructions.RemoveLast();
                this.instructions.Append(processor.Create(field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, field.fieldRef));
                this.instructions.Append(processor.Create(OpCodes.Initobj, field.fieldRef.FieldType));
            }
            else
                this.instructions.Append(processor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef));
        }

        #region Equitable stuff

        public static implicit operator string(FieldInstructionsSet value) => value.ToString();

        public static bool operator !=(FieldInstructionsSet a, FieldInstructionsSet b) => !(a == b);

        public static bool operator ==(FieldInstructionsSet a, FieldInstructionsSet b)
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

            if (obj is FieldInstructionsSet)
                return this.Equals(obj as FieldInstructionsSet);

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