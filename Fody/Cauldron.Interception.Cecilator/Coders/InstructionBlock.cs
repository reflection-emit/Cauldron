using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class InstructionBlock : IEnumerable<Instruction>
    {
        internal readonly Method associatedMethod;

        internal readonly Builder builder;

        internal readonly List<ExceptionHandler> exceptionHandlers = new List<ExceptionHandler>();

        internal readonly ILProcessor ilprocessor;

        private readonly ListEx<Instruction> instructions = new ListEx<Instruction>();

        private TypeReference resultingType;

        private InstructionBlock(Method method)
        {
            this.builder = Builder.Current;
            this.associatedMethod = method;
            this.ilprocessor = this.associatedMethod.GetILProcessor();
            this.associatedMethod.methodDefinition.Body.SimplifyMacros();

            this.instructions.Changed += Changed;
        }

        public Instruction Last => this.instructions.Count == 0 ? null : this.instructions[this.instructions.Count - 1];

        /// <summary>
        /// Gets or sets the resulting type of the instruction block. This is only relevant for casting and relational operations
        /// </summary>
        public TypeReference ResultingType
        {
            get
            {
                if (this.resultingType != null)
                    return this.resultingType;

                return this.instructions.GetTypeOfValueInStack(this.associatedMethod);
            }
            set
            {
                if (value != null && !value.IsGenericInstance && value.HasGenericParameters)
                    this.resultingType = value.ResolveType(value);
                else
                    this.resultingType = value;
            }
        }

        public int Count => this.instructions.Count;

        public static InstructionBlock Call(
                    InstructionBlock instructionBlock,
                    object instance,
                    Method method,
                    params object[] parameters) => CallInternal(instructionBlock, instance, method, OpCodes.Call, parameters);

        /// <summary>
        /// Adds a cast or boxing or unboxing to the <see cref="InstructionBlock"/>.
        /// </summary>
        /// <param name="instructionBlock">The <see cref="InstructionBlock"/> to be modified.</param>
        /// <param name="castToType">The type to cast to.</param>
        public static void CastOrBoxValues(InstructionBlock instructionBlock, BuilderType castToType)
        {
            if (instructionBlock.instructions.Count == 0)
                throw new Exception("Cannot cast or box the last value, since this Coder has no instructions in its list.");

            if (castToType == null)
                return;

            bool GetCodeBlockFromLastType(TypeReference typeReference)
            {
                if (typeReference == null)
                    return false;

                if (typeReference.AreEqual(castToType))
                    return true;

                if (castToType.typeReference.IsPrimitive && !typeReference.IsPrimitive)
                {
                    instructionBlock.Emit(OpCodes.Unbox_Any, Builder.Current.Import(castToType.typeReference));
                    return true;
                }

                if (!castToType.typeReference.IsPrimitive && castToType.typeReference.Resolve().With(x => x.IsInterface || x.IsClass))
                {
                    instructionBlock.Emit(OpCodes.Isinst, Builder.Current.Import(castToType.typeReference));
                    return true;
                }

                CastOrBoxValuesInternal(instructionBlock, castToType);
                return true;
            }

            if (!GetCodeBlockFromLastType(instructionBlock.ResultingType))
                // This can cause exceptions in some cases
                instructionBlock.Emit(OpCodes.Isinst, Builder.Current.Import(castToType.typeReference));
        }

        /// <summary>
        /// Creates a new <see cref="InstructionBlock"/> with code that is automatically generated using the <paramref name="codingInfo"/>.
        /// </summary>
        /// <param name="instructionBlock">The <see cref="InstructionBlock"/> to use as reference.</param>
        /// <param name="targetType">
        /// The target type of the <paramref name="codingInfo"/>.
        /// If the <paramref name="codingInfo"/>'s type does not match the <paramref name="targetType"/>, code for casting will be automatically added.
        /// If the <paramref name="targetType"/> is null, then no casting code is added.
        /// </param>
        /// <param name="codingInfo">
        /// The value used to generate the code.
        /// The value can be null. In this case a null will be added to the code.
        /// </param>
        /// <returns>A new instance of <see cref="InstructionBlock"/>. This code is not added to the <paramref name="instructionBlock"/> argument.</returns>
        public static InstructionBlock CreateCode(InstructionBlock instructionBlock, BuilderType targetType, object codingInfo)
        {
            var result = instructionBlock.Spawn();

            if (codingInfo == null)
            {
                result.Emit_LdNull();
                return result;
            }

            var type = codingInfo.GetType();

            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
                codingInfo = Convert.ChangeType(codingInfo, type);
            }

            switch (codingInfo)
            {
                case string value:
                    result.Emit(OpCodes.Ldstr, value);
                    result.ResultingType = BuilderType.String.typeReference;
                    break;

                case FieldDefinition value:
                    CreateCodeForFieldReference(result, targetType, new Field(value.DeclaringType.ToBuilderType(), value), true);
                    break;

                case FieldReference value:
                    CreateCodeForFieldReference(result, targetType, new Field(value.DeclaringType.ToBuilderType(), value.Resolve(), value), true);
                    break;

                case Field value:
                    CreateCodeForFieldReference(result, targetType, value, true);
                    break;

                case int value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderType.Int32.typeReference;
                    break;

                case uint value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderType.UInt32.typeReference;
                    break;

                case bool value:
                    result.Emit(OpCodes.Ldc_I4, value ? 1 : 0);
                    result.ResultingType = BuilderType.Boolean.typeReference;
                    break;

                case char value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderType.Char.typeReference;
                    break;

                case short value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderType.Int16.typeReference;
                    break;

                case ushort value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderType.UInt16.typeReference;
                    break;

                case byte value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderType.Byte.typeReference;
                    break;

                case sbyte value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderType.SByte.typeReference;
                    break;

                case long value:
                    result.Emit(OpCodes.Ldc_I8, value);
                    result.ResultingType = BuilderType.Int64.typeReference;
                    break;

                case ulong value:
                    result.Emit(OpCodes.Ldc_I8, value);
                    result.ResultingType = BuilderType.UInt64.typeReference;
                    break;

                case double value:
                    result.Emit(OpCodes.Ldc_R8, value);
                    result.ResultingType = BuilderType.Double.typeReference;
                    break;

                case float value:
                    result.Emit(OpCodes.Ldc_R4, value);
                    result.ResultingType = BuilderType.Single.typeReference;
                    break;

                case IntPtr value:
                    result.Emit(OpCodes.Ldc_I4, (int)value);
                    result.ResultingType = BuilderType.IntPtr.typeReference;
                    break;

                case UIntPtr value:
                    result.Emit(OpCodes.Ldc_I4, (uint)value);
                    result.ResultingType = BuilderType.UIntPtr.typeReference;
                    break;

                case LocalVariable value:
                    AddVariableDefinitionToInstruction(result, targetType, value.variable);
                    break;

                case VariableDefinition value:
                    AddVariableDefinitionToInstruction(result, targetType, value);
                    break;

                case ExceptionCodeBlock exceptionCodeBlock:
                    {
                        var variable = char.IsNumber(exceptionCodeBlock.name, 0) ?
                            instructionBlock.associatedMethod.methodDefinition.Body.Variables[int.Parse(exceptionCodeBlock.name)] :
                            instructionBlock.associatedMethod.GetVariable(exceptionCodeBlock.name);

                        result.Emit(OpCodes.Ldloc, variable);
                        result.ResultingType = instructionBlock.builder.Import(variable.VariableType);
                        break;
                    }
                case ParametersCodeBlock parametersCodeBlock:
                    {
                        var parameterInfos = parametersCodeBlock.GetTargetType(instructionBlock.associatedMethod);
                        if (parameterInfos.Item2 < 0)
                        {
                            result.Emit(OpCodes.Ldarg_0);
                            result.ResultingType = parameterInfos.Item1.typeReference;
                            break;
                        }

                        result.Emit(OpCodes.Ldarg, parameterInfos.Item2);
                        result.ResultingType = parameterInfos.Item1.typeReference;
                        break;
                    }

                case ThisCodeBlock thisCodeBlock:
                    result.Emit(instructionBlock.associatedMethod.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0);
                    result.ResultingType = instructionBlock.associatedMethod.OriginType.typeReference;
                    break;

                case InitObjCodeBlock initObjCodeBlock:
                    {
                        var variable = instructionBlock.associatedMethod.GetOrCreateVariable(initObjCodeBlock.typeReference);
                        result.Emit(OpCodes.Ldloca, variable.variable);
                        result.Emit(OpCodes.Initobj, initObjCodeBlock.typeReference);
                        result.Emit(OpCodes.Ldloc, variable.variable);
                        result.ResultingType = initObjCodeBlock.typeReference;
                        break;
                    }
                case DefaultTaskCodeBlock defaultTaskCodeBlock:
                    {
                        var taskType = instructionBlock.associatedMethod.type.Builder.GetType("System.Threading.Tasks.Task");
                        var resultFrom = taskType.GetMethod("FromResult", 1, true).MakeGeneric(typeof(int));

                        result.Append(Call(result, null, resultFrom, 0));
                        result.ResultingType = result.associatedMethod.ReturnType.typeReference;
                        break;
                    }

                case DefaultTaskOfTCodeBlock defaultTaskOfTCodeBlock:
                    {
                        var returnType = instructionBlock.associatedMethod.ReturnType.GetGenericArgument(0);
                        var taskType = instructionBlock.associatedMethod.type.Builder.GetType("System.Threading.Tasks.Task");
                        var resultFrom = taskType.GetMethod("FromResult", 1, true).MakeGeneric(returnType);

                        result.Append(Call(result, null, resultFrom, returnType.DefaultValue));
                        result.ResultingType = returnType.typeReference;
                        break;
                    }
                case DefaultValueCodeBlock value:
                    var defaultValue = value.builderType.DefaultValue;

                    result.Append(InstructionBlock.CreateCode(result,
                        value.builderType.GenericArguments().Any() ? value.builderType.GetGenericArgument(0) : value.builderType, defaultValue));
                    break;

                case CoderBase coder:
                    {
                        result.Append(coder.instructions);
                        result.ResultingType = result.ResultingType;
                        break;
                    }

                case InstructionsCodeBlock instructionsCodeBlock:
                    {
                        result.Append(instructionsCodeBlock.instructions);
                        break;
                    }

                case TypeReference value:
                    result.Append(instructionBlock.ilprocessor.TypeOf(value), instructionBlock.builder.Import(typeof(Type)));
                    break;

                case BuilderType value:
                    result.Append(instructionBlock.ilprocessor.TypeOf(value.typeReference), instructionBlock.builder.Import(typeof(Type)));
                    break;

                case Method method:
                    if (targetType == BuilderType.IntPtr)
                    {
                        if (!method.IsStatic && method.OriginType != instructionBlock.associatedMethod.OriginType && instructionBlock.associatedMethod.OriginType.IsAsyncStateMachine)
                        {
                            var instance = instructionBlock.associatedMethod.AsyncMethodHelper.Instance;
                            result.Append(InstructionBlock.CreateCode(result, targetType, instance));
                        }
                        else
                            result.Emit(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0);

                        result.Emit(OpCodes.Ldftn, method.methodReference);
                        result.ResultingType = BuilderType.IntPtr.typeReference;
                    }
                    else
                    {
                        // methodof
                        result.Emit(OpCodes.Ldtoken, method.methodReference);
                        result.Emit(OpCodes.Ldtoken, method.OriginType.typeReference);
                        result.Emit(OpCodes.Call, instructionBlock.builder.Import(BuilderType.MethodBase.GetMethod("GetMethodFromHandle", 2, true)));

                        result.ResultingType = BuilderType.MethodBase.typeReference;
                    }
                    break;

                case InstructionBlock value:
                    result.Append(value);
                    break;

                case ParameterDefinition value:
                    result.Emit(OpCodes.Ldarg, value);
                    result.ResultingType = value.ParameterType;
                    break;

                case ParameterReference value:
                    result.Emit(OpCodes.Ldarg, value.Index);
                    result.ResultingType = value.ParameterType;
                    break;

                case Position value:
                    result.Append(value.instruction);
                    break;

                case IEnumerable<Instruction> value:
                    result.Append(value);
                    break;

                default:
                    throw new NotImplementedException($"Unknown type: {codingInfo.GetType().FullName}");
            }

            if (result.ResultingType == null || targetType == null || result.ResultingType.AreEqual(targetType))
                return result;

            CastOrBoxValuesInternal(result, targetType);

            return result;
        }

        public static implicit operator InstructionBlock(Method method) => new InstructionBlock(method);

        public static InstructionBlock Invert(InstructionBlock instructionBlock)
        {
            var result = instructionBlock.Spawn();

            result.Emit(OpCodes.Ldc_I4_0);
            result.Emit(OpCodes.Ceq);

            return result;
        }

        public static InstructionBlock Negate(InstructionBlock instructionBlock)
        {
            var result = instructionBlock.Spawn();
            result.Emit(OpCodes.Neg);
            return result;
        }

        public static InstructionBlock NewObj(
            InstructionBlock instructionBlock,
            Method method,
            params object[] parameters) => CallInternal(instructionBlock, null, method, OpCodes.Newobj, parameters);

        public static InstructionBlock SetValue(InstructionBlock instructionBlock, LocalVariable localVariable, object value)
        {
            var result = instructionBlock.Spawn();

            // value to assign
            if (localVariable.Type.IsNullable && value == null)
            {
                // If the nullable is assigned a null
                result.Emit(OpCodes.Ldloca, localVariable);
                result.Emit(OpCodes.Initobj, localVariable.Type);
            }
            else if (localVariable.Type.IsNullable && value.GetType().With(x => x.IsValueType && !x.IsGenericType && Nullable.GetUnderlyingType(x) == null))
            {
                // If the nullable is assigned a value
                result.Emit(OpCodes.Ldloca, localVariable);
                result.Append(InstructionBlock.CreateCode(instructionBlock, null, value));
                result.Emit(OpCodes.Call, localVariable.Type.GetMethod(".ctor", 1).Import());
            }
            else
            {
                // Other types
                result.Append(InstructionBlock.CreateCode(instructionBlock, localVariable.Type, value));

                // Save to local variable
                switch (localVariable.Index)
                {
                    case 0: result.Emit(OpCodes.Stloc_0); break;
                    case 1: result.Emit(OpCodes.Stloc_1); break;
                    case 2: result.Emit(OpCodes.Stloc_2); break;
                    case 3: result.Emit(OpCodes.Stloc_3); break;
                    default: result.Emit(OpCodes.Stloc, localVariable); break;
                }
            }

            return result;
        }

        public static InstructionBlock SetValue(InstructionBlock instructionBlock, ParametersCodeBlock parametersCodeBlock, object value)
        {
            var result = instructionBlock.Spawn();
            var argInfo = parametersCodeBlock.GetTargetType(instructionBlock.associatedMethod);

            // value to assign
            if (argInfo.Item1.IsNullable && value == null)
            {
                // If the nullable is assigned a null
                result.Emit(OpCodes.Ldarga, argInfo.Item2);
                result.Emit(OpCodes.Initobj, argInfo.Item1);
            }
            else if (argInfo.Item1.IsNullable && value.GetType().With(x => x.IsValueType && !x.IsGenericType && Nullable.GetUnderlyingType(x) == null))
            {
                // If the nullable is assigned a value
                result.Emit(OpCodes.Ldarga, argInfo.Item3);
                result.Append(InstructionBlock.CreateCode(instructionBlock, null, value));
                result.Emit(OpCodes.Call, argInfo.Item1.GetMethod(".ctor", 1).Import());
            }
            else
                // Other types
                result.Append(InstructionBlock.CreateCode(instructionBlock, argInfo.Item1, value));

            // Save to local variable
            switch (argInfo.Item2)
            {
                case 0: result.Emit(OpCodes.Ldarg_0); break;
                case 1: result.Emit(OpCodes.Ldarg_1); break;
                case 2: result.Emit(OpCodes.Ldarg_2); break;
                case 3: result.Emit(OpCodes.Ldarg_3); break;
                default: result.Emit(OpCodes.Ldarg, argInfo.Item2); break;
            }

            return result;
        }

        public static InstructionBlock SetValue(InstructionBlock instructionBlock, object instance, Field field, object value)
        {
            var result = instructionBlock.Spawn();

            // Instance
            if (!field.IsStatic && instance != null)
                result.Append(InstructionBlock.CreateCode(instructionBlock, null, instance));

            // value to assign
            if (field.FieldType.IsNullable && value == null)
            {
                // If the nullable is assigned a null
                result.Emit(field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda, field.fieldRef);
                result.Emit(OpCodes.Initobj, field.fieldRef.FieldType);
            }
            else if (field.FieldType.IsNullable && value.GetType().With(x => x.IsValueType && !x.IsGenericType && Nullable.GetUnderlyingType(x) == null))
            {
                // If the nullable is assigned a value
                result.Append(InstructionBlock.CreateCode(instructionBlock, null, value));
                result.Emit(OpCodes.Newobj, field.FieldType.GetMethod(".ctor", 1).Import());
            }
            else
                // Other types
                result.Append(InstructionBlock.CreateCode(instructionBlock, field.FieldType, value));
            // Save to field
            result.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef);

            return result;
        }

        public void Append(InstructionBlock instructionBlock)
        {
            this.exceptionHandlers.AddRange(instructionBlock.exceptionHandlers);
            this.instructions.AddRange(instructionBlock.instructions);
            this.ResultingType = instructionBlock.ResultingType;
        }

        public void Append(Instruction instruction) => this.instructions.Add(instruction);

        public void Append(IEnumerable<Instruction> instructions) => this.instructions.AddRange(instructions);

        public void Append(IEnumerable<Instruction> instructions, TypeReference resultingType)
        {
            this.instructions.AddRange(instructions);
            this.ResultingType = resultingType;
        }

        public void Clear()
        {
            exceptionHandlers.Clear();
            instructions.Clear();
            resultingType = null;
        }

        public void Display()
        {
            var builder = Builder.Current;
            builder.Log(LogTypes.Info, $"-- {this.associatedMethod} --");
            foreach (var item in this.instructions)
                builder.Log(LogTypes.Info, $"IL_{item.Offset.ToString("X4")}: {item.OpCode.ToString()} { (item.Operand is Instruction ? "IL_" + (item.Operand as Instruction).Offset.ToString("X4") : item.Operand?.ToString())} ");
        }

        public void Emit(OpCode opcode, ParameterDefinition parameter) => this.instructions.Add(ilprocessor.Create(opcode, parameter));

        public void Emit(OpCode opcode, ParametersCodeBlock parameter) => this.instructions.Add(ilprocessor.Create(opcode, parameter.GetTargetType(this.associatedMethod).Item3));

        public void Emit(OpCode opcode, VariableDefinition variable) => this.instructions.Add(ilprocessor.Create(opcode, variable));

        public void Emit(OpCode opcode, LocalVariable variable) => this.instructions.Add(ilprocessor.Create(opcode, variable.variable));

        public void Emit(OpCode opcode, Instruction[] targets) => this.instructions.Add(ilprocessor.Create(opcode, targets));

        public void Emit(OpCode opcode, Instruction target) => this.instructions.Add(ilprocessor.Create(opcode, target));

        public void Emit(OpCode opcode, double value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, float value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, long value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, ulong value) => this.instructions.Add(ilprocessor.Create(opcode, unchecked((long)value)));

        public void Emit(OpCode opcode) => this.instructions.Add(ilprocessor.Create(opcode));

        public void Emit(OpCode opcode, byte value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, sbyte value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, string value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, FieldReference field) => this.instructions.Add(ilprocessor.Create(opcode, field));

        public void Emit(OpCode opcode, Field field) => this.instructions.Add(ilprocessor.Create(opcode, field.fieldRef));

        public void Emit(OpCode opcode, MethodReference method) => this.instructions.Add(ilprocessor.Create(opcode, method));

        public void Emit(OpCode opcode, Method method) => this.instructions.Add(ilprocessor.Create(opcode, method.methodReference));

        public void Emit(OpCode opcode, CallSite site) => this.instructions.Add(ilprocessor.Create(opcode, site));

        public void Emit(OpCode opcode, TypeReference type) => this.instructions.Add(ilprocessor.Create(opcode, type));

        public void Emit(OpCode opcode, BuilderType type) => this.instructions.Add(ilprocessor.Create(opcode, type.typeReference));

        public void Emit(OpCode opcode, short value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, ushort value) => this.instructions.Add(ilprocessor.Create(opcode, unchecked((short)value)));

        public void Emit(OpCode opcode, int value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, uint value) => this.instructions.Add(ilprocessor.Create(opcode, unchecked((int)value)));

        public void Emit_LdNull() => this.Emit(OpCodes.Ldnull);

        public void Emit_Nop() => this.Emit(OpCodes.Nop);

        public IEnumerator<Instruction> GetEnumerator() => this.instructions.GetEnumerator();

        public Instruction[] LastElements(int count)
        {
            if (this.instructions.Count < count)
                count = this.instructions.Count;

            var result = new Instruction[count];
            int counter = 0;

            for (int i = this.instructions.Count - count; i < this.instructions.Count; i++)
                result[counter++] = this.instructions[i];

            return result;
        }

        public void Prepend(IEnumerable<Instruction> instructions) => this.instructions.InsertRange(0, instructions);

        public void Remove(int index) => this.instructions.RemoveAt(index);

        public InstructionBlock Spawn() => new InstructionBlock(this.associatedMethod);

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in this.instructions)
                sb.AppendLine($"IL_{item.Offset.ToString("X4")}: {item.OpCode.ToString()} { (item.Operand is Instruction ? "IL_" + (item.Operand as Instruction).Offset.ToString("X4") : item.Operand?.ToString())} ");
            return sb.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.instructions.GetEnumerator();

        internal static bool AddBinaryOperation(InstructionBlock instructionBlock, OpCode opCode, BuilderType a, BuilderType b, object valueB)
        {
            var methodName = "";

            if (opCode == OpCodes.And) methodName = "op_BitwiseAnd";
            else if (opCode == OpCodes.Or) methodName = "op_BitwiseOr";

            if (b == null)
            {
                if (valueB is CoderBase value)
                    b = value.instructions.ResultingType?.ToBuilderType();
                else
                    b = valueB?.GetType().ToBuilderType();
            }

            if (b == null)
                return false;

            var @operator = a.GetMethod(methodName, false, a, b)?.Import();

            if (@operator == null)
                @operator = b.GetMethod(methodName, false, a, b)?.Import();

            if (@operator != null)
            {
                instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, b, valueB));
                instructionBlock.Emit(OpCodes.Call, @operator);
                return true;
            }

            return false;
        }

        internal static void AreEqualInternalWithoutJump(InstructionBlock instructionBlock, BuilderType secondType, object secondValue)
        {
            // TODO - needs to handle Nullables

            if (instructionBlock.Count == 0 && instructionBlock.associatedMethod.IsStatic)
                throw new NotSupportedException($"The method {instructionBlock.associatedMethod.Name} is static. Load a value before using a relational operator.");

            var firstType = instructionBlock.ResultingType?.ToBuilderType();

            if (secondType == null && secondValue is CoderBase coderBase)
                secondType = coderBase.instructions.ResultingType?.ToBuilderType() ?? secondValue?.GetType()?.ToBuilderType();

            if (instructionBlock.Count == 0)
            {
                instructionBlock.Emit(OpCodes.Ldarg_0);
                firstType = instructionBlock.ResultingType?.ToBuilderType();
            }

            if (firstType == secondType && firstType.IsPrimitive)
            {
                instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, secondType, secondValue));
                instructionBlock.Emit(OpCodes.Ceq);

                return;
            }

            var equalityOperator = firstType.GetMethod("op_Equality", false, firstType, secondType)?.Import() ?? secondType.GetMethod("op_Equality", false, firstType, secondType)?.Import();

            if (equalityOperator != null)
            {
                instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, secondType, secondValue));
                instructionBlock.Emit(OpCodes.Call, equalityOperator);
                return;
            }

            // This is a special case for stuff we surely know how to convert
            // It is almost like the first block, but with forced target types
            if (firstType.IsPrimitive && secondType.IsPrimitive)
            {
                var typeToUse = GetTypeWithMoreCapacity(firstType.typeReference, secondType.typeReference).ToBuilderType();
                InstructionBlock.CastOrBoxValues(instructionBlock, typeToUse);
                instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, typeToUse, secondValue));
                instructionBlock.Emit(OpCodes.Ceq);

                return;
            }

            equalityOperator = BuilderType
                .Object
                .GetMethod("Equals", false, BuilderType.Object, BuilderType.Object).Import();

            InstructionBlock.CastOrBoxValues(instructionBlock, BuilderType.Object);
            instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, BuilderType.Object, secondValue));
            instructionBlock.Emit(OpCodes.Call, equalityOperator);
        }

        internal static void CreateCodeForFieldReference(
            InstructionBlock result,
            BuilderType targetType,
            Field valueField,
            bool autoAddThisInstance)
        {
            if (!valueField.IsStatic && autoAddThisInstance)
                result.Emit(OpCodes.Ldarg_0);

            if (valueField.FieldType.IsValueType && targetType == null)
                result.Emit(valueField.IsStatic ?
                    OpCodes.Ldsflda :
                    OpCodes.Ldflda, valueField);
            else
                result.Emit(valueField.IsStatic ?
                    OpCodes.Ldsfld :
                    OpCodes.Ldfld, valueField);

            result.ResultingType = valueField.FieldType.typeReference;
        }

        internal static void CreateCodeForVariableDefinition(
            InstructionBlock result,
            BuilderType targetType,
            LocalVariable localVariable)
        {
            if (localVariable.variable.VariableType.IsValueType &&
                !localVariable.variable.VariableType.IsPrimitive &&
                !localVariable.Type.IsNullable)
                result.Emit(OpCodes.Ldloca, localVariable.variable);
            else
            {
                switch (localVariable.Index)
                {
                    case 0: result.Emit(OpCodes.Ldloc_0); break;
                    case 1: result.Emit(OpCodes.Ldloc_1); break;
                    case 2: result.Emit(OpCodes.Ldloc_2); break;
                    case 3: result.Emit(OpCodes.Ldloc_3); break;
                    default:
                        result.Emit(OpCodes.Ldloc, localVariable.variable);
                        break;
                }
            }

            result.ResultingType = localVariable.Type.typeReference ?? localVariable.Type.typeDefinition;
        }

        private static void AddVariableDefinitionToInstruction(InstructionBlock instructionBlock, BuilderType targetType, VariableDefinition value)
        {
            var index = value.Index;

            if (value.VariableType.IsValueType && targetType == null)
                instructionBlock.Emit(OpCodes.Ldloca, value);
            else
                switch (index)
                {
                    case 0: instructionBlock.Emit(OpCodes.Ldloc_0); break;
                    case 1: instructionBlock.Emit(OpCodes.Ldloc_1); break;
                    case 2: instructionBlock.Emit(OpCodes.Ldloc_2); break;
                    case 3: instructionBlock.Emit(OpCodes.Ldloc_3); break;
                    default:
                        instructionBlock.Emit(OpCodes.Ldloc, value);
                        break;
                }

            instructionBlock.ResultingType = value.VariableType;
        }

        private static InstructionBlock CallInternal(InstructionBlock instructionBlock, object instance, Method method, OpCode opcode, params object[] parameters)
        {
            var result = instructionBlock.Spawn();

            if (instance != null && !method.IsStatic)
                result.Append(InstructionBlock.CreateCode(instructionBlock, null, instance));

            if (parameters != null && parameters.Length > 0 && parameters[0] is ArrayCodeBlock arrayCodeSet)
            {
                var methodParameters = method.methodDefinition.Parameters;
                for (int i = 0; i < methodParameters.Count; i++)
                {
                    result.Append(InstructionBlock.CreateCode(instructionBlock, null, arrayCodeSet));
                    result.Append(InstructionBlock.CreateCode(instructionBlock, null, i));
                    result.Emit(OpCodes.Ldelem_Ref);

                    CastOrBoxValues(result, methodParameters[i].ParameterType.ToBuilderType());
                }
            }
            else if (parameters != null && parameters.Length > 0 && parameters[0] is ParametersCodeBlock parameterCodeSet && parameterCodeSet.IsAllParameters)
            {
                if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli && opcode != OpCodes.Newobj)
                    opcode = OpCodes.Callvirt;

                for (int i = 0; i < method.methodReference.Parameters.Count; i++)
                {
                    var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                        method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                        method.methodDefinition.Parameters[i].ParameterType;

                    result.Append(InstructionBlock.CreateCode(result, parameterType.ToBuilderType().Import(), CodeBlocks.GetParameter(i)));
                }
            }
            else
            {
                if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli && opcode != OpCodes.Newobj)
                    opcode = OpCodes.Callvirt;

                if (parameters != null)
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                            method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                            method.methodDefinition.Parameters[i].ParameterType;

                        result.Append(InstructionBlock.CreateCode(result, parameterType.ToBuilderType().Import(), parameters[i]));
                    }
            }

            try
            {
                result.Emit(opcode, Builder.Current.Import(method.methodReference));
            }
            catch (NullReferenceException)
            {
                result.Emit(opcode, Builder.Current.Import(method.methodReference, result.associatedMethod.methodReference));
            }

            return result;
        }

        private static void CastOrBoxValuesInternal(InstructionBlock instructionBlock, BuilderType targetType)
        {
            // TODO - Support for nullable types required

            if (targetType == null)
                return;

            bool IsInstRequired()
            {
                if (targetType.IsPrimitive)
                    return false;

                if (targetType == BuilderType.String ||
                    instructionBlock.ResultingType.AreReferenceAssignable(targetType.typeReference) ||
                    targetType.IsInterface)
                    return true;

                if (targetType.IsValueType)
                    return false;

                if (targetType.IsArray)
                    return false;

                if (targetType == BuilderType.IEnumerable1)
                    return false;

                return false;
            }

            // Special case - If the values has implcit or explicit converters
            var resultingTypeBuilderType = instructionBlock.ResultingType.ToBuilderType();
            var @operator = resultingTypeBuilderType.GetMethod(Modifiers.PublicStatic, targetType, "op_Implicit", resultingTypeBuilderType)?.Import();
            if (@operator == null) @operator = targetType.GetMethod(Modifiers.PublicStatic, targetType, "op_Implicit", resultingTypeBuilderType)?.Import();
            if (@operator == null) @operator = resultingTypeBuilderType.GetMethod(Modifiers.PublicStatic, targetType, "op_Explicit", resultingTypeBuilderType)?.Import();
            if (@operator == null) @operator = targetType.GetMethod(Modifiers.PublicStatic, targetType, "op_Explicit", resultingTypeBuilderType)?.Import();

            if (@operator != null) instructionBlock.Emit(OpCodes.Call, @operator);
            else if (targetType.IsNullable) instructionBlock.Emit(OpCodes.Newobj, targetType.GetMethod(".ctor", 1).Import());
            // TODO - adds additional checks for not resolved generics
            else if (targetType.IsGenericType && instructionBlock.ResultingType.Resolve() != null) /* This happens if the target type is a generic */ instructionBlock.Emit(OpCodes.Unbox_Any, targetType);
            else if (IsInstRequired()) instructionBlock.Emit(OpCodes.Isinst, targetType.Import());
            else if (targetType.IsEnum)
            {
                if (instructionBlock.ResultingType.AreEqual(BuilderType.String.typeReference))
                {
                    instructionBlock.Prepend(instructionBlock.ilprocessor.TypeOf(targetType));

                    instructionBlock.Append(instructionBlock.ilprocessor.TypeOf(targetType.Import()));
                    instructionBlock.Emit(OpCodes.Call, BuilderType.Enum.GetMethod("GetUnderlyingType", true, typeof(Type)).Import());
                    instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ChangeType", true, typeof(object), typeof(Type)).Import());
                    instructionBlock.Emit(OpCodes.Call, BuilderType.Enum.GetMethod("ToObject", true, typeof(Type), typeof(object)).Import());
                    instructionBlock.Emit(OpCodes.Unbox_Any, targetType);
                }
                else
                    instructionBlock.Emit(OpCodes.Unbox_Any, targetType);

                // Bug #23
                //result.Instructions.InsertRange(0, this.TypeOf(processor, targetType));

                //result.Instructions.AddRange(this.TypeOf(processor, Builder.Current.Import(targetType)));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }))));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }))));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }))));
            }
            else if (instructionBlock.ResultingType.AreEqual(BuilderType.Object) && (targetType.IsArray || targetType == BuilderType.IEnumerable1))
            {
                var childType = Builder.Current.GetChildrenType(targetType.typeReference);
                var castMethod = BuilderType.Enumerable.GetMethod("Cast", true, BuilderType.IEnumerable).MakeGeneric(childType).Import();
                var toArrayMethod = BuilderType.Enumerable.GetMethod("ToArray", 1).MakeGeneric(childType).Import();

                instructionBlock.Emit(OpCodes.Isinst, BuilderType.IEnumerable);
                instructionBlock.Emit(OpCodes.Call, castMethod);

                if (targetType.IsArray)
                    instructionBlock.Emit(OpCodes.Call, toArrayMethod);
            }
            else if
                (
                    (instructionBlock.ResultingType.AreEqual(BuilderType.Object) && targetType.IsValueType) ||
                    (!instructionBlock.ResultingType.AreEqual(targetType) && instructionBlock.ResultingType.IsPrimitive && targetType.IsPrimitive)
                )
            {
                if (targetType == BuilderType.Int32) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToInt32", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.UInt32) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToUInt32", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.Boolean) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToBoolean", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.Byte) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToByte", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.Char) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToChar", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.DateTime) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToDateTime", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.Decimal) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToDecimal", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.Double) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToDouble", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.Int16) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToInt16", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.Int64) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToInt64", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.SByte) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToSByte", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.Single) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToSingle", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.UInt16) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToUInt16", true, instructionBlock.ResultingType).Import());
                else if (targetType == BuilderType.UInt64) instructionBlock.Emit(OpCodes.Call, BuilderType.Convert.GetMethod("ToUInt64", true, instructionBlock.ResultingType).Import());
                else instructionBlock.Emit(OpCodes.Unbox_Any, targetType);
            }
            else if ((instructionBlock.ResultingType.Resolve() == null || instructionBlock.ResultingType.IsValueType) && !targetType.IsValueType)
                instructionBlock.Emit(OpCodes.Box, instructionBlock.ResultingType);
            else if (instructionBlock.instructions.Last().OpCode != OpCodes.Ldnull && targetType == BuilderType.Object)
            {
                // Nope nothing....
            }
            else if (
                    instructionBlock.instructions.Last().OpCode != OpCodes.Ldnull &&
                    !instructionBlock.ResultingType.AreEqual(targetType) &&
                    targetType.typeReference.AreReferenceAssignable(instructionBlock.ResultingType))
                instructionBlock.Emit(OpCodes.Castclass, Builder.Current.Import(instructionBlock.ResultingType));
        }

        private static TypeReference GetTypeWithMoreCapacity(TypeReference a, TypeReference b)
        {
            // Bool makes a big exception here
            if (a.FullName == typeof(bool).FullName) return a;
            if (b.FullName == typeof(bool).FullName) return b;

            if (a.FullName == typeof(decimal).FullName) return a;
            if (b.FullName == typeof(decimal).FullName) return b;

            if (a.FullName == typeof(double).FullName) return a;
            if (b.FullName == typeof(double).FullName) return b;

            if (a.FullName == typeof(float).FullName) return a;
            if (b.FullName == typeof(float).FullName) return b;

            if (a.FullName == typeof(ulong).FullName) return a;
            if (b.FullName == typeof(ulong).FullName) return b;

            if (a.FullName == typeof(long).FullName) return a;
            if (b.FullName == typeof(long).FullName) return b;

            if (a.FullName == typeof(uint).FullName) return a;
            if (b.FullName == typeof(uint).FullName) return b;

            if (a.FullName == typeof(int).FullName) return a;
            if (b.FullName == typeof(int).FullName) return b;

            if (a.FullName == typeof(char).FullName) return a;
            if (b.FullName == typeof(char).FullName) return b;

            if (a.FullName == typeof(ushort).FullName) return a;
            if (b.FullName == typeof(ushort).FullName) return b;

            if (a.FullName == typeof(short).FullName) return a;
            if (b.FullName == typeof(short).FullName) return b;

            if (a.FullName == typeof(byte).FullName) return a;
            if (b.FullName == typeof(byte).FullName) return b;

            if (a.FullName == typeof(sbyte).FullName) return a;
            if (b.FullName == typeof(sbyte).FullName) return b;

            return a;
        }

        private void Changed(object sender, EventArgs e) => this.resultingType = null;
    }
}