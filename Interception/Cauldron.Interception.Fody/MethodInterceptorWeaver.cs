using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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

        protected override void ImplementLockableOnEnter(ILProcessor processor, List<Instruction> onEnterInstructions, MethodDefinition method, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition methodBaseReference, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim)
        {
            // Implement fields
            if (method.IsStatic)
            {
                var cctor = this.GetOrCreateStaticConstructor(method.DeclaringType);

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
                foreach (var constructor in this.GetConstructors(method.DeclaringType))
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

            onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, attributeVariable));

            if (method.IsStatic)
                onEnterInstructions.Add(processor.Create(OpCodes.Ldnull));
            else
                onEnterInstructions.Add(processor.Create(OpCodes.Ldarg_0));

            onEnterInstructions.Add(processor.Create(OpCodes.Ldfld, semaphoreSlim));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType));
            onEnterInstructions.Add(processor.Create(OpCodes.Call, this.ModuleDefinition.Import(typeof(Type).GetMethodReference("GetTypeFromHandle", 1))));
            onEnterInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, methodBaseReference));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, parametersArrayVariable));
            onEnterInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnEnter));
        }

        //private void ImplementMethod(MethodDefinition method, CustomAttribute[] attributes)
        //{
        //    this.LogInfo($"Implenting Method interception: {method.Name} with {string.Join(", ", attributes.Select(x => x.AttributeType.FullName))}");
        //    var processor = method.Body.GetILProcessor();
        //    method.Body.SimplifyMacros();
        //    var attributeInitialization = new List<Instruction>();
        //    var onEnterInstructions = new List<Instruction>();
        //    var exceptionInstructions = new List<Instruction>();
        //    var finallyInstructions = new List<Instruction>();
        //    var originalBody = method.Body.Instructions.ToList();
        //    method.Body.Instructions.Clear();
        //    method.Body.InitLocals = true;
        //    var getMethodFromHandleRef = this.ModuleDefinition.Import(typeof(MethodBase).GetMethodReference("GetMethodFromHandle", 2));
        //    var methodBaseReference = new VariableDefinition(this.ModuleDefinition.Import(typeof(MethodBase).GetTypeReference()));

        //    this.GetConstructors(method.DeclaringType);

        //    processor.Body.Variables.Add(methodBaseReference);
        //    attributeInitialization.Add(processor.Create(OpCodes.Ldtoken, method));
        //    attributeInitialization.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType));
        //    attributeInitialization.Add(processor.Create(OpCodes.Call, getMethodFromHandleRef));
        //    attributeInitialization.Add(processor.Create(OpCodes.Stloc_S, methodBaseReference));

        //    // Create object array for the method args
        //    var objectReference = this.ModuleDefinition.Import(typeof(object).GetTypeReference());
        //    var parametersArrayTypeRef = new ArrayType(objectReference);
        //    var parametersArrayVariable = new VariableDefinition(parametersArrayTypeRef);
        //    processor.Body.Variables.Add(parametersArrayVariable);

        //    attributeInitialization.Add(processor.Create(OpCodes.Ldc_I4, method.Parameters.Count));
        //    attributeInitialization.Add(processor.Create(OpCodes.Newarr, objectReference));
        //    attributeInitialization.Add(processor.Create(OpCodes.Stloc, parametersArrayVariable));

        //    foreach (var parameter in method.Parameters)
        //        attributeInitialization.AddRange(IlHelper.ProcessParam(parameter, parametersArrayVariable));

        //    var exceptionReference = new VariableDefinition(this.ModuleDefinition.Import(typeof(Exception).GetTypeReference()));
        //    processor.Body.Variables.Add(exceptionReference);
        //    exceptionInstructions.Add(processor.Create(OpCodes.Stloc_S, exceptionReference));

        //    // Get attribute instance
        //    foreach (var attribute in attributes)
        //    {
        //        var isLockable = attribute.AttributeType.Resolve().Interfaces.Any(x => x.FullName == this.lockableMethodInterceptor);

        //        var methodInterceptorTypeRef = this.ModuleDefinition.Import(attribute.AttributeType);

        //        var interceptorOnEnter = isLockable ? this.ModuleDefinition.Import(methodInterceptorTypeRef.GetMethodReference("OnEnter", 5)) : this.ModuleDefinition.Import(methodInterceptorTypeRef.GetMethodReference("OnEnter", 4));
        //        var interceptorOnException = this.ModuleDefinition.Import(methodInterceptorTypeRef.GetMethodReference("OnException", 1));
        //        var interceptorOnExit = this.ModuleDefinition.Import(methodInterceptorTypeRef.GetMethodReference("OnExit", 0));

        //        var variableDefinition = new VariableDefinition(methodInterceptorTypeRef);
        //        processor.Body.Variables.Add(variableDefinition);

        //        foreach (var arg in attribute.ConstructorArguments)
        //            attributeInitialization.AddRange(AttributeParameterToOpCode(processor, arg));

        //        attributeInitialization.Add(processor.Create(OpCodes.Newobj, attribute.Constructor));
        //        attributeInitialization.Add(processor.Create(OpCodes.Stloc_S, variableDefinition));
        //        method.CustomAttributes.Remove(attribute);

        //        if (isLockable)
        //        {
        //            var semaphoreSlim = this.ImplementLockableOnEnter(processor, onEnterInstructions, method, variableDefinition, interceptorOnEnter, methodBaseReference, parametersArrayVariable);

        //            var endOfIf = processor.Create(OpCodes.Nop);
        //            finallyInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
        //            finallyInstructions.Add(processor.Create(OpCodes.Ldfld, semaphoreSlim));
        //            finallyInstructions.Add(processor.Create(OpCodes.Callvirt, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference("get_CurrentCount", 0))));
        //            finallyInstructions.Add(processor.Create(OpCodes.Ldc_I4_0));
        //            finallyInstructions.Add(processor.Create(OpCodes.Ceq));
        //            finallyInstructions.Add(processor.Create(OpCodes.Brfalse_S, endOfIf));

        //            finallyInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
        //            finallyInstructions.Add(processor.Create(OpCodes.Ldfld, semaphoreSlim));
        //            finallyInstructions.Add(processor.Create(OpCodes.Callvirt, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference("Release", 0))));
        //            finallyInstructions.Add(processor.Create(OpCodes.Pop));
        //            finallyInstructions.Add(endOfIf);
        //        }
        //        else
        //            this.ImplementOnEnter(processor, onEnterInstructions, method, variableDefinition, interceptorOnEnter, methodBaseReference, parametersArrayVariable);

        //        exceptionInstructions.Add(processor.Create(OpCodes.Ldloc_S, variableDefinition));
        //        exceptionInstructions.Add(processor.Create(OpCodes.Ldloc_S, exceptionReference));
        //        exceptionInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnException));

        //        finallyInstructions.Add(processor.Create(OpCodes.Ldloc_S, variableDefinition));
        //        finallyInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnExit));
        //    }

        //    exceptionInstructions.Add(processor.Create(OpCodes.Rethrow));
        //    finallyInstructions.Add(processor.Create(OpCodes.Endfinally));

        //    var lastReturn = processor.Create(OpCodes.Ret);

        //    var newBody = this.ReplaceReturn(processor, method, originalBody, lastReturn);
        //    originalBody = newBody.Item1;

        //    processor.Append(attributeInitialization);
        //    processor.Append(onEnterInstructions);
        //    processor.Append(originalBody);
        //    processor.Append(exceptionInstructions);
        //    processor.Append(finallyInstructions);
        //    processor.Append(lastReturn);

        //    if (newBody.Item2 != null)
        //        processor.InsertBefore(lastReturn, newBody.Item2);

        //    var exceptionHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
        //    {
        //        TryStart = onEnterInstructions.First(),
        //        TryEnd = exceptionInstructions.First(),
        //        HandlerStart = exceptionInstructions.First(),
        //        HandlerEnd = exceptionInstructions.Last().Next,
        //        CatchType = this.ModuleDefinition.Import(typeof(Exception))
        //    };
        //    var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
        //    {
        //        TryStart = onEnterInstructions.First(),
        //        TryEnd = finallyInstructions.First(),
        //        HandlerStart = finallyInstructions.First(),
        //        HandlerEnd = finallyInstructions.Last().Next,
        //    };
        //    method.Body.ExceptionHandlers.Add(exceptionHandler);
        //    method.Body.ExceptionHandlers.Add(finallyHandler);
        //    processor.Body.OptimizeMacros();
        //}

        protected override void ImplementOnEnter(ILProcessor processor, List<Instruction> onEnterInstructions, MethodDefinition method, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition methodBaseReference, VariableDefinition parametersArrayVariable, List<Instruction> originalBody)
        {
            onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, attributeVariable));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType));
            onEnterInstructions.Add(processor.Create(OpCodes.Call, this.ModuleDefinition.Import(typeof(Type).GetMethodReference("GetTypeFromHandle", 1))));
            onEnterInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, methodBaseReference));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, parametersArrayVariable));
            onEnterInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnEnter));
        }
    }
}