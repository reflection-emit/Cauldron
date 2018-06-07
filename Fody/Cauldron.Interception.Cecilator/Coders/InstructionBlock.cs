using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public int Count => this.instructions.Count;
        public Instruction First => this.instructions.Count == 0 ? null : this.instructions[0];
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

        public Instruction this[int index]
        {
            get => this.instructions[index];
            set => this.instructions[index] = value;
        }

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

                if (castToType.typeReference.IsValueType && !typeReference.IsValueType)
                {
                    instructionBlock.Emit(OpCodes.Unbox_Any, Builder.Current.Import(castToType.typeReference));
                    return true;
                }

                if (!castToType.typeReference.IsValueType && typeReference.IsValueType)
                {
                    instructionBlock.Emit(OpCodes.Box, Builder.Current.Import(typeReference));
                    return true;
                }

                if (!castToType.typeReference.IsValueType && castToType.typeReference.BetterResolve().With(x => x.IsInterface || x.IsClass) && !castToType.typeReference.IsArray)
                {
                    if (!castToType.typeReference.AreEqual((TypeReference)BuilderTypes.Object))
                        instructionBlock.Emit(OpCodes.Isinst, Builder.Current.Import(castToType.typeReference));

                    return true;
                }

                CastOrBoxValuesInternal(instructionBlock, castToType);
                return true;
            }

            if (!GetCodeBlockFromLastType(instructionBlock.ResultingType) &&
                !GetCodeBlockFromLastType(BuilderTypes.Object) &&
                instructionBlock.ResultingType != null &&
                !instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object))
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
                    result.ResultingType = BuilderTypes.String;
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
                    result.ResultingType = BuilderTypes.Int32;
                    break;

                case uint value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderTypes.UInt32;
                    break;

                case bool value:
                    result.Emit(OpCodes.Ldc_I4, value ? 1 : 0);
                    result.ResultingType = BuilderTypes.Boolean;
                    break;

                case char value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderTypes.Char;
                    break;

                case short value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderTypes.Int16;
                    break;

                case ushort value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderTypes.UInt16;
                    break;

                case byte value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderTypes.Byte;
                    break;

                case sbyte value:
                    result.Emit(OpCodes.Ldc_I4, value);
                    result.ResultingType = BuilderTypes.SByte;
                    break;

                case long value:
                    result.Emit(OpCodes.Ldc_I8, value);
                    result.ResultingType = BuilderTypes.Int64;
                    break;

                case ulong value:
                    result.Emit(OpCodes.Ldc_I8, value);
                    result.ResultingType = BuilderTypes.UInt64;
                    break;

                case double value:
                    result.Emit(OpCodes.Ldc_R8, value);
                    result.ResultingType = BuilderTypes.Double;
                    break;

                case float value:
                    result.Emit(OpCodes.Ldc_R4, value);
                    result.ResultingType = BuilderTypes.Single;
                    break;

                case IntPtr value:
                    result.Emit(OpCodes.Ldc_I4, (int)value);
                    result.ResultingType = BuilderTypes.IntPtr;
                    break;

                case UIntPtr value:
                    result.Emit(OpCodes.Ldc_I4, (uint)value);
                    result.ResultingType = BuilderTypes.UIntPtr;
                    break;

                case LocalVariable value:
                    CreateCodeForVariableDefinition(result, targetType, value.variable);
                    break;

                case VariableDefinition value:
                    CreateCodeForVariableDefinition(result, targetType, value);
                    break;

                case ExceptionCodeBlock exceptionCodeBlock:
                    {
                        var variable = char.IsNumber(exceptionCodeBlock.name, 0) ?
                            instructionBlock.associatedMethod.methodDefinition.Body.Variables[int.Parse(exceptionCodeBlock.name)] :
                            instructionBlock.associatedMethod.GetVariable(exceptionCodeBlock.name)?.variable;

                        result.Emit(OpCodes.Ldloc, variable);
                        result.ResultingType = instructionBlock.builder.Import(variable.VariableType);
                        break;
                    }
                case ParametersCodeBlock parametersCodeBlock:
                    {
                        var parameterInfos = parametersCodeBlock.GetTargetType(instructionBlock.associatedMethod);
                        if (parameterInfos == null)
                        {
                            result.Emit_LdNull();
                            result.ResultingType = null;
                            break;
                        }

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
                case ParametersVariableCodeBlock parametersVariableCodeBlock:
                    {
                        CreateCodeForVariableDefinition(result, targetType, parametersVariableCodeBlock.variable);
                        result.ResultingType = parametersVariableCodeBlock.variable.VariableType;
                        break;
                    }
                case NullableCodeBlock nullableCodeBlock:
                    {
                        if (nullableCodeBlock.variable == null)
                        {
                            instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, null, nullableCodeBlock.value));
                            ModifyValueTypeInstance(instructionBlock);
                        }
                        else
                            instructionBlock.Emit(OpCodes.Ldloca, nullableCodeBlock.variable);

                        instructionBlock.Emit(OpCodes.Call, nullableCodeBlock.builderType.GetMethod("GetValueOrDefault").Import());
                        result.ResultingType = nullableCodeBlock.builderType.GetGenericArgument(0)?.typeReference;
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

                    if (targetType != null && targetType.IsGenericParameter)
                    {
                        var variable = instructionBlock.associatedMethod.GetOrCreateVariable(targetType);
                        result.Emit(OpCodes.Ldloca, variable.variable);
                        result.Emit(OpCodes.Initobj, targetType);
                        result.Emit(OpCodes.Ldloc, variable.variable);
                        result.ResultingType = targetType.typeReference;
                        break;
                    }
                    else
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
                    if (targetType == BuilderTypes.IntPtr)
                    {
                        result.Emit(OpCodes.Ldftn, method.methodReference);
                        result.ResultingType = BuilderTypes.IntPtr;
                    }
                    else
                    {
                        // methodof
                        result.Emit(OpCodes.Ldtoken, method.methodReference);
                        result.Emit(OpCodes.Ldtoken, method.OriginType.typeReference);
                        result.Emit(OpCodes.Call, instructionBlock.builder.Import(BuilderTypes.MethodBase.GetMethod_GetMethodFromHandle()));

                        result.ResultingType = BuilderTypes.MethodBase;
                    }
                    break;

                case CallMethodCodeBlock callMethodCodeBlock:
                    result.Append(Call(instructionBlock, CodeBlocks.This, callMethodCodeBlock.method));
                    break;

                case InstructionBlock value:
                    result.Append(value);
                    result.ResultingType = value.ResultingType;
                    break;

                case ParameterArrayCodeBlock value:
                    {
                        var param = ParametersCodeBlock.GetParameter(instructionBlock.associatedMethod, value.index);
                        result.Emit(OpCodes.Ldarg, param.Index + (instructionBlock.associatedMethod.IsStatic ? 0 : 1));
                        result.ResultingType = param.ParameterType;
                        break;
                    }
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

        public static bool operator !=(InstructionBlock a, Instruction b)
        {
            if (!object.ReferenceEquals(a, b))
                return true;

            if (!object.Equals(a, b))
                return true;

            return true;
        }

        public static bool operator ==(InstructionBlock a, Instruction b)
        {
            if (object.ReferenceEquals(a, b))
                return true;

            if (object.Equals(a, b))
                return true;

            return false;
        }

        public static InstructionBlock SetValue(InstructionBlock instructionBlock, LocalVariable localVariable, object value)
        {
            ModifyValueTypeInstance(instructionBlock);
            var result = instructionBlock.Spawn();

            // value to assign
            if (localVariable.Type.IsNullable && value == null)
            {
                // If the nullable is assigned a null
                result.Emit(OpCodes.Ldloca, localVariable);
                result.Emit(OpCodes.Initobj, localVariable.Type);
            }
            else if (localVariable.Type.IsNullable && value.With(x =>
            {
                if (x is ParametersCodeBlock parametersCodeBlock && parametersCodeBlock.index.HasValue &&
                    ParametersCodeBlock.GetTargetType(instructionBlock.associatedMethod, parametersCodeBlock.index.Value).typeReference.AreEqual(localVariable.Type))
                    return false;

                if (x.GetType().AreEqual(localVariable.Type))
                    return false;

                if (x.GetType().With(y => y.IsValueType && !y.IsGenericType && Nullable.GetUnderlyingType(y) == null))
                    return true;

                if (x is CodeBlock)
                    return true;

                return false;
            }))
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
            ModifyValueTypeInstance(instructionBlock);
            var result = instructionBlock.Spawn();
            var argInfo = parametersCodeBlock.GetTargetType(instructionBlock.associatedMethod);

            // value to assign
            if (argInfo.Item1.IsNullable && value == null)
            {
                // If the nullable is assigned a null
                result.Emit(OpCodes.Ldarga, argInfo.Item2);
                result.Emit(OpCodes.Initobj, argInfo.Item1);
            }
            else if (argInfo.Item1.IsNullable && value.With(x =>
            {
                if (x is ParametersCodeBlock parametersCodeBlockValue && parametersCodeBlockValue.index.HasValue &&
                    ParametersCodeBlock.GetTargetType(instructionBlock.associatedMethod, parametersCodeBlockValue.index.Value).typeReference.AreEqual(argInfo.Item1))
                    return false;

                if (x.GetType().AreEqual(argInfo.Item1))
                    return false;

                if (x.GetType().With(y => y.IsValueType && !y.IsGenericType && Nullable.GetUnderlyingType(y) == null))
                    return true;

                if (x is CodeBlock)
                    return true;

                return false;
            }))
            {
                // If the nullable is assigned a value
                result.Emit(OpCodes.Ldarga, argInfo.Item3);
                result.Append(InstructionBlock.CreateCode(instructionBlock, argInfo.Item1.ChildType, value));
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
            ModifyValueTypeInstance(instructionBlock);
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
            else if (field.FieldType.IsNullable && value.With(x =>
            {
                if (x is ParametersCodeBlock parametersCodeBlock && parametersCodeBlock.index.HasValue &&
                    ParametersCodeBlock.GetTargetType(instructionBlock.associatedMethod, parametersCodeBlock.index.Value).typeReference.AreEqual(field.FieldType))
                    return false;

                if (x.GetType().AreEqual(field.FieldType))
                    return false;

                if (x.GetType().With(y => y.IsValueType && !y.IsGenericType && Nullable.GetUnderlyingType(y) == null))
                    return true;

                if (x is CodeBlock)
                    return true;

                return false;
            }))
            {
                // If the nullable is assigned a value
                result.Append(InstructionBlock.CreateCode(instructionBlock, field.FieldType.ChildType, value));
                result.Emit(OpCodes.Newobj, field.FieldType.GetMethod(".ctor", 1).Import());

                // Save to field
                result.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef);
            }
            else
            {
                // Other types
                result.Append(InstructionBlock.CreateCode(instructionBlock, field.FieldType, value));
                // Save to field
                result.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.fieldRef);
            }

            return result;
        }

        public void Append(InstructionBlock instructionBlock)
        {
            if (instructionBlock == this)
                return;

            this.exceptionHandlers.AddRange(instructionBlock.exceptionHandlers);
            this.instructions.AddRange(instructionBlock.instructions);
            this.ResultingType = instructionBlock.ResultingType;
        }

        public void Append(IEnumerable<Instruction> instructions, IEnumerable<ExceptionHandler> exceptionHandler)
        {
            this.Append(instructions);
            this.exceptionHandlers.AddRange(exceptionHandler);
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
            for (int i = 0; i < this.instructions.Count; i++)
            {
                var item = this.instructions[i];
                builder.Log(LogTypes.Info, $"{i.ToString("0000")} IL_{item.Offset.ToString("X4")}: {item.OpCode.ToString()} { (item.Operand is Instruction ? "IL_" + (item.Operand as Instruction).Offset.ToString("X4") : item.Operand?.ToString())} ");
            }
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

        public void Emit(OpCode opcode, MethodReference method) => this.instructions.Add(ilprocessor.Create(opcode, Builder.Current.Import(method)));

        public void Emit(OpCode opcode, Method method) => this.instructions.Add(ilprocessor.Create(opcode, Builder.Current.Import(method.methodReference)));

        public void Emit(OpCode opcode, CallSite site) => this.instructions.Add(ilprocessor.Create(opcode, site));

        public void Emit(OpCode opcode, TypeReference type) => this.instructions.Add(ilprocessor.Create(opcode, Builder.Current.Import(type)));

        public void Emit(OpCode opcode, BuilderType type) => this.instructions.Add(ilprocessor.Create(opcode, Builder.Current.Import(type.typeReference)));

        public void Emit(OpCode opcode, short value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, ushort value) => this.instructions.Add(ilprocessor.Create(opcode, unchecked((short)value)));

        public void Emit(OpCode opcode, int value) => this.instructions.Add(ilprocessor.Create(opcode, value));

        public void Emit(OpCode opcode, uint value) => this.instructions.Add(ilprocessor.Create(opcode, unchecked((int)value)));

        public void Emit_LdNull() => this.Emit(OpCodes.Ldnull);

        public void Emit_Nop() => this.Emit(OpCodes.Nop);

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (object.Equals(this, obj))
                return true;

            return true;
        }

        public InstructionBlock Get(Position startInclusive, Position endExclusive)
        {
            var result = new InstructionBlock(this.associatedMethod);

            for (int i = startInclusive.Index; i < endExclusive.Index; i++)
                result.instructions.Add(this.instructions[i]);

            result.Emit_Nop();

            bool IsOutOfBound(Instruction instruction)
            {
                if (instruction == null)
                    return false;

                var index = this.instructions.IndexOf(instruction);
                return index < startInclusive.Index || index > endExclusive.Index;
            }

            Instruction GetCorrectInstruction(Instruction instruction)
            {
                if (instruction == null)
                    return null;

                if (this.instructions.IndexOf(instruction) == endExclusive.Index)
                    return result.instructions[result.instructions.Count - 1];

                return instruction;
            }

            foreach (var item in this.exceptionHandlers)
            {
                if (IsOutOfBound(item.FilterStart)) continue;
                if (IsOutOfBound(item.HandlerEnd)) continue;
                if (IsOutOfBound(item.HandlerStart)) continue;
                if (IsOutOfBound(item.TryEnd)) continue;
                if (IsOutOfBound(item.TryStart)) continue;

                result.exceptionHandlers.Add(new ExceptionHandler(item.HandlerType)
                {
                    FilterStart = item.FilterStart,
                    CatchType = item.CatchType,
                    HandlerStart = item.HandlerStart,
                    TryEnd = GetCorrectInstruction(item.TryEnd),
                    TryStart = item.TryStart,
                    HandlerEnd = GetCorrectInstruction(item.HandlerEnd),
                });
            }

            return result;
        }

        public IEnumerator<Instruction> GetEnumerator() => this.instructions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.instructions.GetEnumerator();

        public override int GetHashCode() => this.associatedMethod.GetHashCode() ^ this.ilprocessor.GetHashCode();

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the range of elements in the <see cref="InstructionBlock"/>
        /// that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="instruction">The object to locate in the <see cref="InstructionBlock"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the range of elements
        /// in the <see cref="InstructionBlock"/> that starts at index and contains count
        /// number of elements, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the <see cref="InstructionBlock"/>.-or-count
        /// is less than 0.-or-index and count do not specify a valid section in the <see cref="InstructionBlock"/>.
        /// </exception>
        public int IndexOf(Instruction instruction) => this.instructions.IndexOf(instruction);

        /// <summary>
        /// Inserts an instruction at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        public void Insert(int index, Instruction item) => this.instructions.Insert(index, item);

        /// <summary>
        /// Inserts an instruction at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="items">The object to insert. The value can be null for reference types.</param>
        public void Insert(int index, IEnumerable<Instruction> items)
        {
            foreach (var item in items)
                this.instructions.Insert(index++, item);
        }

        /// <summary>
        /// Inserts an instruction at the specified index.
        /// </summary>
        /// <param name="position">The posistion to insert the instruction after.</param>
        /// <param name="items">The object to insert. The value can be null for reference types.</param>
        public void Insert(Instruction position, IEnumerable<Instruction> items)
        {
            var index = this.instructions.IndexOf(position);
            foreach (var item in items)
                this.instructions.Insert(index++, item);
        }

        /// <summary>
        /// Inserts an instruction after the defined position
        /// </summary>
        /// <param name="position">The posistion to insert the instruction after.</param>
        /// <param name="instructionToInsert">The instruction to insert into.</param>
        public void InsertAfter(Instruction position, Instruction instructionToInsert)
        {
            var index = this.instructions.IndexOf(position) + 1;

            if (index == this.instructions.Count)
                this.instructions.Add(instructionToInsert);
            else
                this.instructions.Insert(index, instructionToInsert);
        }

        /// <summary>
        /// Inserts an instruction after the defined position
        /// </summary>
        /// <param name="position">The posistion to insert the instruction after.</param>
        /// <param name="instructionsToInsert">A collection of instructions to insert into.</param>
        public void InsertAfter(Instruction position, IEnumerable<Instruction> instructionsToInsert)
        {
            var index = this.instructions.IndexOf(position) + 1;

            if (index == this.instructions.Count)
                this.instructions.AddRange(instructionsToInsert);
            else
                foreach (var item in instructionsToInsert)
                    this.instructions.Insert(index++, item);
        }

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

        public Instruction Next(Instruction instruction)
        {
            var index = this.instructions.IndexOf(instruction);
            return this.instructions[index + 1];
        }

        public void Prepend(IEnumerable<Instruction> instructions) => this.instructions.InsertRange(0, instructions);

        public void Remove(int index) => this.instructions.RemoveAt(index);

        public void RemoveRange(int index, int count) => this.instructions.RemoveRange(index, count);

        public InstructionBlock Spawn() => new InstructionBlock(this.associatedMethod);

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in this.instructions)
                sb.AppendLine($"IL_{item.Offset.ToString("X4")}: {item.OpCode.ToString()} { (item.Operand is Instruction ? "IL_" + (item.Operand as Instruction).Offset.ToString("X4") : item.Operand?.ToString())} ");
            return sb.ToString();
        }

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
            if (instructionBlock.Count == 0 && instructionBlock.associatedMethod.IsStatic)
                throw new NotSupportedException($"The method {instructionBlock.associatedMethod.Name} is static. Load a value before using a relational operator.");

            var firstType = instructionBlock.ResultingType?.ToBuilderType();
            Instruction nullableJumpTarget = null;
            VariableDefinition nullableVar1 = null;
            VariableDefinition nullableVar2 = null;
            LocalVariable firstValueVariable = null;

            if (secondType == null && secondValue is CoderBase coderBase)
                secondType = coderBase.instructions.ResultingType?.ToBuilderType() ?? secondValue?.GetType()?.ToBuilderType();

            if (firstType == null)
                firstType = secondType;

            if (instructionBlock.Count == 0)
            {
                instructionBlock.Emit(OpCodes.Ldarg_0);
                firstType = instructionBlock.ResultingType?.ToBuilderType();
            }

            if (firstType.IsNullable || secondType.IsNullable)
            {
                nullableJumpTarget = instructionBlock.ilprocessor.Create(OpCodes.Nop);

                if (firstType.IsNullable)
                {
                    nullableVar1 = instructionBlock.associatedMethod.GetOrCreateVariable(firstType).variable;

                    instructionBlock.Emit(OpCodes.Stloc, nullableVar1);
                    instructionBlock.Emit(OpCodes.Ldloca, nullableVar1);
                    instructionBlock.Emit(OpCodes.Call, firstType.GetMethod("get_HasValue").Import());
                    instructionBlock.Emit(OpCodes.Ldc_I4_1);
                    instructionBlock.Emit(OpCodes.Ceq);
                    instructionBlock.Emit(OpCodes.Dup);
                    instructionBlock.Emit(OpCodes.Brfalse, nullableJumpTarget);
                    instructionBlock.Emit(OpCodes.Pop);
                }
                else
                    firstValueVariable = instructionBlock.associatedMethod.GetOrCreateVariable(firstType);

                if (secondType.IsNullable)
                {
                    if (firstValueVariable != null)
                        instructionBlock.Emit(OpCodes.Stloc, firstValueVariable);

                    nullableVar2 = instructionBlock.associatedMethod.GetOrCreateVariable(secondType).variable;

                    instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, secondType, secondValue));
                    instructionBlock.Emit(OpCodes.Stloc, nullableVar2);
                    instructionBlock.Emit(OpCodes.Ldloca, nullableVar2);
                    instructionBlock.Emit(OpCodes.Call, secondType.GetMethod("get_HasValue").Import());
                    instructionBlock.Emit(OpCodes.Ldc_I4_1);
                    instructionBlock.Emit(OpCodes.Ceq);
                    instructionBlock.Emit(OpCodes.Dup);
                    instructionBlock.Emit(OpCodes.Brfalse, nullableJumpTarget);
                    instructionBlock.Emit(OpCodes.Pop);

                    if (firstValueVariable != null)
                        instructionBlock.Emit(OpCodes.Ldloc, firstValueVariable);

                    secondValue = new NullableCodeBlock(nullableVar2, secondType);
                }
            }

            if (firstType.IsNullable)
            {
                if (nullableVar1 == null)
                    ModifyValueTypeInstance(instructionBlock);
                else
                    instructionBlock.Emit(OpCodes.Ldloca, nullableVar1);

                instructionBlock.Emit(OpCodes.Call, firstType.GetMethod("GetValueOrDefault").Import());
                firstType = instructionBlock.ResultingType.ToBuilderType();
            }

            void NullableJumpTargetSpecial()
            {
                if (nullableJumpTarget != null) instructionBlock.Append(nullableJumpTarget);
            }

            if (secondType.IsNullable)
            {
                if (nullableVar2 == null)
                    secondValue = new NullableCodeBlock(secondValue, secondType);
                secondType = secondType.GetGenericArgument(0);
            }

            if (firstType == secondType && firstType.IsPrimitive)
            {
                instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, secondType, secondValue));
                instructionBlock.Emit(OpCodes.Ceq);

                NullableJumpTargetSpecial();
                return;
            }

            var equalityOperator = firstType.GetMethod("op_Equality", false, firstType, secondType)?.Import() ?? secondType.GetMethod("op_Equality", false, firstType, secondType)?.Import();

            if (equalityOperator != null)
            {
                instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, secondType, secondValue));
                instructionBlock.Emit(OpCodes.Call, equalityOperator);

                NullableJumpTargetSpecial();
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

                NullableJumpTargetSpecial();
                return;
            }

            equalityOperator = BuilderTypes.Object.GetMethod_Equals(BuilderTypes.Object, BuilderTypes.Object);

            InstructionBlock.CastOrBoxValues(instructionBlock, BuilderTypes.Object);
            instructionBlock.Append(InstructionBlock.CreateCode(instructionBlock, BuilderTypes.Object, secondValue));
            instructionBlock.Emit(OpCodes.Call, equalityOperator);

            NullableJumpTargetSpecial();
        }

        internal static InstructionBlock AttributeParameterToOpCode(InstructionBlock instructionBlock, CustomAttributeArgument attributeArgument)
        {
            /*
				- One of the following types: bool, byte, char, double, float, int, long, short, string, sbyte, ushort, uint, ulong.
				- The type object.
				- The type System.Type.
				- An enum type, provided it has public accessibility and the types in which it is nested (if any) also have public accessibility (Section 17.2).
				- Single-dimensional arrays of the above types.
			 */
            var result = instructionBlock.Spawn();

            if (attributeArgument.Value == null)
            {
                result.Emit_LdNull();
                return result;
            }

            var valueType = attributeArgument.Value.GetType();

            if (valueType.IsArray)
            {
                var array = (attributeArgument.Value as IEnumerable).Cast<CustomAttributeArgument>().ToArray();

                result.Emit(OpCodes.Ldc_I4, array.Length);
                result.Emit(OpCodes.Newarr, attributeArgument.Type.GetElementType().ToBuilderType());

                if (array.Length > 0)
                    result.Emit(OpCodes.Dup);

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Value == null)
                    {
                        result.Emit_LdNull();
                        result.Emit(OpCodes.Stelem_Ref);
                        return result;
                    }
                    else
                    {
                        var arrayType = array[i].Value.GetType();
                        result.Emit(OpCodes.Ldc_I4, i);
                        CreateInstructionsFromAttributeTypes(result, array[i].Type, arrayType, array[i].Value);

                        if (arrayType.IsValueType && attributeArgument.Type.GetElementType().IsValueType)
                        {
                            if (arrayType == typeof(int) ||
                                arrayType == typeof(uint) ||
                                arrayType.IsEnum)
                                result.Emit(OpCodes.Stelem_I4);
                            else if (arrayType == typeof(bool) ||
                                arrayType == typeof(byte) ||
                                arrayType == typeof(sbyte))
                                result.Emit(OpCodes.Stelem_I1);
                            else if (arrayType == typeof(short) ||
                                arrayType == typeof(ushort) ||
                                arrayType == typeof(char))
                                result.Emit(OpCodes.Stelem_I2);
                            else if (arrayType == typeof(long) ||
                                arrayType == typeof(ulong))
                                result.Emit(OpCodes.Stelem_I8);
                            else if (arrayType == typeof(float))
                                result.Emit(OpCodes.Stelem_R4);
                            else if (arrayType == typeof(double))
                                result.Emit(OpCodes.Stelem_R8);
                        }
                        else
                            result.Emit(OpCodes.Stelem_Ref);
                    }
                    if (i < array.Length - 1)
                        result.Emit(OpCodes.Dup);
                }
            }
            else
                CreateInstructionsFromAttributeTypes(result, attributeArgument.Type, valueType, attributeArgument.Value);

            return result;
        }

        internal static void CreateCodeForFieldReference(
            InstructionBlock result,
            BuilderType targetType,
            Field valueField,
            bool autoAddThisInstance)
        {
            if (!valueField.IsStatic && autoAddThisInstance)
                result.Emit(OpCodes.Ldarg_0);

            if ((valueField.FieldType.IsValueType && targetType == null) || (targetType != null && targetType.IsByReference))
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
            if ((localVariable.variable.VariableType.IsValueType && targetType == null) || (targetType != null && targetType.IsByReference))
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

        internal static Instruction GetCtorBaseOrThisCall(MethodDefinition methodDefinition)
        {
            // public MyClass() : base()
            var ctorCall = methodDefinition.Body.Instructions.FirstOrDefault(inst =>
            {
                if (inst.OpCode != OpCodes.Call)
                    return false;

                if (!(inst.Operand is MethodReference))
                    return false;

                var methodRef = inst.Operand as MethodReference;

                if (methodRef.Name != ".ctor")
                    return false;

                if (methodRef.DeclaringType.FullName == methodDefinition.DeclaringType.BaseType.FullName)
                    return true;

                return false;
            });

            if (ctorCall != null)
                return ctorCall.Next;

            // public MyClass() : this()
            return methodDefinition.Body.Instructions.FirstOrDefault(inst =>
            {
                if (inst.OpCode != OpCodes.Call)
                    return false;

                if (!(inst.Operand is MethodReference))
                    return false;

                var methodRef = inst.Operand as MethodReference;

                if (methodRef.Name != ".ctor")
                    return false;

                if (methodRef.DeclaringType.FullName == methodDefinition.DeclaringType.FullName)
                    return true;

                return false;
            })?.Next;
        }

        internal static void ModifyValueTypeInstance(InstructionBlock instructionBlock)
        {
            if (!instructionBlock.ResultingType?.IsValueType ?? true)
                return;

            var last = instructionBlock.Last;

            if (
                last.OpCode == OpCodes.Ldloc ||
                last.OpCode == OpCodes.Ldloc_0 ||
                last.OpCode == OpCodes.Ldloc_1 ||
                last.OpCode == OpCodes.Ldloc_2 ||
                last.OpCode == OpCodes.Ldloc_3 ||
                last.OpCode == OpCodes.Ldloc_S)
            {
                var local = instructionBlock.associatedMethod.GetVariable(last);
                if (local.VariableType.IsValueType)
                {
                    last.OpCode = OpCodes.Ldloca;
                    last.Operand = local;
                }
            }

            if (
                last.OpCode == OpCodes.Ldarg ||
                last.OpCode == OpCodes.Ldarg_0 ||
                last.OpCode == OpCodes.Ldarg_1 ||
                last.OpCode == OpCodes.Ldarg_2 ||
                last.OpCode == OpCodes.Ldarg_3 ||
                last.OpCode == OpCodes.Ldarg_S)
            {
                var argument = ParametersCodeBlock.GetParameter(instructionBlock.associatedMethod, last);
                if (argument != null && argument.ParameterType.IsValueType)
                {
                    last.OpCode = OpCodes.Ldarga;
                    last.Operand = argument;
                }
            }

            if (last.OpCode == OpCodes.Ldfld)
            {
                if ((last.Operand as FieldReference).FieldType.IsValueType)
                    last.OpCode = OpCodes.Ldflda;
            }
        }

        private static InstructionBlock CallInternal(InstructionBlock instructionBlock, object instance, Method method, OpCode opcode, params object[] parameters)
        {
            var result = instructionBlock.Spawn();
            try
            {
                if (instance != null && !method.IsStatic)
                {
                    result.Append(InstructionBlock.CreateCode(instructionBlock, null, instance));
                    ModifyValueTypeInstance(result);
                }
                else if (instance == null && !method.IsStatic)
                    ModifyValueTypeInstance(instructionBlock);

                if (parameters != null && parameters.Length > 0 && parameters[0] is ArrayCodeBlock arrayCodeSet)
                {
                    var methodParameters = method.methodDefinition.Parameters;
                    for (int i = 0; i < methodParameters.Count; i++)
                    {
                        result.Append(InstructionBlock.CreateCode(instructionBlock, null, arrayCodeSet));
                        result.Append(InstructionBlock.CreateCode(instructionBlock, null, i));
                        result.Emit(OpCodes.Ldelem_Ref);

                        var parameterBuilderType = GetResolvedParameterType(method, methodParameters[i]);
                        CastOrBoxValues(result, parameterBuilderType);
                    }
                }
                else if (parameters != null && parameters.Length > 0 && parameters[0] is ParametersCodeBlock parameterCodeSet && parameterCodeSet.IsAllParameters)
                {
                    if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli && opcode != OpCodes.Newobj)
                        opcode = OpCodes.Callvirt;

                    for (int i = 0; i < method.methodReference.Parameters.Count; i++)
                    {
                        var parameterBuilderType = GetResolvedParameterType(method, method.methodReference.Parameters[i]);
                        result.Append(InstructionBlock.CreateCode(result, parameterBuilderType, CodeBlocks.GetParameter(i)));
                    }
                }
                else
                {
                    if ((method.OriginType.IsInterface || method.IsAbstract) && opcode != OpCodes.Calli && opcode != OpCodes.Newobj)
                        opcode = OpCodes.Callvirt;

                    if (parameters != null)
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var parameterBuilderType = GetResolvedParameterType(method, method.methodReference.Parameters[i]);
                            result.Append(InstructionBlock.CreateCode(result, parameterBuilderType, parameters[i]));
                        }
                }
            }
            catch (Exception e)
            {
                for (int i = 0; i < method.methodReference.Parameters.Count; i++)
                {
                    var parameterType = method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter ?
                        method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                        method.methodDefinition.Parameters[i].ParameterType;

                    Builder.Current.Log(LogTypes.Info, $"ERROR: {i} - {method.methodReference.Parameters[i].ParameterType.FullName}");
                    Builder.Current.Log(LogTypes.Info, $"ERROR:       Method IsGenericInstance: {method.methodReference.IsGenericInstance}");
                    Builder.Current.Log(LogTypes.Info, $"ERROR:       Param IsGenericInstance: {method.methodReference.Parameters[i].ParameterType.IsGenericInstance}");
                    Builder.Current.Log(LogTypes.Info, $"ERROR:       Param IsGenericParameter: {method.methodReference.Parameters[i].ParameterType.IsGenericParameter}");
                    Builder.Current.Log(LogTypes.Info, $"ERROR:       Param HasGenericParameters: {method.methodReference.Parameters[i].ParameterType.HasGenericParameters}");
                    Builder.Current.Log(LogTypes.Info, $"ERROR:       Param ContainsGenericParameter: {method.methodReference.Parameters[i].ParameterType.ContainsGenericParameter}");
                    if (method.methodDefinition.Parameters[i].ParameterType.IsGenericInstance || method.methodDefinition.Parameters[i].ParameterType.IsGenericParameter)
                        Builder.Current.Log(LogTypes.Info, $"ERROR: Resolves to '{method.methodDefinition.Parameters[i].ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference)}'");
                }

                throw new Exception($"An error has occured while trying to weave a call. '{method}'", e);
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

        private static void CastOrBoxValuesInternal(InstructionBlock instructionBlock, GenericParameter genericParameter)
        {
            var genericParameterResulting = instructionBlock.ResultingType as GenericParameter;

            if (genericParameterResulting == null && instructionBlock.ResultingType != null)
            {
                if (genericParameter.HasReferenceTypeConstraint)
                {
                    instructionBlock.Emit(OpCodes.Isinst, genericParameter);
                    instructionBlock.Emit(OpCodes.Unbox_Any, genericParameter);
                }
                else
                    instructionBlock.Emit(OpCodes.Unbox_Any, genericParameter);
            }
            else if (genericParameter.FullName == genericParameterResulting.FullName && genericParameter.Owner == genericParameterResulting.Owner)
                // No casting or convertions required here
                return;
            else
                instructionBlock.Emit(OpCodes.Unbox_Any, genericParameter);
        }

        private static void CastOrBoxValuesInternal(InstructionBlock instructionBlock, BuilderType targetType)
        {
            // TODO - Support for nullable types required

            if (targetType == null)
                return;

            if (targetType.typeReference is GenericParameter genericParameter)
            {
                CastOrBoxValuesInternal(instructionBlock, genericParameter);
                return;
            }

            bool IsInstRequired()
            {
                if (targetType.IsArray)
                    return false;

                if (targetType.IsEnum)
                    return false;

                if (targetType == BuilderTypes.IEnumerable1)
                    return false;

                if (targetType.Implements(BuilderTypes.IList))
                    return false;

                if (targetType.IsPrimitive)
                    return false;

                if (targetType.IsValueType)
                    return false;

                if (targetType == BuilderTypes.String ||
                    instructionBlock.ResultingType.AreReferenceAssignable(targetType.typeReference) ||
                    targetType.IsInterface)
                    return true;

                if (targetType.typeReference.AreEqual((TypeReference)BuilderTypes.Object))
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
                if (instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.String))
                {
                    // Bug #23
                    instructionBlock.Prepend(instructionBlock.ilprocessor.TypeOf(targetType));

                    instructionBlock.Append(instructionBlock.ilprocessor.TypeOf(targetType.Import()));
                    instructionBlock.Emit(OpCodes.Call, BuilderTypes.Enum.GetMethod_GetUnderlyingType());
                    instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ChangeType(BuilderTypes.Object, BuilderTypes.Type));
                    instructionBlock.Emit(OpCodes.Call, BuilderTypes.Enum.GetMethod_ToObject(BuilderTypes.Type, BuilderTypes.Object));
                    instructionBlock.Emit(OpCodes.Unbox_Any, targetType);
                }
                else
                    instructionBlock.Emit(OpCodes.Unbox_Any, targetType);
            }
            else if ((instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object) || instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.IEnumerable)) && (targetType.IsArray || targetType == BuilderTypes.IEnumerable1))
            {
                var childType = Builder.Current.GetChildrenType(targetType.typeReference).Item1;
                var castMethod = BuilderTypes.Enumerable.GetMethod_Cast(childType).Import();
                var toArrayMethod = BuilderTypes.Enumerable.GetMethod_ToArray(childType).Import();

                if (instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object))
                    instructionBlock.Emit(OpCodes.Isinst, (TypeReference)BuilderTypes.IEnumerable);

                if (!childType.AreEqual(Builder.Current.GetChildrenType(instructionBlock.ResultingType).Item1))
                    instructionBlock.Emit(OpCodes.Call, castMethod);

                if (targetType.IsArray)
                    instructionBlock.Emit(OpCodes.Call, toArrayMethod);
            }
            else if ((instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object) || instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.IEnumerable) || instructionBlock.ResultingType.IsArray) && targetType == BuilderTypes.List1)
            {
                var childType = Builder.Current.GetChildrenType(targetType.typeReference).Item1;
                var castMethod = BuilderTypes.Enumerable.GetMethod_Cast(childType).Import();
                var toList = BuilderTypes.Enumerable.GetMethod_ToList(childType).Import();

                if (instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object))
                    instructionBlock.Emit(OpCodes.Isinst, (TypeReference)BuilderTypes.IEnumerable);

                Builder.Current.Log(LogTypes.Info, $"-------->  {targetType} {instructionBlock.ResultingType}");

                if (!childType.AreEqual(Builder.Current.GetChildrenType(instructionBlock.ResultingType).Item1))
                    instructionBlock.Emit(OpCodes.Call, castMethod);
                instructionBlock.Emit(OpCodes.Call, toList);
            }
            else if ((instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object) || instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.IEnumerable) || instructionBlock.ResultingType.IsArray) && targetType.Implements(BuilderTypes.IList))
                CastToIList(instructionBlock, targetType);
            else if
                (
                    (instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object) && targetType.IsValueType) ||
                    (!instructionBlock.ResultingType.AreEqual(targetType) && instructionBlock.ResultingType.IsPrimitive && targetType.IsPrimitive)
                )
            {
                if (targetType == BuilderTypes.Int32)           /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToInt32(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.UInt32)     /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToUInt32(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.Boolean)    /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToBoolean(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.Byte)       /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToByte(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.Char)       /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToChar(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.DateTime)   /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToDateTime(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.Decimal)    /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToDecimal(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.Double)     /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToDouble(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.Int16)      /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToInt16(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.Int64)      /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToInt64(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.SByte)      /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToSByte(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.Single)     /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToSingle(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.UInt16)     /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToUInt16(instructionBlock.ResultingType));
                else if (targetType == BuilderTypes.UInt64)     /* */   instructionBlock.Emit(OpCodes.Call, BuilderTypes.Convert.GetMethod_ToUInt64(instructionBlock.ResultingType));
                else instructionBlock.Emit(OpCodes.Unbox_Any, targetType);
            }
            else if ((instructionBlock.ResultingType.Resolve() == null || instructionBlock.ResultingType.IsValueType) && !targetType.IsValueType)
                instructionBlock.Emit(OpCodes.Box, instructionBlock.ResultingType);
            else if (instructionBlock.instructions.Last().OpCode != OpCodes.Ldnull && targetType == BuilderTypes.Object)
            {
                // Nope nothing....
            }
            else if (
                    instructionBlock.instructions.Last().OpCode != OpCodes.Ldnull &&
                    !instructionBlock.ResultingType.AreEqual(targetType) &&
                    targetType.typeReference.AreReferenceAssignable(instructionBlock.ResultingType))
                instructionBlock.Emit(OpCodes.Castclass, Builder.Current.Import(instructionBlock.ResultingType));
        }

        private static void CastToIList(InstructionBlock instructionBlock, BuilderType targetType)
        {
            var childType = Builder.Current.GetChildrenType(targetType.typeReference).Item1;
            var ctorCollection = targetType
                .typeDefinition?.Methods
                    .FirstOrDefault(x => x.Name == ".ctor" && x.HasParameters && x.Parameters[0].ParameterType.FullName.StartsWith("System.Collections.Generic.IList`1<") && x.Parameters.Count == 1);
            if (ctorCollection != null)
            {
                var castMethod = BuilderTypes.Enumerable.GetMethod_Cast(childType).Import();
                var toList = BuilderTypes.Enumerable.GetMethod_ToList(childType).Import();

                if (instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object))
                    instructionBlock.Emit(OpCodes.Isinst, (TypeReference)BuilderTypes.IEnumerable);

                if (!childType.AreEqual(Builder.Current.GetChildrenType(instructionBlock.ResultingType).Item1))
                    instructionBlock.Emit(OpCodes.Call, castMethod);
                instructionBlock.Emit(OpCodes.Call, toList);
                instructionBlock.Emit(OpCodes.Newobj, Builder.Current.Import(ctorCollection.MakeHostInstanceGeneric(targetType.GenericArguments().Select(x => x.typeReference))));
                return;
            }

            if (ctorCollection == null)
                ctorCollection = targetType
                .typeDefinition?.Methods
                    .FirstOrDefault(x => x.Name == ".ctor" && x.HasParameters && x.Parameters[0].ParameterType.FullName.StartsWith("System.Collections.Generic.IEnumerable`1<") && x.Parameters.Count == 1);
            if (ctorCollection != null)
            {
                var castMethod = BuilderTypes.Enumerable.GetMethod_Cast(childType).Import();

                if (instructionBlock.ResultingType.AreEqual((TypeReference)BuilderTypes.Object))
                    instructionBlock.Emit(OpCodes.Isinst, (TypeReference)BuilderTypes.IEnumerable);

                if (!childType.AreEqual(Builder.Current.GetChildrenType(instructionBlock.ResultingType).Item1))
                    instructionBlock.Emit(OpCodes.Call, castMethod);
                instructionBlock.Emit(OpCodes.Newobj, Builder.Current.Import(ctorCollection.MakeHostInstanceGeneric(targetType.GenericArguments().Select(x => x.typeReference))));
                return;
            }

            instructionBlock.Emit(OpCodes.Isinst, targetType.Import());
        }

        private static void CreateCodeForVariableDefinition(InstructionBlock instructionBlock, BuilderType targetType, VariableDefinition value)
        {
            var index = value.Index;

            if ((value.VariableType.IsValueType && targetType == null) || (targetType != null && targetType.IsByReference))
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

        private static void CreateInstructionsFromAttributeTypes(InstructionBlock instructionBlock, TypeReference targetType, Type type, object value)
        {
            if (type == typeof(CustomAttributeArgument))
            {
                var attrib = (CustomAttributeArgument)value;
                type = attrib.Value.GetType();
                value = attrib.Value;
            }

            if (type == typeof(string))
            {
                instructionBlock.Emit(OpCodes.Ldstr, value.ToString());
            }

            if (type == typeof(TypeReference) || type.IsSubclassOf(typeof(TypeReference)))
            {
                instructionBlock.Append(instructionBlock.ilprocessor.TypeOf(value as TypeReference));
                return;
            }

            if (type.IsEnum) instructionBlock.Emit(OpCodes.Ldc_I4, (int)value);
            else if (type == typeof(int)) instructionBlock.Emit(OpCodes.Ldc_I4, (int)value);
            else if (type == typeof(uint)) instructionBlock.Emit(OpCodes.Ldc_I4, (int)(uint)value);
            else if (type == typeof(bool)) instructionBlock.Emit(OpCodes.Ldc_I4, (bool)value ? 1 : 0);
            else if (type == typeof(char)) instructionBlock.Emit(OpCodes.Ldc_I4, (char)value);
            else if (type == typeof(short)) instructionBlock.Emit(OpCodes.Ldc_I4, (short)value);
            else if (type == typeof(ushort)) instructionBlock.Emit(OpCodes.Ldc_I4, (ushort)value);
            else if (type == typeof(byte)) instructionBlock.Emit(OpCodes.Ldc_I4, (int)(byte)value);
            else if (type == typeof(sbyte)) instructionBlock.Emit(OpCodes.Ldc_I4, (int)(sbyte)value);
            else if (type == typeof(long)) instructionBlock.Emit(OpCodes.Ldc_I8, (long)value);
            else if (type == typeof(ulong)) instructionBlock.Emit(OpCodes.Ldc_I8, (long)(ulong)value);
            else if (type == typeof(double)) instructionBlock.Emit(OpCodes.Ldc_R8, (double)value);
            else if (type == typeof(float)) instructionBlock.Emit(OpCodes.Ldc_R4, (float)value);

            if (type.IsValueType && !targetType.IsValueType)
                instructionBlock.Emit(OpCodes.Box, type.IsEnum ?
                    Enum.GetUnderlyingType(type).ToBuilderType().Import() : type.ToBuilderType().Import());
        }

        private static BuilderType GetResolvedParameterType(Method method, ParameterDefinition parameterDefinition)
        {
            var parameterType = parameterDefinition.ParameterType.IsGenericInstance || parameterDefinition.ParameterType.IsGenericParameter ?
                parameterDefinition.ParameterType.ResolveType(method.OriginType.typeReference, method.methodReference) :
                parameterDefinition.ParameterType;
            return parameterType.ToBuilderType().Import();
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