using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class FieldInstructionsSet : AssignInstructionsSet<Field>, IFieldCode
    {
        internal FieldInstructionsSet(InstructionsSet instructionsSet, Field target, InstructionContainer instructions) : base(instructionsSet, target, instructions)
        {
        }

        internal FieldInstructionsSet(InstructionsSet instructionsSet, IEnumerable<Field> targets, InstructionContainer instructions) : base(instructionsSet, targets, instructions)
        {
        }

        protected override TypeReference TargetType { get { return this.target.Last().fieldRef.FieldType; } }

        protected override IFieldCode CreateFieldInstructionSet(Field field)
        {
            var newList = new List<Field>();
            newList.AddRange(this.target);
            newList.Add(field);
            return new FieldInstructionsSet(this, newList, this.instructions);
        }

        protected override void StoreCall()
        {
            var field = this.target.Last();
            this.instructions.Append(processor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef));
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