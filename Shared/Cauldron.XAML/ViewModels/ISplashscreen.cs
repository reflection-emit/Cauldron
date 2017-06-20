namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Indicates that the viewmodel belongs to a splashscreen.
    /// Splashscreen windows in desktop will lose their "MainWindowness" if another window is created.
    /// In UWP the Splashscreen will only reset the history of the pages.
    /// </summary>
    public interface ISplashscreen
    {
    }
}