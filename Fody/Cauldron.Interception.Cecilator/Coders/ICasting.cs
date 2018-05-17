namespace Cauldron.Interception.Cecilator.Coders
{
    public interface ICasting<TResult> : ICasting
    {
        TResult As(BuilderType type);
    }

    public interface ICasting
    {
        CoderBase As(BuilderType type);
    }
}