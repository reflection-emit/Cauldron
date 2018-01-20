using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class Extensions
    {
        public static bool AreReferenceAssignable(this BuilderType type, BuilderType toBeAssigned)
        {
            if ((toBeAssigned == null && !type.IsValueType) || type == toBeAssigned ||
                    (!type.typeDefinition.IsValueType && !toBeAssigned.typeDefinition.IsValueType && type.IsAssignableFrom(toBeAssigned)) ||
                    (type.IsInterface && toBeAssigned.typeReference == Builder.Current.TypeSystem.Object))
                return true;

            return false;
        }

        public static bool AreReferenceAssignable(this TypeReference type, TypeReference toBeAssigned)
        {
            if (
                (toBeAssigned == null && !type.IsValueType) ||
                type == toBeAssigned ||
                (!type.IsValueType && !toBeAssigned.IsValueType && type.IsAssignableFrom(toBeAssigned)) ||
                (type.Resolve().IsInterface && toBeAssigned == Builder.Current.TypeSystem.Object) ||
                type.FullName == toBeAssigned.FullName)
                return true;

            return false;
        }

        /// <summary>
        /// Creates a coder.
        /// </summary>
        /// <param name="method">The coder.</param>
        /// <returns></returns>
        public static Coder Body(this Method method) => new Coder(method);

        /// <summary>
        /// Creates a new coder.
        /// </summary>
        /// <param name="coder">The coder.</param>
        /// <returns></returns>
        public static Coder Body(this Coder coder) => new Coder(coder.method);

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
    }
}