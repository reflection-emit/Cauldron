using System.Windows;
using System.Windows.Input;

namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents a viewmodel that is aware of some of its view status
    /// </summary>
    public interface IViewAware : IViewModel
    {
        /// <summary>
        /// Occures after the viewmodel is assigned to the <see cref="FrameworkElement.DataContext"/>.
        /// </summary>
        /// <param name="inputBindingCollection">The input binding collection that can be used to add new command bindings (hot-keys) to the view from the viewmodel</param>
        void OnAssignToDataContext(InputBindingCollection inputBindingCollection);
    }
}