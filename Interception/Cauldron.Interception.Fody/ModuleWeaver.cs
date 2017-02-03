using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cauldron.Interception.Fody
{
    public sealed class ModuleWeaver
    {
        public IAssemblyResolver AssemblyResolver { get; set; }
        public Action<string> LogError { get; set; }
        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarning { get; set; }
        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            try
            {
                Extensions.Asemblies = this.GetAssemblies();
                Extensions.Types = Extensions.Asemblies.SelectMany(x => x.Modules).SelectMany(x => x.Types).ToArray();
                this.ImplementMethodInterception();
            }
            catch (Exception e)
            {
                this.LogError(e.Message);
            }
        }

        public IEnumerable<AssemblyDefinition> GetAssemblies() => this.ModuleDefinition.AssemblyReferences.Select(x => this.AssemblyResolver.Resolve(x)).Concat(new AssemblyDefinition[] { this.ModuleDefinition.Assembly }).ToArray();

        private AssemblyDefinition GetCauldronCore()
        {
            var assemblyNameReference = this.ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Cauldron.Core");
            if (assemblyNameReference == null)
                throw new Exception($"The project {this.ModuleDefinition.Name} does not reference to 'Cauldron.Core'. Please add Cauldron.Core to your project.");
            return this.AssemblyResolver.Resolve(assemblyNameReference);
        }

        private TypeDefinition GetType(string interfaceName) => this.GetCauldronCore().Modules.SelectMany(x => x.Types).FirstOrDefault(x => x.FullName == interfaceName);

        private void ImplementMethod(MethodDefinition method, CustomAttribute[] attributes)
        {
            this.LogInfo($"Implenting Method interception: {method.Name} with {string.Join(", ", attributes.Select(x => x.AttributeType.FullName))}");
            var processor = method.Body.GetILProcessor();
            method.Body.SimplifyMacros();
            var methodInterceptorInterface = this.GetType("Cauldron.Core.Interceptors.IMethodInterceptor");
            var attributeInitialization = new List<Instruction>();
            var onEnterInstructions = new List<Instruction>();
            var exceptionInstructions = new List<Instruction>();
            var finallyInstructions = new List<Instruction>();
            var originalBody = method.Body.Instructions.ToList();
            var localVariables = new List<VariableDefinition>();
            var interceptorOnEnter = this.ModuleDefinition.Import(methodInterceptorInterface.GetMethodReference("OnEnter", 2));
            var interceptorOnException = this.ModuleDefinition.Import(methodInterceptorInterface.GetMethodReference("OnException", 1));
            var interceptorOnExit = this.ModuleDefinition.Import(methodInterceptorInterface.GetMethodReference("OnExit", 0));
            method.Body.Instructions.Clear();
            method.Body.InitLocals = true;
            var interceptorInterface = method.Module.Import(methodInterceptorInterface);
            var getMethodFromHandleRef = this.ModuleDefinition.Import(typeof(MethodBase).GetMethodReference("GetMethodFromHandle", 2));
            var methodBaseReference = new VariableDefinition(this.ModuleDefinition.Import(typeof(MethodBase).GetTypeReference()));
            processor.Body.Variables.Add(methodBaseReference);
            attributeInitialization.Add(processor.Create(OpCodes.Ldtoken, method));
            attributeInitialization.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType));
            attributeInitialization.Add(processor.Create(OpCodes.Call, getMethodFromHandleRef));
            attributeInitialization.Add(processor.Create(OpCodes.Stloc_S, methodBaseReference));
            // Get attribute instance
            foreach (var attribute in attributes)
            {
                var variableDefinition = new VariableDefinition(interceptorInterface);
                localVariables.Add(variableDefinition);
                processor.Body.Variables.Add(variableDefinition);
                foreach (var arg in attribute.ConstructorArguments)
                    attributeInitialization.AddRange(ValueToOpCode(processor, arg.Value, arg.Type));
                attributeInitialization.Add(processor.Create(OpCodes.Newobj, attribute.Constructor));
                attributeInitialization.Add(processor.Create(OpCodes.Stloc_S, variableDefinition));
                method.CustomAttributes.Remove(attribute);
                onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, variableDefinition));
                onEnterInstructions.Add(processor.Create(OpCodes.Ldarg_0));
                onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, methodBaseReference));
                onEnterInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnEnter));
            }
            var exceptionReference = new VariableDefinition(this.ModuleDefinition.Import(typeof(Exception).GetTypeReference()));
            processor.Body.Variables.Add(exceptionReference);
            exceptionInstructions.Add(processor.Create(OpCodes.Stloc_S, exceptionReference));
            foreach (var interceptor in localVariables)
            {
                exceptionInstructions.Add(processor.Create(OpCodes.Ldloc_S, interceptor));
                exceptionInstructions.Add(processor.Create(OpCodes.Ldloc_S, exceptionReference));
                exceptionInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnException));
            }
            exceptionInstructions.Add(processor.Create(OpCodes.Rethrow));
            foreach (var interceptor in localVariables)
            {
                finallyInstructions.Add(processor.Create(OpCodes.Ldloc_S, interceptor));
                finallyInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnExit));
            }
            finallyInstructions.Add(processor.Create(OpCodes.Endfinally));
            var lastReturn = processor.Create(OpCodes.Ret);

            processor.Append(attributeInitialization);
            processor.Append(onEnterInstructions);
            processor.Append(originalBody);
            processor.Append(exceptionInstructions);
            processor.Append(finallyInstructions);
            processor.Append(lastReturn);

            var exceptionHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = onEnterInstructions.First(),
                TryEnd = exceptionInstructions.First(),
                HandlerStart = exceptionInstructions.First(),
                HandlerEnd = exceptionInstructions.Last().Next,
                CatchType = this.ModuleDefinition.Import(typeof(Exception))
            };
            var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart = onEnterInstructions.First(),
                TryEnd = finallyInstructions.First(),
                HandlerStart = finallyInstructions.First(),
                HandlerEnd = finallyInstructions.Last().Next,
            };
            method.Body.ExceptionHandlers.Add(exceptionHandler);
            method.Body.ExceptionHandlers.Add(finallyHandler);

            this.ReplaceReturn(processor, method, originalBody, lastReturn);
            processor.Body.OptimizeMacros();
        }

        private void ImplementMethodInterception()
        {
            var methodInterceptorInterface = this.GetType("Cauldron.Core.Interceptors.IMethodInterceptor");
            if (methodInterceptorInterface == null)
                throw new Exception($"Unable to find the interface IMethodInterceptor.");
            var methodInterceptors = methodInterceptorInterface.GetTypesThatImplementsInterface();
            // find all types with methods that are decorated with any of the found method interceptors
            var methodsAndAttributes = this.ModuleDefinition.Types.SelectMany(x => x.Methods).Where(x => x.HasCustomAttributes)
                .Select(x => new { Method = x, Attributes = x.CustomAttributes.Where(y => methodInterceptors.Any(t => y.AttributeType.FullName == t.FullName)).ToArray() })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0);
            foreach (var method in methodsAndAttributes)
                this.ImplementMethod(method.Method, method.Attributes);
        }

        private void ImplementPropertyInterception()
        {
            var propertyInterceptorInterface = this.GetType("Cauldron.Core.Interceptors.IPropertyInterceptor");
            if (propertyInterceptorInterface == null)
                throw new Exception($"Unable to find the interface IPropertyInterceptor.");
        }

        private void ReplaceReturn(ILProcessor processor, MethodDefinition method, IEnumerable<Instruction> body, Instruction leaveInstruction)
        {
            if (method.ReturnType == TypeSystem.Void)
            {
                var instructionsSet = body.ToArray();

                for (int i = 0; i < instructionsSet.Length; i++)
                {
                    if (instructionsSet[i].OpCode != OpCodes.Ret)
                        continue;

                    var instruction = instructionsSet[i];
                    instruction.OpCode = OpCodes.Leave_S;
                    instruction.Operand = leaveInstruction;
                }
            }
        }

        private IEnumerable<Instruction> ValueToOpCode(ILProcessor processor, object value, TypeReference parameterType)
        {
            var result = new List<Instruction>();
            if (value == null)
            {
                result.Add(processor.Create(OpCodes.Ldnull));
                return result;
            }

            var type = value.GetType();

            if (type == typeof(string))
            {
                result.Add(processor.Create(OpCodes.Ldstr, value.ToString()));
                return result;
            }

            if (type == typeof(Type))
            {
                result.Add(processor.Create(OpCodes.Ldtoken, type.GetTypeReference()));
                result.Add(processor.Create(OpCodes.Call, type.GetMethodReference("GetTypeFromHandle", 1)));
            }

            if (type.IsValueType && !parameterType.IsValueType)
                result.Add(processor.Create(OpCodes.Box, type.IsEnum ? typeof(int).GetTypeReference() : type.GetTypeReference()));

            if (type == typeof(int) || type.IsEnum)
            {
                var intValue = (int)value;
                switch (intValue)
                {
                    case 0: result.Add(processor.Create(OpCodes.Ldc_I4_0)); break;
                    case 1: result.Add(processor.Create(OpCodes.Ldc_I4_1)); break;
                    case 2: result.Add(processor.Create(OpCodes.Ldc_I4_2)); break;
                    case 3: result.Add(processor.Create(OpCodes.Ldc_I4_3)); break;
                    case 4: result.Add(processor.Create(OpCodes.Ldc_I4_4)); break;
                    case 5: result.Add(processor.Create(OpCodes.Ldc_I4_5)); break;
                    case 6: result.Add(processor.Create(OpCodes.Ldc_I4_6)); break;
                    case 7: result.Add(processor.Create(OpCodes.Ldc_I4_7)); break;
                    case 8: result.Add(processor.Create(OpCodes.Ldc_I4_8)); break;
                    default: result.Add(processor.Create(OpCodes.Ldc_I4_S, intValue)); break;
                }
            }
            else if (type == typeof(long))
            {
                var intValue = (long)value;
                switch (intValue)
                {
                    case 0: result.Add(processor.Create(OpCodes.Ldc_I4_0)); break;
                    case 1: result.Add(processor.Create(OpCodes.Ldc_I4_1)); break;
                    case 2: result.Add(processor.Create(OpCodes.Ldc_I4_2)); break;
                    case 3: result.Add(processor.Create(OpCodes.Ldc_I4_3)); break;
                    case 4: result.Add(processor.Create(OpCodes.Ldc_I4_4)); break;
                    case 5: result.Add(processor.Create(OpCodes.Ldc_I4_5)); break;
                    case 6: result.Add(processor.Create(OpCodes.Ldc_I4_6)); break;
                    case 7: result.Add(processor.Create(OpCodes.Ldc_I4_7)); break;
                    case 8: result.Add(processor.Create(OpCodes.Ldc_I4_8)); break;
                    default: result.Add(processor.Create(OpCodes.Ldc_I4_S, intValue)); break;
                }
                result.Add(processor.Create(OpCodes.Conv_I8));
            }
            else if (type == typeof(double))
                result.Add(processor.Create(OpCodes.Ldc_R8, (double)value));
            else if (type == typeof(float))
                result.Add(processor.Create(OpCodes.Ldc_R4, (float)value));

            return result;
        }
    }
}