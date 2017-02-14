using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Threading;

namespace Cauldron.Interception.Fody
{
    public class MethodInterceptorWeaver : ModuleWeaverBase
    {
        private string lockableMethodInterceptor = "Cauldron.Interception.ILockableMethodInterceptor";
        private string methodInterceptor = "Cauldron.Interception.IMethodInterceptor";

        public MethodInterceptorWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            var methodInterceptorInterface = this.methodInterceptor.ToTypeDefinition();
            if (methodInterceptorInterface == null)
                throw new Exception($"Unable to find the interface {this.methodInterceptor}.");

            var methodInterceptors = methodInterceptorInterface.GetTypesThatImplementsInterface()
                .Concat(this.lockableMethodInterceptor.ToTypeDefinition().GetTypesThatImplementsInterface());
            // find all types with methods that are decorated with any of the found method interceptors
            var methodsAndAttributes = this.ModuleDefinition.Types.SelectMany(x => x.Methods).Where(x => x.HasCustomAttributes)
                .Select(x => new { Method = x, Attributes = x.CustomAttributes.Where(y => methodInterceptors.Any(t => y.AttributeType.FullName == t.FullName)).ToArray() })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0)
                .ToArray();
            foreach (var method in methodsAndAttributes)
                this.ImplementMethod(method.Method, method.Attributes, (r, isLockable) => isLockable ? r.GetMethodReference("OnEnter", 5) : r.GetMethodReference("OnEnter", 4));
        }

        protected override void ImplementLockableOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim)
        {
            methodWeaverInfo.DeclaringType.InsertToCtorOrCctor(methodWeaverInfo.IsStatic, (processor, instructions) =>
            {
                instructions.AddRange(processor.Newobj(semaphoreSlim, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference(".ctor", new Type[] { typeof(int), typeof(int) })), new object[] { 1, 1 }));
                return InsertionPosition.InsertBeforeBaseCall;
            });

            methodWeaverInfo.OnEnterInstructions.AddRange(methodWeaverInfo.Processor.Callvirt(attributeVariable, interceptorOnEnter, new object[] {
                    semaphoreSlim,
                    methodWeaverInfo.Processor.TypeOf(methodWeaverInfo.DeclaringType),
                    new This(),
                    methodWeaverInfo.MethodBaseField,
                    parametersArrayVariable
                }));
        }

        protected override void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable)
        {
            methodWeaverInfo.OnEnterInstructions.AddRange(methodWeaverInfo.Processor.Callvirt(attributeVariable, interceptorOnEnter, new object[] {
                    methodWeaverInfo.Processor.TypeOf(methodWeaverInfo.DeclaringType),
                    new This(),
                    methodWeaverInfo.MethodBaseField,
                    parametersArrayVariable
                }));
        }
    }
}