namespace Cauldron.WindowsService
{
    /// <summary>
    /// Describes the recovery actions on service failure
    /// </summary>
    public enum RecoveryAction
    {
        /// <summary>
        /// None - No recovery action in place.
        /// </summary>
        None = 0,

        /// <summary>
        /// Restarts the service on failure.
        /// </summary>
        Restart = 1,

        /// <summary>
        /// Reboots the machine on failure.
        /// </summary>
        Reboot = 2,

        /// <summary>
        /// Runs a command on failure.
        /// </summary>
        RunCommand = 3
    }
}