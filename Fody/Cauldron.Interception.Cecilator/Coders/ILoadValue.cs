namespace Cauldron.Interception.Cecilator.Coders
{
    public interface ILoadValue<T>
    {
        T Load(object value);
    }
}