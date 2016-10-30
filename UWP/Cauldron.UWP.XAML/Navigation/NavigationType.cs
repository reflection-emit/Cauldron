namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Describes the type of navigation that occured
    /// </summary>
    public enum NavigationType
    {
        /// <summary>
        /// The navigation was invoked by the user
        /// </summary>
        User,

        /// <summary>
        /// The navigation was invoked by code
        /// </summary>
        Code,

        /// <summary>
        /// The device's back button was pressed
        /// </summary>
        BackButton
    }
}