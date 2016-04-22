using System.Windows;

namespace Cauldron.ViewModels
{
    /// <summary>
    /// Represents an interface that can be used by <see cref="UIElement"/> to trigger viewmodels to close
    /// </summary>
    public interface IClose
    {
        /// <summary>
        /// Occures if the control requests a closing
        /// </summary>
        void Close();
    }
}