namespace Cauldron.Interception
{
    /// <summary>
    /// Represents an interceptor that can intercept a property's getter and setter method.
    /// This interceptor can also be applied to abstract properties and all overriding methods will implement this interceptor.
    /// </summary>
    public interface IPropertyInterceptor : IPropertyGetterInterceptor, IPropertySetterInterceptor, IInterceptor
    {
    }
}