using Cauldron;

namespace Cauldron.XAML
{
    /// <summary>
    /// Provides event information when an app is launched.
    /// </summary>
    public sealed class LaunchActivatedEventArgs
    {
        internal LaunchActivatedEventArgs(string[] args)
        {
            this.Arguments = args;
        }

        /// <summary>
        /// Gets the arguments that are passed to the app during its launch activation.
        /// </summary>
        public string[] Arguments { get; private set; }

        /// <summary>
        /// Gets the user that the app was activated for.
        /// </summary>
        public User User { get { return UserInformation.CurrentUser; } }
    }
}