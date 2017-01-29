using System;
using System.Threading.Tasks;
using Cauldron.Core.Extensions;
using System.Diagnostics;

#if WINDOWS_UWP

using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Networking;

#elif NETCORE

using System.Net.Http;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Sockets;

#elif ANDROID

using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Sockets;

#else

using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Provides properties and methods useful for gathering information about the network
    /// </summary>
    public static partial class Network
    {
        /// <summary>
        /// Detect the current connection type
        /// </summary>
        /// <returns>
        /// 2 for 2G, 3 for 3G, 4 for 4G
        /// 100 for WiFi
        /// 0 for unknown or not connected</returns>
        public static ConnectionGenerationTypes ConnectionType
        {
            get
            {
#if WINDOWS_UWP
                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();

                if (profile.IsWwanConnectionProfile)
                {
                    var dataClass = profile.WwanConnectionProfileDetails.GetCurrentDataClass();

                    if (dataClass.HasFlag(WwanDataClass.Gprs) || dataClass.HasFlag(WwanDataClass.Edge))
                    {
                        return ConnectionGenerationTypes._2G;
                    }
                    else if (dataClass.HasFlag(WwanDataClass.Cdma1xEvdo) ||
                        dataClass.HasFlag(WwanDataClass.Cdma1xEvdoRevA) ||
                        dataClass.HasFlag(WwanDataClass.Cdma1xEvdoRevB) ||
                        dataClass.HasFlag(WwanDataClass.Cdma1xEvdv) ||
                        dataClass.HasFlag(WwanDataClass.Cdma1xRtt) ||
                        dataClass.HasFlag(WwanDataClass.Cdma3xRtt) ||
                        dataClass.HasFlag(WwanDataClass.CdmaUmb) ||
                        dataClass.HasFlag(WwanDataClass.Umts) ||
                        dataClass.HasFlag(WwanDataClass.Hsdpa) ||
                        dataClass.HasFlag(WwanDataClass.Hsupa))
                        return ConnectionGenerationTypes._3G;
                    else if (dataClass.HasFlag(WwanDataClass.LteAdvanced))
                        return ConnectionGenerationTypes._4G;
                    else if (dataClass.HasFlag(WwanDataClass.Custom))
                        return ConnectionGenerationTypes.Unknown;
                    else
                        return ConnectionGenerationTypes.NotConnected;
                }
                else if (profile.IsWlanConnectionProfile)
                    return ConnectionGenerationTypes.WLAN;
                else if (profile.NetworkAdapter.IanaInterfaceType == 6)
                    return ConnectionGenerationTypes.LAN;

                return ConnectionGenerationTypes.Unknown;

#else
                var result = ConnectionGenerationTypes.Unknown;

                foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                        {
                            if (result < ConnectionGenerationTypes.WLAN)
                                result = ConnectionGenerationTypes.WLAN;
                        }
                        else if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ppp)
                        {
                            if (result < ConnectionGenerationTypes._3G)
                                result = ConnectionGenerationTypes._3G;
                        }
                        else if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
                            result = ConnectionGenerationTypes.LAN;
                            break;
                        }
                    }
                }

                return result;
#endif
            }
        }

        /// <summary>
        /// Gets a value that indicates if internet connection is available
        /// </summary>
        public static bool HasInternetConnection
        {
            get
            {
                try
                {
#if WINDOWS_UWP

                    var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                    return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
#elif NETCORE

                    if (!IsNetworkAvailable)
                        return false;

                    using (var client = new HttpClient())
                    {
                        var result = client.GetAsync("http://www.microsoft.com").RunSync();
                        return result.IsSuccessStatusCode;
                    }
#else
                    if (!IsNetworkAvailable)
                        return false;

                    using (var client = new WebClient())
                    using (var stream = client.OpenRead("http://www.microsoft.com"))
                        return true;
#endif
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Allows an application to determine whether a remote computer is accessible over the network.
        /// </summary>
        /// <param name="hostname">The hostname of the remote computer</param>
        /// <param name="port">The port to ping</param>
        /// <returns>An object that represents the ping results</returns>
        public static async Task<PingResults> Ping(string hostname, uint port)
        {
            try
            {
#if WINDOWS_UWP
                using (var tcpClient = new StreamSocket())
                {
                    await tcpClient.ConnectAsync(
                        new HostName(hostname),
                        port.ToString(),
                        SocketProtectionLevel.PlainSocket);

                    return new PingResults
                    {
                        remoteIpAddress = tcpClient.Information.RemoteAddress.DisplayName,
                        remotePort = tcpClient.Information.RemotePort.ToInteger(),
                        localIpAddress = tcpClient.Information.LocalAddress.DisplayName,
                        localPort = tcpClient.Information.LocalPort.ToInteger(),
                        roundTripTimeMin = tcpClient.Information.RoundTripTimeStatistics.Min,
                        roundTripTimeMax = tcpClient.Information.RoundTripTimeStatistics.Max
                    };
                }
#else
                var times = new List<double>();
                var stopwatch = new Stopwatch();

                for (int i = 0; i < 4; i++)
                    using (var tcpClient = new TcpClient())
                    {
                        tcpClient.SendBufferSize = 32;
                        tcpClient.Client.Blocking = false;
                        stopwatch.Start();

                        await tcpClient.ConnectAsync(hostname, (int)port);
                        stopwatch.Stop();

                        times.Add(stopwatch.Elapsed.TotalMilliseconds);
                    }

                using (var tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(hostname, (int)port);
                    return new PingResults
                    {
                        remoteIpAddress = tcpClient.Client.RemoteEndPoint.As<IPEndPoint>().Address.ToString(),
                        remotePort = tcpClient.Client.RemoteEndPoint.As<IPEndPoint>().Port,
                        localIpAddress = tcpClient.Client.LocalEndPoint.As<IPEndPoint>().Address.ToString(),
                        localPort = tcpClient.Client.LocalEndPoint.As<IPEndPoint>().Port,
                        roundTripTimeMin = (uint)(times.Min()),
                        roundTripTimeMax = (uint)times.Max()
                    };
                }
#endif
            }
            catch
            {
                throw;
            }
        }
    }
}