using Cauldron.Core;
using System.Threading.Tasks;

namespace Cauldron.Potions
{
    /// <summary>
    /// Provides properties and methods useful for gathering information about the network
    /// </summary>
    public partial interface INetwork
    {
        /// <summary>
        /// Detect the current connection type
        /// </summary>
        /// <returns>
        /// 2 for 2G, 3 for 3G, 4 for 4G
        /// 100 for WiFi
        /// 0 for unknown or not connected</returns>
        ConnectionGenerationTypes ConnectionType { get; }

        /// <summary>
        /// Gets a value that indicates if internet connection is available
        /// </summary>
        bool HasInternetConnection { get; }

        /// <summary>
        /// Allows an application to determine whether a remote computer is accessible over the network.
        /// </summary>
        /// <param name="hostname">The hostname of the remote computer</param>
        /// <param name="port">The port to ping</param>
        /// <returns>An object that represents the ping results</returns>
        Task<PingResults> Ping(string hostname, uint port);
    }
}