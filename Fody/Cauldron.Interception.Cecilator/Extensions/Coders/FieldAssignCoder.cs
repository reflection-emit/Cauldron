using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public sealed class FieldAssignCoder : AssignCoder<Field>
    {
        internal FieldAssignCoder(Coder coder, Field target) :
            base(coder, target)
        {
        }

        internal FieldAssignCoder(FieldAssignCoder coder, Field target) :
            base(coder.coder, target)
        {
        }

        public override TypeReference TargetType => this.target?.fieldRef.FieldType;

        internal override void StoreCall()
        {
            var field = this.target;

            if (this.coder.instructions.LastOrDefault().OpCode == OpCodes.Ldnull && field.FieldType.IsNullable)
            {
                this.coder.instructions.RemoveLast();
                this.coder.instructions.Append(coder.processor.Create(field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, field.fieldRef));
                this.coder.instructions.Append(coder.processor.Create(OpCodes.Initobj, field.fieldRef.FieldType));
            }
            else
                this.coder.instructions.Append(coder.processor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef));
        }
    }
}