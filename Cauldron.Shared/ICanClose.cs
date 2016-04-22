namespace Couldron.ViewModels
{
    public interface ICanClose : IViewModel
    {
        /// <summary>
        /// Occures if a control is about to close. If returns false, the closing will be cancelled.
        /// </summary>
        /// <returns>Should return true if item can be closed.</returns>
        bool CanClose();

        void GotFocus();

        void LostFocus();
    }
}