namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed class TryCatchEndCoder : TryCatchFinallyCoderBase
    {
        internal TryCatchEndCoder(TryCatchFinallyCoderBase tryCatchFinallyCoderBase) : base(tryCatchFinallyCoderBase, null)
        {
        }

        public Coder EndTry() => base.EndTryInternal();
    }
}