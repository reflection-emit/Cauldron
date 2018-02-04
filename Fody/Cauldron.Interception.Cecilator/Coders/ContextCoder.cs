namespace Cauldron.Interception.Cecilator.Coders
{
    public abstract class ContextCoder
    {
        internal readonly bool autoAddThisInstance = true;
        internal readonly Coder coder;

        internal ContextCoder(Coder coder, bool autoAddThisInstance)
        {
            this.coder = coder;
            this.autoAddThisInstance = autoAddThisInstance;
        }
    }
}