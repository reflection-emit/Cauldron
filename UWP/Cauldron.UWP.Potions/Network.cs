using System;
using System.Threading.Tasks;
using Cauldron.Core.Extensions;
using Cauldron.Activator;

#if WINDOWS_UWP

using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Networking;

#else

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endif

namespace Cauldron.Potions
{
    /// <summary>
    /// Provides properties and methods useful for gathering information about the network
    /// </summary>
    [Component(typeof(INetwork), FactoryCreationPolicy.Instanced)]
    public sealed partial class Network : FactoryObject<INetwork>, INetwork
    {
        [ComponentConstructor]
        private Network()
        {
        }

        /// <summary>
        /// Detect the current connection type
        /// </summary>
        /// <returns>
        /// 2 for 2G, 3 for 3G, 4 for 4G
        /// 100 for WiFi
        /// 0 for unknown or not connected</returns>
        public ConnectionGenerationTypes ConnectionType
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

                foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
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
        public bool HasInternetConnection
        {
            get
            {
                try
                {
#if WINDOWS_UWP

                    ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                    return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
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
        public async Task<PingResults> Ping(string hostname, uint port)
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
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}