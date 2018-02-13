using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator.Coders
{
    public abstract class TryCatchFinallyCoderBase : CoderBase
    {
        internal readonly List<InstructionMarker> markers;

        internal TryCatchFinallyCoderBase(InstructionBlock instructionBlock, InstructionMarker instructionMarker) : base(instructionBlock)
        {
            this.markers = new List<InstructionMarker>();

            if (instructionMarker != null)
                this.markers.Add(instructionMarker);
        }

        internal TryCatchFinallyCoderBase(TryCatchFinallyCoderBase tryCatchFinallyCoderBase, InstructionMarker instructionMarker) : base(tryCatchFinallyCoderBase.instructions)
        {
            this.markers = tryCatchFinallyCoderBase.markers;

            if (instructionMarker != null)
                this.markers.Add(instructionMarker);
        }

        public bool RequiresReturn =>
            this.instructions.Count > 0 &&
            this.instructions.Last.OpCode != OpCodes.Rethrow &&
            this.instructions.Last.OpCode != OpCodes.Throw &&
            this.instructions.Last.OpCode != OpCodes.Ret;

        protected CatchCoder CatchInternal(BuilderType exceptionType, Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code)
        {
            var result = new CatchCoder(this, exceptionType);

            this.instructions.Append(code(new CatchThrowerCoder(this.NewCoder()), () => result.Exception));

            if (this.RequiresReturn)
                this.instructions.Emit(OpCodes.Ret);

            return result;
        }

        protected Coder EndTryInternal()
        {
            if (this.instructions.Count == 0)
                return new Coder(this.instructions);

            this.markers.Add(new InstructionMarker
            {
                instruction = this.instructions.Last,
                markerType = MarkerType.EndTry
            });

            this.instructions.Emit_Nop();
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
                    CatchType = handlerType == ExceptionHandlerType.Finally ? null : this.markers[i].exceptionType.typeReference
                };

                this.instructions.exceptionHandlers.Add(handler);
            }

            return new Coder(this.instructions);
        }

        protected TryCatchEndCoder FinallyInternal(Func<Coder, Coder> code)
        {
            var result = new TryCatchEndCoder(this);

            this.instructions.Append(code(this.NewCoder()));

            if (this.RequiresReturn)
                this.instructions.Emit(OpCodes.Ret);

            return result;
        }
    }
}