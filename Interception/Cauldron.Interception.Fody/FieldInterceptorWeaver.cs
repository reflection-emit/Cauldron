using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public class FieldInterceptorWeaver : ModuleWeaverBase
    {
        public FieldInterceptorWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            this.LogInfo("Implementing Field interceptors");
            var fieldInterceptorInterface = this.GetType("Cauldron.Core.Interceptors.IFieldInterceptor");

            if (fieldInterceptorInterface == null)
                throw new Exception($"Unable to find the interface Cauldron.Core.Interceptors.IFieldInterceptor.");

            var fieldInterceptors = fieldInterceptorInterface.GetTypesThatImplementsInterface();

            var fieldsAndAttributes = this.ModuleDefinition.Types.Where(x => x.HasFields).SelectMany(x => x.Fields).Where(x => x.HasCustomAttributes)
                .Select(x => new { Field = x, Attributes = x.CustomAttributes.Where(y => fieldInterceptors.Any(t => y.AttributeType.FullName == t.FullName)).ToArray() })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0)
                .ToArray();

            if (fieldsAndAttributes.Any(x => !x.Field.IsPrivate))
                throw new NotSupportedException("The FieldInterceptor does not support fields that are not private.");

            foreach (var field in fieldsAndAttributes.Where(x => x.Field.IsPrivate))
                this.ImplementField(field.Field, field.Attributes);
        }

        protected override void ImplementLockableOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim)
        {
            throw new NotImplementedException();
        }

        protected override void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable)
        {
            throw new NotImplementedException();
        }

        private void ImplementField(FieldDefinition field, CustomAttribute[] attributes)
        {
            this.LogInfo($"Implenting Field interception: {field.FullName} with {string.Join(", ", attributes.Select(x => x.AttributeType.FullName))}");

            var methodsWithLoadFields = this.GetMethodsWhere(x => (x.OpCode == OpCodes.Ldfld || x.OpCode == OpCodes.Ldsfld) && (x.Operand as FieldDefinition).FullName == field.FullName);
            var getFieldFromHandleRef = typeof(System.Reflection.FieldInfo).Import().GetMethodReference("GetFieldFromHandle", 2).Import();

            foreach (var method in methodsWithLoadFields)
            {
                var processor = method.Method.Body.GetILProcessor();

                foreach (var instruction in method.Instruction)
                {
                    var afterLoadField = instruction.Next;

                    processor.InsertBefore(afterLoadField, processor.Create(OpCodes.Ldtoken, field));
                    processor.InsertBefore(afterLoadField, processor.Create(OpCodes.Ldtoken, field.DeclaringType));
                    processor.InsertBefore(afterLoadField, processor.Create(OpCodes.Call, getFieldFromHandleRef));
                    processor.InsertAfter(instruction, processor.Create(OpCodes.Ldnull));
                }
            }
        }
    }
}