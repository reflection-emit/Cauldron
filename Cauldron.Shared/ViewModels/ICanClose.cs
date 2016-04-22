namespace Cauldron.ViewModels
{
    public interface ICanClose : IViewModel
    {
        void Activated();

        /// <summary>
        /// Occures if a control is about to close. If returns false, the closing will be cancelled.
        /// </summary>
        /// <returns>Should return true if item can be closed.</returns>
        bool CanClose();

        void Deactivated();
    }
}