using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal class ParamResult
    {
        private TypeReference _type;
        public List<Instruction> Instructions { get; set; } = new List<Instruction>();

        public TypeReference Type
        {
            get { return this._type; }
            set
            {
                if (!value.IsGenericInstance && value.HasGenericParameters)
                    this._type = value.ResolveType(value);
                else
                    this._type = value;
            }
        }
    }
}