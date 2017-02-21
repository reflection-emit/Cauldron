using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class MarkerInstructionSet : InstructionsSet, ITry, ICatch, ICatchCode, IFinally, ITryCode
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<InstructionMarker> markers = new List<InstructionMarker>();

        internal MarkerInstructionSet(InstructionsSet instructionsSet, MarkerType markerType, Instruction mark, IEnumerable<Instruction> instructions) : base(instructionsSet, instructions)
        {
            this.markers.Add(new InstructionMarker { instruction = mark, markerType = markerType });
        }

        internal MarkerInstructionSet(MarkerInstructionSet instructionsSet, MarkerType markerType, Instruction mark, TypeReference exceptionType) : base(instructionsSet, instructionsSet.instructions)
        {
            this.markers.AddRange(instructionsSet.markers);
            this.markers.Add(new InstructionMarker { instruction = mark, markerType = markerType, exceptionType = exceptionType });
        }

        internal MarkerInstructionSet(InstructionsSet instructionsSet, IEnumerable<Instruction> instructions) : base(instructionsSet, instructions)
        {
        }

        public ICatch Catch(Type exceptionType, Func<ICatchCode, ICode> body) => this.Catch(this.moduleDefinition.Import(GetTypeDefinition(exceptionType)), body);

        public ICatch Catch(BuilderType exceptionType, Func<ICatchCode, ICode> body) => this.Catch(exceptionType.typeReference, body);

        public ICode EndTry()
        {
            if (!this.instructions.Any())
                return new InstructionsSet(this, this.instructions);

            var tryInstructionStart = this.markers[0].instruction ?? this.instructions.First();

            for (int i = 1; i < this.markers.Count; i++)
            {
                ExceptionHandlerType handlerType;
                if (this.markers[i].markerType == MarkerType.Catch)
                    handlerType = ExceptionHandlerType.Catch;
                else
                    handlerType = ExceptionHandlerType.Finally;

                var handlerStart = this.markers[i].instruction;
                var handlerEnd = i + 1 == this.markers.Count ? null : this.markers[i + 1].instruction;

                if (i + 1 == this.markers.Count)
                {
                    this.instructions.Add(this.processor.Create(OpCodes.Nop));
                    handlerEnd = this.instructions.Last();
                }

                var handler = new ExceptionHandler(handlerType)
                {
                    TryStart = tryInstructionStart,
                    TryEnd = this.markers[i].instruction,
                    HandlerStart = handlerStart,
                    HandlerEnd = handlerEnd,
                    CatchType = handlerType == ExceptionHandlerType.Finally ? null : this.moduleDefinition.Import(this.markers[i].exceptionType)
                };

                this.processor.Body.ExceptionHandlers.Add(handler);
            }

            return new InstructionsSet(this, this.instructions);
        }

        public IFinally Finally(Func<ICatchCode, ICode> body)
        {
            var block = new List<Instruction>();
            int index = this.instructions.Count == 0 ? 0 : this.instructions.Count - 1;
            var iset = body(new MarkerInstructionSet(this, block)) as InstructionsSet;

            if (iset != null)
                this.instructions.AddRange(iset.instructions);

            this.instructions.Add(this.processor.Create(OpCodes.Endfinally));

            if (index > 0 && index < this.instructions.Count)
                index++;

            this.markers.Add(new InstructionMarker
            {
                instruction = this.instructions[index],
                markerType = MarkerType.Finally
            });

            return this;
        }

        public ICode OriginalBody()
        {
            this.instructions.AddRange(this.processor.Body.Instructions);
            return new InstructionsSet(this, this.instructions);
        }

        public ICode Rethrow()
        {
            this.instructions.Add(this.processor.Create(OpCodes.Rethrow));
            return new InstructionsSet(this, this.instructions);
        }

        private ICatch Catch(TypeReference exceptionType, Func<ICatchCode, ICode> body)
        {
            var block = new List<Instruction>();
            int index = this.instructions.Count == 0 ? 0 : this.instructions.Count - 1;

            var iset = body(new MarkerInstructionSet(this, block)) as InstructionsSet;
            if (iset != null)
                this.instructions.AddRange(iset.instructions);

            if (this.RequiresReturn)
                this.instructions.Add(this.processor.Create(OpCodes.Ret));

            if (index > 0 && index < this.instructions.Count)
                index++;

            this.markers.Add(new InstructionMarker
            {
                exceptionType = this.moduleDefinition.Import(exceptionType),
                instruction = this.instructions[index],
                markerType = MarkerType.Catch
            });

            return this;
        }
    }

    internal class InstructionMarker
    {
        public TypeReference exceptionType;
        public Instruction instruction;
        public MarkerType markerType;
    }
}