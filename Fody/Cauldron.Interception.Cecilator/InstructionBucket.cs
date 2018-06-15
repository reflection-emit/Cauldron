using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    internal static class InstructionBucket
    {
        private static InstructionMethod[] instructions;

        public static InstructionMethod[] Bucket
        {
            get
            {
                if (instructions == null)
                {
                    instructions = Builder.Current.GetTypesInternal(SearchContext.Module)
                        .AsParallel()
                        .SelectMany(x => x.Resolve().Methods)
                        .Where(x => x.HasBody)
                        .SelectMany(x => GetInstructions(x))
                        .ToArray();
                }

                return instructions;
            }
        }

        public static IEnumerable<InstructionMethod> Find(MethodDefinition methodDefinition)
        {
            return Bucket
                .Where(x =>
                {
                    if (x == null)
                        return false;

                    if (x.instruction.OpCode != OpCodes.Call && x.instruction.OpCode != OpCodes.Callvirt && x.instruction.OpCode == OpCodes.Newobj)
                        return false;

                    if (x.instruction.Operand == null)
                        return false;

                    var value = (x.instruction.Operand as MethodReference)?.Resolve();

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

        public static void Reset() => instructions = null;

        private static IEnumerable<InstructionMethod> GetInstructions(MethodDefinition method)
        {
            foreach (var item in method.Body.Instructions)
                yield return new InstructionMethod(method, item);
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
    }
}