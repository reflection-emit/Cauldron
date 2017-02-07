using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Threading;

namespace Cauldron.Interception.Fody
{
    public class MethodInterceptorWeaver : ModuleWeaverBase
    {
        private string lockableMethodInterceptor = "Cauldron.Core.Interceptors.ILockableMethodInterceptor";
        private string methodInterceptor = "Cauldron.Core.Interceptors.IMethodInterceptor";

        public MethodInterceptorWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            var methodInterceptorInterface = this.GetType(this.methodInterceptor);
            if (methodInterceptorInterface == null)
                throw new Exception($"Unable to find the interface {this.methodInterceptor}.");

            var methodInterceptors = methodInterceptorInterface.GetTypesThatImplementsInterface()
                .Concat(this.GetType(this.lockableMethodInterceptor).GetTypesThatImplementsInterface());
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
            // Implement fields
            if (methodWeaverInfo.MethodDefinition.IsStatic)
            {
                var cctor = this.GetOrCreateStaticConstructor(methodWeaverInfo.MethodDefinition.DeclaringType);

                var body = cctor.Body;
                var ctorProcessor = body.GetILProcessor();
                var first = body.Instructions.FirstOrDefault();

                ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldc_I4_1));
                ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldc_I4_1));
                ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Newobj, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference(".ctor", new Type[] { typeof(int), typeof(int) }))));
                ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Stsfld, semaphoreSlim));
            }
            else
            {
                foreach (var constructor in this.GetConstructors(methodWeaverInfo.MethodDefinition.DeclaringType))
                {
                    var body = constructor.Resolve().Body;
                    var ctorProcessor = body.GetILProcessor();
                    var first = body.Instructions.First();

                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldarg_0));
                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldc_I4_1));
                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldc_I4_1));
                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Newobj, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference(".ctor", new Type[] { typeof(int), typeof(int) }))));
                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Stfld, semaphoreSlim));
                }
            }

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldloc_S, attributeVariable));
            if (!methodWeaverInfo.MethodDefinition.IsStatic)
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, semaphoreSlim));
            }
            else
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, semaphoreSlim));

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldtoken, methodWeaverInfo.MethodDefinition.DeclaringType));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Call, this.ModuleDefinition.Import(typeof(Type).GetMethodReference("GetTypeFromHandle", 1))));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(methodWeaverInfo.MethodDefinition.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            if (!methodWeaverInfo.MethodDefinition.IsStatic)
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, methodWeaverInfo.MethodBaseField));
            }
            else
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, methodWeaverInfo.MethodBaseField));

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldloc_S, parametersArrayVariable));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Callvirt, interceptorOnEnter));
        }

        protected override void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable)
        {
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldloc_S, attributeVariable));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldtoken, methodWeaverInfo.MethodDefinition.DeclaringType));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Call, this.ModuleDefinition.Import(typeof(Type).GetMethodReference("GetTypeFromHandle", 1))));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(methodWeaverInfo.MethodDefinition.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            if (!methodWeaverInfo.MethodDefinition.IsStatic)
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, methodWeaverInfo.MethodBaseField));
            }
            else
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, methodWeaverInfo.MethodBaseField));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldloc_S, parametersArrayVariable));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Callvirt, interceptorOnEnter));
        }
    }
}