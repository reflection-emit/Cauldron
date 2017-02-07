using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Cauldron.Interception.Fody
{
    public sealed class MethodAndInstruction
    {
        public MethodAndInstruction(MethodDefinition method, IEnumerable<Instruction> instruction)
        {
            this.Method = method;
            this.Instruction = instruction;
        }

        public IEnumerable<Instruction> Instruction { get; private set; }

        public MethodDefinition Method { get; private set; }
    }
}