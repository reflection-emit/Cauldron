using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public static class Extension
    {
        public static void Concat(this ICode a, ICode b)
        {
            if (a == null || b == null)
                return;

            (a as InstructionsSet).instructions.AddRange((b as InstructionsSet).instructions);
        }

        public static Builder CreateBuilder(this IWeaver weaver)
        {
            if (weaver == null)
                throw new ArgumentNullException(nameof(weaver), $"Argument '{nameof(weaver)}' cannot be null");

            return new Builder(weaver);
        }

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

        internal static void InsertAfter(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            var last = target;

            foreach (var instruction in instructions)
            {
                processor.InsertAfter(last, instruction);
                last = instruction;
            }
        }

        internal static void InsertBefore(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
                processor.InsertBefore(target, instruction);
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

        internal static TypeReference ResolveType(this TypeReference type, TypeReference inheritingOrImplementingType)
        {
            if (type.HasGenericParameters && inheritingOrImplementingType is GenericInstanceType)
            {
                var genericArgumentNames = inheritingOrImplementingType.GenericParameters.Select(x => x.FullName).ToArray();
                var genericArgumentsOfCurrentType = (inheritingOrImplementingType as GenericInstanceType).GenericArguments.ToArray();

                var genericInstanceType = type as GenericInstanceType;
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
            {
                var genericArgumentNames = type.GenericParameters.Select(x => x.FullName).ToArray();
                var genericArgumentsOfCurrentType = (type as GenericInstanceType).GenericArguments.ToArray();

                var genericInstanceType = type as GenericInstanceType;
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
            else
                return type;
        }
    }
}