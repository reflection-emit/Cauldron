namespace Cauldron.ViewModels
{
    public interface IDialogViewModel<TResult> : IViewModel
    {
        TResult Result { get; set; }
    }
}