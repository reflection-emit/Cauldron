using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        protected virtual VariableDefinition CreateParameterObject(MethodWeaverInfo methodWeaverInfo, TypeReference objectReference, ArrayType parametersArrayTypeRef)
        {
            var parametersArrayVariable = new VariableDefinition(parametersArrayTypeRef);

            methodWeaverInfo.Processor.Body.Variables.Add(parametersArrayVariable);

            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldc_I4, methodWeaverInfo.MethodDefinition.Parameters.Count));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Newarr, objectReference));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Stloc, parametersArrayVariable));

            foreach (var parameter in methodWeaverInfo.MethodDefinition.Parameters)
                methodWeaverInfo.Initializations.AddRange(IlHelper.ProcessParam(parameter, parametersArrayVariable));

            return parametersArrayVariable;
        }

        protected IEnumerable<MethodReference> GetConstructors(TypeDefinition type)
        {
            var result = new List<MethodReference>();

            foreach (var constructor in type.Methods.Where(x => x.Name == ".ctor"))
            {
                foreach (var instruction in constructor.Body.Instructions.Where(x => x.OpCode == OpCodes.Call))
                {
                    var methodReference = instruction.Operand as MethodReference;

                    if (methodReference == null)
                        continue;

                    if (type.BaseType.FullName != methodReference.DeclaringType.FullName && !methodReference.FullName.Contains(".ctor"))
                        continue;

                    if (methodReference.DeclaringType.FullName != type.FullName)
                    {
                        result.Add(constructor);
                        break;
                    }
                }
            }

            return result;
        }

        protected IEnumerable<MethodAndInstruction> GetMethodsWhere(Func<Instruction, bool> predicate) => this.ModuleDefinition.Types
                .SelectMany(x => x.Methods)
                .Where(x => x.Body != null)
                .Select(x => new MethodAndInstruction(x, x.Body.Instructions.Where(predicate).ToArray()))
                .ToArray();

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

        protected abstract void ImplementLockableOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim);

        protected void ImplementMethod(MethodDefinition method, CustomAttribute[] attributes, Func<TypeReference, bool, MethodReference> onEnterMethod)
        {
            if (attributes == null || attributes.Length == 0)
                return;

            this.LogInfo($"Implenting Method interception: {method.Name} with {string.Join(", ", attributes.Select(x => x.AttributeType.FullName))}");
            var methodWeavingInfo = new MethodWeaverInfo(method);

            method.Body.SimplifyMacros();
            method.Body.Instructions.Clear();
            method.Body.InitLocals = true;

            // Dont create these if we have properties
            if (methodWeavingInfo.Property == null)
            {
                var getMethodFromHandleRef = typeof(System.Reflection.MethodBase).GetMethodReference("GetMethodFromHandle", 2).Import();

                var methodBaseVariableEndIf = methodWeavingInfo.Processor.Create(OpCodes.Nop);
                if (!methodWeavingInfo.MethodDefinition.IsStatic)
                {
                    methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldarg_0));
                    methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldfld, methodWeavingInfo.MethodBaseField));
                }
                else
                    methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldsfld, methodWeavingInfo.MethodBaseField));

                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldnull));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ceq));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Brfalse_S, methodBaseVariableEndIf));

                if (!methodWeavingInfo.MethodDefinition.IsStatic)
                    methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldtoken, method));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldtoken, method.DeclaringType));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Call, getMethodFromHandleRef));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(method.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, methodWeavingInfo.MethodBaseField));
                methodWeavingInfo.Initializations.Add(methodBaseVariableEndIf);
            }

            this.OnImplementMethod(methodWeavingInfo);

            // Create object array for the method args
            var objectReference = this.ModuleDefinition.Import(typeof(object).GetTypeReference());
            var parametersArrayTypeRef = new ArrayType(objectReference);
            var parametersArrayVariable = CreateParameterObject(methodWeavingInfo, objectReference, parametersArrayTypeRef);

            var exceptionReference = new VariableDefinition(this.ModuleDefinition.Import(typeof(Exception).GetTypeReference()));
            methodWeavingInfo.Processor.Body.Variables.Add(exceptionReference);
            methodWeavingInfo.ExceptionInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Stloc_S, exceptionReference));

            // Get attribute instance
            foreach (var attribute in attributes)
            {
                var isLockable = attribute.AttributeType.Resolve().Interfaces.Any(x => x.Name.StartsWith("ILockable"));
                var methodInterceptorTypeRef = this.ModuleDefinition.Import(attribute.AttributeType);
                var interceptorOnEnter = this.ModuleDefinition.Import(onEnterMethod(methodInterceptorTypeRef, isLockable));
                var interceptorOnException = this.ModuleDefinition.Import(methodInterceptorTypeRef.GetMethodReference("OnException", 1));
                var interceptorOnExit = this.ModuleDefinition.Import(methodInterceptorTypeRef.GetMethodReference("OnExit", 0));

                var variableDefinition = new VariableDefinition(methodInterceptorTypeRef);
                methodWeavingInfo.Processor.Body.Variables.Add(variableDefinition);

                foreach (var arg in attribute.ConstructorArguments)
                    methodWeavingInfo.Initializations.AddRange(AttributeParameterToOpCode(methodWeavingInfo.Processor, arg));

                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Newobj, attribute.Constructor));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Stloc_S, variableDefinition));
                method.CustomAttributes.Remove(attribute);

                if (isLockable)
                {
                    var semaphoreSlimAttribute = FieldAttributes.Private;

                    if (method.IsStatic)
                        semaphoreSlimAttribute |= FieldAttributes.Static;

                    // we need to check how we will name the semaphore... If this is a method that is part of a property, then we have to make sure, that
                    // getter and setter are using the same naming and also reusing the same fields and delegates

                    var semaphoreSlim = methodWeavingInfo.GetOrCreateField(typeof(SemaphoreSlim));
                    this.ImplementLockableOnEnter(methodWeavingInfo, variableDefinition, interceptorOnEnter, parametersArrayVariable, semaphoreSlim);

                    var endOfIf = methodWeavingInfo.Processor.Create(OpCodes.Nop);
                    if (method.IsStatic)
                        methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldsfld, semaphoreSlim));
                    else
                    {
                        methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldarg_0));
                        methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldfld, semaphoreSlim));
                    }
                    methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Callvirt, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference("get_CurrentCount", 0))));
                    methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldc_I4_0));
                    methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ceq));
                    methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Brfalse_S, endOfIf));

                    if (method.IsStatic)
                        methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldsfld, semaphoreSlim));
                    else
                    {
                        methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldarg_0));
                        methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldfld, semaphoreSlim));
                    }
                    methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Callvirt, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference("Release", 0))));
                    methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Pop));
                    methodWeavingInfo.FinallyInstructions.Add(endOfIf);
                }
                else
                    this.ImplementOnEnter(methodWeavingInfo, variableDefinition, interceptorOnEnter, parametersArrayVariable);

                methodWeavingInfo.ExceptionInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldloc_S, variableDefinition));
                methodWeavingInfo.ExceptionInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldloc_S, exceptionReference));
                methodWeavingInfo.ExceptionInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Callvirt, interceptorOnException));

                methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldloc_S, variableDefinition));
                methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Callvirt, interceptorOnExit));
            }

            methodWeavingInfo.ExceptionInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Rethrow));
            methodWeavingInfo.FinallyInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Endfinally));

            var lastReturn = this.ReplaceReturn(methodWeavingInfo);
            methodWeavingInfo.Build();

            if (lastReturn != null)
                methodWeavingInfo.Processor.InsertBefore(methodWeavingInfo.LastReturn, lastReturn);

            var exceptionHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = methodWeavingInfo.OnEnterInstructions.First(),
                TryEnd = methodWeavingInfo.ExceptionInstructions.First(),
                HandlerStart = methodWeavingInfo.ExceptionInstructions.First(),
                HandlerEnd = methodWeavingInfo.ExceptionInstructions.Last().Next,
                CatchType = this.ModuleDefinition.Import(typeof(Exception))
            };
            var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart = methodWeavingInfo.OnEnterInstructions.First(),
                TryEnd = methodWeavingInfo.FinallyInstructions.First(),
                HandlerStart = methodWeavingInfo.FinallyInstructions.First(),
                HandlerEnd = methodWeavingInfo.FinallyInstructions.Last().Next,
            };
            method.Body.ExceptionHandlers.Add(exceptionHandler);
            method.Body.ExceptionHandlers.Add(finallyHandler);
            methodWeavingInfo.Processor.Body.OptimizeMacros();
        }

        protected abstract void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable);

        protected virtual void OnImplementMethod(MethodWeaverInfo methodWeaverInfo)
        {
        }

        protected Instruction ReplaceReturn(MethodWeaverInfo methodWeaverInfo)
        {
            var instructionsSet = methodWeaverInfo.OriginalBody.ToList();

            if (methodWeaverInfo.MethodDefinition.ReturnType == this.ModuleDefinition.TypeSystem.Void)
            {
                for (int i = 0; i < instructionsSet.Count; i++)
                {
                    if (instructionsSet[i].OpCode != OpCodes.Ret)
                        continue;

                    var instruction = instructionsSet[i];
                    instruction.OpCode = OpCodes.Leave_S;
                    instruction.Operand = methodWeaverInfo.LastReturn;
                }

                methodWeaverInfo.OriginalBody = instructionsSet;
                return null;
            }
            else
            {
                var returnVariable = new VariableDefinition("__var_" + methodWeaverInfo.Id, this.ModuleDefinition.Import(methodWeaverInfo.MethodDefinition.ReturnType));
                methodWeaverInfo.Processor.Body.Variables.Add(returnVariable);
                var loadReturnVariable = methodWeaverInfo.Processor.Create(OpCodes.Ldloc_S, returnVariable);

                for (int i = 0; i < instructionsSet.Count; i++)
                {
                    if (instructionsSet[i].OpCode != OpCodes.Ret)
                        continue;

                    var instruction = instructionsSet[i];
                    instruction.OpCode = OpCodes.Leave_S;
                    instruction.Operand = loadReturnVariable;
                    instructionsSet.Insert(i, methodWeaverInfo.Processor.Create(OpCodes.Stloc_S, returnVariable));
                }

                methodWeaverInfo.OriginalBody = instructionsSet;
                return loadReturnVariable;
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
                return processor.TypeOf(value as TypeReference);

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