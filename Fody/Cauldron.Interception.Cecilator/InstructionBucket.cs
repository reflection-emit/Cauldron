using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    internal static class InstructionBucket
    {
        private static InstructionMethod[] instructions;
        private static TypeDeclaringType[] types;

        public static InstructionMethod[] Bucket
        {
            get
            {
                if (instructions == null)
                {
                    instructions = Builder.Current.GetTypesInternal(SearchContext.Module)
                        .AsParallel()
                        .SelectMany(x => x.BetterResolve().Methods)
                        .Where(x => x.HasBody)
                        .SelectMany(x => GetInstructions(x))
                        .ToArray();
                }

                return instructions;
            }
        }

        public static TypeDeclaringType[] Types
        {
            get
            {
                if (types == null)
                {
                    types = Builder.Current.GetTypesInternal(SearchContext.Module)
                        .AsParallel()
                        .SelectMany(x => x.BetterResolve().Methods)
                        .Where(x => x.HasBody)
                        .SelectMany(x => GetTypeReference(x))
                        .Concat(
                            Builder.Current.GetTypesInternal(SearchContext.Module)
                                .AsParallel()
                                .SelectMany(x => GetTypeReference(x)))
                        .ToArray();
                }

                return types;
            }
        }

        public static IEnumerable<InstructionMethod> Find(FieldDefinition fieldDefinition)
        {
            return Bucket
                .Where(x =>
                {
                    if (x == null)
                        return false;

                    if (fieldDefinition == null)
                        return false;

                    var instruction = x.instruction;

                    if ((instruction.OpCode == OpCodes.Ldsfld ||
                        instruction.OpCode == OpCodes.Ldflda ||
                        instruction.OpCode == OpCodes.Ldsflda ||
                        instruction.OpCode == OpCodes.Ldfld ||
                        instruction.OpCode == OpCodes.Stsfld ||
                        instruction.OpCode == OpCodes.Stfld) &&
                        (instruction.Operand as FieldDefinition ?? instruction.Operand as FieldReference).FullName == fieldDefinition.FullName)
                        return true;

                    return false;
                });
        }

        public static IEnumerable<InstructionMethod> Find(MethodDefinition methodDefinition)
        {
            return Bucket
                .Where(x =>
                {
                    if (x == null)
                        return false;

                    if (methodDefinition == null)
                        return false;

                    if (x.instruction.OpCode != OpCodes.Call && x.instruction.OpCode != OpCodes.Callvirt && x.instruction.OpCode == OpCodes.Newobj)
                        return false;

                    if (x.instruction.Operand == null)
                        return false;

                    var value = (x.instruction.Operand as MethodReference)?.BetterResolve();

                    if (value == null)
                        return false;

                    if (!value.DeclaringType.AreEqual(methodDefinition.DeclaringType))
                        return false;

                    if (value.Name != methodDefinition.Name)
                        return false;

                    if (value.Parameters.Count != methodDefinition.Parameters.Count)
                        return false;

                    if (!value.ReturnType.AreEqual(methodDefinition.ReturnType))
                        return false;

                    if (value.GenericParameters.Count != methodDefinition.GenericParameters.Count)
                        return false;

                    for (int i = 0; i < value.Parameters.Count; i++)
                        if (!value.Parameters[i].ParameterType.AreEqual(methodDefinition.Parameters[i].ParameterType))
                            return false;

                    for (int i = 0; i < value.GenericParameters.Count; i++)
                        if (!value.GenericParameters[i].AreEqual(methodDefinition.GenericParameters[i]))
                            return false;

                    return true;
                });
        }

        public static bool IsUsed(TypeReference typeReference)
        {
            var definition = typeReference.BetterResolve();

            var ctors = definition.Methods.Where(x => x.Name == ".ctor");
            var usages = definition.Methods.SelectMany(x => Find(x)).ToArray();

            var fieldUsage = definition.Fields.SelectMany(x => Find(x)).ToArray();

            return
                fieldUsage.Any() ||
                usages.Any() ||
                Bucket
                    .Any(x =>
                    {
                        if (typeReference == null)
                            return false;

                        if (x.instruction.Operand == null)
                            return false;

                        if (x.method.DeclaringType.AreEqual(typeReference))
                            return false;

                        var value = x.instruction.Operand as TypeReference;

                        if (value == null)
                            return false;

                        if (value.AreEqual(typeReference))
                            return true;

                        return false;
                    }) ||
                Types
                    .Any(x =>
                    {
                        if (x.declaringType.AreEqual(typeReference))
                            return false;

                        if (x.typeReference.AreEqual(typeReference))
                            return true;

                        return false;
                    }) ||
               ctors
                .Select(x => Find(x).Any())
                .Any(x => x);
        }

        public static void Reset()
        {
            instructions = null;
            types = null;
        }

        private static IEnumerable<InstructionMethod> GetInstructions(MethodDefinition method)
        {
            foreach (var item in method.Body.Instructions)
                yield return new InstructionMethod(method, item);
        }

        private static IEnumerable<TypeDeclaringType> GetTypeReference(MethodDefinition method)
        {
            foreach (var item in method.Body.Variables)
                yield return new TypeDeclaringType(method, item.VariableType);
        }

        private static IEnumerable<TypeDeclaringType> GetTypeReference(TypeReference typeReference)
        {
            var typeDefinition = typeReference.BetterResolve() ?? typeReference as TypeDefinition;

            foreach (var item in typeDefinition.CustomAttributes)
                yield return new TypeDeclaringType(typeReference, item.AttributeType);

            foreach (var field in typeDefinition.Fields)
                foreach (var item in field.CustomAttributes)
                    yield return new TypeDeclaringType(typeReference, item.AttributeType);

            foreach (var property in typeDefinition.Properties)
                foreach (var item in property.CustomAttributes)
                    yield return new TypeDeclaringType(typeReference, item.AttributeType);

            foreach (var method in typeDefinition.Methods)
                foreach (var item in method.CustomAttributes)
                    yield return new TypeDeclaringType(typeReference, item.AttributeType);

            foreach (var @event in typeDefinition.Events)
                foreach (var item in @event.CustomAttributes)
                    yield return new TypeDeclaringType(typeReference, item.AttributeType);
        }

        public sealed class InstructionMethod
        {
            internal readonly Instruction instruction;
            internal readonly MethodDefinition method;

            public InstructionMethod(MethodDefinition method, Instruction instruction)
            {
                this.method = method;
                this.instruction = instruction;
            }
        }

        public sealed class TypeDeclaringType
        {
            internal readonly TypeReference declaringType;
            internal readonly TypeReference typeReference;

            public TypeDeclaringType(TypeReference declaringType, TypeReference typeReference)
            {
                this.declaringType = declaringType;
                this.typeReference = typeReference;
            }

            public TypeDeclaringType(MethodDefinition method, TypeReference typeReference)
            {
                this.declaringType = method.DeclaringType;
                this.typeReference = typeReference;
            }
        }
    }
}