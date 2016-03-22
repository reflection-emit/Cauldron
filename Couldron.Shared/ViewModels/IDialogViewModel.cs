namespace Couldron.ViewModels
{
    public interface IDialogViewModel<TResult> : IViewModel
    {
        TResult Result { get; set; }
    }
}