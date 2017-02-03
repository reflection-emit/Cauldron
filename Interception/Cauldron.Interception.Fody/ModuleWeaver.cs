using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections;
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

        private IEnumerable<Instruction> AttributeParameterToOpCode(ILProcessor processor, CustomAttributeArgument attributeArgument)
        {
            /*
                - One of the following types: bool, byte, char, double, float, int, long, short, string, sbyte, ushort, uint, ulong.
                - The type object.
                - The type System.Type.
                - An enum type, provided it has public accessibility and the types in which it is nested (if any) also have public accessibility (Section 17.2).
                - Single-dimensional arrays of the above types.
             */

            if (attributeArgument.Value == null)
                return new Instruction[] { processor.Create(OpCodes.Ldnull) };

            var valueType = attributeArgument.Value.GetType();

            var createInstructions = new Func<Type, object, IEnumerable<Instruction>>((type, value) =>
            {
                if (type == typeof(string))
                    return new Instruction[] { processor.Create(OpCodes.Ldstr, attributeArgument.Value.ToString()) };

                if (type == typeof(TypeReference))
                {
                    return new Instruction[]
                    {
                        processor.Create(OpCodes.Ldtoken, this.ModuleDefinition.Import(value as TypeReference)),
                        processor.Create(OpCodes.Call, this.ModuleDefinition.Import(typeof(Type).GetMethodReference("GetTypeFromHandle", 1)))
                    };
                }

                var createInstructionsResult = new List<Instruction>();

                if (
                    type == typeof(int) ||
                    type == typeof(uint) ||
                    type == typeof(bool) ||
                    type == typeof(byte) ||
                    type == typeof(sbyte) ||
                    type == typeof(char) ||
                    type == typeof(short) ||
                    type == typeof(ushort) ||
                    type.IsEnum)
                    createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4_S, (int)value));
                else if (type == typeof(long) || type == typeof(ulong))
                    createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I8, (long)value));
                else if (type == typeof(double))
                    createInstructionsResult.Add(processor.Create(OpCodes.Ldc_R8, (double)attributeArgument.Value));
                else if (type == typeof(float))
                    createInstructionsResult.Add(processor.Create(OpCodes.Ldc_R4, (float)attributeArgument.Value));

                if (type.IsValueType && !attributeArgument.Type.IsValueType)
                    createInstructionsResult.Add(processor.Create(OpCodes.Box, type.IsEnum ? Enum.GetUnderlyingType(type).GetTypeReference() : type.GetTypeReference()));

                return createInstructionsResult;
            });

            var result = new List<Instruction>();
            if (valueType.IsArray)
            {
                var array = (attributeArgument.Value as IEnumerable).ToArray_<object>();
                var arrayType = array.GetType().GetElementType();
                var arrayTypeReference = arrayType.GetTypeReference();
                result.Add(processor.Create(OpCodes.Ldc_I4_S, array.Length));
                result.Add(processor.Create(OpCodes.Newarr, arrayTypeReference));
                result.Add(processor.Create(OpCodes.Dup));

                for (int i = 0; i < array.Length; i++)
                {
                    result.Add(processor.Create(OpCodes.Ldc_I4_S, i));
                    result.AddRange(createInstructions(arrayType, array[i]));

                    if (arrayType.IsValueType)
                    {
                        if (arrayType == typeof(int) ||
                            arrayType == typeof(uint) ||
                            arrayType == typeof(bool) ||
                            arrayType == typeof(byte) ||
                            arrayType == typeof(sbyte) ||
                            arrayType == typeof(char) ||
                            arrayType.IsEnum)
                            result.Add(processor.Create(OpCodes.Stelem_I4));
                        else if (arrayType == typeof(short))
                            result.Add(processor.Create(OpCodes.Stelem_I2));
                        else if (arrayType == typeof(long))
                            result.Add(processor.Create(OpCodes.Stelem_I8));
                        else if (arrayType == typeof(float))
                            result.Add(processor.Create(OpCodes.Stelem_R4));
                        else if (arrayType == typeof(double))
                            result.Add(processor.Create(OpCodes.Stelem_R8));
                    }
                    else
                        result.Add(processor.Create(OpCodes.Stelem_Ref));
                    result.Add(processor.Create(OpCodes.Dup));
                }
            }
            else
                result.AddRange(createInstructions(valueType, attributeArgument.Value));

            return result;
        }

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
            var interceptorOnEnter = this.ModuleDefinition.Import(methodInterceptorInterface.GetMethodReference("OnEnter", 3));
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

            // Create object array for the method args
            var objectReference = this.ModuleDefinition.Import(typeof(object).GetTypeReference());
            var parametersArrayTypeRef = new ArrayType(objectReference);
            var parametersArrayVariable = new VariableDefinition(parametersArrayTypeRef);
            processor.Body.Variables.Add(parametersArrayVariable);

            attributeInitialization.Add(processor.Create(OpCodes.Ldc_I4, method.Parameters.Count));
            attributeInitialization.Add(processor.Create(OpCodes.Newarr, objectReference));
            attributeInitialization.Add(processor.Create(OpCodes.Stloc, parametersArrayVariable));

            foreach (var parameter in method.Parameters)
                attributeInitialization.AddRange(IlHelper.ProcessParam(parameter, parametersArrayVariable));

            // Get attribute instance
            foreach (var attribute in attributes)
            {
                var variableDefinition = new VariableDefinition(interceptorInterface);
                localVariables.Add(variableDefinition);
                processor.Body.Variables.Add(variableDefinition);
                foreach (var arg in attribute.ConstructorArguments)
                    attributeInitialization.AddRange(AttributeParameterToOpCode(processor, arg));
                attributeInitialization.Add(processor.Create(OpCodes.Newobj, attribute.Constructor));
                attributeInitialization.Add(processor.Create(OpCodes.Stloc_S, variableDefinition));
                method.CustomAttributes.Remove(attribute);
                onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, variableDefinition));
                onEnterInstructions.Add(processor.Create(OpCodes.Ldarg_0));
                onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, methodBaseReference));
                onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, parametersArrayVariable));
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

            var newBody = this.ReplaceReturn(processor, method, originalBody, lastReturn);
            originalBody = newBody.Item1;

            processor.Append(attributeInitialization);
            processor.Append(onEnterInstructions);
            processor.Append(originalBody);
            processor.Append(exceptionInstructions);
            processor.Append(finallyInstructions);
            processor.Append(lastReturn);

            if (newBody.Item2 != null)
                processor.InsertBefore(lastReturn, newBody.Item2);

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

        private Tuple<List<Instruction>, Instruction> ReplaceReturn(ILProcessor processor, MethodDefinition method, IEnumerable<Instruction> body, Instruction leaveInstruction)
        {
            var instructionsSet = body.ToList();

            if (method.ReturnType == this.ModuleDefinition.TypeSystem.Void)
            {
                for (int i = 0; i < instructionsSet.Count; i++)
                {
                    if (instructionsSet[i].OpCode != OpCodes.Ret)
                        continue;

                    var instruction = instructionsSet[i];
                    instruction.OpCode = OpCodes.Leave_S;
                    instruction.Operand = leaveInstruction;
                }

                return new Tuple<List<Instruction>, Instruction>(instructionsSet, null);
            }
            else
            {
                var returnVariable = new VariableDefinition("__var_" + Guid.NewGuid().ToString().Replace('-', '_'), method.ReturnType);
                processor.Body.Variables.Add(returnVariable);
                var loadReturnVariable = processor.Create(OpCodes.Ldloc_S, returnVariable);

                for (int i = 0; i < instructionsSet.Count; i++)
                {
                    if (instructionsSet[i].OpCode != OpCodes.Ret)
                        continue;

                    var instruction = instructionsSet[i];
                    instruction.OpCode = OpCodes.Leave_S;
                    instruction.Operand = loadReturnVariable;
                    instructionsSet.Insert(i, processor.Create(OpCodes.Stloc_S, returnVariable));
                }

                return new Tuple<List<Instruction>, Instruction>(instructionsSet, loadReturnVariable);
            }
        }
    }
}