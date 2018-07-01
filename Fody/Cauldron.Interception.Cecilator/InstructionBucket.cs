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
                        .SelectMany(x => GetTypeReference(x))
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

                    return value.AreEqual(methodDefinition);
                });
        }

        public static bool IsUsed(TypeReference typeReference)
        {
            var declaringType = typeReference.BetterResolve();

            return
                Types
                    .Any(x =>
                    {
                        if (x.declaringType.AreEqual(typeReference))
                            return false;

                        if (x.typeReference.AreEqual(typeReference))
                        {
                            if (declaringType.HasNestedTypes && x.declaringType.IsNested)
                                return false;

                            return true;
                        }

                        return false;
                    })
                    ||
                declaringType
                    .Methods
                    .SelectMany(methodDefinition => Bucket
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

                            if (x.method.DeclaringType.AreEqual(typeReference))
                                return false;

                            var value = (x.instruction.Operand as MethodReference)?.BetterResolve();

                            if (value == null)
                                return false;

                            return value.AreEqual(methodDefinition);
                        }))
                    .Any();
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

        private static IEnumerable<TypeDeclaringType> GetTypeReference(TypeReference typeReference)
        {
            var typeDefinition = typeReference.BetterResolve() ?? typeReference as TypeDefinition;
            yield return new TypeDeclaringType(typeReference, typeReference);

            foreach (var item in typeDefinition.CustomAttributes)
                yield return new TypeDeclaringType(typeReference, item.AttributeType);

            foreach (var field in typeDefinition.Fields)
            {
                foreach (var item in field.CustomAttributes)
                    yield return new TypeDeclaringType(typeReference, item.AttributeType);

                yield return new TypeDeclaringType(typeReference, field.FieldType);
            }

            foreach (var property in typeDefinition.Properties)
            {
                foreach (var item in property.CustomAttributes)
                    yield return new TypeDeclaringType(typeReference, item.AttributeType);

                yield return new TypeDeclaringType(typeReference, property.PropertyType);
            }

            foreach (var method in typeDefinition.Methods)
            {
                foreach (var item in method.CustomAttributes)
                {
                    yield return new TypeDeclaringType(typeReference, item.AttributeType);

                    foreach (var @param in item.ConstructorArguments.Where(x => x.Value is TypeReference))
                        yield return new TypeDeclaringType(typeReference, param.Value as TypeReference);

                    foreach (var @param in item.Fields.Where(x => x.Argument.Value is TypeReference))
                        yield return new TypeDeclaringType(typeReference, param.Argument.Value as TypeReference);

                    foreach (var @param in item.Properties.Where(x => x.Argument.Value is TypeReference))
                        yield return new TypeDeclaringType(typeReference, param.Argument.Value as TypeReference);
                }

                if (method.ReturnType.FullName != "System.Void")
                    yield return new TypeDeclaringType(typeReference, method.ReturnType);

                foreach (var item in method.Parameters)
                    yield return new TypeDeclaringType(typeReference, item.ParameterType);

                foreach (var item in method.GenericParameters)
                    yield return new TypeDeclaringType(typeReference, item);
            }

            foreach (var @event in typeDefinition.Events)
            {
                foreach (var item in @event.CustomAttributes)
                    yield return new TypeDeclaringType(typeReference, item.AttributeType);

                yield return new TypeDeclaringType(typeReference, @event.EventType);
            }
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

            public override string ToString() => $"{this.declaringType} - {this.typeReference}";
        }
    }
}