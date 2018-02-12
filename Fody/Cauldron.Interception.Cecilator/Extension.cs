using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Cauldron.Interception.Cecilator
{
    public static class Extension
    {
        public static TypeDefinition BetterResolve(this TypeReference value) => value.Resolve() ?? WeaverBase.AllTypes.Get(value.FullName);

        public static Builder CreateBuilder(this WeaverBase weaver)
        {
            if (weaver == null)
                throw new ArgumentNullException(nameof(weaver), $"Argument '{nameof(weaver)}' cannot be null");

            Builder.Current = new Builder(weaver);
            return Builder.Current;
        }

        public static bool EqualsEx(this string me, string other) => me.GetHashCode() == other.GetHashCode() && me == other;

        public static CustomAttribute Get(this Mono.Collections.Generic.Collection<CustomAttribute> collection, string name)
        {
            if (name.IndexOf('.') > 0)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    var fullname = collection[i].AttributeType.BetterResolve().FullName;
                    if (fullname.GetHashCode() == name.GetHashCode() && fullname == name)
                        return collection[i];
                }
            }
            else
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    var shortname = collection[i].AttributeType.BetterResolve().Name;
                    if (shortname.GetHashCode() == name.GetHashCode() && shortname == name)
                        return collection[i];
                }
            }

            return null;
        }

        public static T Get<T>(this IEnumerable<T> source, string name) where T : MemberReference
        {
            var collection = source.ToArray();
            if (name.IndexOf('.') > 0)
            {
                for (int i = 0; i < collection.Length; i++)
                {
                    var fullname = collection[i].FullName;
                    if (fullname.GetHashCode() == name.GetHashCode() && fullname == name)
                        return collection[i];
                }
            }
            else
            {
                for (int i = 0; i < collection.Length; i++)
                {
                    var shortname = collection[i].Name;
                    if (shortname.GetHashCode() == name.GetHashCode() && shortname == name)
                        return collection[i];
                }
            }

            return null;
        }

        public static IEnumerable<TypeReference> GetBaseClasses(this TypeReference type)
        {
            var typeDef = type.BetterResolve();

            if (typeDef != null && typeDef.BaseType != null)
            {
                yield return typeDef.BaseType;
                foreach (var item in GetBaseClasses(typeDef.BaseType))
                    yield return item;
            }
        }

        public static TypeReference GetChildrenType(this ModuleDefinition module, TypeReference type)
        {
            if (type.IsArray)
                return type.GetElementType();

            TypeReference getIEnumerableInterfaceChild(TypeReference typeReference)
            {
                if (typeReference.IsGenericInstance)
                {
                    if (typeReference is GenericInstanceType genericType)
                    {
                        var genericInstances = genericType.GetGenericInstances();

                        // We have to make some exceptions to dictionaries
                        var ienumerableInterface = genericInstances.FirstOrDefault(x => x.FullName.StartsWith("System.Collections.Generic.IDictionary`2<"));

                        // If we have more than 1 generic argument then we try to get a IEnumerable<>
                        // interface otherwise we just return the last argument in the list
                        if (ienumerableInterface == null)
                            ienumerableInterface = genericInstances.FirstOrDefault(x => x.FullName.StartsWith("System.Collections.Generic.IEnumerable`1<"));

                        // A Nullable special
                        if (ienumerableInterface == null && genericType.AreEqual(BuilderType.Nullable))
                            return genericType.GenericArguments[0];

                        // We just don't know :(
                        if (ienumerableInterface == null)
                            return module.ImportReference(typeof(object));

                        return (ienumerableInterface as GenericInstanceType).GenericArguments[0];
                    }
                }

                return null;
            }

            var result = getIEnumerableInterfaceChild(type);

            if (result != null)
                return result;

            // This might be a type that inherits from list<> or something... lets find out
            if (type.BetterResolve().GetInterfaces().Any(x => x.FullName == "System.Collections.Generic.IEnumerable`1"))
            {
                // if this is the case we will dig until we find a generic instance type
                var baseType = type.BetterResolve().BaseType;

                while (baseType != null)
                {
                    result = getIEnumerableInterfaceChild(baseType);

                    if (result != null)
                        return result;

                    baseType = baseType.BetterResolve().BaseType;
                };
            }

            return module.ImportReference(typeof(object));
        }

        public static IReadOnlyDictionary<string, TypeReference> GetGenericResolvedTypeName(this GenericInstanceType type)
        {
            var genericArgumentNames = type.BetterResolve().GenericParameters.Select(x => x.FullName).ToArray();
            var genericArgumentsOfCurrentType = type.GenericArguments.ToArray();

            var result = new Dictionary<string, TypeReference>();

            for (int i = 0; i < genericArgumentNames.Length; i++)
                result.Add(genericArgumentNames[i], genericArgumentsOfCurrentType[i]);

            return new ReadOnlyDictionary<string, TypeReference>(result);
        }

        public static IReadOnlyDictionary<string, TypeReference> GetGenericResolvedTypeNames(this GenericInstanceType type)
        {
            var genericArgumentNames = type.BetterResolve().GenericParameters.Select(x => x.FullName).ToArray();
            var genericArgumentsOfCurrentType = type.GenericArguments.ToArray();
            var baseType = type as TypeReference;

            var result = new Dictionary<string, TypeReference>();

            while (baseType != null)
            {
                if (baseType.IsGenericInstance)
                {
                    if (baseType is GenericInstanceType genericType)
                    {
                        genericArgumentNames = genericType.BetterResolve().GenericParameters.Select(x => x.FullName).ToArray();
                        genericArgumentsOfCurrentType = genericType.GenericArguments.ToArray();

                        for (int i = 0; i < genericArgumentNames.Length; i++)
                        {
                            if (!result.ContainsKey(genericArgumentNames[i]))
                                result.Add(genericArgumentNames[i], genericArgumentsOfCurrentType[i]);
                        }
                    }
                }

                baseType = baseType.BetterResolve().BaseType;
            }

            return new ReadOnlyDictionary<string, TypeReference>(result);
        }

        public static IReadOnlyDictionary<string, TypeReference> GetGenericResolvedTypeNames(this GenericInstanceMethod method)
        {
            var genericArgumentNames = method.Resolve().GenericParameters.Select(x => x.FullName).ToArray();
            var genericArgumentsOfCurrentType = method.GenericArguments.ToArray();

            var result = new Dictionary<string, TypeReference>();
            var genericType = method as GenericInstanceMethod;

            genericArgumentNames = genericType.Resolve().GenericParameters.Select(x => x.FullName).ToArray();
            genericArgumentsOfCurrentType = genericType.GenericArguments.ToArray();

            for (int i = 0; i < genericArgumentNames.Length; i++)
            {
                if (!result.ContainsKey(genericArgumentNames[i]))
                    result.Add(genericArgumentNames[i], genericArgumentsOfCurrentType[i]);
            }

            return new ReadOnlyDictionary<string, TypeReference>(result);
        }

        public static IEnumerable<TypeReference> GetInterfaces(this TypeReference type)
        {
            var typeDef = type.BetterResolve();
            var result = new List<TypeReference>();

            while (true)
            {
                if (typeDef == null)
                    break;

                try
                {
                    if (typeDef.BaseType == null && (typeDef.Interfaces == null || typeDef.Interfaces.Count == 0))
                        break;

                    if (typeDef.Interfaces != null && typeDef.Interfaces.Count > 0)
                        result.AddRange(type.Recursive(x => x.BetterResolve().Interfaces.Select(y => y.InterfaceType)).Select(x => x.ResolveType(type)));

                    type = typeDef.BaseType;

                    typeDef = type?.BetterResolve();
                }
                catch
                {
                    break;
                }
            };

            return result;
        }

        public static MethodReference GetMethodReference(this TypeReference type, string methodName, int parameterCount)
        {
            var definition = type.BetterResolve();
            var result = definition
                .Methods
                .FirstOrDefault(x => x.Name == methodName && x.Parameters.Count == parameterCount);

            if (result != null)
                return result;

            throw new Exception($"Unable to proceed. The type '{type.FullName}' does not contain a method '{methodName}'");
        }

        public static MethodReference GetMethodReference(this TypeReference type, string methodName, Type[] parameterTypes)
        {
            var definition = type.BetterResolve();
            var result = definition
                .Methods
                .FirstOrDefault(x => x.Name == methodName && parameterTypes.Select(y => y.FullName).SequenceEqual(x.Parameters.Select(y => y.ParameterType.FullName)));

            if (result != null)
                return result;

            throw new Exception($"Unable to proceed. The type '{type.FullName}' does not contain a method '{methodName}'");
        }

        public static MethodReference GetMethodReference(this TypeReference type, string methodName, TypeReference[] parameterTypes)
        {
            var definition = type.BetterResolve();
            var result = definition
                .Methods
                .FirstOrDefault(x => x.Name == methodName && parameterTypes.Select(y => y.FullName).SequenceEqual(x.Parameters.Select(y => y.ParameterType.FullName)));

            if (result != null)
                return result;

            throw new Exception($"Unable to proceed. The type '{type.FullName}' does not contain a method '{methodName}'");
        }

        public static IEnumerable<(MethodReference reference, MethodDefinition definition)> GetMethodReferences(this TypeReference type) => GetMethodReferences(type, false);

        public static IEnumerable<(MethodReference reference, MethodDefinition definition)> GetMethodReferencesByInterfaces(this TypeReference type) => GetMethodReferences(type, true);

        public static IEnumerable<TypeReference> GetNestedTypes(this TypeReference type)
        {
            var typeDef = type.BetterResolve();

            if (typeDef == null)
                return new TypeReference[0];

            if (typeDef.NestedTypes != null && typeDef.NestedTypes.Count > 0)
                return type.Recursive(x => x.BetterResolve().NestedTypes).Select(x => x.ResolveType(type));

            if (typeDef.BaseType != null)
                return GetNestedTypes(typeDef.BaseType);

            return new TypeReference[0];
        }

        public static SequencePoint GetSequencePoint(this MethodDefinition methodDefinition)
        {
            if (methodDefinition == null || methodDefinition.Body == null || methodDefinition.Body.Instructions == null)
                return null;

            foreach (var instruction in methodDefinition.Body.Instructions)
            {
                var result = methodDefinition.DebugInformation.GetSequencePoint(instruction);
                if (result == null)
                    continue;

                return result;
            }

            return null;
        }

        public static bool Implements(this TypeReference type, string interfaceName) => type.GetInterfaces().Any(x => x.FullName == interfaceName);

        public static int IndexOf(this IList<Instruction> instructions, int offset)
        {
            for (int i = 0; i < instructions.Count; i++)
                if (instructions[i].Offset == offset)
                    return i;

            return -1;
        }

        public static bool IsAssignableFrom(this BuilderType[] target, BuilderType[] types)
        {
            if (target == null && types == null)
                return true;

            if (target == null || types == null)
                return false;

            if (target.Length != types.Length)
                return false;

            for (int i = 0; i < target.Length; i++)
            {
                if (!target[i].IsAssignableFrom(types[i]))
                    return false;
            }

            return true;
        }

        public static bool IsAssignableFrom(this BuilderType target, BuilderType source) =>
                                    target == source ||
                    target.typeDefinition == source.typeDefinition ||
                    target.Fullname == "System.Object" ||
                    source.IsSubclassOf(target) ||
                    target.IsInterface && source.BaseClasses.Any(x => x.Implements(target));

        public static bool IsAssignableFrom(this TypeReference target, TypeReference source) =>
                            target == source ||
                    target == source ||
                    target.FullName == "System.Object" ||
                    source.IsSubclassOf(target) ||
                    target.BetterResolve().IsInterface && source.GetBaseClasses().Any(x => x.Implements(target.FullName));

        public static bool IsSubclassOf(this BuilderType child, BuilderType parent) => child.typeDefinition != parent.typeDefinition && child.BaseClasses.Any(x => x.typeDefinition == parent.typeDefinition);

        public static bool IsSubclassOf(this TypeReference child, TypeReference parent) => child != parent && child.GetBaseClasses().Any(x => x == parent);

        public static void Log(this CecilatorObject cecilatorObject, LogTypes logTypes, BuilderType type, object arg) => cecilatorObject.Log(logTypes, type.GetRelevantConstructors().FirstOrDefault() ?? type.Methods.FirstOrDefault(), arg);

        public static void Log(this CecilatorObject cecilatorObject, LogTypes logTypes, Property property, object arg) => cecilatorObject.Log(logTypes, property.Getter ?? property.Setter, arg);

        public static void Log(this CecilatorObject cecilatorObject, LogTypes logTypes, Method method, object arg)
        {
            //if (method.IsAsync)
            //{
            //    var result = cecilatorObject.GetAsyncMethod(method.methodDefinition);
            //    cecilatorObject.Log(logTypes, result.Value.MethodDefinition.GetSequencePoint(), arg);
            //}
            //else
            cecilatorObject.Log(logTypes, method.methodDefinition.GetSequencePoint(), arg);
        }

        public static void Log(this CecilatorObject cecilatorObject, LogTypes logTypes, object arg) => cecilatorObject.Log(logTypes, sequencePoint: null, arg: arg);

        public static GenericInstanceType ResolveGenericArguments(this GenericInstanceType self, GenericInstanceType inheritingOrImplementingType)
        {
            if (self.FullName == inheritingOrImplementingType.FullName)
                return self;

            var genericParameters = inheritingOrImplementingType.GetGenericResolvedTypeName();
            var genericArguments = new TypeReference[self.GenericArguments.Count];

            for (int i = 0; i < genericArguments.Length; i++)
                genericArguments[i] = genericParameters.ContainsKey(self.GenericArguments[i].FullName) ? genericParameters[self.GenericArguments[i].FullName] : self.GenericArguments[i];

            return self.BetterResolve().MakeGenericInstanceType(genericArguments);
        }

        public static BuilderType ToBuilderType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() != type)
            {
                var builder = Builder.Current;
                var definition = type.GetGenericTypeDefinition();
                var typeDefinition = WeaverBase.AllTypes.Get(definition.FullName);
                var typeReference = typeDefinition.MakeGenericInstanceType(type.GetGenericArguments().Select(x => x.ToBuilderType().typeReference).ToArray());

                return new BuilderType(Builder.Current, typeReference);
            }

            return new BuilderType(Builder.Current, WeaverBase.AllTypes.Get(type.FullName));
        }

        public static BuilderType ToBuilderType(this TypeDefinition value) => new BuilderType(Builder.Current, value);

        public static BuilderType ToBuilderType(this TypeReference value) => new BuilderType(Builder.Current, value);

        public static Type ToType(this TypeReference typeReference) => Type.GetType(typeReference.FullName + ", " + typeReference.Module.Assembly.FullName);

        public static TNew With<TType, TNew>(this TType target, Func<TType, TNew> predicate) => predicate(target);

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

        internal static MethodReference CreateMethodReference(this MethodDefinition method, TypeReference originType)
        {
            if (method.DeclaringType.HasGenericParameters)
            {
                var declaringType = new GenericInstanceType(method.DeclaringType);
                return method.MakeHostInstanceGeneric(originType.GenericParameters.ToArray());
            }

            return method;
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

        internal static Method GetAsyncMethod(this Builder builder, MethodDefinition method)
        {
            var result = (builder as CecilatorObject).GetAsyncMethod(method);

            if (result.HasValue)
                return new Method(new BuilderType(builder, result.Value.AsyncType), result.Value.MethodDefinition);

            return null;
        }

        /// <summary>
        /// Gets a specified length of bytes
        /// </summary>
        /// <param name="target">The Array that contains the data to copy.</param>
        /// <param name="startingPosition">
        /// A 32-bit integer that represents the index in the sourceArray at which copying begins.
        /// </param>
        /// <param name="length">A 32-bit integer that represents the number of elements to copy.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Parameter <paramref name="startingPosition"/> and <paramref name="length"/> are out of range
        /// </exception>
        internal static byte[] GetBytes(this byte[] target, int startingPosition, int length)
        {
            if (length + startingPosition > target.Length)
                throw new ArgumentOutOfRangeException("length", "Parameter startingPosition and length are out of range");

            byte[] value = new byte[length];

            Array.Copy(target, startingPosition, value, 0, length);
            return value;
        }

        internal static IEnumerable<Instruction> GetJumpSources(this IEnumerable<Instruction> body, Instruction jumpTarget)
        {
            foreach (var item in body)
            {
                if (item.Operand is Instruction target && target.Offset == jumpTarget.Offset)
                    yield return item;
            }
        }

        internal static ParameterReference GetParameter(this Method method, Instruction instruction)
            => method.methodReference.GetParameter(instruction);

        internal static ParameterReference GetParameter(this MethodReference methodReference, Instruction instruction)
        {
            var isStatic = methodReference.Resolve().IsStatic;

            if (instruction.OpCode == OpCodes.Ldarg_0)
                return isStatic ? methodReference.Parameters[0] : null;
            else if (instruction.OpCode == OpCodes.Ldarg_1)
                return methodReference.Parameters[isStatic ? 1 : 0];
            else if (instruction.OpCode == OpCodes.Ldarg_2)
                return methodReference.Parameters[isStatic ? 2 : 1];
            else if (instruction.OpCode == OpCodes.Ldarg_3)
                return methodReference.Parameters[isStatic ? 3 : 2];
            else
                return methodReference.Parameters[(int)instruction.Operand];
        }

        internal static string GetStackTrace(this Exception e)
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

        internal static VariableReference GetVariable(this Method method, Instruction instruction)
            => method.methodDefinition.GetVariable(instruction);

        internal static VariableReference GetVariable(this MethodDefinition methodDefinition, Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Ldloc_0)
                return methodDefinition.Body.Variables[0];
            else if (instruction.OpCode == OpCodes.Ldloc_1)
                return methodDefinition.Body.Variables[1];
            else if (instruction.OpCode == OpCodes.Ldloc_2)
                return methodDefinition.Body.Variables[2];
            else if (instruction.OpCode == OpCodes.Ldloc_3)
                return methodDefinition.Body.Variables[3];
            else
                return instruction.Operand as VariableReference;
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

        internal static void InsertAfter(this ILProcessor processor, int index, IEnumerable<Instruction> instructions)
        {
            var last = processor.Body.Instructions[index];

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

        internal static void InsertBefore(this ILProcessor processor, int index, IEnumerable<Instruction> instructions)
        {
            var target = processor.Body.Instructions[index];

            foreach (var instruction in instructions)
                processor.InsertBefore(target, instruction);
        }

        internal static bool IsCall(this Instruction instruction)
        {
            var opCode = instruction.OpCode;
            return
                opCode == OpCodes.Call ||
                opCode == OpCodes.Calli ||
                opCode == OpCodes.Callvirt;
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

        internal static bool IsComparer(this Instruction instruction)
        {
            var opCode = instruction.OpCode;
            return
                opCode == OpCodes.Ceq ||
                opCode == OpCodes.Cgt ||
                opCode == OpCodes.Cgt_Un ||
                opCode == OpCodes.Clt_Un ||
                opCode == OpCodes.Clt;
        }

        internal static bool IsDelegate(this TypeDefinition typeDefinition)
        {
            if (typeDefinition == null || typeDefinition.BaseType == null)
                return false;

            return typeDefinition.BaseType.FullName == "System.MulticastDelegate";
        }

        internal static bool IsDelegate(this TypeReference typeReference) => typeReference.BetterResolve().IsDelegate();

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

        internal static bool IsPrivate(this PropertyDefinition property)
        {
            var getter = property.GetMethod?.Attributes.HasFlag(MethodAttributes.Private) ?? true;
            var setter = property.SetMethod?.Attributes.HasFlag(MethodAttributes.Private) ?? true;

            return getter | setter;
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

        internal static bool IsValueOpCode(this Instruction instruction)
        {
            var opCode = instruction.OpCode;
            return
                IsCallOrNew(instruction) ||
                IsLoadArgument(instruction) ||
                IsLoadLocal(instruction) ||
                IsLoadField(instruction) ||
                IsComparer(instruction) ||
                opCode == OpCodes.Isinst ||
                opCode == OpCodes.Dup ||
                opCode == OpCodes.Sizeof ||
                opCode == OpCodes.Ceq ||
                opCode == OpCodes.Xor ||
                opCode == OpCodes.Shl ||
                opCode == OpCodes.Shr ||
                opCode == OpCodes.Shr_Un ||
                opCode == OpCodes.Neg ||
                opCode == OpCodes.Or ||
                opCode == OpCodes.And ||
                opCode == OpCodes.Mul ||
                opCode == OpCodes.Mul_Ovf_Un ||
                opCode == OpCodes.Mul_Ovf ||
                opCode == OpCodes.Add ||
                opCode == OpCodes.Add_Ovf ||
                opCode == OpCodes.Add_Ovf_Un ||
                opCode == OpCodes.Sub ||
                opCode == OpCodes.Sub_Ovf ||
                opCode == OpCodes.Sub_Ovf_Un ||
                opCode == OpCodes.Castclass ||
                opCode == OpCodes.Conv_I ||
                opCode == OpCodes.Conv_I1 ||
                opCode == OpCodes.Conv_I2 ||
                opCode == OpCodes.Conv_I4 ||
                opCode == OpCodes.Conv_I8 ||
                opCode == OpCodes.Conv_Ovf_I4 ||
                opCode == OpCodes.Conv_Ovf_I ||
                opCode == OpCodes.Conv_Ovf_I1 ||
                opCode == OpCodes.Conv_Ovf_I4_Un ||
                opCode == OpCodes.Ldc_I4 ||
                opCode == OpCodes.Ldc_I4_0 ||
                opCode == OpCodes.Ldc_I4_1 ||
                opCode == OpCodes.Ldc_I4_2 ||
                opCode == OpCodes.Ldc_I4_3 ||
                opCode == OpCodes.Ldc_I4_4 ||
                opCode == OpCodes.Ldc_I4_5 ||
                opCode == OpCodes.Ldc_I4_6 ||
                opCode == OpCodes.Ldc_I4_7 ||
                opCode == OpCodes.Ldc_I4_8 ||
                opCode == OpCodes.Ldc_I4_S ||
                opCode == OpCodes.Ldc_I4_M1 ||
                opCode == OpCodes.Ldc_I8 ||
                opCode == OpCodes.Ldc_R4 ||
                opCode == OpCodes.Ldc_R8 ||
                opCode == OpCodes.Unbox_Any ||
                opCode == OpCodes.Unbox ||
                opCode == OpCodes.Box;
        }

        internal static MethodReference MakeGeneric(this MethodReference method, TypeReference returnType, params TypeReference[] args)
        {
            if (args.Length == 0)
                return method;

            if (method.GenericParameters.Count != args.Length)
                throw new ArgumentException("Invalid number of generic type arguments supplied");

            var genericTypeRef = new GenericInstanceMethod(method);

            foreach (var arg in args)
                genericTypeRef.GenericArguments.Add(arg);

            if (returnType != null)
                genericTypeRef.ReturnType = returnType;

            return genericTypeRef;
        }

        internal static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            // https://groups.google.com/forum/#!topic/mono-cecil/mCat5UuR47I by ShdNx

            var genericDeclaringType = self.DeclaringType.MakeGenericInstanceType(arguments);
            var reference = new MethodReference(self.Name, self.ReturnType, genericDeclaringType)
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

                foreach (var child in children(current))
                    stack.Push(child);

                yield return current;
            }
        }

        internal static MethodReference ResolveMethod(this MethodReference method, TypeReference declaringType)
        {
            if (method.ContainsGenericParameter && declaringType is GenericInstanceType)
            {
                var declaringTypeInstance = declaringType as GenericInstanceType;
                var genericParameters = declaringTypeInstance.GetGenericResolvedTypeName();

                var genericArguments = new TypeReference[declaringTypeInstance.GenericArguments.Count];

                for (int i = 0; i < genericArguments.Length; i++)
                    genericArguments[i] = genericParameters.ContainsKey(declaringTypeInstance.GenericArguments[i].FullName) ? genericParameters[declaringTypeInstance.GenericArguments[i].FullName] : declaringTypeInstance.GenericArguments[i];

                //if (method.ReturnType.FullName != "System.Void" && !genericParameters.ContainsKey(method.ReturnType.FullName))
                //    return method; // TODO - go from highest implemented interface / base class generic to lowest and parse thier names ... TItem in KeyedCollection<TKey, TItem> -> T in IList<T>

                return method.MakeHostInstanceGeneric(genericArguments);
            }
            else if (declaringType.HasGenericParameters)
                return method.Resolve().CreateMethodReference();
            else
                return method;
        }

        internal static TypeReference ResolveType(this TypeReference type, MethodReference ownerMethod = null)
        {
            //try
            //{
            TypeReference resolveType(IReadOnlyDictionary<string, TypeReference> genericParameters, TypeReference ptype)
            {
                var genericType = genericParameters[ptype.FullName];

                while (genericType.IsGenericParameter)
                {
                    if (!genericParameters.TryGetValue(genericType.FullName, out genericType))
                        break;

                    if (!genericType.IsGenericParameter)
                        return genericType;
                }

                return genericParameters[ptype.FullName];
            }

            if (type.IsGenericInstance && ownerMethod is GenericInstanceMethod)
            {
                var genericParameters = (ownerMethod as GenericInstanceMethod).GetGenericResolvedTypeNames();
                var genericTypeInstance = type as GenericInstanceType;

                var result = new GenericInstanceType(genericTypeInstance.BetterResolve());

                foreach (var item in genericTypeInstance.GenericArguments)
                    result.GenericArguments.Add(resolveType(genericParameters, item));

                return result;
            }
            else if (ownerMethod is GenericInstanceMethod)
            {
                var genericParameters = (ownerMethod as GenericInstanceMethod).GetGenericResolvedTypeNames();
                return resolveType(genericParameters, type);
            }
            else
                return type;
            ////}
            ////catch (Exception e)
            ////{
            ////    throw new TypeResolveException($"Unable to resolve type '{type?.FullName ?? "Unknown"}'", e);
            ////}
        }

        internal static TypeReference ResolveType(this TypeReference type, TypeReference inheritingOrImplementingType, MethodReference ownerMethod = null)
        {
            if (type.IsGenericParameter && inheritingOrImplementingType is GenericInstanceType)
            {
                var genericParameters = (inheritingOrImplementingType as GenericInstanceType).GetGenericResolvedTypeName();

                if (genericParameters.ContainsKey(type.FullName))
                    return genericParameters[type.FullName];

                genericParameters = (inheritingOrImplementingType as GenericInstanceType).GetGenericResolvedTypeNames();
                var genericType = genericParameters[type.FullName];

                while (genericType.IsGenericParameter)
                {
                    genericType = genericParameters[genericType.FullName];

                    if (!genericType.IsGenericParameter)
                        return genericType;
                }

                return genericParameters[type.FullName];
            }
            else if (type.HasGenericParameters && inheritingOrImplementingType is GenericInstanceType)
            {
                var genericInstanceType = type as GenericInstanceType ?? type.MakeGenericInstanceType(type.GenericParameters.ToArray());
                return genericInstanceType.ResolveGenericArguments(inheritingOrImplementingType as GenericInstanceType);
            }
            else if (type.ContainsGenericParameter && inheritingOrImplementingType is GenericInstanceType)
            {
                if (!type.IsGenericInstance)
                    return type;

                var genericInstanceType = type as GenericInstanceType;
                return genericInstanceType.ResolveGenericArguments(inheritingOrImplementingType as GenericInstanceType);
            }
            else if (type.HasGenericParameters)
                return type.MakeGenericInstanceType(type.GenericParameters.ToArray());
            else if (ownerMethod != null)
                return type.ResolveType(ownerMethod);
            else
                return type;
        }

        private static (MethodDefinition MethodDefinition, TypeReference AsyncType)? GetAsyncMethod(this CecilatorObject cecilatorObject, MethodDefinition method)
        {
            var asyncStateMachine = method.CustomAttributes.Get("System.Runtime.CompilerServices.AsyncStateMachineAttribute");

            if (asyncStateMachine != null)
            {
                var asyncType = asyncStateMachine.ConstructorArguments[0].Value as TypeReference;
                var asyncTypeMethod = asyncType.Resolve().Methods.Get("MoveNext");

                if (asyncTypeMethod == null)
                {
                    cecilatorObject.Log(LogTypes.Error, method, "Unable to find the method MoveNext of async method " + method.Name);
                    throw new Exception("Unable to find the method MoveNext of async method " + method.Name);
                }

                return (asyncTypeMethod, asyncType);
            }

            return null;
        }

        private static IEnumerable<TypeReference> GetGenericInstances(this GenericInstanceType type)
        {
            var result = new List<TypeReference>
            {
                type
            };

            var resolved = type.BetterResolve();
            var genericArgumentsNames = resolved.GenericParameters.Select(x => x.FullName).ToArray();
            var genericArguments = type.GenericArguments.ToArray();

            if (resolved.BaseType != null)
                result.AddRange(resolved.BaseType.GetGenericInstances(genericArgumentsNames, genericArguments));

            if (resolved.Interfaces != null && resolved.Interfaces.Count > 0)
            {
                foreach (var item in resolved.Interfaces)
                    result.AddRange(item.InterfaceType.GetGenericInstances(genericArgumentsNames, genericArguments));
            }

            return result;
        }

        private static IEnumerable<TypeReference> GetGenericInstances(this TypeReference type, string[] genericArgumentNames, TypeReference[] genericArgumentsOfCurrentType)
        {
            var resolvedBase = type.BetterResolve();

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

        private static IEnumerable<(MethodReference reference, MethodDefinition definition)> GetMethodReferences(this TypeReference type, bool byInterfaces)
        {
            var result = new List<(MethodReference reference, MethodDefinition definition)>();
            GetMethodReferences(type, result, byInterfaces);

            return result.Where(x =>
            {
                if (!x.reference.DeclaringType.IsGenericInstance)
                    return true;

                var genericArgument = (x.reference.DeclaringType as GenericInstanceType).GenericArguments;
                for (int i = 0; i < genericArgument.Count; i++)
                {
                    if (genericArgument[i].IsGenericParameter)
                        return false;
                }

                return true;
            });
        }

        private static void GetMethodReferences(TypeReference typeToInspect, List<(MethodReference, MethodDefinition)> result, bool byInterfaces)
        {
            if (typeToInspect == null)
                return;

            if (typeToInspect.IsGenericInstance)
            {
                if (typeToInspect is GenericInstanceType genericType)
                {
                    var instances = genericType.GetGenericInstances().Where(x => (!x.BetterResolve().IsInterface || byInterfaces));
                    foreach (var item in instances)
                    {
                        var methods = item
                            .BetterResolve()
                            .Methods
                            .Select(x => (
                                            x.MakeHostInstanceGeneric((item as GenericInstanceType).GenericArguments.ToArray()),
                                            x
                                          ));

                        if (methods.Any())
                            result.AddRange(methods);
                    }
                }
            }
            else if (typeToInspect.IsGenericParameter)
            {
                // Do nothing
            }
            else
            {
                var methods = typeToInspect
                    .BetterResolve()
                    .Methods
                    .Select(x => (x.CreateMethodReference(), x));
                if (methods.Any())
                    result.AddRange(methods);
            }

            var typeReference = typeToInspect.BetterResolve();

            if (typeToInspect.IsGenericParameter || typeReference == null)
                return;

            if (byInterfaces)
            {
                for (int i = 0; i < typeReference.Interfaces.Count; i++)
                    GetMethodReferences(typeReference.Interfaces[i].InterfaceType, result, true);
            }
            else if (typeReference.BaseType != null)
                GetMethodReferences(typeReference.BaseType, result, false);
        }
    }
}