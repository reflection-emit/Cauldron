namespace Cauldron.Net
{
    /// <summary>
    /// Represents ping result information
    /// </summary>
    public struct PingResults
    {
        /// <summary>
        /// The local ip address
        /// </summary>
        public string localIpAddress;

        /// <summary>
        /// The local port
        /// </summary>
        public int localPort;

        /// <summary>
        /// The remote ip address
        /// </summary>
        public string remoteIpAddress;

        /// <summary>
        /// The remote port
        /// </summary>
        public int remotePort;

        /// <summary>
        /// The max round trip time
        /// </summary>
        public uint roundTripTimeMax;

        /// <summary>
        /// The min round trip time
        /// </summary>
        public uint roundTripTimeMin;
    }
}