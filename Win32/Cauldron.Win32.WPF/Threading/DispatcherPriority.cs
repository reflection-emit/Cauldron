namespace Cauldron.XAML.Threading
{
    /// <summary>
    /// Describes the priorities at which operations can be invoked
    /// </summary>
    public enum DispatcherPriority
    {
        /// <summary>
        /// Lowest priority. Use this priority for background tasks. Delegates are processed when the window's main thread is idle and there is no input pending in the queue.
        /// </summary>
        Idle = -2,

        /// <summary>
        /// Low priority. Delegates are processed when the window's main thread is idle and there is
        /// no input pending in the queue.
        /// </summary>
        Low = -1,

        /// <summary>
        /// Normal priority. Delegates are processed in the order they are scheduled.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// High priority. Delegates are invoked immediately for all synchronous requests.
        /// Asynchronous requests are queued and processed before any other request type.
        /// </summary>
        High = 1,
    }
}