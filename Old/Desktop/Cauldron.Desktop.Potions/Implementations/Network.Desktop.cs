namespace Cauldron.Potions.Implementation
{
    /// <summary>
    /// A wrapper for <see cref="Cauldron.Core.Network"/>. See <see cref="Cauldron.Core.Network"/> for further details.
    /// </summary>
    public sealed partial class Network : INetwork
    {
        /// <summary>
        /// Get a value that indicates whether any network connection is available.
        /// <para/>
        /// Returns true if a network connection is available, othwise false
        /// </summary>
        public bool IsNetworkAvailable
        {
            get
            {
                return Core.Network.IsNetworkAvailable;
            }
        }
    }
}