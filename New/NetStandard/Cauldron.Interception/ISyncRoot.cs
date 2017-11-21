namespace Cauldron.Interception
{
    /// <summary>
    /// Represents an interceptor that can be locked.
    /// </summary>
    public interface ISyncRoot
    {
        /// <summary>
        /// Gets or sets the lock object of the interceptor. This is automatically set by Cauldron. Any value will be overriden.
        /// </summary>
        object SyncRoot { get; set; }
    }
}