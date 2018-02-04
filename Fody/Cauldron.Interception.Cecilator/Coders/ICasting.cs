namespace Cauldron.Interception.Cecilator.Coders
{
    public interface ICasting<TResult>
    {
        TResult As(BuilderType type);
    }
}