using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public class FieldAssignCoder : FieldContextCoder
    {
        internal FieldAssignCoder(Coder coder, Field target) : base(coder, target)
        {
        }

        internal FieldAssignCoder(FieldAssignCoder coder, Field target) : base(coder.coder, coder.autoAddThisInstance, target)
        {
        }

        internal FieldAssignCoder(Coder coder, bool autoAddThisInstance, Field target) : base(coder, autoAddThisInstance, target)
        {
        }

        public BuilderType TargetType => this.target?.FieldType;

        public FieldContextCoder As(BuilderType type)
        {
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, type, this.target));
            return new FieldContextCoder(this.coder, false, null);
        }

        public Coder Set(object value)
        {
            // Instance
            if (!this.target.IsStatic && this.autoAddThisInstance)
                this.coder.instructions.Emit(OpCodes.Ldarg_0);

            // value to assign
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, this.TargetType, value));

            // Store
            this.StoreCall();
            return this.coder;
        }

        public Coder Set(Func<Coder, object> valueToAssignToField)
        {
            // Instance
            if (!this.target.IsStatic && this.autoAddThisInstance)
                this.coder.instructions.Emit(OpCodes.Ldarg_0);

            // value to assign
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, this.TargetType, valueToAssignToField(this.coder.NewCoder())));

            // Store
            this.StoreCall();
            return this.coder;
        }

        private void StoreCall()
        {
            var field = this.target;

            if (this.coder.LastInstruction?.OpCode == OpCodes.Ldnull && field.FieldType.IsNullable)
            {
                this.coder.RemoveLast();
                this.coder.instructions.Emit(field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, field.fieldRef);
                this.coder.instructions.Emit(OpCodes.Initobj, field.fieldRef.FieldType);
            }
            else
                this.coder.instructions.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef);
        }
    }
}