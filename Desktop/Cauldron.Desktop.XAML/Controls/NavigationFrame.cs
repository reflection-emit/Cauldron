using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.XAML.Navigation;
using Cauldron.XAML.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Cauldron.XAML.Controls
{
    /// <summary>
    /// Displays Page instances, supports navigation to new pages, and maintains a navigation history to support forward and backward navigation.
    /// </summary>
    public class NavigationFrame : ContentControl
    {
        private static bool isCustomWindow = false;
        private ViewOrientation currentViewOrientation;
        private ConcurrentDictionary<Guid, Window> dialogs = new ConcurrentDictionary<Guid, Window>();
        private CauldronTemplateSelector selector = new CauldronTemplateSelector();

        private Type windowType;

        /// <summary>
        /// Initializes a new instance of <see cref="NavigationFrame"/>
        /// </summary>
        public NavigationFrame()
        {
            this.BackStack = new List<PageStackEntry>();
            this.ForwardStack = new List<PageStackEntry>();
            this.Unloaded += NavigationFrame_Unloaded;

            Application.Current.Activated += Current_Activated;
            Application.Current.Deactivated += Current_Deactivated;
            Application.Current.MainWindow.SizeChanged += Current_SizeChanged;
            Application.Current.MainWindow.Closed += Current_Closed;

            this.InputBindings.Add(new KeyBinding(new RelayCommand(async () => await this.GoBack(NavigationType.BackButton)), Key.Back, ModifierKeys.None));
            this.currentViewOrientation = MonitorInfo.GetCurrentOrientation();
        }

        /// <summary>
        /// Occures when back navigation is requested
        /// </summary>
        public static event EventHandler<CauldronBackRequestedEventArgs> BackRequested;

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        public event EventHandler Navigated;

        /// <summary>
        /// Gets a collection of <see cref="PageStackEntry"/> instances representing the backward navigation history of the <see cref="NavigationFrame"/>.
        /// </summary>
        public List<PageStackEntry> BackStack { get; private set; }

        /// <summary>
        /// Gets a collection of <see cref="PageStackEntry"/> instances representing the forward navigation history of the <see cref="NavigationFrame"/>.
        /// </summary>
        public List<PageStackEntry> ForwardStack { get; private set; }

        private IViewModel ContentDataContext
        {
            get { return this.Content.As<FrameworkElement>()?.DataContext.As<IViewModel>(); }
            set
            {
                var content = this.Content.As<FrameworkElement>();
                if (content != null)
                    content.DataContext = value;
            }
        }

        /// <summary>
        /// Resets the navigation history of the <see cref="NavigationFrame"/>
        /// </summary>
        public void ClearStack()
        {
            this.ForwardStack.Clear();
            this.BackStack.Clear();

            this.CanGoBack = false;
            this.CanGoForward = false;

            this.Navigated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// </summary>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> GoBack() => await this.GoBack(NavigationType.User);

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// </summary>
        /// <param name="navigationType">The type of navigation attempt</param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> GoBack(NavigationType navigationType)
        {
            var eventArgs = new CauldronBackRequestedEventArgs();

            BackRequested?.Invoke(this, eventArgs);

            if (eventArgs.IsHandled)
                return true;

            if (this.BackStack.Count < 2)
                return false;

            var stackEntry = this.BackStack[this.BackStack.Count - 2];

            this.ForwardStack.Add(this.BackStack[this.BackStack.Count - 1]);
            this.BackStack.RemoveAt(this.BackStack.Count - 1);
            this.BackStack.RemoveAt(this.BackStack.Count - 1);

            return await this.Navigate(stackEntry.ViewModelType, stackEntry.Parameters, NavigationMode.Back, navigationType);
        }

        /// <summary>
        /// Gets a collection of PageStackEntry instances representing the forward navigation history of the Frame.
        /// </summary>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> GoForward() => await this.GoForward(NavigationType.User);

        /// <summary>
        /// Gets a collection of PageStackEntry instances representing the forward navigation history of the Frame.
        /// </summary>
        /// <param name="navigationType">The type of navigation attempt</param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> GoForward(NavigationType navigationType)
        {
            if (this.ForwardStack.Count <= 0)
                return false;

            var stackEntry = this.ForwardStack[0];
            return await this.Navigate(stackEntry.ViewModelType, stackEntry.Parameters, NavigationMode.Forward, navigationType);
        }

        /// <summary>
        /// Causes the <see cref="NavigationFrame"/> to load content represented by the specified <see cref="IViewModel"/>, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <param name="viewModelType">The type of the viewmodel to construct</param>
        /// <param name="arguments">
        /// The navigation parameter to pass to the target view model; must have a basic type (string, char, numeric, or GUID) to support parameter serialization.
        /// </param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="viewModelType"/> is null</exception>
        public async Task<bool> Navigate(Type viewModelType, params object[] arguments)
        {
            if (viewModelType == null)
                throw new ArgumentNullException(nameof(viewModelType));

            return await this.Navigate(viewModelType, arguments, NavigationMode.Forward, NavigationType.User);
        }

        /// <summary>
        /// Causes the <see cref="NavigationFrame"/> to load content represented by the specified <see cref="IViewModel"/>, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <param name="viewModelType">The type of the viewmodel to construct</param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        /// <exception cref="ArgumentNullException"><paramref name="viewModelType"/> is null</exception>
        public async Task<bool> Navigate(Type viewModelType)
        {
            if (viewModelType == null)
                throw new ArgumentNullException(nameof(viewModelType));

            return await this.Navigate(viewModelType, new object[0], NavigationMode.Forward, NavigationType.User);
        }

        /// <summary>
        /// Create a new <see cref="ContentDialog"/> or <see cref="Flyout"/> with the view defined by the view model, depending on the views definition.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model to create</typeparam>
        /// <param name="arguments">The parameters used to construct the view model</param>
        /// <param name="callback">A delegate that is invoked when the dialog is closed</param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task Navigate<TViewModel>(object[] arguments, Func<Task> callback) where TViewModel : class, IDialogViewModel
        {
            var viewModelType = typeof(TViewModel);
            var dialog = this.CreateContentDialog(viewModelType);

            var view = GetView(viewModelType);
            var viewModel = Factory.Create(viewModelType, arguments).As<TViewModel>(); // Use Factory create here so that the factory extensions are invoked

            dialogs.TryAdd(viewModel.Id, dialog);
            dialog.Content = view;

            await this.Dispatcher.InvokeAsync(() => view.DataContext = viewModel, System.Windows.Threading.DispatcherPriority.Normal);

            Common.AddTransistionStoryboard(view);
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
                await callback();

            dialogs.TryRemove(viewModel.Id, out dialog);

            view.DataContext = null;
            viewModel.TryDispose();
        }

        /// <summary>
        /// Create a new <see cref="ContentDialog"/> or <see cref="Flyout"/> with the view defined by the view model, depending on the views definition.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model to create</typeparam>
        /// <typeparam name="TResult">The result of the dialog</typeparam>
        /// <param name="arguments">The parameters used to construct the view model</param>
        /// <param name="callback">A delegate that is invoked when the dialog is closed</param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task Navigate<TViewModel, TResult>(object[] arguments, Func<TResult, Task> callback) where TViewModel : class, IDialogViewModel<TResult>
        {
            var viewModelType = typeof(TViewModel);
            var dialog = this.CreateContentDialog(viewModelType);

            var view = GetView(viewModelType);
            var viewModel = Factory.Create(viewModelType, arguments).As<TViewModel>(); // Use Factory create here so that the factory extensions are invoked

            dialogs.TryAdd(viewModel.Id, dialog);
            dialog.Content = view;

            await this.Dispatcher.InvokeAsync(() => view.DataContext = viewModel, System.Windows.Threading.DispatcherPriority.Normal);

            Common.AddTransistionStoryboard(view);
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
                await callback(viewModel.Result);

            dialogs.TryRemove(viewModel.Id, out dialog);

            view.DataContext = null;
            viewModel.TryDispose();
        }

        /// <summary>
        /// Reloads the current view and viewmodel
        /// </summary>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> Reload()
        {
            if (this.BackStack.Count == 0)
                return false;

            var stackEntry = this.BackStack[this.BackStack.Count - 1];
            this.BackStack.RemoveAt(this.BackStack.Count - 1);

            return await this.Navigate(stackEntry.ViewModelType, stackEntry.Parameters, NavigationMode.Forward, NavigationType.Code);
        }

        /// <summary>
        /// Tries to close all dialog <see cref="Window"/>s
        /// </summary>
        /// <returns>Returns true if there are open dialogs, otherwise false</returns>
        public bool TryClose()
        {
            if (this.dialogs.Count > 0)
            {
                foreach (var dialog in this.dialogs.ToArray() /* So that we can change the collection while we are looping through */)
                    dialog.As<Window>()?.Close();
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Tries to close a view model associated <see cref="Window"/>
        /// </summary>
        /// <param name="viewModel">The viewmodel to that was assigned to the window's data context</param>
        /// <returns>Returns true if successfully closed, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="viewModel"/> is null</exception>
        public bool TryClose(IViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (!this.dialogs.ContainsKey(viewModel.Id))
                return false;

            var d = this.dialogs[viewModel.Id];
            d.As<Window>()?.Close();

            return true;
        }

        internal async Task<bool> Navigate(Type viewModelType, object[] arguments, NavigationMode navigationMode, NavigationType navigationType)
        {
            if (!await this.CanChangePage(viewModelType, navigationMode, navigationType))
                return false;

            FrameworkElement view = null;
            IViewModel viewModel = null;

            // Create a view
            if (navigationMode == NavigationMode.Back || navigationMode == NavigationMode.Forward || navigationMode == NavigationMode.New)
                view = GetView(viewModelType);

            // Create a view model
            if (navigationMode == NavigationMode.Back || navigationMode == NavigationMode.Forward || navigationMode == NavigationMode.Refresh)
                viewModel = Factory.Create(viewModelType, arguments).As<IViewModel>(); // Use Factory create here so that the factory extensions are invoked

            if (navigationMode == NavigationMode.Back || navigationMode == NavigationMode.Forward)
            {
                // we need to keep our old datacontext to dispose them properly later on
                var oldDataContext = this.ContentDataContext;
                var oldView = this.Content.As<FrameworkElement>();
                // Assign our new content
                this.Content = view;
                await this.Dispatcher.InvokeAsync(() => view.DataContext = viewModel, System.Windows.Threading.DispatcherPriority.Normal);
                // Invoke the Navigation stuff in the current datacontext
                await AsyncHelper.NullGuard(viewModel.As<INavigable>()?.OnNavigatedTo(new NavigationInfo(navigationMode, navigationType, oldDataContext?.GetType())));
                // Remove the reference of the old vm from the old view
                oldView.IsNotNull(x => x.DataContext = null);
                // Now invoke the navigated from on our old data context
                await AsyncHelper.NullGuard(oldDataContext?.As<INavigable>()?.OnNavigatedFrom(new NavigationInfo(navigationMode, navigationType, viewModelType)));
                // dispose the old viewmodel
                oldDataContext?.TryDispose();
            }
            else if (navigationMode == NavigationMode.New)
            {
                // NavigationMode.New means recreate the view but preserve the viewmodel
                // we need to keep our current datacontext for later reassigning
                var currentDataContext = this.ContentDataContext;
                var oldView = this.Content.As<FrameworkElement>();

                // cancel this if the oldview and the new view are the same
                if (oldView.GetType() == view.GetType())
                    return false;

                // Assign our content
                this.Content = view;
                await this.Dispatcher.InvokeAsync(() => view.DataContext = currentDataContext, System.Windows.Threading.DispatcherPriority.Normal);
                // Invoke the Navigation stuff in the current datacontext
                await AsyncHelper.NullGuard(currentDataContext.As<INavigable>()?.OnNavigatedTo(new NavigationInfo(navigationMode, navigationType, currentDataContext.GetType())));
                // Remove the reference of the current viewmodel from the old view
                oldView.IsNotNull(x => x.DataContext = null);
            }
            else if (navigationMode == NavigationMode.Refresh)
            {
                // we need to keep our old datacontext to dispose them properly later on
                var oldDataContext = this.ContentDataContext;
                // NavigationMode.Refresh means recreate the view model but preserve the view
                viewModel = Factory.Create(viewModelType, arguments).As<IViewModel>();
                await this.Dispatcher.InvokeAsync(() => view.DataContext = viewModel, System.Windows.Threading.DispatcherPriority.Normal);
                // Invoke the Navigation stuff in the current datacontext
                await AsyncHelper.NullGuard(viewModel.As<INavigable>()?.OnNavigatedTo(new NavigationInfo(navigationMode, navigationType, viewModelType)));
                // dispose the old viewmodel
                oldDataContext?.TryDispose();
            }

            // Refresh the stacks
            if (navigationMode == NavigationMode.Forward)
            {
                // we have to check if the current viewmodel is the same as the one the top of the forwardstack
                // if that is the case we remove it from the stack
                // if that is not the case, then we clear the stack
                if (this.ForwardStack.Count > 0)
                {
                    var stackEntry = this.ForwardStack[0];

                    // Remove the stack if equal
                    if (viewModelType == stackEntry.ViewModelType &&
                        ((stackEntry.Parameters != null && arguments != null && stackEntry.Parameters.SequenceEqual(arguments)) ||
                            (stackEntry.Parameters == null && arguments == null)))
                        this.ForwardStack.RemoveAt(0);
                    else
                        this.ForwardStack.Clear();
                }
            }

            if (navigationMode == NavigationMode.Back || navigationMode == NavigationMode.Forward)
                this.BackStack.Add(new PageStackEntry(viewModelType, arguments));

            // remove entries from stack
            if (this.MaxStackSize > 1)
            {
                if (this.BackStack.Count > this.MaxStackSize + 1)
                    this.BackStack.RemoveAt(0);

                if (this.ForwardStack.Count > this.MaxStackSize)
                    this.ForwardStack.RemoveAt(this.ForwardStack.Count - 1);
            }

            this.CanGoBack = this.BackStack.Count > 1;
            this.CanGoForward = this.ForwardStack.Count > 0;

            this.Navigated?.Invoke(this, EventArgs.Empty);

            return true;
        }

        private async Task<bool> CanChangePage(Type requestingViewModel, NavigationMode navigationMode, NavigationType navigationType)
        {
            // We will do this for the UWP only interface INavigable
            var navigableViewModel = this.ContentDataContext as INavigable;

            if (navigableViewModel != null)
            {
                var args = new NavigatingInfo(navigationMode, navigationType, requestingViewModel);
                await navigableViewModel.OnNavigatingFrom(args);
                return !args.Cancel;
            }

            // And also for the interface that also exist for desktop for compatibility reasons
            var frameAware = this.ContentDataContext as IFrameAware;

            if (frameAware != null)
                return frameAware.CanClose();

            return true;
        }

        private Window CreateContentDialog(Type viewModelType)
        {
            var window = Common.CreateWindow(ref windowType, ref isCustomWindow);
            window.BeginInit();

            // set the configs
            if (isCustomWindow)
                window.ResizeMode = ResizeMode.NoResize;
            else
            {
                window.ResizeMode = ResizeMode.NoResize;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
            }

            window.MinHeight = 50;
            window.MinWidth = 120;
            window.ShowInTaskbar = false;
            window.Topmost = false;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.WindowState = WindowState.Normal;
            window.Icon = Application.Current.MainWindow.Icon;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Owner = Application.Current.MainWindow;
            window.SetBinding(Window.TitleProperty, new Binding
            {
                Path = new PropertyPath("Content.DataContext.Title"),
                Source = window
            });

            window.EndInit();

            return window;
        }

        private void Current_Activated(object sender, EventArgs e) =>
                    this.ContentDataContext?.As<IFrameAware>()?.Activated();

        private void Current_Closed(object sender, EventArgs e) =>
            this.ContentDataContext?.TryDispose();

        private void Current_Deactivated(object sender, EventArgs e) =>
                this.ContentDataContext?.As<IFrameAware>()?.Deactivated();

        #region Dependency Property CanGoForward

        /// <summary>
        /// Identifies the CanGoForward dependency property
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty = DependencyProperty.Register(nameof(CanGoForward), typeof(bool), typeof(NavigationFrame), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the CanGoForward Property
        /// </summary>
        public bool CanGoForward
        {
            get { return (bool)this.GetValue(CanGoForwardProperty); }
            private set { this.SetValue(CanGoForwardProperty, value); }
        }

        #endregion Dependency Property CanGoForward

        #region Dependency Property CanGoBack

        /// <summary>
        /// Identifies the CanGoBack dependency property
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(NavigationFrame), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the CanGoBack Property
        /// </summary>
        public bool CanGoBack
        {
            get { return (bool)this.GetValue(CanGoBackProperty); }
            private set { this.SetValue(CanGoBackProperty, value); }
        }

        #endregion Dependency Property CanGoBack

        #region Dependency Property MaxStackSize

        /// <summary>
        /// Identifies the MaxStackSize dependency property
        /// </summary>
        public static readonly DependencyProperty MaxStackSizeProperty = DependencyProperty.Register(nameof(MaxStackSize), typeof(int), typeof(NavigationFrame), new PropertyMetadata(10));

        /// <summary>
        /// Gets or sets the MaxStackSize Property
        /// </summary>
        public int MaxStackSize
        {
            get { return (int)this.GetValue(MaxStackSizeProperty); }
            set { this.SetValue(MaxStackSizeProperty, value); }
        }

        #endregion Dependency Property MaxStackSize

        private async void Current_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var orientation = e.NewSize.Width > e.NewSize.Height ? ViewOrientation.Landscape : ViewOrientation.Portrait;

            if (this.currentViewOrientation != orientation)
            {
                this.currentViewOrientation = orientation;
                await this.Navigate(this.ContentDataContext?.GetType(), null, NavigationMode.New, NavigationType.Code);
            }

            this.ContentDataContext?.As<ISizeAware>()?.SizeChanged(e.NewSize.Width, e.NewSize.Height);
        }

        private FrameworkElement GetView(Type viewModelType)
        {
            var attrib = viewModelType.GetTypeInfo().GetCustomAttribute<ViewAttribute>();

            if (attrib != null)
            {
                string viewTypeName = attrib.ViewType.Name;
                Type viewType = null;

                if (viewType == null)
                {
                    var orientation = Application.Current.MainWindow.Width > Application.Current.MainWindow.Height ? ViewOrientation.Landscape : ViewOrientation.Portrait;

                    if (orientation == ViewOrientation.Landscape)
                        viewTypeName += "Landscape";
                    else if (orientation == ViewOrientation.Portrait)
                        viewTypeName += "Portrait";

                    // Example: public class MainViewMobileLandscape

                    viewType = Assemblies.GetTypeFromName(viewTypeName);
                }

                if (viewType == null)
                    // Use createinstance because we dont need anything other than creating an object
                    // The Factory uses a compile expression to create objects which is faster than Activator.CreateInstance.
                    // First create is always slow
                    return Factory.CreateInstance(attrib.ViewType).As<FrameworkElement>();
                else
                    return Factory.CreateInstance(viewType).As<FrameworkElement>();
            }
            else
            {
                var dt = this.selector.SelectTemplate(viewModelType, this);

                if (dt == null)
                {
                    var textBlock = new TextBlock();
                    textBlock.Text = viewModelType.FullName;
                    textBlock.Foreground = new SolidColorBrush(Colors.Tomato);
                    textBlock.TextWrapping = TextWrapping.NoWrap;
                    textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                    textBlock.FontSize = 18;
                    return textBlock;
                }

                return dt.LoadContent().As<FrameworkElement>();
            }
        }

        private void NavigationFrame_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= NavigationFrame_Unloaded;

            Application.Current.Activated -= Current_Activated;
            Application.Current.Deactivated -= Current_Deactivated;

            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.SizeChanged -= Current_SizeChanged;
                Application.Current.MainWindow.Closed -= Current_Closed;
            }

            this.InputBindings.Clear();
        }
    }
}