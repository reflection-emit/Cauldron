using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public class ChildOfWeaver : ModuleWeaverBase
    {
        public ChildOfWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            if (!this.ModuleDefinition.AllReferencedAssemblies().Any(x => x.Name.Name == "Cauldron.Core"))
            {
                this.LogInfo("Skipping implementation of Cauldron.Core.Reflection.ChildTypeOf. Cauldron.Core is not referenced in the project");
                return;
            }

            this.LogInfo("Implementing Cauldron.Core.Reflection.ChildTypeOf");

            var methodOf = "Cauldron.Core.Reflection".ToTypeDefinition().GetMethodReference("ChildTypeOf", 1);
            var allMethodsWithMethodOfCalls = this.GetMethodsWhere(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).FullName == methodOf.FullName);
            var getMethodFromHandleRef = typeof(System.Reflection.MethodBase).Import().GetMethodReference("GetMethodFromHandle", 2).Import();

            foreach (var method in allMethodsWithMethodOfCalls)
            {
                var processor = method.Method.Body.GetILProcessor();

                foreach (var getMethodBaseCall in method.Instruction)
                {
                    var ldToken = getMethodBaseCall.Previous.Previous;
                    var getTypeFromHandleCall = getMethodBaseCall.Previous;
                    TypeReference typeToken = getMethodBaseCall.Previous.Previous.Operand as TypeReference;

                    processor.Remove(ldToken);
                    processor.Remove(getTypeFromHandleCall);

                    processor.InsertBefore(getMethodBaseCall, processor.TypeOf(typeToken.GetChildrenType().Import()));
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