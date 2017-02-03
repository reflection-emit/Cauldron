using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
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

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable"/></param>
        /// <returns>The total count of items in the <see cref="IEnumerable"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        public static int Count_(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int count = 0;

            if (source.GetType().IsArray)
                return (source as Array).Length;

            var collection = source as ICollection;
            if (collection != null)
                return collection.Count;

            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
                count++;

            (enumerator as IDisposable)?.Dispose();

            return count;
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

        /// <summary>
        /// Converts a <see cref="IEnumerable"/> to an array
        /// </summary>
        /// <typeparam name="T">The type of elements the <see cref="IEnumerable"/> contains</typeparam>
        /// <param name="items">The <see cref="IEnumerable"/> to convert</param>
        /// <returns>An array of <typeparamref name="T"/></returns>
        public static T[] ToArray_<T>(this IEnumerable items)
        {
            if (items == null)
                return new T[0];

            T[] result = new T[items.Count_()];
            int counter = 0;

            foreach (T item in items)
            {
                result[counter] = item;
                counter++;
            }

            return result;
        }
    }
}