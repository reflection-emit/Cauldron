using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class Coder
    {
        internal ParamResult AddParameter(ILProcessor processor, TypeReference targetType, object parameter)
        {
            var result = new ParamResult();
            var targetDef = targetType?.Resolve();

            if (parameter == null)
            {
                result.Instructions.Add(processor.Create(OpCodes.Ldnull));
                return result;
            }

            var type = parameter.GetType();

            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
                parameter = Convert.ChangeType(parameter, type);
            }

            switch (parameter)
            {
                case string value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldstr, parameter.ToString()));
                    result.Type = Builder.Current.TypeSystem.String;
                    break;

                case FieldDefinition value:
                    if (!value.IsStatic)
                        result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                    if (value.FieldType.IsValueType && targetType == null)
                        result.Instructions.Add(processor.Create(value.IsStatic ?
                            OpCodes.Ldsflda :
                            OpCodes.Ldflda, value.CreateFieldReference()));
                    else
                        result.Instructions.Add(processor.Create(value.IsStatic ?
                            OpCodes.Ldsfld :
                            OpCodes.Ldfld, value.CreateFieldReference()));
                    result.Type = value.FieldType;
                    break;

                case FieldReference value:
                    var fieldDef = value.Resolve();

                    if (!fieldDef.IsStatic)
                        result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                    if (value.FieldType.IsValueType && targetType == null)
                        result.Instructions.Add(processor.Create(fieldDef.IsStatic ?
                            OpCodes.Ldsflda :
                            OpCodes.Ldflda, value));
                    else
                        result.Instructions.Add(processor.Create(fieldDef.IsStatic ?
                            OpCodes.Ldsfld :
                            OpCodes.Ldfld, value));
                    result.Type = value.FieldType;
                    break;

                case Field value:
                    if (!value.IsStatic)
                        result.Instructions.Add(processor.Create(OpCodes.Ldarg_0));

                    if (value.FieldType.IsValueType && targetType == null)
                        result.Instructions.Add(processor.Create(value.IsStatic ?
                            OpCodes.Ldsflda :
                            OpCodes.Ldflda, value.fieldRef));
                    else
                        result.Instructions.Add(processor.Create(value.IsStatic ?
                            OpCodes.Ldsfld :
                            OpCodes.Ldfld, value.fieldRef));

                    result.Type = value.fieldRef.FieldType;
                    break;

                case int value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, value));
                    result.Type = Builder.Current.TypeSystem.Int32;
                    break;

                case uint value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, unchecked((int)value)));
                    result.Type = Builder.Current.TypeSystem.UInt32;
                    break;

                case bool value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, value ? 1 : 0));
                    result.Type = Builder.Current.TypeSystem.Boolean;
                    break;

                case char value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, value));
                    result.Type = Builder.Current.TypeSystem.Char;
                    break;

                case short value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, value));
                    result.Type = Builder.Current.TypeSystem.Int16;
                    break;

                case ushort value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, unchecked((short)value)));
                    result.Type = Builder.Current.TypeSystem.UInt16;
                    break;

                case byte value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, value));
                    result.Type = Builder.Current.TypeSystem.Byte;
                    break;

                case sbyte value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, value));
                    result.Type = Builder.Current.TypeSystem.SByte;
                    break;

                case long value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, value));
                    result.Type = Builder.Current.TypeSystem.Int64;
                    break;

                case ulong value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, unchecked((long)value)));
                    result.Type = Builder.Current.TypeSystem.UInt64;
                    break;

                case double value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_R8, value));
                    result.Type = Builder.Current.TypeSystem.Double;
                    break;

                case float value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_R4, value));
                    result.Type = Builder.Current.TypeSystem.Single;
                    break;

                case IntPtr value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (int)value));
                    result.Type = Builder.Current.TypeSystem.IntPtr;
                    break;

                case UIntPtr value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, unchecked((int)(uint)value)));
                    result.Type = Builder.Current.TypeSystem.UIntPtr;
                    break;

                case LocalVariable value:
                    result.Type = AddVariableDefinitionToInstruction(processor, result.Instructions, targetType, value.variable).VariableType;
                    break;

                case VariableDefinition value:
                    result.Type = AddVariableDefinitionToInstruction(processor, result.Instructions, targetType, value).VariableType;
                    break;

                case ExceptionCodeBlock exceptionCodeBlock:
                    {
                        var variable = char.IsNumber(exceptionCodeBlock.name, 0) ?
                            this.method.methodDefinition.Body.Variables[int.Parse(exceptionCodeBlock.name)] :
                            this.method.GetLocalVariable(exceptionCodeBlock.name);

                        result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable));
                        result.Type = Builder.Current.Import(variable.VariableType);
                        break;
                    }
                case ParametersCodeBlock parametersCodeBlock:
                    if (parametersCodeBlock.index.HasValue)
                    {
                        if (this.method.methodDefinition.Parameters.Count == 0)
                            throw new ArgumentException($"The method {this.method.Name} does not have any parameters");

                        result.Instructions.Add(processor.Create(OpCodes.Ldarg, this.method.IsStatic ? parametersCodeBlock.index.Value : parametersCodeBlock.index.Value + 1));
                        result.Type = Builder.Current.Import(this.method.methodDefinition.Parameters[parametersCodeBlock.index.Value].ParameterType);
                    }
                    else
                    {
                        var variable = char.IsNumber(parametersCodeBlock.name, 0) ?
                            this.method.methodDefinition.Body.Variables[int.Parse(parametersCodeBlock.name)] :
                            this.method.GetLocalVariable(parametersCodeBlock.name);

                        result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable));
                        result.Type = Builder.Current.Import(variable.VariableType);
                    }
                    break;

                case ThisCodeBlock thisCodeBlock:
                    result.Instructions.Add(processor.Create(this.method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                    result.Type = this.method.OriginType.typeReference;
                    break;

                case InitObjCodeBlock initObjCodeBlock:
                    {
                        var variable = this.CreateVariable(initObjCodeBlock.typeReference);
                        result.Instructions.Add(processor.Create(OpCodes.Ldloca, variable.variable));
                        result.Instructions.Add(processor.Create(OpCodes.Initobj, initObjCodeBlock.typeReference));
                        result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable.variable));
                        result.Type = initObjCodeBlock.typeReference;
                        break;
                    }
                case DefaultTaskCodeBlock defaultTaskCodeBlock:
                    {
                        var taskType = this.method.type.Builder.GetType("System.Threading.Tasks.Task");
                        var resultFrom = taskType.GetMethod("FromResult", 1, true).MakeGeneric(typeof(int));
                        var code = this.NewCoder().Call(resultFrom, 0);

                        result.Instructions.AddRange(code.instructions);
                        result.Type = this.method.ReturnType.typeReference;
                        break;
                    }

                case DefaultTaskOfTCodeBlock defaultTaskOfTCodeBlock:
                    {
                        var returnType = this.method.ReturnType.GetGenericArgument(0);
                        var taskType = this.method.type.Builder.GetType("System.Threading.Tasks.Task");
                        var resultFrom = taskType.GetMethod("FromResult", 1, true).MakeGeneric(returnType);
                        var code = this.NewCoder().Call(resultFrom, returnType.DefaultValue);

                        result.Instructions.AddRange(code.instructions);
                        result.Type = returnType.typeReference;
                        break;
                    }

                case InstructionsCodeBlock instructionsCodeBlock:
                    {
                        if (object.ReferenceEquals(instructionsCodeBlock.instructions, this.instructions))
                            throw new NotSupportedException("Nope... Not gonna work... Use NewCoder() if you want to pass an instructions set as parameters.");

                        result.Instructions.AddRange(instructionsCodeBlock.instructions);
                        break;
                    }

                case BooleanExpressionParameter booleanExpressionParameter:
                    {
                        var instructions = this.AddParameter(processor, booleanExpressionParameter.targetType, booleanExpressionParameter.value);
                        result.Instructions.AddRange(instructions.Instructions);

                        if (booleanExpressionParameter.negate)
                            result.Instructions.Add(processor.Create(OpCodes.Neg));

                        if (booleanExpressionParameter.invert)
                        {
                            result.Instructions.Add(processor.Create(OpCodes.Ldc_I4_0));
                            result.Instructions.Add(processor.Create(OpCodes.Ceq));
                        }

                        result.Type = result.Type;
                    }
                    break;

                case TypeReference value:
                    result.Instructions.AddRange(processor.TypeOf(value));
                    result.Type = Builder.Current.Import(typeof(Type));
                    break;

                case BuilderType value:
                    result.Instructions.AddRange(processor.TypeOf(value.typeReference));
                    result.Type = Builder.Current.Import(typeof(Type));
                    break;

                case Method method:
                    if (targetType.FullName == typeof(IntPtr).FullName)
                    {
                        if (!method.IsStatic && method.OriginType != this.method.OriginType && this.method.OriginType.IsAsyncStateMachine)
                        {
                            var instance = this.method.AsyncMethodHelper.Instance;
                            var inst = this.AddParameter(processor, targetType, instance);
                            result.Instructions.AddRange(inst.Instructions);
                        }
                        else
                            result.Instructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));

                        result.Instructions.Add(processor.Create(OpCodes.Ldftn, method.methodReference));
                        result.Type = Builder.Current.TypeSystem.IntPtr;
                    }
                    else
                    {
                        var methodBaseRef = Builder.Current.Import(typeof(System.Reflection.MethodBase));
                        // methodof
                        result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.methodReference));
                        result.Instructions.Add(processor.Create(OpCodes.Ldtoken, method.OriginType.typeReference));
                        result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(methodBaseRef.BetterResolve().Methods.FirstOrDefault(x => x.Name == "GetMethodFromHandle" && x.Parameters.Count == 2))));

                        result.Type = methodBaseRef;
                    }
                    break;

                case ParameterDefinition value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg, value));
                    result.Type = value.ParameterType;
                    break;

                case ParameterReference value:
                    result.Instructions.Add(processor.Create(OpCodes.Ldarg, value.Index));
                    result.Type = value.ParameterType;
                    break;

                case Position value:
                    result.Instructions.Add(value.instruction);
                    break;

                case IEnumerable<Instruction> value:
                    result.Instructions.AddRange(value);
                    break;

                default:
                    throw new NotImplementedException($"Unknown type: {parameter.GetType().FullName}");
            }

            if (result.Type == null)
                result.Type = result.Instructions.GetTypeOfValueInStack(this.method);

            if (result.Type == null || targetType == null || result.Type.FullName == targetType.FullName)
                return result;

            processor.CastOrBoxValues(targetType, result, targetDef);

            return result;
        }

        private static VariableDefinition AddVariableDefinitionToInstruction(ILProcessor processor, List<Instruction> instructions, TypeReference targetType, object parameter)
        {
            var value = parameter as VariableDefinition;
            var index = value.Index;

            if (value.VariableType.IsValueType && targetType == null)
                instructions.Add(processor.Create(OpCodes.Ldloca, value));
            else
                switch (index)
                {
                    case 0: instructions.Add(processor.Create(OpCodes.Ldloc_0)); break;
                    case 1: instructions.Add(processor.Create(OpCodes.Ldloc_1)); break;
                    case 2: instructions.Add(processor.Create(OpCodes.Ldloc_2)); break;
                    case 3: instructions.Add(processor.Create(OpCodes.Ldloc_3)); break;
                    default:
                        instructions.Add(processor.Create(OpCodes.Ldloc, value));
                        break;
                }

            return value;
        }
    }
}