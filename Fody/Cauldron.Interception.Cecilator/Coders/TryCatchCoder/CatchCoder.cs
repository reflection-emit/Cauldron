using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class CatchCoder : TryCatchFinallyCoderBase
    {
        internal readonly Instruction beforeCatchBody;
        private readonly BuilderType exceptionType;
        private readonly string identification;
        private bool hasExceptionVariable = false;

        internal CatchCoder(TryCatchFinallyCoderBase tryCatchFinallyCoderBase, BuilderType exceptionType) :
                            base(tryCatchFinallyCoderBase, new InstructionMarker
                            {
                                instruction = tryCatchFinallyCoderBase.instructions.Last,
                                markerType = MarkerType.Catch,
                                exceptionType = exceptionType.Import()
                            })
        {
            // save the exception object to a local variable if required
            // but... we will only do this if required... so we save the current position and
            // if the exception property is called, we insert the store local opcode here
            this.beforeCatchBody = tryCatchFinallyCoderBase.instructions.Last;
            this.identification = CodeBlocks.GenerateName();
            this.exceptionType = exceptionType;
        }

        internal ExceptionCodeBlock Exception
        {
            get
            {
                if (this.beforeCatchBody == null)
                    throw new InvalidOperationException("Exception property does not work outside of a catch");

                var exceptionVariableName = "<>exception_" + this.identification;

                if (!this.hasExceptionVariable)
                {
                    var exceptionVariable = this.instructions.associatedMethod.GetOrCreateVariable(this.exceptionType, exceptionVariableName);
                    this.hasExceptionVariable = true;
                    this.instructions.InsertAfter(this.beforeCatchBody, this.instructions.ilprocessor.Create(OpCodes.Stloc, exceptionVariable.variable));
                }

                return new ExceptionCodeBlock

                {
                    name = exceptionVariableName,
                    typeReference = this.exceptionType.typeReference
                };
            }
        }

        public CatchCoder Catch(TypeReference exceptionType, Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code) => this.CatchInternal(exceptionType.ToBuilderType(), code);

        public CatchCoder Catch(Type exceptionType, Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code) => this.CatchInternal(exceptionType.ToBuilderType(), code);

        public CatchCoder Catch(BuilderType exceptionType, Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code) => this.CatchInternal(exceptionType, code);

        public CatchCoder Catch(Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code) => this.CatchInternal(typeof(Exception).ToBuilderType(), code);

        public Coder EndTry() => base.EndTryInternal();

        public TryCatchEndCoder Finally(Func<Coder, Coder> code) => FinallyInternal(code);
    }

    public sealed class CatchThrowerCoder : Coder
    {
        internal CatchThrowerCoder(Method method) : base(method)
        {
        }

        internal CatchThrowerCoder(InstructionBlock instructionBlock) : base(instructionBlock)
        {
        }

        public Coder Rethrow()
        {
            this.instructions.Emit(OpCodes.Rethrow);
            return this;
        }
    }
}