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

            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldc_I4, methodWeaverInfo.MethodReference.Parameters.Count));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Newarr, objectReference));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Stloc, parametersArrayVariable));

            foreach (var parameter in methodWeaverInfo.MethodReference.Parameters)
                methodWeaverInfo.Initializations.AddRange(IlHelper.ProcessParam(parameter, parametersArrayVariable));

            return parametersArrayVariable;
        }

        protected IEnumerable<MethodAndInstruction> GetMethodsWhere(TypeDefinition type, Func<Instruction, bool> predicate)
        {
            return type.GetNestedTypes().Concat(new TypeDefinition[] { type }).SelectMany(x => x.Methods)
                .Where(x => x != null && x.Body != null)
                .Select(x => new MethodAndInstruction(x, x.Body.Instructions.Where(predicate).ToArray()))
                .ToList();
        }

        protected IEnumerable<MethodAndInstruction> GetMethodsWhere(Func<Instruction, bool> predicate)
        {
            var allModuleTypes = new List<TypeDefinition>();
            var types = this.ModuleDefinition.Types.Where(x => x != null);

            foreach (var item in types)
                allModuleTypes.AddRange(item.GetNestedTypes());

            allModuleTypes.AddRange(types.Distinct(new TypeDefinitionEqualityComparer()));

            return allModuleTypes
                .SelectMany(x => x.Methods)
                .Where(x => x != null && x.Body != null)
                .Select(x => new MethodAndInstruction(x, x.Body.Instructions.Where(predicate).ToArray()))
                .ToList();
        }

        protected void ImplementFieldSetterDelegate(MethodReference method, FieldReference field, bool isStatic)
        {
            var methodDefinition = method.Resolve();
            var processor = methodDefinition.Body.GetILProcessor();
            methodDefinition.Body.Instructions.Clear();
            var returnOpCode = processor.Create(OpCodes.Ret);

            if (field.FieldType.Resolve() == null) /* This happens if the field type is a generic */
            {
                if (!isStatic)
                    processor.Append(processor.Create(OpCodes.Ldarg_0));

                processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
                processor.Append(processor.Create(OpCodes.Unbox_Any, field.FieldType.Import()));
                processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));
            }
            else if (field.FieldType.Resolve().IsEnum)
            {
                if (!isStatic)
                    processor.Append(processor.Create(OpCodes.Ldarg_0));

                processor.Append(processor.TypeOf(field.FieldType.Import()));
                processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
                processor.Append(processor.TypeOf(field.FieldType.Import()));
                processor.Append(processor.Create(OpCodes.Call, typeof(Enum).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }).Import()));
                processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }).Import()));
                processor.Append(processor.Create(OpCodes.Call, typeof(Enum).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }).Import()));
                processor.Append(processor.Create(OpCodes.Unbox_Any, field.FieldType.Import()));
                processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));
            }
            else if (field.FieldType.IsIEnumerable())
                EmitSpecializedEnumerableSetter(field, isStatic, processor, returnOpCode);
            else
            {
                if (!isStatic)
                    processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));

                if (field.FieldType.FullName == typeof(int).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToInt32", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(uint).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToUInt32", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(bool).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToBoolean", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(byte).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToByte", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(char).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToChar", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(DateTime).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToDateTime", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(decimal).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToDecimal", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(double).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToDouble", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(short).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToInt16", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(long).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToInt64", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(sbyte).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToSByte", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(float).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToSingle", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(string).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToString", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(ushort).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToUInt16", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.FullName == typeof(ulong).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToUInt64", new Type[] { typeof(object) }).Import()));
                else if (field.FieldType.Resolve().IsInterface) processor.Append(processor.Create(OpCodes.Isinst, field.FieldType.Import()));
                else processor.Append(processor.Create(field.FieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, field.FieldType.Import()));

                processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));
            }
            processor.Append(returnOpCode);
        }

        protected abstract void ImplementLockableOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim);

        protected void ImplementMethod(MethodDefinition method, CustomAttribute[] attributes, Func<TypeReference, bool, MethodReference> onEnterMethod)
        {
            if (attributes == null || attributes.Length == 0)
                return;

            this.LogInfo($"Implementing Method interception: {method.Name} with {string.Join(", ", attributes.Select(x => x.AttributeType.FullName))}");
            var methodWeavingInfo = new MethodWeaverInfo(method);

            method.Body.SimplifyMacros();
            method.Body.Instructions.Clear();
            method.Body.InitLocals = true;

            // Dont create these if we have properties
            if (methodWeavingInfo.Property == null)
            {
                var getMethodFromHandleRef = typeof(System.Reflection.MethodBase).GetMethodReference("GetMethodFromHandle", 2).Import();

                var methodBaseVariableEndIf = methodWeavingInfo.Processor.Create(OpCodes.Nop);
                if (!methodWeavingInfo.IsStatic)
                {
                    methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldarg_0));
                    methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldfld, methodWeavingInfo.MethodBaseField));
                }
                else
                    methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldsfld, methodWeavingInfo.MethodBaseField));

                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldnull));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ceq));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Brfalse_S, methodBaseVariableEndIf));

                if (!methodWeavingInfo.IsStatic)
                    methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldtoken, methodWeavingInfo.MethodReference));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Ldtoken, methodWeavingInfo.DeclaringType));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(OpCodes.Call, getMethodFromHandleRef));
                methodWeavingInfo.Initializations.Add(methodWeavingInfo.Processor.Create(methodWeavingInfo.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, methodWeavingInfo.MethodBaseField));
                methodWeavingInfo.Initializations.Add(methodBaseVariableEndIf);
            }

            this.OnImplementMethod(methodWeavingInfo);

            // Create object array for the method args
            var objectReference = typeof(object).GetTypeReference().Import();
            var parametersArrayTypeRef = new ArrayType(objectReference);
            var parametersArrayVariable = CreateParameterObject(methodWeavingInfo, objectReference, parametersArrayTypeRef);

            var exceptionReference = new VariableDefinition(this.ModuleDefinition.Import(typeof(Exception).GetTypeReference()));
            methodWeavingInfo.Processor.Body.Variables.Add(exceptionReference);
            methodWeavingInfo.ExceptionInstructions.Add(methodWeavingInfo.Processor.Create(OpCodes.Stloc_S, exceptionReference));

            // Get attribute instance
            foreach (var attribute in attributes)
            {
                var isLockable = attribute.AttributeType.Resolve().Interfaces.Any(x => x.Name.StartsWith("ILockable"));
                var methodInterceptorTypeRef = attribute.AttributeType.Import();
                var interceptorOnEnter = onEnterMethod(methodInterceptorTypeRef, isLockable).Import();
                var interceptorOnException = methodInterceptorTypeRef.GetMethodReference("OnException", 1).Import();
                var interceptorOnExit = methodInterceptorTypeRef.GetMethodReference("OnExit", 0).Import();

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

            if (methodWeaverInfo.MethodReference.ReturnType == this.ModuleDefinition.TypeSystem.Void)
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
                var returnVariable = new VariableDefinition("__var_" + methodWeaverInfo.Id, this.ModuleDefinition.Import(methodWeaverInfo.MethodReference.ReturnType));
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
                    Enum.GetUnderlyingType(type).GetTypeReference().Import() : type.GetTypeReference().Import()));

            return createInstructionsResult;
        }

        private void EmitSpecializedEnumerableSetter(FieldReference field, bool isStatic, ILProcessor processor, Instruction returnOpCode)
        {
            var childType = field.FieldType.GetChildrenType();
            var elseClause = processor.Create(OpCodes.Nop);
            var endifBackingFieldNull = processor.Create(OpCodes.Nop);
            var endifBackingFieldType = processor.Create(OpCodes.Nop);

            processor.Body.InitLocals = true;

            #region backingField.TryDispose();

            if (!isStatic)
                processor.Append(processor.Create(OpCodes.Ldarg_0));

            processor.Append(processor.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field));
            processor.Append(processor.Create(OpCodes.Call, "Cauldron.Interception.Extensions".ToTypeDefinition().GetMethodReference("TryDispose", 1).Import()));

            #endregion backingField.TryDispose();

            #region if (value == null)

            processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Ldnull));
            processor.Append(processor.Create(OpCodes.Ceq));
            processor.Append(processor.Create(OpCodes.Brfalse_S, elseClause));

            #endregion if (value == null)

            #region backingField = null;

            if (!isStatic)
                processor.Append(processor.Create(OpCodes.Ldarg_0));

            processor.Append(processor.Create(OpCodes.Ldnull));
            processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));
            processor.Append(processor.Create(OpCodes.Br_S, returnOpCode));

            #endregion backingField = null;

            // else
            processor.Append(elseClause);

            #region if(value is <backingField>)

            processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Isinst, field.FieldType.Import()));
            processor.Append(processor.Create(OpCodes.Ldnull));
            processor.Append(processor.Create(OpCodes.Cgt_Un));
            processor.Append(processor.Create(OpCodes.Brfalse_S, endifBackingFieldType));

            #endregion if(value is <backingField>)

            #region backingField = value as <backingField>;

            if (!isStatic)
                processor.Append(processor.Create(OpCodes.Ldarg_0));
            processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
            processor.Append(processor.Create(OpCodes.Isinst, field.FieldType.Import()));
            processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));
            processor.Append(processor.Create(OpCodes.Br_S, returnOpCode));

            #endregion backingField = value as <backingField>;

            processor.Append(endifBackingFieldType);

            var ctor = field.FieldType.GetDefaultConstructor();
            if (ctor != null)
            {
                // We will only use public constructors
                if (ctor.Resolve().IsPublic)
                {
                    #region if(backingField == null)

                    if (!isStatic)
                        processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field));
                    processor.Append(processor.Create(OpCodes.Ldnull));
                    processor.Append(processor.Create(OpCodes.Ceq));
                    processor.Append(processor.Create(OpCodes.Brfalse_S, endifBackingFieldNull));

                    #endregion if(backingField == null)

                    #region backingField = new ITestInterface();

                    if (!isStatic)
                        processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(OpCodes.Newobj, ctor.Import()));
                    processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));

                    #endregion backingField = new ITestInterface();

                    processor.Append(endifBackingFieldNull);
                }
            }
            if (field.FieldType.IsArray || field.FieldType.Resolve().FullName == typeof(IEnumerable<>).FullName)
            {
                #region backingField = (value as IEnumerable).Cast<ITestInterface>().ToArray<ITestInterface>();

                if (!isStatic)
                    processor.Append(processor.Create(OpCodes.Ldarg_0));

                var castMethod = typeof(System.Linq.Enumerable).GetMethodReference("Cast", new Type[] { typeof(IEnumerable) }).MakeGeneric(childType).Import();
                var toArrayMethod = typeof(System.Linq.Enumerable).GetMethodReference("ToArray", 1).MakeGeneric(childType).Import();

                processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
                processor.Append(processor.Create(OpCodes.Isinst, typeof(IEnumerable).GetTypeReference().Import()));
                processor.Append(processor.Create(OpCodes.Call, castMethod));
                processor.Append(processor.Create(OpCodes.Call, toArrayMethod));
                processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));

                #endregion backingField = (value as IEnumerable).Cast<ITestInterface>().ToArray<ITestInterface>();
            }
            else if (field.FieldType.HasMethod("Add", 1))
            {
                #region backingField.Clear();

                // TODO - Causes error 0x8013189F on IL ... returnOpCode jumps to somewhere else
                //if (field.FieldType.HasMethod("Clear"))
                //{
                //    if (!isStatic)
                //        processor.Append(processor.Create(OpCodes.Ldarg_0));
                //    processor.Append(processor.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field));
                //    processor.Append(processor.Create(OpCodes.Callvirt, field.FieldType.GetMethodReference("Clear", 0).Import()));
                //}

                #endregion backingField.Clear();

                // Check if there is a AddRange... We prefer add range
                if (field.FieldType.HasMethod("AddRange", 1))
                {
                    // Find out if this method requires an object or T
                    var addRangeMethods = field.FieldType.GetMethodReferences("AddRange", 1);
                    MethodReference addRangeReference = null;

                    if (addRangeMethods.Count() == 1)
                        addRangeReference = addRangeMethods.First();
                    else
                        addRangeReference = addRangeMethods.FirstOrDefault(x => x.Parameters[0].ParameterType.FullName == typeof(IEnumerable<>).GetTypeReference().MakeGenericInstanceType(childType).FullName);

                    if (addRangeReference == null)
                        addRangeReference = addRangeMethods.FirstOrDefault(x => x.Parameters[0].ParameterType.FullName == typeof(IEnumerable<>).GetTypeReference().MakeGenericInstanceType(typeof(object).GetTypeReference()).FullName);

                    if (addRangeReference == null) // Just take the first one... Code could break here... A lot of reasons why it could
                        addRangeReference = addRangeMethods.First();

                    #region backingField.AddRange(value as IEnumerable);

                    var castMethod = typeof(System.Linq.Enumerable).GetMethodReference("Cast", new Type[] { typeof(IEnumerable) }).MakeGeneric(childType).Import();

                    if (!isStatic)
                        processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field));
                    processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
                    processor.Append(processor.Create(OpCodes.Isinst, typeof(IEnumerable).GetTypeReference().Import()));
                    processor.Append(processor.Create(OpCodes.Call, castMethod));
                    processor.Append(processor.Create(OpCodes.Callvirt, addRangeReference.Import()));

                    #endregion backingField.AddRange(value as IEnumerable);
                }
                else
                {
                    var addMethods = field.FieldType.GetMethodReferences("Add", 1);

                    MethodReference addReference = null;

                    if (addMethods.Count() == 1)
                        addReference = addMethods.First();
                    else
                        addReference = addMethods.FirstOrDefault(x => x.Parameters[0].ParameterType.FullName == childType.FullName);

                    if (addReference == null)
                        addReference = addMethods.FirstOrDefault(x => x.Parameters[0].ParameterType.FullName == typeof(object).FullName);

                    if (addReference == null)
                        addReference = addMethods.First();

                    #region var array = (value as IEnumerable).Cast<ITestInterface>().ToArray<ITestInterface>();

                    var castMethod = typeof(System.Linq.Enumerable).GetMethodReference("Cast", new Type[] { typeof(IEnumerable) }).MakeGeneric(childType).Import();
                    var toArrayMethod = typeof(System.Linq.Enumerable).GetMethodReference("ToArray", 1).MakeGeneric(childType).Import();
                    var localVariable = new VariableDefinition(childType.MakeArrayType().Import());
                    processor.Body.Variables.Add(localVariable);

                    processor.Append(processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
                    processor.Append(processor.Create(OpCodes.Isinst, typeof(IEnumerable).GetTypeReference().Import()));
                    processor.Append(processor.Create(OpCodes.Call, castMethod));
                    processor.Append(processor.Create(OpCodes.Call, toArrayMethod));
                    processor.Append(processor.Create(OpCodes.Stloc, localVariable));

                    #endregion var array = (value as IEnumerable).Cast<ITestInterface>().ToArray<ITestInterface>();

                    #region for(int index = 0; index < localVariable.Length; index++)

                    var indexer = new VariableDefinition(typeof(int).GetTypeReference().Import());
                    var lengthCheck = processor.Create(OpCodes.Ldloc, indexer);

                    processor.Body.Variables.Add(indexer);
                    processor.Append(processor.Create(OpCodes.Ldc_I4_0));
                    processor.Append(processor.Create(OpCodes.Stloc, indexer));
                    processor.Append(processor.Create(OpCodes.Br_S, lengthCheck));

                    var start = processor.Create(OpCodes.Nop);
                    processor.Append(start);
                    if (!isStatic)
                        processor.Append(processor.Create(OpCodes.Ldarg_0));
                    processor.Append(processor.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field));
                    processor.Append(processor.Create(OpCodes.Ldloc, localVariable));
                    processor.Append(processor.Create(OpCodes.Ldloc, indexer));
                    processor.Append(processor.Create(OpCodes.Ldelem_Ref));
                    processor.Append(processor.Create(OpCodes.Callvirt, addReference.Import()));

                    processor.Append(processor.Create(OpCodes.Ldloc, indexer));
                    processor.Append(processor.Create(OpCodes.Ldc_I4_1));
                    processor.Append(processor.Create(OpCodes.Add));
                    processor.Append(processor.Create(OpCodes.Stloc, indexer));

                    processor.Append(lengthCheck);
                    processor.Append(processor.Create(OpCodes.Ldloc, localVariable));
                    processor.Append(processor.Create(OpCodes.Ldlen));
                    processor.Append(processor.Create(OpCodes.Conv_I4));
                    processor.Append(processor.Create(OpCodes.Clt));
                    processor.Append(processor.Create(OpCodes.Brtrue_S, start));

                    #endregion for(int index = 0; index < localVariable.Length; index++)
                }
            }
        }
    }
}