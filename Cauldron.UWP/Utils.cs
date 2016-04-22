using System.Linq;
using Windows.Networking.Connectivity;

namespace Cauldron
{
    /// <summary>
    /// Provides a collection of utility methods
    /// </summary>
    public static partial class Utils
    {
        /// <summary>
        /// Gets the NetBIOS name of this local computer.
        /// </summary>
        public static string ComputerName
        {
            get
            {
                var hostNames = NetworkInformation.GetHostNames();
                var localName = hostNames.FirstOrDefault(name => name.DisplayName.Contains(".local"));
                return localName.DisplayName.Replace(".local", "");
            }
        }

        /// <summary>
        /// Get a value that indicates whether any network connection is available.
        /// <para/>
        /// Returns true if a network connection is available, othwise false
        /// </summary>
        public static bool IsNetworkAvailable
        {
            get
            {
                ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                return connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }
        }
    }
}