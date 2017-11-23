namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents a view model that reacts to certain events in the NavigationFrame (UWP) or Window (Desktop).
    /// </summary>
    public interface IFrameAware : ICloseAwareViewModel
    {
        /// <summary>
        /// Occures if the page or window is activated
        /// </summary>
        void Activated();

#if NETFX_CORE
        /// <summary>
        /// Occures if a page or window is about to close. If returns false, the closing will be cancelled.
        /// </summary>
        /// <returns>Should return true if page or window can be closed.</returns>
        bool CanClose();
#endif

        /// <summary>
        /// Occures if the page or window is deactivated
        /// </summary>
        void Deactivated();
    }
}