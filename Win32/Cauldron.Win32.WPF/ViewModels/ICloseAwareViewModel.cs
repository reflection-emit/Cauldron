using System.Collections.ObjectModel;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
#else

using System.Windows.Controls;

#endif

/* TODO */

namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents an interface that can be used by <see cref="UIElement"/> to trigger viewmodels to close
    /// </summary>
    public interface ICloseAwareViewModel : IViewModel
    {
#if !NETFX_CORE

        /// <summary>
        /// Occures if the window is closing. Returning a false will prevent the window from closing.
        /// </summary>
        /// <returns>Should return true if window can be closed.</returns>
        bool CanClose();

#endif

        /// <summary>
        /// Occures if the control requests a closing. The viewmodel has to react to the requst by
        /// for example removing the tab from the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        void Close();
    }
}