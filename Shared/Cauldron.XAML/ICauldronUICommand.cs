namespace Cauldron.XAML
{
    /// <summary>
    /// Represents a command in the MessageDialog and ContentDialog
    /// </summary>
    public interface ICauldronUICommand
    {
        /// <summary>
        /// Gets or sets the label of the command button
        /// </summary>
        string Label { get; }

#if WINDOWS_UWP

        /// <summary>
        /// Converts a <see cref="CauldronUICommand"/> to a <see cref="UICommand"/>
        /// </summary>
        /// <returns></returns>
        UICommand ToUICommand();
#else

        /// <summary>
        /// Executes the command
        /// </summary>
        void Invoke();

#endif
    }
}