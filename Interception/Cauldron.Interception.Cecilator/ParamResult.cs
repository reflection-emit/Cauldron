using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class ParamResult
    {
        public List<Instruction> Instructions { get; set; } = new List<Instruction>();

        public TypeReference Type { get; set; }
    }
}