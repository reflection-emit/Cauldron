namespace Cauldron.Potions
{
    /// <summary>
    /// Provides properties and methods useful for gathering information about the network
    /// </summary>
    public partial interface INetwork
    {
        /// <summary>
        /// Get a value that indicates whether any network connection is available.
        /// <para/>
        /// Returns true if a network connection is available, othwise false
        /// </summary>
        bool IsNetworkAvailable { get; }
    }
}