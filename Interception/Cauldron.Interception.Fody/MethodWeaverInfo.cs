using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class MethodWeaverInfo
    {
        public MethodWeaverInfo(MethodDefinition method)
        {
            this.Id = Guid.NewGuid().ToString().Replace('-', '_');
            this.MethodDefinition = method;
            this.OriginalBody = method.Body.Instructions.ToList();
            this.Processor = method.Body.GetILProcessor();
            this.LastReturn = this.Processor.Create(OpCodes.Ret);

            var fieldAttribute = FieldAttributes.Private;

            if (method.IsStatic)
                fieldAttribute |= FieldAttributes.Static;

            this.MethodBaseField = new FieldDefinition($"<{method.Name}>_methodBase_{this.Id}_field", fieldAttribute, typeof(System.Reflection.MethodBase).GetTypeReference().Import());
        }

        public List<Instruction> ExceptionInstructions { get; private set; } = new List<Instruction>();
        public List<Instruction> FinallyInstructions { get; private set; } = new List<Instruction>();
        public string Id { get; private set; }
        public List<Instruction> Initializations { get; private set; } = new List<Instruction>();
        public Instruction LastReturn { get; private set; }
        public FieldDefinition MethodBaseField { get; private set; }
        public MethodDefinition MethodDefinition { get; private set; }
        public List<Instruction> OnEnterInstructions { get; private set; } = new List<Instruction>();
        public List<Instruction> OriginalBody { get; set; }
        public ILProcessor Processor { get; private set; }

        public void Build()
        {
            this.Processor.Append(this.Initializations);
            this.Processor.Append(this.OnEnterInstructions);
            this.Processor.Append(this.OriginalBody);
            this.Processor.Append(this.ExceptionInstructions);
            this.Processor.Append(this.FinallyInstructions);
            this.Processor.Append(this.LastReturn);
        }
    }
}