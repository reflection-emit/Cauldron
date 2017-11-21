using Cauldron.XAML.Navigation;
using System.Threading.Tasks;

namespace Cauldron.XAML.ViewModels
{
    /// <summary>
    /// Represents a navigable view model
    /// </summary>
    public interface INavigable : IViewModel
    {
        /// <summary>
        /// Invoked immediately after the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="args">The arguments of the navigation</param>
        Task OnNavigatedFrom(NavigationInfo args);

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="args"></param>
        Task OnNavigatedTo(NavigationInfo args);

        /// <summary>
        /// Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="args"></param>
        Task OnNavigatingFrom(NavigatingInfo args);
    }
}