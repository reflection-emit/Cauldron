using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class MarkerInstructionSet : InstructionsSet, ITry, ICatch, ICatchCode, IFinally
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<InstructionMarker> markers = new List<InstructionMarker>();

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Instruction beforeCatchBody;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TypeReference exceptionType;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool hasExceptionVariable;

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

        public Crumb Exception
        {
            get
            {
                if (this.beforeCatchBody == null)
                    throw new InvalidOperationException("Exception property does not work outside of a catch");

                var exceptionVariableName = "<>exception_" + this.Identification;

                if (!this.hasExceptionVariable)
                {
                    var exceptionVariable = this.CreateVariable(exceptionVariableName, new BuilderType(this.method.type, this.exceptionType));
                    this.hasExceptionVariable = true;
                    this.instructions.InsertAfter(this.beforeCatchBody, this.processor.Create(OpCodes.Stloc, exceptionVariable.variable));
                }

                return new Crumb
                {
                    CrumbType = CrumbTypes.Exception,
                    Name = exceptionVariableName,
                    ExceptionType = this.exceptionType
                };
            }
        }

        public ICatch Catch(Type exceptionType, Action<ICatchCode> body) => this.Catch(this.moduleDefinition.ImportReference(exceptionType.GetTypeDefinition()), body);

        public ICatch Catch(BuilderType exceptionType, Action<ICatchCode> body) => this.Catch(exceptionType.typeReference, body);

        public ICode EndTry()
        {
            if (this.instructions.Count == 0)
                return new InstructionsSet(this, this.instructions);

            this.markers.Add(new InstructionMarker
            {
                instruction = this.instructions.LastOrDefault(),
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
                    CatchType = handlerType == ExceptionHandlerType.Finally ? null : this.moduleDefinition.ImportReference(this.markers[i].exceptionType)
                };

                this.instructions.ExceptionHandlers.Add(handler);
            }

            return new InstructionsSet(this, this.instructions);
        }

        public IFinally Finally(Action<ICatchCode> body)
        {
            var markerStart = this.instructions.LastOrDefault();
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
            this.exceptionType = exceptionType;
            var markerStart = this.instructions.LastOrDefault();

            // save the exception object to a local variable if required
            // but... we will only do this if required... so we save the current position and
            // if the exception property is called, we insert the store local opcode here
            this.beforeCatchBody = markerStart;
            body(this);

            if (this.RequiresReturn)
                this.instructions.Append(this.processor.Create(OpCodes.Ret));

            this.markers.Add(new InstructionMarker
            {
                exceptionType = this.moduleDefinition.ImportReference(exceptionType),
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