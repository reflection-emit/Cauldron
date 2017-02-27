using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public static class Extension
    {
        public static Builder CreateBuilder(this IWeaver weaver)
        {
            if (weaver == null)
                throw new ArgumentNullException(nameof(weaver), $"Argument '{nameof(weaver)}' cannot be null");

            return new Builder(weaver);
        }

        public static IEnumerable<TypeReference> GetBaseClasses(this TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef != null && typeDef.BaseType != null)
            {
                yield return typeDef.BaseType;
                foreach (var item in GetBaseClasses(typeDef.BaseType))
                    yield return item;
            }
        }

        public static IEnumerable<TypeReference> GetInterfaces(this TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef == null)
                return new TypeReference[0];

            if (typeDef.Interfaces != null && typeDef.Interfaces.Count > 0)
                return type.Recursive(x => x.Resolve().Interfaces).Select(x => x.ResolveType(type));

            if (typeDef.BaseType != null)
                return GetInterfaces(typeDef.BaseType);

            return new TypeReference[0];
        }

        public static MethodReference GetMethodReference(this TypeReference type, string methodName, Type[] parameterTypes)
        {
            var definition = type.Resolve();
            var result = definition
                .Methods
                .FirstOrDefault(x => x.Name == methodName && parameterTypes.Select(y => y.FullName).SequenceEqual(x.Parameters.Select(y => y.ParameterType.FullName)));

            if (result != null)
                return result;

            throw new Exception($"Unable to proceed. The type '{type.FullName}' does not contain a method '{methodName}'");
        }

        public static IEnumerable<TypeReference> GetNestedTypes(this TypeReference type)
        {
            var typeDef = type.Resolve();

            if (typeDef == null)
                return new TypeReference[0];

            if (typeDef.NestedTypes != null && typeDef.NestedTypes.Count > 0)
                return type.Recursive(x => x.Resolve().NestedTypes).Select(x => x.ResolveType(type));

            if (typeDef.BaseType != null)
                return GetNestedTypes(typeDef.BaseType);

            return new TypeReference[0];
        }

        public static bool Implements(this TypeReference type, string interfaceName) => type.GetInterfaces().Any(x => x.FullName == interfaceName);

        public static bool IsAssignableFrom(this BuilderType target, BuilderType source) =>
                            target == source ||
            target.typeDefinition == source.typeDefinition ||
            source.IsSubclassOf(target) ||
            target.IsInterface && source.BaseClasses.Any(x => x.Implements(target));

        public static bool IsAssignableFrom(this TypeReference target, TypeReference source) =>
                    target == source ||
            target == source ||
            source.IsSubclassOf(target) ||
            target.Resolve().IsInterface && source.GetBaseClasses().Any(x => x.Implements(target.FullName));

        public static bool IsIEnumerable(this TypeReference type)
        {
            var resolved = type.Resolve();

            if (resolved == null)
                return false;

            return
                type.FullName != typeof(string).FullName /* Strings are arrays too */ &&
                (
                    type is ArrayType ||
                    type.GetInterfaces().Any(x => x.FullName == typeof(IList).FullName || x.FullName == typeof(IEnumerable).FullName) ||
                    type.IsArray ||
                    type.FullName.EndsWith("[]") ||
                    resolved.IsArray ||
                    resolved.FullName.EndsWith("[]")
                );
        }

        public static bool IsSubclassOf(this BuilderType child, BuilderType parent) => child.typeDefinition != parent.typeDefinition && child.BaseClasses.Any(x => x.typeDefinition == parent.typeDefinition);

        public static bool IsSubclassOf(this TypeReference child, TypeReference parent) => child != parent && child.GetBaseClasses().Any(x => x == parent);

        public static T LogContent<T>(this T target) where T : IEnumerable<BuilderType>
        {
            foreach (var item in target)
                item.LogInfo(item.Fullname);

            return target;
        }

        public static TNew New<TType, TNew>(this TType target, Func<TType, TNew> predicate) => predicate(target);

        internal static void Append(this ILProcessor processor, Instruction[] instructions) => processor.Append(instructions as IEnumerable<Instruction>);

        internal static void Append(this ILProcessor processor, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
                processor.Append(instruction);
        }

        internal static FieldReference CreateFieldReference(this FieldDefinition field)
        {
            if (field.DeclaringType.HasGenericParameters)
            {
                var declaringType = new GenericInstanceType(field.DeclaringType);

                foreach (var parameter in field.DeclaringType.GenericParameters)
                    declaringType.GenericArguments.Add(parameter);

                return new FieldReference(field.Name, field.FieldType, declaringType);
            }

            return field;
        }

        internal static FieldReference CreateFieldReference(this FieldReference field)
        {
            if (field.DeclaringType.HasGenericParameters)
            {
                var declaringType = new GenericInstanceType(field.DeclaringType);

                foreach (var parameter in field.DeclaringType.GenericParameters)
                    declaringType.GenericArguments.Add(parameter);

                return new FieldReference(field.Name, field.FieldType, declaringType);
            }

            return field;
        }

        internal static MethodReference CreateMethodReference(this MethodDefinition method)
        {
            if (method.DeclaringType.HasGenericParameters)
            {
                var declaringType = new GenericInstanceType(method.DeclaringType);
                return method.MakeHostInstanceGeneric(method.DeclaringType.GenericParameters.ToArray());
            }

            return method;
        }

        internal static IEnumerable<Instruction> GetJumpSources(this IEnumerable<Instruction> body, Instruction jumpTarget)
        {
            foreach (var item in body)
            {
                var target = item.Operand as Instruction;

                if (target != null && target.Offset == jumpTarget.Offset)
                    yield return item;
            }
        }

        internal static void InsertAfter(this ILProcessor processor, Instruction target, Instruction[] instructions) => processor.InsertAfter(target, instructions as IEnumerable<Instruction>);

        internal static void InsertAfter(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            var last = target;

            foreach (var instruction in instructions)
            {
                processor.InsertAfter(last, instruction);
                last = instruction;
            }
        }

        internal static void InsertBefore(this ILProcessor processor, Instruction target, Instruction[] instructions) => processor.InsertBefore(target, instructions as IEnumerable<Instruction>);

        internal static void InsertBefore(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
                processor.InsertBefore(target, instruction);
        }

        internal static bool IsCallOrNew(this Instruction instruction)
        {
            var opCode = instruction.OpCode;
            return
                opCode == OpCodes.Call ||
                opCode == OpCodes.Calli ||
                opCode == OpCodes.Callvirt ||
                opCode == OpCodes.Newobj;
        }

        internal static bool IsDelegate(this TypeDefinition typeDefinition)
        {
            if (typeDefinition.BaseType == null)
                return false;

            return typeDefinition.BaseType.FullName == "System.MulticastDelegate";
        }

        internal static bool IsDelegate(this TypeReference typeReference) => typeReference.Resolve().IsDelegate();

        internal static bool IsLoadArgument(this Instruction instruction)
        {
            var opCode = instruction.OpCode;
            return
                opCode == OpCodes.Ldarg ||
                opCode == OpCodes.Ldarga ||
                opCode == OpCodes.Ldarga_S ||
                opCode == OpCodes.Ldarg_0 ||
                opCode == OpCodes.Ldarg_1 ||
                opCode == OpCodes.Ldarg_2 ||
                opCode == OpCodes.Ldarg_3;
        }

        internal static bool IsLoadField(this Instruction instruction)
        {
            var opCode = instruction.OpCode;
            return
                opCode == OpCodes.Ldsfld ||
                opCode == OpCodes.Ldsflda ||
                opCode == OpCodes.Ldfld ||
                opCode == OpCodes.Ldflda;
        }

        internal static bool IsLoadLocal(this Instruction instruction)
        {
            var opCode = instruction.OpCode;
            return
                opCode == OpCodes.Ldloc ||
                opCode == OpCodes.Ldloc_S ||
                opCode == OpCodes.Ldloca ||
                opCode == OpCodes.Ldloca_S ||
                opCode == OpCodes.Ldloc_0 ||
                opCode == OpCodes.Ldloc_1 ||
                opCode == OpCodes.Ldloc_2 ||
                opCode == OpCodes.Ldloc_3;
        }

        internal static bool IsStoreLocal(this Instruction instruction)
        {
            var opCode = instruction.OpCode;
            return
                opCode == OpCodes.Stloc ||
                opCode == OpCodes.Stloc_S ||
                opCode == OpCodes.Stloc_0 ||
                opCode == OpCodes.Stloc_1 ||
                opCode == OpCodes.Stloc_2 ||
                opCode == OpCodes.Stloc_3;
        }

        internal static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] arguments)
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

        internal static IEnumerable<T> Recursive<T>(this T root, Func<T, IEnumerable<T>> children)
        {
            // http://codereview.stackexchange.com/questions/5648/any-way-to-make-this-recursive-function-better-faster
            // Eric Lippert

            var stack = new Stack<T>();
            stack.Push(root);

            while (stack.Count != 0)
            {
                var current = stack.Pop();
                // If you don't care about maintaining child order then remove the Reverse.
                foreach (var child in children(current).Reverse())
                    stack.Push(child);

                yield return current;
            }
        }

        internal static TypeReference ResolveType(this TypeReference type, TypeReference inheritingOrImplementingType)
        {
            if (type.HasGenericParameters && inheritingOrImplementingType is GenericInstanceType)
            {
                var genericArgumentNames = inheritingOrImplementingType.GenericParameters.Select(x => x.FullName).ToArray();
                var genericArgumentsOfCurrentType = (inheritingOrImplementingType as GenericInstanceType).GenericArguments.ToArray();

                var genericInstanceType = type as GenericInstanceType ?? type.MakeGenericInstanceType(type.GenericParameters.ToArray());
                var genericArguments = new TypeReference[genericInstanceType.GenericArguments.Count];

                for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
                {
                    var t = genericArgumentNames.FirstOrDefault(x => x == genericInstanceType.GenericArguments[i].FullName);

                    if (t == null)
                        genericArguments[i] = genericInstanceType.GenericArguments[i];
                    else
                        genericArguments[i] = genericArgumentsOfCurrentType[Array.IndexOf(genericArgumentNames, t)];
                }

                return type.MakeGenericInstanceType(genericArguments);
            }
            else if (type.HasGenericParameters)
                return type.MakeGenericInstanceType(type.GenericParameters.ToArray());
            else
                return type;
        }
    }
}