using System.Collections.ObjectModel;

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
        /// <summary>
        /// Occures if the control requests a closing. The viewmodel has to react to the requst by
        /// for example removing the tab from the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        void Close();
    }
}