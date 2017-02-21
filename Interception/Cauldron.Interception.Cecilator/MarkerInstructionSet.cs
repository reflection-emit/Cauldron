using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class MarkerInstructionSet : InstructionsSet, ITry, ICatch, ICatchCode, IFinally
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<InstructionMarker> markers = new List<InstructionMarker>();

        internal MarkerInstructionSet(InstructionsSet instructionsSet, MarkerType markerType, Instruction mark, InstructionContainer instructions) : base(instructionsSet, instructions)
        {
            this.markers.Add(new InstructionMarker { instruction = mark, markerType = markerType });
        }

        internal MarkerInstructionSet(MarkerInstructionSet instructionsSet, MarkerType markerType, Instruction mark, TypeReference exceptionType) : base(instructionsSet, instructionsSet.instructions)
        {
            this.markers.AddRange(instructionsSet.markers);
            this.markers.Add(new InstructionMarker { instruction = mark, markerType = markerType, exceptionType = exceptionType });
        }

        internal MarkerInstructionSet(InstructionsSet instructionsSet, InstructionContainer instructions) : base(instructionsSet, instructions)
        {
        }

        public ICatch Catch(Type exceptionType, Action<ICatchCode> body) => this.Catch(this.moduleDefinition.Import(GetTypeDefinition(exceptionType)), body);

        public ICatch Catch(BuilderType exceptionType, Action<ICatchCode> body) => this.Catch(exceptionType.typeReference, body);

        public ICode EndTry()
        {
            if (this.instructions.Count == 0)
                return new InstructionsSet(this, this.instructions);

            this.markers.Add(new InstructionMarker
            {
                instruction = this.instructions.Last(),
                markerType = MarkerType.EndTry
            });

            this.instructions.Append(this.processor.Create(OpCodes.Nop));
            var tryInstructionStart = this.instructions.Next(this.markers[0].instruction);

            for (int i = 1; i < this.markers.Count - 1; i++)
            {
                var handlerType = (ExceptionHandlerType)this.markers[i].markerType;
                var handlerStart = this.instructions.Next(this.markers[i].instruction);
                var handlerEnd = this.instructions.Next(this.markers[i + 1].instruction);

                var handler = new ExceptionHandler(handlerType)
                {
                    TryStart = tryInstructionStart,
                    TryEnd = this.instructions.Next(this.markers[i].instruction),
                    HandlerStart = handlerStart,
                    HandlerEnd = handlerEnd,
                    CatchType = handlerType == ExceptionHandlerType.Finally ? null : this.moduleDefinition.Import(this.markers[i].exceptionType)
                };

                this.processor.Body.ExceptionHandlers.Add(handler);
            }

            return new InstructionsSet(this, this.instructions);
        }

        public IFinally Finally(Action<ICatchCode> body)
        {
            var markerStart = this.instructions.Last();
            body(this);
            this.instructions.Append(this.processor.Create(OpCodes.Endfinally));

            this.markers.Add(new InstructionMarker
            {
                instruction = markerStart,
                markerType = MarkerType.Finally
            });

            return this;
        }

        public ICode Rethrow()
        {
            this.instructions.Append(this.processor.Create(OpCodes.Rethrow));
            return new InstructionsSet(this, this.instructions);
        }

        private ICatch Catch(TypeReference exceptionType, Action<ICatchCode> body)
        {
            var markerStart = this.instructions.Last();
            body(this);

            if (this.RequiresReturn)
                this.instructions.Append(this.processor.Create(OpCodes.Ret));

            this.markers.Add(new InstructionMarker
            {
                exceptionType = this.moduleDefinition.Import(exceptionType),
                instruction = markerStart,
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