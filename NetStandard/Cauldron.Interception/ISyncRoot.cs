namespace Cauldron.Interception
{
    /// <summary>
    /// Adds a sync-root to the interceptor. The sync-root object is the same for all interceptors applied to the property, field or method.
    /// <para/>
    /// This interceptor extension is available for <see cref="IPropertyGetterInterceptor"/>, <see cref="IPropertySetterInterceptor"/>,
    /// <see cref="IPropertyInterceptor"/> and <see cref="IMethodInterceptor"/>.
    /// </summary>
    public interface ISyncRoot
    {
        /// <summary>
        /// Gets or sets the lock object of the interceptor. This is automatically set by Cauldron. Any value will be overriden.
        /// </summary>
        object SyncRoot { get; set; }
    }
}