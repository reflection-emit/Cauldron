using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class FieldInstructionsSet : InstructionsSet, IFieldCode
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Field> fields = new List<Field>();

        internal FieldInstructionsSet(InstructionsSet instructionsSet, IEnumerable<Field> fields, IEnumerable<Instruction> instructions) : base(instructionsSet, instructions)
        {
            this.fields.AddRange(fields);
        }

        internal FieldInstructionsSet(InstructionsSet instructionsSet, Field field, IEnumerable<Instruction> instructions) : base(instructionsSet, instructions)
        {
            this.fields.Add(field);
        }

        public ICode NewObj(AttributedField attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(AttributedMethod attribute) => this.NewObj(attribute.customAttribute);

        public ICode NewObj(Method constructor, params object[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var inst = this.AddParameter(false, this.processor, constructor.methodDefinition.Parameters[i].ParameterType, parameters[i]);
                this.instructions.AddRange(inst.Instructions);
            }

            var field = this.fields.Last();

            this.instructions.Add(processor.Create(OpCodes.Newobj, this.moduleDefinition.Import(constructor.methodReference)));
            this.instructions.Add(processor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef));

            return new InstructionsSet(this, this.instructions);
        }

        public ICode Set(object value)
        {
            var field = this.fields.Last();
            var inst = this.AddParameter(field.IsStatic, this.processor, field.fieldRef.FieldType, value);
            this.instructions.AddRange(inst.Instructions);
            this.instructions.Add(processor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef));

            return new InstructionsSet(this, this.instructions);
        }

        protected override IFieldCode CreateFieldInstructionSet(Field field)
        {
            var newList = new List<Field>();
            newList.AddRange(this.fields);
            newList.Add(field);
            return new FieldInstructionsSet(this, newList, this.instructions);
        }

        protected override void StoreCall()
        {
            var field = this.fields.Last();
            this.instructions.Add(processor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef));
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
        public override int GetHashCode() => this.fields.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.GetType().FullName;

        #endregion Equitable stuff
    }
}