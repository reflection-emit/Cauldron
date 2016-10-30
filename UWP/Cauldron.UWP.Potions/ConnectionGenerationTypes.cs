namespace Cauldron.Potions
{
    public enum ConnectionGenerationTypes
    {
        NotConnected = 0,
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