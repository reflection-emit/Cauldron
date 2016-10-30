using Cauldron.XAML.ViewModels;
using System;
using System.Threading.Tasks;

namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Represents a class that handles the creation of a new Window (Desktop) or Page (UWP) and association of the viewmodel
    /// </summary>
    public partial interface INavigator
    {
        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history.
        /// <para/>
        /// Only relevant for UWP and Desktop single page application
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in forward navigation history.
        /// <para/>
        /// Only relevant for UWP and Desktop single page application
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// <para/>
        /// Only relevant for UWP and Desktop single page application
        /// </summary>
        Task<bool> GoBack();

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a Frame manages its own navigation history.
        /// <para/>
        /// Only relevant for UWP and Desktop single page application
        /// </summary>
        Task<bool> GoForward();

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>.
        /// </summary>
        /// <param name="viewModelType">The type of the viewmodel to construct</param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        Task<bool> NavigateAsync(Type viewModelType);

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <param name="viewModelType">The type of the viewmodel to construct</param>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model; must have a basic type (string, char, numeric, or GUID) to support parameter serialization.
        /// </param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        Task<bool> NavigateAsync(Type viewModelType, params object[] parameters);

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        Task<bool> NavigateAsync<T>() where T : IViewModel;

        /// <summary>
        /// Create a new popup with the view defined by the view model, depending on the views definition.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        Task NavigateAsync<T, TResult>(Func<TResult, Task> callback) where T : class, IDialogViewModel<TResult>;

        /// <summary>
        /// Create a new popup with the view defined by the view model, depending on the views definition.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        Task NavigateAsync<T>(Func<Task> callback) where T : class, IDialogViewModel;

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model; must have a basic type (string, char, numeric, or GUID) to support parameter serialization.
        /// </param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        Task<bool> NavigateAsync<T>(params object[] parameters) where T : IViewModel;

        /// <summary>
        /// Create a new popup with the view defined by the view model, depending on the views definition, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model.
        /// </param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        Task NavigateAsync<T, TResult>(Func<TResult, Task> callback, params object[] parameters) where T : class, IDialogViewModel<TResult>;

        /// <summary>
        /// Create a new popup with the view defined by the view model, depending on the views definition, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model.
        /// </param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        Task NavigateAsync<T>(Func<Task> callback, params object[] parameters) where T : class, IDialogViewModel;

        /// <summary>
        /// Tries to close a view model associated popup
        /// </summary>
        /// <param name="viewModel">The viewmodel to that was assigned to the window's data context</param>
        /// <returns>Returns true if successfully closed, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="viewModel"/> is null</exception>
        bool TryClose(IViewModel viewModel);
    }
}