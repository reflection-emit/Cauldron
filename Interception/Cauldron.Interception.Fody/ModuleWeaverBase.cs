using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cauldron.Interception.Fody
{
    public abstract class ModuleWeaverBase
    {
        private ModuleWeaver weaver;

        public ModuleWeaverBase(ModuleWeaver weaver)
        {
            this.weaver = weaver;
        }

        public Action<string> LogError { get { return this.weaver.LogError; } }

        public Action<string> LogInfo { get { return this.weaver.LogInfo; } }

        public Action<string> LogWarning { get { return this.weaver.LogWarning; } }

        public ModuleDefinition ModuleDefinition { get { return this.weaver.ModuleDefinition; } }

        public abstract void Implement();

        protected IEnumerable<Instruction> AttributeParameterToOpCode(ILProcessor processor, CustomAttributeArgument attributeArgument)
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

            var result = new List<Instruction>();
            if (valueType.IsArray)
            {
                var array = (attributeArgument.Value as IEnumerable).ToArray_<CustomAttributeArgument>();

                result.Add(processor.Create(OpCodes.Ldc_I4, array.Length));
                result.Add(processor.Create(OpCodes.Newarr, this.ModuleDefinition.Import(attributeArgument.Type.GetElementType())));
                result.Add(processor.Create(OpCodes.Dup));

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Value == null)
                        return new Instruction[] { processor.Create(OpCodes.Ldnull), processor.Create(OpCodes.Stelem_Ref) };
                    else
                    {
                        var arrayType = array[i].Value.GetType();
                        result.Add(processor.Create(OpCodes.Ldc_I4, i));
                        result.AddRange(this.CreateInstructionsFromAttributeTypes(processor, array[i].Type, arrayType, array[i].Value));

                        if (arrayType.IsValueType && attributeArgument.Type.GetElementType().IsValueType)
                        {
                            if (arrayType == typeof(int) ||
                                arrayType == typeof(uint) ||
                                arrayType.IsEnum)
                                result.Add(processor.Create(OpCodes.Stelem_I4));
                            else if (arrayType == typeof(bool) ||
                                arrayType == typeof(byte) ||
                                arrayType == typeof(sbyte))
                                result.Add(processor.Create(OpCodes.Stelem_I1));
                            else if (arrayType == typeof(short) ||
                                arrayType == typeof(ushort) ||
                                arrayType == typeof(char))
                                result.Add(processor.Create(OpCodes.Stelem_I2));
                            else if (arrayType == typeof(long) ||
                                arrayType == typeof(ulong))
                                result.Add(processor.Create(OpCodes.Stelem_I8));
                            else if (arrayType == typeof(float))
                                result.Add(processor.Create(OpCodes.Stelem_R4));
                            else if (arrayType == typeof(double))
                                result.Add(processor.Create(OpCodes.Stelem_R8));
                        }
                        else
                            result.Add(processor.Create(OpCodes.Stelem_Ref));
                    }
                    if (i < array.Length - 1)
                        result.Add(processor.Create(OpCodes.Dup));
                }
            }
            else
                result.AddRange(this.CreateInstructionsFromAttributeTypes(processor, attributeArgument.Type, valueType, attributeArgument.Value));

            return result;
        }

        protected virtual VariableDefinition CreateParameterObject(MethodDefinition method, ILProcessor processor, List<Instruction> attributeInitialization, TypeReference objectReference, ArrayType parametersArrayTypeRef)
        {
            var parametersArrayVariable = new VariableDefinition(parametersArrayTypeRef);
            processor.Body.Variables.Add(parametersArrayVariable);

            attributeInitialization.Add(processor.Create(OpCodes.Ldc_I4, method.Parameters.Count));
            attributeInitialization.Add(processor.Create(OpCodes.Newarr, objectReference));
            attributeInitialization.Add(processor.Create(OpCodes.Stloc, parametersArrayVariable));

            foreach (var parameter in method.Parameters)
                attributeInitialization.AddRange(IlHelper.ProcessParam(parameter, parametersArrayVariable));

            return parametersArrayVariable;
        }

        protected IEnumerable<MethodReference> GetConstructors(TypeDefinition type)
        {
            var result = new List<MethodReference>();

            foreach (var constructor in type.GetConstructors())
            {
                foreach (var instruction in constructor.Body.Instructions.Where(x => x.OpCode == OpCodes.Call))
                {
                    var methodReference = instruction.Operand as MethodReference;

                    if (methodReference == null)
                        continue;

                    if (methodReference.DeclaringType.FullName != type.FullName)
                        result.Add(constructor);
                }
            }

            return result;
        }

        protected MethodDefinition GetOrCreateStaticConstructor(TypeDefinition type)
        {
            var cctor = type.GetStaticConstructor();

            if (cctor != null)
                return cctor;

            var method = new MethodDefinition(".cctor", MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, this.ModuleDefinition.TypeSystem.Void);

            var processor = method.Body.GetILProcessor();
            processor.Append(processor.Create(OpCodes.Ret));

            type.Methods.Add(method);

            return method;
        }

        protected TypeDefinition GetType(string typeName) => this.weaver.GetType(typeName);

        protected abstract void ImplementLockableOnEnter(ILProcessor processor, List<Instruction> onEnterInstructions, MethodDefinition method, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition methodBaseReference, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim);

        protected void ImplementMethod(MethodDefinition method, CustomAttribute[] attributes, Func<TypeReference, bool, MethodReference> onEnterMethod)
        {
            this.LogInfo($"Implenting Method interception: {method.Name} with {string.Join(", ", attributes.Select(x => x.AttributeType.FullName))}");
            var processor = method.Body.GetILProcessor();
            method.Body.SimplifyMacros();
            var attributeInitialization = new List<Instruction>();
            var onEnterInstructions = new List<Instruction>();
            var exceptionInstructions = new List<Instruction>();
            var finallyInstructions = new List<Instruction>();
            var originalBody = method.Body.Instructions.ToList();
            method.Body.Instructions.Clear();
            method.Body.InitLocals = true;
            var getMethodFromHandleRef = this.ModuleDefinition.Import(typeof(System.Reflection.MethodBase).GetMethodReference("GetMethodFromHandle", 2));
            var methodBaseReference = new VariableDefinition(this.ModuleDefinition.Import(typeof(System.Reflection.MethodBase).GetTypeReference()));

            this.GetConstructors(method.DeclaringType);

            processor.Body.Variables.Add(methodBaseReference);
            attributeInitialization.Add(processor.Create(OpCodes.Ldtoken, method));
            attributeInitialization.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType));
            attributeInitialization.Add(processor.Create(OpCodes.Call, getMethodFromHandleRef));
            attributeInitialization.Add(processor.Create(OpCodes.Stloc_S, methodBaseReference));

            // Create object array for the method args
            var objectReference = this.ModuleDefinition.Import(typeof(object).GetTypeReference());
            var parametersArrayTypeRef = new ArrayType(objectReference);
            VariableDefinition parametersArrayVariable = CreateParameterObject(method, processor, attributeInitialization, objectReference, parametersArrayTypeRef);

            var exceptionReference = new VariableDefinition(this.ModuleDefinition.Import(typeof(Exception).GetTypeReference()));
            processor.Body.Variables.Add(exceptionReference);
            exceptionInstructions.Add(processor.Create(OpCodes.Stloc_S, exceptionReference));

            // Get attribute instance
            foreach (var attribute in attributes)
            {
                var isLockable = attribute.AttributeType.Resolve().Interfaces.Any(x => x.Name.StartsWith("ILockable"));
                var methodInterceptorTypeRef = this.ModuleDefinition.Import(attribute.AttributeType);
                var interceptorOnEnter = this.ModuleDefinition.Import(onEnterMethod(methodInterceptorTypeRef, isLockable));
                var interceptorOnException = this.ModuleDefinition.Import(methodInterceptorTypeRef.GetMethodReference("OnException", 1));
                var interceptorOnExit = this.ModuleDefinition.Import(methodInterceptorTypeRef.GetMethodReference("OnExit", 0));

                var variableDefinition = new VariableDefinition(methodInterceptorTypeRef);
                processor.Body.Variables.Add(variableDefinition);

                foreach (var arg in attribute.ConstructorArguments)
                    attributeInitialization.AddRange(AttributeParameterToOpCode(processor, arg));

                attributeInitialization.Add(processor.Create(OpCodes.Newobj, attribute.Constructor));
                attributeInitialization.Add(processor.Create(OpCodes.Stloc_S, variableDefinition));
                method.CustomAttributes.Remove(attribute);

                if (isLockable)
                {
                    FieldDefinition semaphoreSlim;

                    if (method.IsStatic)
                        semaphoreSlim = new FieldDefinition($"<{method.Name}>_semaphore_static_field", Mono.Cecil.FieldAttributes.Private | Mono.Cecil.FieldAttributes.Static, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetTypeReference()));
                    else
                        semaphoreSlim = new FieldDefinition($"<{method.Name}>_semaphore_field", Mono.Cecil.FieldAttributes.Private, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetTypeReference()));

                    method.DeclaringType.Fields.Add(semaphoreSlim);

                    this.ImplementLockableOnEnter(processor, onEnterInstructions, method, variableDefinition, interceptorOnEnter, methodBaseReference, parametersArrayVariable, semaphoreSlim);

                    var endOfIf = processor.Create(OpCodes.Nop);
                    finallyInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                    finallyInstructions.Add(processor.Create(OpCodes.Ldfld, semaphoreSlim));
                    finallyInstructions.Add(processor.Create(OpCodes.Callvirt, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference("get_CurrentCount", 0))));
                    finallyInstructions.Add(processor.Create(OpCodes.Ldc_I4_0));
                    finallyInstructions.Add(processor.Create(OpCodes.Ceq));
                    finallyInstructions.Add(processor.Create(OpCodes.Brfalse_S, endOfIf));

                    finallyInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                    finallyInstructions.Add(processor.Create(OpCodes.Ldfld, semaphoreSlim));
                    finallyInstructions.Add(processor.Create(OpCodes.Callvirt, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference("Release", 0))));
                    finallyInstructions.Add(processor.Create(OpCodes.Pop));
                    finallyInstructions.Add(endOfIf);
                }
                else
                    this.ImplementOnEnter(processor, onEnterInstructions, method, variableDefinition, interceptorOnEnter, methodBaseReference, parametersArrayVariable, originalBody);

                exceptionInstructions.Add(processor.Create(OpCodes.Ldloc_S, variableDefinition));
                exceptionInstructions.Add(processor.Create(OpCodes.Ldloc_S, exceptionReference));
                exceptionInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnException));

                finallyInstructions.Add(processor.Create(OpCodes.Ldloc_S, variableDefinition));
                finallyInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnExit));
            }

            exceptionInstructions.Add(processor.Create(OpCodes.Rethrow));
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

        protected abstract void ImplementOnEnter(ILProcessor processor, List<Instruction> onEnterInstructions, MethodDefinition method, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition methodBaseReference, VariableDefinition parametersArrayVariable, List<Instruction> originalBody);

        protected Tuple<List<Instruction>, Instruction> ReplaceReturn(ILProcessor processor, MethodDefinition method, IEnumerable<Instruction> body, Instruction leaveInstruction)
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
                var returnVariable = new VariableDefinition("__var_" + Guid.NewGuid().ToString().Replace('-', '_'), this.ModuleDefinition.Import(method.ReturnType));
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

        private IEnumerable<Instruction> CreateInstructionsFromAttributeTypes(ILProcessor processor, TypeReference targetType, Type type, object value)
        {
            if (type == typeof(CustomAttributeArgument))
            {
                var attrib = (CustomAttributeArgument)value;
                type = attrib.Value.GetType();
                value = attrib.Value;
            }

            if (type == typeof(string))
                return new Instruction[] { processor.Create(OpCodes.Ldstr, value.ToString()) };

            if (type == typeof(TypeReference))
            {
                return new Instruction[]
                {
                    processor.Create(OpCodes.Ldtoken, this.ModuleDefinition.Import(value as TypeReference)),
                    processor.Create(OpCodes.Call, this.ModuleDefinition.Import(typeof(Type).GetMethodReference("GetTypeFromHandle", 1)))
                };
            }

            var createInstructionsResult = new List<Instruction>();

            if (type.IsEnum) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)value));
            else if (type == typeof(int)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)value));
            else if (type == typeof(uint)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)(uint)value));
            else if (type == typeof(bool)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (bool)value ? 1 : 0));
            else if (type == typeof(char)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (char)value));
            else if (type == typeof(short)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (short)value));
            else if (type == typeof(ushort)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (ushort)value));
            else if (type == typeof(byte)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)(byte)value));
            else if (type == typeof(sbyte)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I4, (int)(sbyte)value));
            else if (type == typeof(long)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I8, (long)value));
            else if (type == typeof(ulong)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_I8, (long)(ulong)value));
            else if (type == typeof(double)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_R8, (double)value));
            else if (type == typeof(float)) createInstructionsResult.Add(processor.Create(OpCodes.Ldc_R4, (float)value));

            if (type.IsValueType && !targetType.IsValueType)
                createInstructionsResult.Add(processor.Create(OpCodes.Box, type.IsEnum ?
                    this.ModuleDefinition.Import(Enum.GetUnderlyingType(type).GetTypeReference()) : this.ModuleDefinition.Import(type.GetTypeReference())));

            return createInstructionsResult;
        }
    }
}