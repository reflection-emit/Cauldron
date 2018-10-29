namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents a view model with a return value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value</typeparam>
    public interface IDialogViewModel<TResult> : IDialogViewModel
    {
        /// <summary>
        /// Gets or sets the result value
        /// </summary>
        TResult Result { get; set; }
    }

    /// <summary>
    /// Represents a view model with a return value.
    /// </summary>
    public interface IDialogViewModel : IViewModel
    {
        /// <summary>
        /// Gets the title of the dialog
        /// </summary>
        string Title { get; }
    }
}