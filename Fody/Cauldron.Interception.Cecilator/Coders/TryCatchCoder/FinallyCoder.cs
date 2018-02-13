using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator.Coders.TryCatchCoder
{
    public sealed class FinallyCoder : TryCatchFinallyCoderBase
    {
        internal FinallyCoder(TryCatchFinallyCoderBase tryCatchFinallyCoderBase, Instruction marker) :
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