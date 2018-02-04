using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class LocalVariableAssignCoder : LocalVariableContextCoder
    {
        internal LocalVariableAssignCoder(Coder coder, LocalVariable target) : base(coder, target)
        {
        }

        internal LocalVariableAssignCoder(LocalVariableAssignCoder coder, LocalVariable target) : base(coder.coder, target)
        {
        }

        public BuilderType TargetType => this.target?.Type;

        public LocalVariableContextCoder As(BuilderType type)
        {
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, type, this.target));
            return new LocalVariableContextCoder(this.coder, null);
        }

        public Coder Set(object value)
        {
            // value to assign
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, this.TargetType, value));

            // Store
            this.StoreCall();
            return this.coder;
        }

        public Coder Set(Func<Coder, object> valueToAssignToVariable)
        {
            // value to assign
            this.coder.instructions.Append(InstructionBlock.CreateCode(this.coder, this.TargetType, valueToAssignToVariable(coder.NewCoder())));

            // Store
            this.StoreCall();
            return coder;
        }

        private void StoreCall()
        {
            var last = this.target;

            if (this.coder.instructions.Count > 0 && this.coder.LastInstruction?.OpCode == OpCodes.Ldnull && last.Type.IsNullable)
            {
                this.coder.RemoveLast();
                this.coder.instructions.Emit(OpCodes.Ldloca, last.variable);
                this.coder.instructions.Emit(OpCodes.Initobj, last.variable.VariableType);
            }
            else
                switch (last.Index)
                {
                    case 0: this.coder.instructions.Emit(OpCodes.Stloc_0); break;
                    case 1: this.coder.instructions.Emit(OpCodes.Stloc_1); break;
                    case 2: this.coder.instructions.Emit(OpCodes.Stloc_2); break;
                    case 3: this.coder.instructions.Emit(OpCodes.Stloc_3); break;
                    default:
                        this.coder.instructions.Emit(OpCodes.Stloc, last.variable);
                        break;
                }
        }
    }
}