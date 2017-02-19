using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class InstructionsSet : CecilatorBase
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Instruction> instructions = new List<Instruction>();

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Method method;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ILProcessor processor;

        internal InstructionsSet(BuilderType type, Method method) : base(type)
        {
            this.method = method;
            this.processor = method.GetILProcessor();
        }

        internal InstructionsSet(InstructionsSet instructionsSet, IEnumerable<Instruction> instructions) : base(instructionsSet.method.DeclaringType)
        {
            this.method = instructionsSet.method;
            this.processor = instructionsSet.processor;
            this.instructions.AddRange(instructions);
        }

        public InstructionsSet AssignToField(string fieldName, object value)
        {
            if (!this.method.DeclaringType.Fields.Contains(fieldName))
                throw new KeyNotFoundException($"The field with the name '{fieldName}' does not exist in '{method.DeclaringType}'");

            var field = this.method.DeclaringType.Fields[fieldName];
            return this.AssignToField(field, value);
        }

        public InstructionsSet AssignToField(Field field, object value)
        {
            if (!field.IsStatic)
            {
                if (field.DeclaringType != this.method.DeclaringType)
                    throw new NotImplementedException();

                this.instructions.Add(processor.Create(OpCodes.Ldarg_0));
            }
            var inst = this.AddParameter(field.IsStatic, this.processor, field.fieldRef.FieldType, value);
            this.instructions.AddRange(inst.Instructions);
            this.instructions.Add(processor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef));

            return new InstructionsSet(this, this.instructions);
        }

        public void Insert(InsertionPosition position)
        {
            if (position == InsertionPosition.Beginning)
            {
                if (processor.Body.Instructions.Count == 0)
                    processor.Append(processor.Create(OpCodes.Ret));

                processor.InsertBefore(processor.Body.Instructions[0], this.instructions);
            }
            else
            {
            }
        }
    }
}