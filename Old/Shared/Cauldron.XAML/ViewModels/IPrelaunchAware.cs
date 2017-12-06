namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents a viewmodel that is aware of application prelaunch
    /// </summary>
    public interface IPrelaunchAware : IViewModel
    {
        /// <summary>
        /// Occures if the app becomes visible when it is prelaunched
        /// </summary>
        void AppIsVisible();
    }
}