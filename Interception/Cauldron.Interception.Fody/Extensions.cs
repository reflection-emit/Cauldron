using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    internal static class Extensions
    {
        public static IEnumerable<AssemblyDefinition> Asemblies;
        public static IEnumerable<TypeDefinition> Types;

        public static void Append(this ILProcessor processor, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
                processor.Append(instruction);
        }

        public static MethodReference GetMethodReference(this Type type, string methodName, Type[] parameterTypes)
        {
            var definition = type.GetTypeDefinition();
            return definition.Methods.FirstOrDefault(x => x.Name == methodName && parameterTypes.Select(y => y.FullName).SequenceEqual(x.Parameters.Select(y => y.ParameterType.FullName)));
        }

        public static MethodReference GetMethodReference(this TypeReference typeReference, string methodName, int parameterCount)
        {
            var definition = typeReference.Resolve();
            return definition.Methods.FirstOrDefault(x => x.Name == methodName && x.Parameters.Count == parameterCount);
        }

        public static MethodReference GetMethodReference(this Type type, string methodName, int parameterCount)
        {
            var definition = type.GetTypeDefinition();
            return definition.Methods.FirstOrDefault(x => x.Name == methodName && x.Parameters.Count == parameterCount);
        }

        public static TypeDefinition GetTypeDefinition(this Type type) => Types.FirstOrDefault(x => x.FullName == type.FullName);

        public static TypeReference GetTypeReference(this Type type) => Types.FirstOrDefault(x => x.FullName == type.FullName);

        public static IEnumerable<TypeDefinition> GetTypesThatImplementsInterface(this TypeDefinition typeDefinitionOfInterface) =>
             Types.Where(x => x.Implements(typeDefinitionOfInterface.Name)).ToArray();

        public static bool Implements(this TypeDefinition typeDefinition, string interfaceName)
        {
            while (typeDefinition != null)
            {
                if (typeDefinition.Interfaces != null && typeDefinition.Interfaces.Any(x => x.Name == interfaceName))
                    return true;

                typeDefinition = typeDefinition.BaseType?.Resolve();
            }

            return false;
        }

        public static void InsertBefore(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
                processor.InsertBefore(target, instruction);
        }

        public static bool IsAttribute(this TypeDefinition typeDefinition)
        {
            typeDefinition = typeDefinition.BaseType?.Resolve();

            while (typeDefinition != null)
            {
                if (typeDefinition.FullName == "System.Attribute")
                    return true;

                typeDefinition = typeDefinition.BaseType?.Resolve();
            }

            return false;
        }
    }
}