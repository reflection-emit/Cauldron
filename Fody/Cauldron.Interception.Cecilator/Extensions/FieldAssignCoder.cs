using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class FieldAssignCoder : AssignCoder<Field>
    {
        internal FieldAssignCoder(Coder coder, Field target, AssignInstructionType instructionType) :
            base(coder, target, instructionType)
        {
        }

        internal FieldAssignCoder(Coder coder, IEnumerable<Field> targets, AssignInstructionType instructionType) :
            base(coder, targets, instructionType)
        {
        }

        public override TypeReference TargetType => this.target.Last().fieldRef.FieldType;

        protected override void StoreCall()
        {
            if (this.instructionType == AssignInstructionType.Load)
                return;

            var field = this.target.Last();

            if (this.instructions.LastOrDefault().OpCode == OpCodes.Ldnull && field.FieldType.IsNullable)
            {
                this.instructions.RemoveLast();
                this.instructions.Append(processor.Create(field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, field.fieldRef));
                this.instructions.Append(processor.Create(OpCodes.Initobj, field.fieldRef.FieldType));
            }
            else
                this.instructions.Append(processor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef));
        }
    }
}