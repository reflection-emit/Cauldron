namespace Cauldron.Interception
{
    /// <summary>
    /// Represents an interceptor that can intercept a property's getter and setter method
    /// </summary>

    public interface IPropertyInterceptor : IPropertyGetterInterceptor, IPropertySetterInterceptor, IInterceptor
    {
    }
}