using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class ParameterCoderExtensions
    {
        internal static ParamResult AddParameter(this Coder coder, ILProcessor processor, TypeReference targetType, object parameter)
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
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, value /* TODO: This could cause a bug */));
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
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, value));
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
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I8, value));
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
                    result.Instructions.Add(processor.Create(OpCodes.Ldc_I4, (uint)value));
                    result.Type = Builder.Current.TypeSystem.UIntPtr;
                    break;

                case LocalVariable value:
                    result.Type = AddVariableDefinitionToInstruction(processor, result.Instructions, targetType, value.variable).VariableType;
                    break;

                case VariableDefinition value:
                    result.Type = AddVariableDefinitionToInstruction(processor, result.Instructions, targetType, value).VariableType;
                    break;

                case ExceptionCodeSet exceptionCodeSet:
                    {
                        var variable = char.IsNumber(exceptionCodeSet.name, 0) ?
                            coder.method.methodDefinition.Body.Variables[int.Parse(exceptionCodeSet.name)] :
                            coder.method.GetLocalVariable(exceptionCodeSet.name);

                        result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable));
                        result.Type = Builder.Current.Import(variable.VariableType);
                        break;
                    }
                case ParametersCodeSet parametersCodeSet:
                    if (parametersCodeSet.index.HasValue)
                    {
                        if (coder.method.methodDefinition.Parameters.Count == 0)
                            throw new ArgumentException($"The method {coder.method.Name} does not have any parameters");

                        result.Instructions.Add(processor.Create(OpCodes.Ldarg, coder.method.IsStatic ? parametersCodeSet.index.Value : parametersCodeSet.index.Value + 1));
                        result.Type = Builder.Current.Import(coder.method.methodDefinition.Parameters[parametersCodeSet.index.Value].ParameterType);
                    }
                    else
                    {
                        var variable = char.IsNumber(parametersCodeSet.name, 0) ?
                            coder.method.methodDefinition.Body.Variables[int.Parse(parametersCodeSet.name)] :
                            coder.method.GetLocalVariable(parametersCodeSet.name);

                        result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable));
                        result.Type = Builder.Current.Import(variable.VariableType);
                    }
                    break;

                case ThisCodeSet thisCodeSet:
                    result.Instructions.Add(processor.Create(coder.method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                    result.Type = coder.method.OriginType.typeReference;
                    break;

                case InitObjCodeSet initObjCodeSet:
                    {
                        var variable = coder.CreateVariable(initObjCodeSet.typeReference);
                        result.Instructions.Add(processor.Create(OpCodes.Ldloca, variable.variable));
                        result.Instructions.Add(processor.Create(OpCodes.Initobj, initObjCodeSet.typeReference));
                        result.Instructions.Add(processor.Create(OpCodes.Ldloc, variable.variable));
                        result.Type = initObjCodeSet.typeReference;
                        break;
                    }
                case DefaultTaskCodeSet defaultTaskCodeSet:
                    {
                        var taskType = coder.method.type.Builder.GetType("System.Threading.Tasks.Task");
                        var resultFrom = taskType.GetMethod("FromResult", 1, true).MakeGeneric(typeof(int));
                        var code = coder.NewCoder().Call(resultFrom, 0);

                        result.Instructions.AddRange(code.instructions);
                        result.Type = coder.method.ReturnType.typeReference;
                        break;
                    }

                case DefaultTaskOfTCodeSet defaultTaskOfTCodeSet:
                    {
                        var returnType = coder.method.ReturnType.GetGenericArgument(0);
                        var taskType = coder.method.type.Builder.GetType("System.Threading.Tasks.Task");
                        var resultFrom = taskType.GetMethod("FromResult", 1, true).MakeGeneric(returnType);
                        var code = coder.NewCoder().Call(resultFrom, returnType.DefaultValue);

                        result.Instructions.AddRange(code.instructions);
                        result.Type = returnType.typeReference;
                        break;
                    }

                case InstructionsCodeSet instructionsCodeSet:
                    {
                        if (object.ReferenceEquals(instructionsCodeSet.instructions, coder.instructions))
                            throw new NotSupportedException("Nope... Not gonna work... Use NewCoder() if you want to pass an instructions set as parameters.");

                        result.Instructions.AddRange(instructionsCodeSet.instructions);
                        break;
                    }

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
                        if (!method.IsStatic && method.OriginType != coder.method.OriginType && coder.method.OriginType.IsAsyncStateMachine)
                        {
                            var instance = coder.method.AsyncMethodHelper.Instance;
                            var inst = coder.AddParameter(processor, targetType, instance);
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
                    throw new NotImplementedException();
            }

            if (result.Type == null)
                result.Type = GetTypeOfValueInStack(result.Instructions);

            if (result.Type == null || targetType == null || result.Type.FullName == targetType.FullName)
                return result;

            processor.CastOrBoxValues(targetType, result, targetDef);

            return result;
        }

        internal static void CastOrBoxValues(this ILProcessor processor, TypeReference targetType, ParamResult result, TypeDefinition targetDef)
        {
            bool IsInstRequired()
            {
                if (targetDef.FullName == typeof(string).FullName || result.Type.FullName == typeof(object).FullName || targetDef.IsInterface)
                    return true;

                if (targetType.IsValueType)
                    return false;

                if (targetType.IsArray)
                    return false;

                if (targetDef.FullName.StartsWith("System.Collections.Generic.IEnumerable`1"))
                    return false;

                return false;
            }

            // TODO - adds additional checks for not resolved generics
            if (targetDef == null && result.Type.Resolve() != null) /* This happens if the target type is a generic */ result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            else if (IsInstRequired()) result.Instructions.Add(processor.Create(OpCodes.Isinst, Builder.Current.Import(targetType)));
            else if (targetDef.IsEnum)
            {
                if (result.Type.FullName == typeof(string).FullName)
                {
                    result.Instructions.InsertRange(0, processor.TypeOf(targetType));

                    result.Instructions.AddRange(processor.TypeOf(Builder.Current.Import(targetType)));
                    result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }))));
                    result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
                }
                else
                    result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));

                // Bug #23
                //result.Instructions.InsertRange(0, this.TypeOf(processor, targetType));

                //result.Instructions.AddRange(this.TypeOf(processor, Builder.Current.Import(targetType)));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("GetUnderlyingType", new Type[] { typeof(Type) }))));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ChangeType", new Type[] { typeof(object), typeof(Type) }))));
                //result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Enum)).GetMethodReference("ToObject", new Type[] { typeof(Type), typeof(object) }))));
            }
            else if (result.Type.FullName == typeof(object).FullName && (targetType.IsArray || targetDef.FullName.StartsWith("System.Collections.Generic.IEnumerable`1")))
            {
                var childType = Builder.Current.GetChildrenType(targetType);
                var castMethod = Builder.Current.Import(Builder.Current.Import(typeof(System.Linq.Enumerable)).GetMethodReference("Cast", new Type[] { typeof(IEnumerable) }).MakeGeneric(null, childType));
                var toArrayMethod = Builder.Current.Import(Builder.Current.Import(typeof(System.Linq.Enumerable)).GetMethodReference("ToArray", 1).MakeGeneric(null, childType));

                result.Instructions.Add(processor.Create(OpCodes.Isinst, Builder.Current.Import(typeof(IEnumerable))));
                result.Instructions.Add(processor.Create(OpCodes.Call, castMethod));

                if (targetType.IsArray)
                    result.Instructions.Add(processor.Create(OpCodes.Call, toArrayMethod));
            }
            else if (result.Type.FullName == typeof(object).FullName && targetDef.IsValueType)
            {
                if (targetDef.FullName == typeof(int).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToInt32", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(uint).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToUInt32", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(bool).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToBoolean", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(byte).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToByte", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(char).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToChar", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(DateTime).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToDateTime", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(decimal).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToDecimal", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(double).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToDouble", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(short).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToInt16", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(long).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToInt64", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(sbyte).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToSByte", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(float).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToSingle", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(ushort).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToUInt16", new Type[] { typeof(object) }))));
                else if (targetDef.FullName == typeof(ulong).FullName) result.Instructions.Add(processor.Create(OpCodes.Call, Builder.Current.Import(Builder.Current.Import(typeof(Convert)).GetMethodReference("ToUInt64", new Type[] { typeof(object) }))));
                else result.Instructions.Add(processor.Create(OpCodes.Unbox_Any, targetType));
            }
            else if ((result.Type.Resolve() == null || result.Type.IsValueType) && !targetType.IsValueType)
                result.Instructions.Add(processor.Create(OpCodes.Box, result.Type));
            else if (result.Instructions.Last().OpCode != OpCodes.Ldnull && targetType.FullName == "System.Object")
            {
                // Nope nothing....
            }
            else if (result.Instructions.Last().OpCode != OpCodes.Ldnull && targetType.FullName != result.Type.FullName && targetType.AreReferenceAssignable(Builder.Current.Import(result.Type)))
                result.Instructions.Add(processor.Create(OpCodes.Castclass, Builder.Current.Import(result.Type)));
        }

        internal static TypeReference GetTypeOfValueInStack(this List<Instruction> instructions)
        {
            TypeReference GetTypeOfValueInStack(Instruction ins)
            {
                if (ins.IsCallOrNew())
                    return (ins.Operand as MethodReference).ReturnType.With(x => x == Builder.Current.TypeSystem.Void ? null : x);

                if (ins.IsLoadField())
                    return (ins.Operand as FieldReference).FieldType;

                if (ins.IsLoadLocal())
                    return (ins.Operand as VariableReference).VariableType;

                return null;
            }

            if (instructions == null || instructions.Count == 0)
                return null;

            var instruction = instructions.Last();
            var result = GetTypeOfValueInStack(instruction);

            if (result == null && instruction.IsValueOpCode())
            {
                for (int i = instructions.Count - 2; i >= 0; i--)
                {
                    if (!instructions[i].IsValueOpCode())
                        break;

                    result = GetTypeOfValueInStack(instructions[i]);

                    if (result != null)
                        return result;
                }
            }

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