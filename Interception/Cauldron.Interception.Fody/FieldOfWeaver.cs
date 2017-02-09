using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public class FieldOfWeaver : ModuleWeaverBase
    {
        public FieldOfWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            if (!this.ModuleDefinition.AllReferencedAssemblies().Any(x => x.Name.Name == "Cauldron.Core"))
            {
                this.LogInfo("Skipping implementation of Cauldron.Core.Reflection.GetFieldInfo. Cauldron.Core is not referenced in the project");
                return;
            }

            this.LogInfo("Implementing Cauldron.Core.Reflection.GetFieldInfo");

            var fieldOf = "Cauldron.Core.Reflection".ToTypeDefinition().GetMethodReference("GetFieldInfo", 1);
            var allMethodsWithMethodOfCalls = this.GetMethodsWhere(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).FullName == fieldOf.FullName);
            var getFieldFromHandleRef = typeof(System.Reflection.FieldInfo).Import().GetMethodReference("GetFieldFromHandle", 2).Import();

            foreach (var method in allMethodsWithMethodOfCalls)
            {
                var processor = method.Method.Body.GetILProcessor();

                foreach (var getMethodBaseCall in method.Instruction)
                {
                    var fieldName = getMethodBaseCall.Previous.Operand as string;
                    FieldDefinition fieldDefinition = null;

                    // This is a concrete definition of the field with class and namespace
                    if (fieldName.Contains("."))
                    {
                        var typeName = fieldName.Substring(0, fieldName.LastIndexOf('.'));
                        var typeDefinition = typeName.ToTypeDefinition();

                        if (typeDefinition == null)
                        {
                            this.LogError($"Unable to resolve type '{typeName}'. The field '{fieldName}' cannot be processed.");

                            foreach (var uuu in this.ModuleDefinition.AllReferencedAssemblies())
                                this.LogInfo(uuu.FullName);

                            continue;
                        }

                        fieldDefinition = typeDefinition.Import().Resolve().Fields.FirstOrDefault(x => x.Name == fieldName.Split('.').Last());
                    }
                    else
                        fieldDefinition = method.Method.DeclaringType.Fields.FirstOrDefault(x => x.Name == fieldName);

                    if (fieldDefinition == null)
                        this.LogError($"Unable to find field '{fieldName}' GetFieldInfo in method '{method.Method.FullName}'");

                    processor.Remove(getMethodBaseCall.Previous);
                    processor.InsertBefore(getMethodBaseCall, processor.Create(OpCodes.Ldtoken, fieldDefinition));
                    processor.InsertBefore(getMethodBaseCall, processor.Create(OpCodes.Ldtoken, fieldDefinition.DeclaringType));
                    processor.InsertBefore(getMethodBaseCall, processor.Create(OpCodes.Call, getFieldFromHandleRef));
                    processor.Remove(getMethodBaseCall);
                }
            }
        }

        protected override void ImplementLockableOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim)
        {
            throw new NotImplementedException();
        }

        protected override void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable)
        {
            throw new NotImplementedException();
        }
    }
}