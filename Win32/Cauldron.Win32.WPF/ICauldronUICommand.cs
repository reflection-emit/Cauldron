namespace Cauldron.XAML
{
    /// <summary>
    /// Represents a UI command.
    /// </summary>
    public interface ICauldronUICommand
    {
        /// <summary>
        /// The label for command
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        void Invoke();
    }
}