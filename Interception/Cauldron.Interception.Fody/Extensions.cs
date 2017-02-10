using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cauldron.Interception.Fody
{
    internal static class Extensions
    {
        private static ModuleDefinition _moduleDefinition;
        private static IEnumerable<AssemblyDefinition> allAssemblies;
        private static IEnumerable<TypeDefinition> allTypes;

        public static ModuleDefinition ModuleDefinition
        {
            get { return _moduleDefinition; }
            set
            {
                _moduleDefinition = value;
                allAssemblies = value.AssemblyReferences.GetAll().Select(x => value.AssemblyResolver.Resolve(x))
                    .Concat(new AssemblyDefinition[] { value.Assembly }).ToArray();
                allTypes = allAssemblies.SelectMany(x => x.Modules).Where(x => x != null).SelectMany(x => x.Types).Where(x => x != null)
                    .Concat(value.Types).ToArray();
            }
        }

        public static IEnumerable<AssemblyDefinition> AllReferencedAssemblies(this ModuleDefinition target) => allAssemblies;

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

        public static IEnumerable<AssemblyNameReference> GetAll(this IEnumerable<AssemblyNameReference> target)
        {
            var result = new List<AssemblyNameReference>();
            result.AddRange(target);

            foreach (var item in target)
            {
                var assembly = ModuleDefinition.AssemblyResolver.Resolve(item);

                if (assembly == null)
                    continue;

                if (assembly.MainModule.HasAssemblyReferences)
                    result.AddRange(assembly.MainModule.AssemblyReferences);
            }

            return result.Distinct(new AssemblyNameReferenceEqualityComparer()).OrderBy(x => x.FullName);
        }

        public static IEnumerable<TypeReference> GetBaseClassAndInterfaces(this TypeDefinition type)
        {
            var result = new List<TypeReference>();
            result.Add(type);

            if (type.Interfaces != null && type.Interfaces.Count > 0)
                result.AddRange(type.Interfaces.Select(x => x.Resolve()));

            if (type.BaseType != null)
                result.AddRange(type.BaseType.Resolve().GetBaseClassAndInterfaces());

            return result;
        }

        public static TypeReference GetChildrenType(this TypeReference type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericInstance)
            {
                var genericType = type as GenericInstanceType;

                if (genericType != null)
                {
                    // If we have more than 1 generic argument then we try to get a IEnumerable<> interface
                    // otherwise we just return the last argument in the list
                    var ienumerableInterface = genericType.GetGenericInstances().FirstOrDefault(x => x.FullName.StartsWith("System.Collections.Generic.IEnumerable`1<"));

                    // We just don't know :(
                    if (ienumerableInterface == null)
                        return typeof(object).GetTypeReference();

                    return (ienumerableInterface as GenericInstanceType).GenericArguments[0];
                }
            }

            // This might be a type that inherits from list<> or something...
            // lets find out
            if (type.Resolve().GetBaseClassAndInterfaces().Any(x => x.FullName == "System.Collections.Generic.IEnumerable`1"))
            {
                // if this is the case we will dig until we find a generic instance type
                var baseType = type.Resolve().BaseType;

                while (baseType != null)
                {
                    if (baseType.IsGenericInstance)
                    {
                        var genericType = baseType as GenericInstanceType;

                        if (genericType != null)
                        {
                            var ienumerableInterface = genericType.GetGenericInstances().FirstOrDefault(x => x.FullName.StartsWith("System.Collections.Generic.IEnumerable`1<"));

                            if (ienumerableInterface == null)
                                break;

                            return (ienumerableInterface as GenericInstanceType).GenericArguments[0];
                        }
                    }

                    baseType = baseType.Resolve().BaseType;
                };
            }

            return typeof(object).GetTypeReference();
        }

        public static MethodReference GetDefaultConstructor(this TypeReference type)
        {
            if (!type.HasMethod(".ctor", 0))
                return null;

            var ctor = type.GetMethodReference(".ctor", 0);

            if (type.IsGenericInstance)
                return ctor.MakeHostInstanceGeneric((type as GenericInstanceType).GenericArguments.ToArray());

            return ctor;
        }

        public static IEnumerable<TypeReference> GetGenericInstances(this GenericInstanceType type)
        {
            var result = new List<TypeReference>();
            result.Add(type);

            var resolved = type.Resolve();
            var genericArgumentsNames = resolved.GenericParameters.Select(x => x.FullName).ToArray();
            var genericArguments = type.GenericArguments.ToArray();

            if (resolved.BaseType != null)
                result.AddRange(resolved.BaseType.GetGenericInstances(genericArgumentsNames, genericArguments));

            if (resolved.Interfaces != null && resolved.Interfaces.Count > 0)
            {
                foreach (var item in resolved.Interfaces)
                    result.AddRange(item.GetGenericInstances(genericArgumentsNames, genericArguments));
            }

            return result;
        }

        public static IEnumerable<TypeReference> GetInterfaces(this TypeDefinition type)
        {
            var result = new List<TypeReference>();

            if (type.Interfaces != null && type.Interfaces.Count > 0)
                result.AddRange(type.Interfaces.Select(x => x.Resolve()));

            if (type.BaseType != null)
                result.AddRange(type.BaseType.Resolve().GetInterfaces());

            return result;
        }

        public static IEnumerable<MethodReference> GetMethodDefinitionsByName(this TypeReference type, string methodName)
        {
            var result = new List<MethodReference>();

            var baseType = type;

            while (baseType != null)
            {
                if (baseType.IsGenericInstance)
                {
                    var genericType = baseType as GenericInstanceType;

                    if (genericType != null)
                    {
                        var instances = genericType.GetGenericInstances();

                        foreach (var item in instances)
                        {
                            var methods = item.Resolve().Methods.Where(x => x.Name == methodName).Select(x => x.MakeHostInstanceGeneric((item as GenericInstanceType).GenericArguments.ToArray()));

                            if (methods != null || methods.Any())
                                result.AddRange(methods);
                        }
                    }
                }
                else
                {
                    var methods = baseType.Resolve().Methods.Where(x => x.Name == methodName);

                    if (methods != null || methods.Any())
                        result.AddRange(methods);
                }

                baseType = baseType.Resolve().BaseType;
            };

            return result;
        }

        public static MethodReference GetMethodReference(this Type type, string methodName, Type[] parameterTypes)
        {
            var definition = type.GetTypeDefinition();
            var result = definition.GetMethodDefinitionsByName(methodName).FirstOrDefault(x => x.Name == methodName && parameterTypes.Select(y => y.FullName).SequenceEqual(x.Parameters.Select(y => y.ParameterType.FullName)));

            if (result != null)
                return result;

            throw new Exception($"Unable to proceed. The type '{type.FullName}' does not contain a method '{methodName}'");
        }

        public static MethodReference GetMethodReference(this TypeReference typeReference, string methodName, int parameterCount)
        {
            var definition = typeReference.Resolve();
            var result = definition.GetMethodDefinitionsByName(methodName).FirstOrDefault(x => x.Name == methodName && x.Parameters.Count == parameterCount);

            if (result != null)
                return result;

            throw new Exception($"Unable to proceed. The type '{typeReference.FullName}' does not contain a method '{methodName}'");
        }

        public static MethodReference GetMethodReference(this Type type, string methodName, int parameterCount)
        {
            var definition = type.GetTypeDefinition();
            var result = definition.GetMethodDefinitionsByName(methodName).FirstOrDefault(x => x.Name == methodName && x.Parameters.Count == parameterCount);

            if (result != null)
                return result;

            throw new Exception($"Unable to proceed. The type '{type.FullName}' does not contain a method '{methodName}'");
        }

        public static IEnumerable<MethodReference> GetMethodReferences(this TypeReference typeReference, string methodName, int parameterCount)
        {
            var definition = typeReference.Resolve();
            return definition.GetMethodDefinitionsByName(methodName).Where(x => x.Name == methodName && x.Parameters.Count == parameterCount);
        }

        public static PropertyDefinition GetPropertyDefinition(this MethodDefinition method) =>
                                                    method.DeclaringType.Properties.FirstOrDefault(x => x.GetMethod == method || x.SetMethod == method);

        /// <summary>
        /// Gets the stacktrace of the exception and the inner exceptions recursively
        /// </summary>
        /// <param name="e">The exception with the stack trace</param>
        /// <returns>A string representation of the stacktrace</returns>
        public static string GetStackTrace(this Exception e)
        {
            var sb = new StringBuilder();
            var ex = e;

            do
            {
                sb.AppendLine("Exception Type: " + ex.GetType().Name);
                sb.AppendLine("Source: " + ex.Source);
                sb.AppendLine(ex.Message);
                sb.AppendLine("------------------------");
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine("------------------------");

                ex = ex.InnerException;
            } while (ex != null);

            return sb.ToString();
        }

        public static TypeDefinition GetTypeDefinition(this Type type)
        {
            var result = allTypes.FirstOrDefault(x => x.FullName == type.FullName);

            if (result == null)
                throw new Exception($"Unable to proceed. The type '{type.FullName}' was not found.");

            return result;
        }

        public static TypeReference GetTypeReference(this Type type)
        {
            var result = allTypes.FirstOrDefault(x => x.FullName == type.FullName);

            if (result == null)
                throw new Exception($"Unable to proceed. The type '{type.FullName}' was not found.");

            return result;
        }

        public static IEnumerable<TypeDefinition> GetTypesThatImplementsInterface(this TypeDefinition typeDefinitionOfInterface) =>
            allTypes.Where(x => x.ImplementsInterface(typeDefinitionOfInterface.FullName)).ToArray();

        public static bool HasMethod(this TypeReference typeReference, string methodName, int parameterCount)
        {
            var definition = typeReference.Resolve();
            return definition.Methods.FirstOrDefault(x => x.Name == methodName && x.Parameters.Count == parameterCount) != null;
        }

        public static bool ImplementsInterface(this TypeDefinition type, Type interfaceType) =>
            type.GetInterfaces().Any(x => x.FullName == interfaceType.FullName);

        public static bool ImplementsInterface(this TypeDefinition type, string interfaceName) =>
            type.GetInterfaces().Any(x => x.FullName == interfaceName);

        public static TypeReference Import(this TypeReference value) => ModuleDefinition.Import(value);

        public static MethodReference Import(this System.Reflection.MethodBase value) => ModuleDefinition.Import(value);

        public static TypeReference Import(this Type value) => ModuleDefinition.Import(value);

        public static MethodReference Import(this MethodReference value) => ModuleDefinition.Import(value);

        public static FieldReference Import(this FieldReference value) => ModuleDefinition.Import(value);

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

        public static bool IsIEnumerable(this TypeReference type)
        {
            var resolved = type.Resolve();
            return
                resolved.ImplementsInterface(typeof(IList)) ||
                resolved.ImplementsInterface(typeof(IEnumerable)) ||
                type.IsArray ||
                type.FullName.EndsWith("[]") ||
                resolved.IsArray ||
                resolved.FullName.EndsWith("[]");
        }

        public static MethodReference MakeGeneric(this MethodReference method, params TypeReference[] args)
        {
            if (args.Length == 0)
                return method;

            if (method.GenericParameters.Count != args.Length)
                throw new ArgumentException("Invalid number of generic type arguments supplied");

            var genericTypeRef = new GenericInstanceMethod(method);

            foreach (var arg in args)
                genericTypeRef.GenericArguments.Add(arg);

            return genericTypeRef;
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            // https://groups.google.com/forum/#!topic/mono-cecil/mCat5UuR47I by ShdNx

            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericInstanceType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };

            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var generic_parameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            return reference;
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

        public static TypeDefinition ToTypeDefinition(this string typeName) => allTypes.FirstOrDefault(x => x.FullName == typeName || x.FullName.EndsWith(typeName) || x.Name == typeName);

        public static IEnumerable<Instruction> TypeOf(this ILProcessor processor, TypeReference type)
        {
            return new Instruction[] {
                processor.Create(OpCodes.Ldtoken, type),
                processor.Create(OpCodes.Call, ModuleDefinition.Import(typeof(Type).GetMethodReference("GetTypeFromHandle", 1)))
            };
        }

        private static IEnumerable<TypeReference> GetGenericInstances(this TypeReference type, string[] genericArgumentNames, TypeReference[] genericArgumentsOfCurrentType)
        {
            var resolvedBase = type.Resolve();

            if (resolvedBase.HasGenericParameters)
            {
                var genericArguments = new List<TypeReference>();

                foreach (var arg in (type as GenericInstanceType).GenericArguments)
                {
                    var t = genericArgumentNames.FirstOrDefault(x => x == arg.FullName);

                    if (t == null)
                        genericArguments.Add(arg);
                    else
                        genericArguments.Add(genericArgumentsOfCurrentType[Array.IndexOf(genericArgumentNames, t)]);
                }

                var genericType = resolvedBase.MakeGenericInstanceType(genericArguments.ToArray());
                return genericType.GetGenericInstances();
            }

            return new TypeReference[0];
        }
    }
}