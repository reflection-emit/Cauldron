namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Represents a class that handles the creation of a new Window (Desktop) or Page (UWP) and association of the viewmodel
    /// </summary>
    public partial interface INavigator
    {
        /// <summary>
        /// Closes the current focused <see cref="Window"/>.
        /// </summary>
        /// <returns>Returns true if successfully closed, otherwise false</returns>
        bool TryCloseFocusedWindow();
    }
}