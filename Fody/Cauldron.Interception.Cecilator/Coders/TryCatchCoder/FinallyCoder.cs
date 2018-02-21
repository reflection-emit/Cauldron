using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class FinallyCoder : TryCatchFinallyCoderBase
    {
        internal FinallyCoder(TryCatchFinallyCoderBase tryCatchFinallyCoderBase) :
            base(tryCatchFinallyCoderBase, new InstructionMarker
            {
                instruction = tryCatchFinallyCoderBase.instructions.Last,
                markerType = MarkerType.Finally
            })
        {
        }

        public Coder EndTry() => base.EndTryInternal();
    }
}