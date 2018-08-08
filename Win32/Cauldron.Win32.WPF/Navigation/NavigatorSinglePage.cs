using Cauldron.Activator;
using Cauldron.XAML.Controls;
using Cauldron.XAML.Threading;
using Cauldron.XAML.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Handles creation of a new <see cref="Page"/> and association of the viewmodel
    /// </summary>
    [Component(typeof(INavigator), FactoryCreationPolicy.Singleton)]
    public sealed class NavigatorSinglePage : Factory<INavigator>, INavigator
    {
        private IDispatcher _dispatcher;
        private NavigationFrame rootFrame;

        /// <exclude/>
        [ComponentConstructor]
        public NavigatorSinglePage()
        {
            this.rootFrame = Application.Current.MainWindow.Content as NavigationFrame;
            this.rootFrame.Navigated += RootFrame_Navigated;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history.
        /// </summary>
        public bool CanGoBack { get { return this.rootFrame.CanGoBack; } }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in forward navigation history.
        /// </summary>
        public bool CanGoForward { get { return this.rootFrame.CanGoForward; } }

        private IDispatcher Dispatcher
        {
            get
            {
                if (this._dispatcher == null)
                    this._dispatcher = Factory.Create<IDispatcher>();

                return this._dispatcher;
            }
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// </summary>
        public Task<bool> GoBack() => this.rootFrame.GoBack();

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a Frame manages its own navigation history.
        /// </summary>
        public Task<bool> GoForward() => this.rootFrame.GoForward();

        /// <summary>
        /// Handles creation of a new page or window and association of the viewmodel
        /// </summary>
        /// <param name="viewModelType">The viewModel type to create</param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> NavigateAsync(Type viewModelType) =>
          await this.rootFrame.Navigate(viewModelType);

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <param name="viewModelType">The type of the viewmodel to construct</param>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model; must have a basic type (string, char, numeric, or GUID) to support parameter serialization.
        /// </param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> NavigateAsync(Type viewModelType, params object[] parameters) =>
          await this.rootFrame.Navigate(viewModelType, parameters);

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model; must have a basic type (string, char, numeric, or GUID) to support parameter serialization.
        /// </param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> NavigateAsync<T>(params object[] parameters) where T : IViewModel =>
          await this.rootFrame.Navigate(typeof(T), parameters);

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> NavigateAsync<T>() where T : IViewModel =>
          await this.rootFrame.Navigate(typeof(T));

        /// <summary>
        /// Create a new ContentDialog or Flyout with the view defined by the view model, depending on the views definition.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task NavigateAsync<T, TResult>(Func<TResult, Task> callback) where T : class, IDialogViewModel<TResult> =>
          await this.rootFrame.Navigate<T, TResult>(new object[0], callback);

        /// <summary>
        /// Create a new ContentDialog or Flyout with the view defined by the view model, depending on the views definition.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task NavigateAsync<T>(Func<Task> callback) where T : class, IDialogViewModel =>
          await this.rootFrame.Navigate<T>(new object[0], callback);

        /// <summary>
        /// Create a new ContentDialog or Flyout with the view defined by the view model, depending on the views definition, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model.
        /// </param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task NavigateAsync<T, TResult>(Func<TResult, Task> callback, params object[] parameters) where T : class, IDialogViewModel<TResult> =>
          await this.rootFrame.Navigate<T, TResult>(parameters, callback);

        /// <summary>
        /// Create a new ContentDialog or Flyout with the view defined by the view model, depending on the views definition, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model.
        /// </param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task NavigateAsync<T>(Func<Task> callback, params object[] parameters) where T : class, IDialogViewModel =>
          await this.rootFrame.Navigate<T>(parameters, callback);

        /// <summary>
        /// Tries to close a view model associated popup
        /// </summary>
        /// <param name="viewModel">The viewmodel to that was assigned to the window's data context</param>
        /// <returns>Returns true if successfully closed, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="viewModel"/> is null</exception>
        public bool TryClose(IViewModel viewModel) => this.rootFrame.TryClose(viewModel);

        /// <summary>
        /// Closes the current focused <see cref="Window"/>.
        /// </summary>
        /// <returns>Returns true if successfully closed, otherwise false</returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool TryCloseFocusedWindow()
        {
            throw new NotImplementedException();
        }

        private async void RootFrame_Navigated(object sender, EventArgs e)
        {
            await this.Dispatcher.RunAsync(() =>
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(INavigator.CanGoBack)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(INavigator.CanGoForward)));
            });
        }
    }
}