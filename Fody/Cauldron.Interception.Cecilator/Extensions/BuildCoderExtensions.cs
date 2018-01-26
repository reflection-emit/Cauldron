using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public static class BuildCoderExtensions
    {
        /// <summary>
        /// Replaces the current methods body with the <see cref="Instruction"/>s in the <see cref="Coder"/>'s instruction set.
        /// </summary>
        /// <param name="coder"></param>
        public static void Replace(this Coder coder)
        {
            // Special case for .ctors
            if (coder.method.IsCtor && coder.method.methodDefinition.Body?.Instructions != null && coder.method.methodDefinition.Body.Instructions.Count > 0)
            {
                var first = coder.method.methodDefinition.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).Name == ".ctor");
                if (first == null)
                    throw new NullReferenceException($"The constructor of type '{coder.method.OriginType}' seems to have no call to base class.");

                // In ctors we only replace the instructions after base call
                var callsBeforeBase = coder.method.methodDefinition.Body.Instructions.TakeWhile(x => x != first).ToList();
                callsBeforeBase.Add(first);

                coder.method.methodDefinition.Body.Instructions.Clear();
                coder.method.methodDefinition.Body.ExceptionHandlers.Clear();

                coder.processor.Append(callsBeforeBase);
                coder.processor.Append(coder.instructions);
            }
            else
            {
                coder.method.methodDefinition.Body.Instructions.Clear();
                coder.method.methodDefinition.Body.ExceptionHandlers.Clear();

                coder.processor.Append(coder.instructions);
            }

            foreach (var item in coder.instructions.ExceptionHandlers)
                coder.processor.Body.ExceptionHandlers.Add(item);

            ReplaceReturns(coder);

            // TODO: Add a method that removes unused variables coder.CleanLocalVariableList();
            coder.method.methodDefinition.Body.InitLocals = coder.method.methodDefinition.Body.Variables.Count > 0;

            coder.method.methodDefinition.Body.OptimizeMacros();
            coder.instructions.Clear();
        }

        private static void ReplaceJumps(Method method, Instruction tobeReplaced, Instruction replacement)
        {
            for (var i = 0; i < method.methodDefinition.Body.Instructions.Count - 1; i++)
            {
                var instruction = method.methodDefinition.Body.Instructions[i];

                if (instruction.Operand == tobeReplaced)
                    instruction.Operand = replacement;
            }

            for (var i = 0; i < method.methodDefinition.Body.ExceptionHandlers.Count; i++)
            {
                var handler = method.methodDefinition.Body.ExceptionHandlers[i];

                if (handler.FilterStart == tobeReplaced)
                    handler.FilterStart = replacement;

                if (handler.HandlerEnd == tobeReplaced)
                    handler.HandlerEnd = replacement;

                if (handler.HandlerStart == tobeReplaced)
                    handler.HandlerStart = replacement;

                if (handler.TryEnd == tobeReplaced)
                    handler.TryEnd = replacement;

                if (handler.TryStart == tobeReplaced)
                    handler.TryStart = replacement;
            }
        }

        private static void ReplaceReturns(Coder coder)
        {
            if (coder.instructions.Count == 0)
                return;

            if (coder.method.IsAbstract)
                throw new NotSupportedException("Interceptors does not support abstract methods.");

            if (coder.method.IsVoid || coder.instructions.LastOrDefault().OpCode != OpCodes.Ret)
            {
                var realReturn = coder.method.methodDefinition.Body.Instructions.Last();

                for (var i = 0; i < coder.method.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = coder.method.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    instruction.OpCode = coder.method.IsInclosedInHandlers(instruction) ? OpCodes.Leave : OpCodes.Br;
                    instruction.Operand = realReturn;
                }
            }
            else
            {
                var realReturn = coder.method.methodDefinition.Body.Instructions.Last();
                var resultJump = false;

                if (!realReturn.Previous.IsValueOpCode() && realReturn.Previous.OpCode != OpCodes.Ldnull)
                {
                    resultJump = true;
                    //this.processor.InsertBefore(realReturn, this.processor.Create(OpCodes.Ldloc, returnVariable));
                    coder.processor.InsertBefore(realReturn, coder.AddParameter(coder.processor, coder.method.ReturnType.typeReference, coder.GetOrCreateReturnVariable()).Instructions);

                    realReturn = realReturn.Previous;
                }
                else if (realReturn.Previous.IsLoadField() || realReturn.Previous.IsLoadLocal() || realReturn.Previous.OpCode == OpCodes.Ldnull)
                {
                    realReturn = realReturn.Previous;

                    // Think twice before removing this ;)
                    if (realReturn.OpCode == OpCodes.Ldfld || realReturn.OpCode == OpCodes.Ldflda)
                        realReturn = realReturn.Previous;
                }
                else
                    realReturn = realReturn.Previous;

                for (var i = 0; i < coder.method.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = coder.method.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    if (coder.method.IsInclosedInHandlers(instruction))
                    {
                        instruction.OpCode = OpCodes.Leave;
                        instruction.Operand = realReturn;

                        if (coder.method.methodDefinition.ReturnType.FullName == "System.Void" || coder.method.methodDefinition.ReturnType.FullName == "System.Threading.Task")
                            continue;

                        if (resultJump)
                        {
                            var returnVariable = coder.GetOrCreateReturnVariable();
                            var previousInstruction = instruction.Previous;

                            if (previousInstruction != null && previousInstruction.IsLoadLocal())
                            {
                                if (
                                    (returnVariable.Index == 0 && previousInstruction.OpCode == OpCodes.Ldloc_0) ||
                                    (returnVariable.Index == 1 && previousInstruction.OpCode == OpCodes.Ldloc_1) ||
                                    (returnVariable.Index == 2 && previousInstruction.OpCode == OpCodes.Ldloc_2) ||
                                    (returnVariable.Index == 3 && previousInstruction.OpCode == OpCodes.Ldloc_3) ||
                                    (previousInstruction.OpCode == OpCodes.Ldloc_S && returnVariable.Index == (int)previousInstruction.Operand) ||
                                    (returnVariable == previousInstruction.Operand as VariableDefinition)
                                    )
                                {
                                    ReplaceJumps(coder.method, previousInstruction, instruction);

                                    // In this case also remove the redundant ldloc opcode
                                    i--;
                                    coder.method.methodDefinition.Body.Instructions.Remove(previousInstruction);
                                    continue;
                                }
                            }

                            if (previousInstruction != null && previousInstruction.IsStoreLocal())
                            {
                                if (
                                    (returnVariable.Index == 0 && previousInstruction.OpCode == OpCodes.Stloc_0) ||
                                    (returnVariable.Index == 1 && previousInstruction.OpCode == OpCodes.Stloc_1) ||
                                    (returnVariable.Index == 2 && previousInstruction.OpCode == OpCodes.Stloc_2) ||
                                    (returnVariable.Index == 3 && previousInstruction.OpCode == OpCodes.Stloc_3) ||
                                    (previousInstruction.OpCode == OpCodes.Stloc_S && returnVariable.Index == (int)previousInstruction.Operand) ||
                                    (returnVariable == previousInstruction.Operand as VariableDefinition)
                                    )
                                    continue; // Just continue and do not add an additional store opcode
                            }

                            coder.processor.InsertBefore(instruction, coder.processor.Create(OpCodes.Stloc, returnVariable));
                        }
                    }
                }
            }
        }
    }
}