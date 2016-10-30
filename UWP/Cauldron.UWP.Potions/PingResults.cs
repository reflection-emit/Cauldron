namespace Cauldron.Potions
{
    /// <summary>
    /// Represents ping result information
    /// </summary>
    public struct PingResults
    {
        public string localIpAddress;
        public int localPort;
        public string remoteIpAddress;
        public int remotePort;
        public uint roundTripTimeMax;
        public uint roundTripTimeMin;
    }
}