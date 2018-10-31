namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents a view model that is aware of the window sizing
    /// </summary>
    public interface ISizeAware : IViewModel
    {
        /// <summary>
        /// Occures if the window size has changed
        /// </summary>
        /// <param name="width">The new width of the window</param>
        /// <param name="height">Thw new height of the window</param>
        void SizeChanged(double width, double height);
    }
}