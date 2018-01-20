using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public class LocalVariableAssignCoder : AssignCoder<LocalVariable>
    {
        internal LocalVariableAssignCoder(Coder coder, LocalVariable target, AssignInstructionType instructionType) :
            base(coder, target, instructionType)
        {
        }

        internal LocalVariableAssignCoder(Coder coder, IEnumerable<LocalVariable> targets, AssignInstructionType instructionType) :
            base(coder, targets, instructionType)
        {
        }

        public override TypeReference TargetType => this.target.Last().variable.VariableType;

        protected override void StoreCall()
        {
            if (this.instructionType == AssignInstructionType.Load)
                return;

            var last = this.target.Last();

            if (this.instructions.Count > 0 && this.instructions.LastOrDefault().OpCode == OpCodes.Ldnull && last.Type.IsNullable)
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
    }
}