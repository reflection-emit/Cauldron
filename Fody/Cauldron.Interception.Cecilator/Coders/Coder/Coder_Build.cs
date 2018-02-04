using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;

namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class Coder
    {
        /// <summary>
        /// Replaces the current methods body with the <see cref="Instruction"/>s in the <see cref="Coder"/>'s instruction set.
        /// </summary>
        /// <param name="coder"></param>
        public void Replace()
        {
            // Special case for .ctors
            if (this.instructions.associatedMethod.IsCtor &&
                this.instructions.associatedMethod.methodDefinition.Body?.Instructions != null &&
                this.instructions.associatedMethod.methodDefinition.Body.Instructions.Count > 0)
            {
                var first = this.instructions.associatedMethod.methodDefinition.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).Name == ".ctor");
                if (first == null)
                    throw new NullReferenceException($"The constructor of type '{this.instructions.associatedMethod.OriginType}' seems to have no call to base class.");

                // In ctors we only replace the instructions after base call
                var callsBeforeBase = this.instructions.associatedMethod.methodDefinition.Body.Instructions.TakeWhile(x => x != first).ToList();
                callsBeforeBase.Add(first);

                this.instructions.associatedMethod.methodDefinition.Body.Instructions.Clear();
                this.instructions.associatedMethod.methodDefinition.Body.ExceptionHandlers.Clear();

                this.instructions.ilprocessor.Append(callsBeforeBase);
                this.instructions.ilprocessor.Append(this.instructions.instructions);
            }
            else
            {
                this.instructions.associatedMethod.methodDefinition.Body.Instructions.Clear();
                this.instructions.associatedMethod.methodDefinition.Body.ExceptionHandlers.Clear();

                this.instructions.ilprocessor.Append(this.instructions.instructions);
            }

            foreach (var item in this.instructions.exceptionHandlers)
                this.instructions.ilprocessor.Body.ExceptionHandlers.Add(item);

            ReplaceReturns(this);

            // TODO: Add a method that removes unused variables this.CleanLocalVariableList();
            this.instructions.associatedMethod.methodDefinition.Body.InitLocals = this.instructions.associatedMethod.methodDefinition.Body.Variables.Count > 0;

            this.instructions.associatedMethod.methodDefinition.Body.OptimizeMacros();
            this.instructions.Clear();
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

            if (coder.instructions.associatedMethod.IsAbstract)
                throw new NotSupportedException("Interceptors does not support abstract methods.");

            if (coder.instructions.associatedMethod.IsVoid || coder.instructions.instructions.LastOrDefault().OpCode != OpCodes.Ret)
            {
                var realReturn = coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Last();

                for (var i = 0; i < coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = coder.instructions.associatedMethod.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    instruction.OpCode = coder.instructions.associatedMethod.IsInclosedInHandlers(instruction) ? OpCodes.Leave : OpCodes.Br;
                    instruction.Operand = realReturn;
                }
            }
            else
            {
                var realReturn = coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Last();
                var resultJump = false;

                if (!realReturn.Previous.IsValueOpCode() && realReturn.Previous.OpCode != OpCodes.Ldnull)
                {
                    resultJump = true;
                    //this.processor.InsertBefore(realReturn, this.processor.Create(OpCodes.Ldloc, returnVariable));
                    coder.instructions.ilprocessor.InsertBefore(realReturn,
                        InstructionBlock.CreateCode(coder.instructions, coder.instructions.associatedMethod.ReturnType, coder.GetOrCreateReturnVariable())
                            .instructions);

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

                for (var i = 0; i < coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Count - 1; i++)
                {
                    var instruction = coder.instructions.associatedMethod.methodDefinition.Body.Instructions[i];

                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    if (coder.instructions.associatedMethod.IsInclosedInHandlers(instruction))
                    {
                        instruction.OpCode = OpCodes.Leave;
                        instruction.Operand = realReturn;

                        if (coder.instructions.associatedMethod.methodDefinition.ReturnType.FullName == "System.Void" ||
                            coder.instructions.associatedMethod.methodDefinition.ReturnType.FullName == "System.Threading.Task")
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
                                    ReplaceJumps(coder.instructions.associatedMethod, previousInstruction, instruction);

                                    // In this case also remove the redundant ldloc opcode
                                    i--;
                                    coder.instructions.associatedMethod.methodDefinition.Body.Instructions.Remove(previousInstruction);
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

                            coder.instructions.ilprocessor.InsertBefore(instruction, coder.instructions.ilprocessor.Create(OpCodes.Stloc, returnVariable));
                        }
                    }
                }
            }
        }
    }
}