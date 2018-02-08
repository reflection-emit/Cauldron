using Cauldron.Interception.Cecilator.Coders;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class Extensions
    {
        public static bool AreEqual(this TypeDefinition a, TypeDefinition b) =>
            a.Module.Assembly.Name.GetHashCode() == b.Module.Assembly.Name.GetHashCode() &&
            a.Module.Assembly.Name == b.Module.Assembly.Name &&
            a.FullName.GetHashCode() == b.FullName.GetHashCode() &&
            a.FullName == b.FullName;

        public static bool AreEqual(this TypeReference a, TypeReference b) =>
            a.Module.Assembly.Name.GetHashCode() == b.Module.Assembly.Name.GetHashCode() &&
            a.Module.Assembly.Name == b.Module.Assembly.Name &&
            a.FullName.GetHashCode() == b.FullName.GetHashCode() &&
            a.FullName == b.FullName;

        public static bool AreEqual(this TypeReference a, BuilderType b) => a.AreEqual(b.typeReference) || a.AreEqual(b.typeDefinition);

        /// <summary>
        /// Checks if <paramref name="toBeAssigned"/> is assignable to <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to assign to.</param>
        /// <param name="toBeAssigned">The type that will be assigned to <paramref name="type"/>.</param>
        /// <returns>Returns true if <paramref name="toBeAssigned"/> is assignable to <paramref name="type"/>; otherwise false.</returns>
        public static bool AreReferenceAssignable(this BuilderType type, BuilderType toBeAssigned)
        {
            if ((toBeAssigned == null && !type.IsValueType) || type == toBeAssigned ||
                    (!type.typeDefinition.IsValueType && !toBeAssigned.typeDefinition.IsValueType && type.IsAssignableFrom(toBeAssigned)) ||
                    (type.IsInterface && toBeAssigned == BuilderType.Object))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if <paramref name="toBeAssigned"/> is assignable to <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to assign to.</param>
        /// <param name="toBeAssigned">The type that will be assigned to <paramref name="type"/>.</param>
        /// <returns>Returns true if <paramref name="toBeAssigned"/> is assignable to <paramref name="type"/>; otherwise false.</returns>
        public static bool AreReferenceAssignable(this TypeReference type, TypeReference toBeAssigned)
        {
            if (
                (toBeAssigned == null && !type.IsValueType) ||
                type == toBeAssigned ||
                (!type.IsValueType && !toBeAssigned.IsValueType && type.IsAssignableFrom(toBeAssigned)) ||
                (type.Resolve().IsInterface && toBeAssigned.AreEqual(BuilderType.Object)) ||
                type.FullName == toBeAssigned.FullName)
                return true;

            return false;
        }

        public static TypeDefinition GetTypeDefinition(this Type type)
        {
            var result = WeaverBase.AllTypes.Get(type.FullName);

            if (result == null)
                throw new Exception($"Unable to proceed. The type '{type.FullName}' was not found.");

            return Builder.Current.Import(type).Resolve() ?? result;
        }

        /// <summary>
        /// Returns true if the instruction defined by <paramref name="instruction"/> is enclosed by a try-catch-finally.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="instruction">The instruction to check</param>
        /// <returns>True if enclosed; otherwise false.</returns>
        public static bool IsInclosedInHandlers(this Method method, Instruction instruction)
        {
            foreach (var item in method.methodDefinition.Body.ExceptionHandlers)
            {
                if (item.TryStart.Offset >= instruction.Offset && item.TryStart.Offset <= instruction.Offset)
                    return true;

                if (item.HandlerStart != null && item.HandlerStart.Offset >= instruction.Offset && item.HandlerEnd.Offset <= instruction.Offset)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a new coder.
        /// </summary>
        /// <param name="method">The coder.</param>
        /// <returns></returns>
        public static Coder NewCoder(this Method method) => new Coder(method);

        /// <summary>
        /// Creates a new coder.
        /// </summary>
        /// <param name="coder">The coder.</param>
        /// <returns></returns>
        public static Coder NewCoder(this CoderBase coder) => new Coder(coder.instructions.associatedMethod);

        /// <summary>
        /// Creates a typeof() implementation for the given type <paramref name="type"/>
        /// </summary>
        /// <param name="processor">The processor to use.</param>
        /// <param name="type">The type to get the type from.</param>
        /// <returns>A collection of instructions that canbe added to the coder's instruction set.</returns>
        public static IEnumerable<Instruction> TypeOf(this ILProcessor processor, BuilderType type) =>
            processor.TypeOf(type.typeReference);

        /// <summary>
        /// Creates a typeof() implementation for the given type <paramref name="type"/>
        /// </summary>
        /// <param name="processor">The processor to use.</param>
        /// <param name="type">The type to get the type from.</param>
        /// <returns>A collection of instructions that canbe added to the coder's instruction set.</returns>
        public static IEnumerable<Instruction> TypeOf(this ILProcessor processor, TypeReference type)
        {
            return new Instruction[] {
                processor.Create(OpCodes.Ldtoken, type),
                processor.Create(OpCodes.Call,
                    Builder.Current.Import(
                        typeof(Type).GetTypeDefinition()
                            .Methods.FirstOrDefault(x=>x.Name == "GetTypeFromHandle" && x.Parameters.Count == 1)))
            };
        }

        internal static TypeReference GetTypeOfValueInStack(this IEnumerable<Instruction> instructions, Method method)
        {
            TypeReference GetTypeOfValueInStack(Instruction ins)
            {
                if (ins.IsCallOrNew())
                    return (ins.Operand as MethodReference).ReturnType.With(x => x.AreEqual(BuilderType.Void) ? null : x);

                if (ins.IsLoadField())
                    return (ins.Operand as FieldReference).FieldType;

                if (ins.IsLoadLocal())
                    return method.methodDefinition.GetVariable(ins)?.VariableType;

                if (ins.OpCode == OpCodes.Ldarg || ins.OpCode == OpCodes.Ldarga || ins.OpCode == OpCodes.Ldarga_S)
                    switch (ins.Operand)
                    {
                        case ParameterDefinition value: return CodeBlocks.GetParameter(value.Index).GetTargetType(method)?.Item1?.typeReference;
                        case ParameterReference value: return CodeBlocks.GetParameter(value.Index).GetTargetType(method)?.Item1?.typeReference;
                        case int value: return CodeBlocks.GetParameter(value).GetTargetType(method)?.Item1?.typeReference;
                    }

                if (ins.OpCode == OpCodes.Ldarg_0) return CodeBlocks.GetParameter(0).GetTargetType(method)?.Item1?.typeReference;
                if (ins.OpCode == OpCodes.Ldarg_1) return CodeBlocks.GetParameter(1).GetTargetType(method)?.Item1?.typeReference;
                if (ins.OpCode == OpCodes.Ldarg_2) return CodeBlocks.GetParameter(2).GetTargetType(method)?.Item1?.typeReference;
                if (ins.OpCode == OpCodes.Ldarg_3) return CodeBlocks.GetParameter(3).GetTargetType(method)?.Item1?.typeReference;

                if (ins.OpCode == OpCodes.Isinst)
                    return ins.Operand as TypeReference;

                return null;
            }

            if (instructions == null || !instructions.Any())
                return null;

            var instruction = instructions.Last();
            var result = GetTypeOfValueInStack(instruction);
            var array = instructions.ToArray();

            if (
                result == null &&
                instruction.OpCode != OpCodes.Unbox_Any &&
                instruction.OpCode != OpCodes.Unbox &&
                instruction.OpCode != OpCodes.Isinst &&
                instruction.OpCode != OpCodes.Castclass &&
                instruction.IsValueOpCode())
            {
                for (int i = array.Length - 2; i >= 0; i--)
                {
                    if (!array[i].IsValueOpCode())
                        break;

                    result = GetTypeOfValueInStack(array[i]);

                    if (result != null)
                        return result;
                }
            }

            return result;
        }
    }
}