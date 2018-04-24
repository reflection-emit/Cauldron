using Mono.Cecil;
using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class TryCoder : TryCatchFinallyCoderBase
    {
        internal TryCoder(InstructionBlock instructionBlock) :
            base(instructionBlock, new InstructionMarker
            {
                instruction = instructionBlock.Last ?? instructionBlock.First,
                markerType = MarkerType.Try
            })
        {
        }

        public CatchCoder Catch(TypeReference exceptionType, Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code) => this.CatchInternal(exceptionType.ToBuilderType(), code);

        public CatchCoder Catch(Type exceptionType, Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code) => this.CatchInternal(exceptionType.ToBuilderType(), code);

        public CatchCoder Catch(BuilderType exceptionType, Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code) => this.CatchInternal(exceptionType, code);

        public CatchCoder Catch(Func<CatchThrowerCoder, Func<ExceptionCodeBlock>, Coder> code) => this.CatchInternal(typeof(Exception).ToBuilderType(), code);

        public FinallyCoder Finally(Func<Coder, Coder> code) => base.FinallyInternal<FinallyCoder>(code, x => new FinallyCoder(x));
    }
}