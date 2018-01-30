using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class LocalVariableAssignCoder : AssignCoder<LocalVariable>
    {
        internal LocalVariableAssignCoder(Coder coder, LocalVariable target) :
            base(coder, target)
        {
        }

        internal LocalVariableAssignCoder(LocalVariableAssignCoder coder, LocalVariable target) :
            base(coder.coder, target)
        {
        }

        public override TypeReference TargetType => this.target?.variable.VariableType;

        internal override void StoreCall()
        {
            var last = this.target;

            if (this.coder.instructions.Count > 0 && this.coder.instructions.LastOrDefault().OpCode == OpCodes.Ldnull && last.Type.IsNullable)
            {
                this.coder.instructions.RemoveLast();
                this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Ldloca, last.variable));
                this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Initobj, last.variable.VariableType));
            }
            else
                switch (last.Index)
                {
                    case 0: this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Stloc_0)); break;
                    case 1: this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Stloc_1)); break;
                    case 2: this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Stloc_2)); break;
                    case 3: this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Stloc_3)); break;
                    default:
                        this.coder.instructions.Append(this.coder.processor.Create(OpCodes.Stloc, last.variable));
                        break;
                }
        }
    }
}