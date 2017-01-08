namespace Cauldron.Core
{
    /// <summary>
    /// Representing a list of connection types
    /// </summary>
    public enum ConnectionGenerationTypes
    {
        /// <summary>
        /// Not Connected
        /// </summary>
        NotConnected = 0,

        /// <summary>
        /// Unknown connection
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// 2G
        /// </summary>
        _2G = 2,

        /// <summary>
        /// 3G
        /// </summary>
        _3G = 3,

        /// <summary>
        /// LTE 4G
        /// </summary>
        _4G = 4,

        /// <summary>
        /// WLAN
        /// </summary>
        WLAN = 5,

        /// <summary>
        /// LAN
        /// </summary>
        LAN = 6
    }
}