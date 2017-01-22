using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.XAML.Navigation;
using Cauldron.XAML.ValueConverters;
using Cauldron.XAML.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using XamlNav = Windows.UI.Xaml.Navigation;

namespace Cauldron.XAML.Controls
{
    /// <summary>
    /// Displays Page instances, supports navigation to new pages, and maintains a navigation history to support forward and backward navigation.
    /// </summary>
    [WebHostHidden]
    [Threading(ThreadingModel.Both)]
    [MarshalingBehavior(MarshalingType.Agile)]
    [ContractVersion(typeof(UniversalApiContract), 65536)]
    public class NavigationFrame : ContentControl
    {
        private ApplicationViewOrientation currentViewOrientation;
        private ConcurrentDictionary<Guid, DependencyObject> dialogs = new ConcurrentDictionary<Guid, DependencyObject>();
        private CauldronTemplateSelector selector = new CauldronTemplateSelector();

        /// <summary>
        /// Initializes a new instance of <see cref="NavigationFrame"/>
        /// </summary>
        public NavigationFrame()
        {
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            this.BackStack = new List<PageStackEntry>();
            this.ForwardStack = new List<PageStackEntry>();
            this.Unloaded += NavigationFrame_Unloaded;

            Window.Current.Activated += Current_Activated;
            Window.Current.SizeChanged += Current_SizeChanged;
            Window.Current.Closed += Current_Closed;
            Window.Current.VisibilityChanged += Current_VisibilityChanged;

            SystemNavigationManager.GetForCurrentView().BackRequested += NavigationFrame_BackRequested;
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += NavigationFrame_VisibleBoundsChanged;

            this.currentViewOrientation = ApplicationView.GetForCurrentView().Orientation;
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

            return await this.Navigate(stackEntry.ViewModelType, stackEntry.Parameters, XamlNav.NavigationMode.Back, navigationType);
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
            return await this.Navigate(stackEntry.ViewModelType, stackEntry.Parameters, XamlNav.NavigationMode.Forward, navigationType);
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

            return await this.Navigate(viewModelType, arguments, XamlNav.NavigationMode.Forward, NavigationType.User);
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

            return await this.Navigate(viewModelType, new object[0], XamlNav.NavigationMode.Forward, NavigationType.User);
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
            if (this.dialogs.Values.Any(x => x is ContentDialog))
            {
                // let us close every dialog
                foreach (var item in this.dialogs.Values)
                {
                    var vm = item.As<ContentDialog>()?.Content?.As<FrameworkElement>().DataContext?.As<IViewModel>();
                    if (vm != null)
                    {
                        if (!this.TryClose(vm))
                            throw new NotSupportedException("The is already an open ContentDialog. Multiple ContentDialogs are not supported");
                    }
                }
            }

            var viewModelType = typeof(TViewModel);
            var dialog = new ContentDialog();
            dialog.VerticalContentAlignment = VerticalAlignment.Stretch;
            dialog.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            var view = GetView(viewModelType);
            var viewModel = Factory.Create(viewModelType, arguments).As<TViewModel>(); // Use Factory create here so that the factory extensions are invoked

            dialogs.TryAdd(viewModel.Id, dialog);
            dialog.Content = view;

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => view.DataContext = viewModel);

            if (await dialog.ShowAsync() == ContentDialogResult.None)
                await callback();

            DependencyObject obj;
            dialogs.TryRemove(viewModel.Id, out obj);

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
            if (this.dialogs.Values.Any(x => x is ContentDialog))
            {
                // let us close every dialog
                foreach (var item in this.dialogs.Values)
                {
                    var vm = item.As<ContentDialog>()?.Content?.As<FrameworkElement>().DataContext?.As<IViewModel>();
                    if (vm != null)
                    {
                        if (!this.TryClose(vm))
                            throw new NotSupportedException("The is already an open ContentDialog. Multiple ContentDialogs are not supported");
                    }
                }
            }

            var viewModelType = typeof(TViewModel);
            var dialog = new ContentDialog();
            dialog.VerticalContentAlignment = VerticalAlignment.Stretch;
            dialog.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            var view = GetView(viewModelType);
            var viewModel = Factory.Create(viewModelType, arguments).As<TViewModel>(); // Use Factory create here so that the factory extensions are invoked

            dialogs.TryAdd(viewModel.Id, dialog);
            dialog.Content = view;

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => view.DataContext = viewModel);

            if (await dialog.ShowAsync() == ContentDialogResult.None)
                await callback(viewModel.Result);

            DependencyObject obj;
            dialogs.TryRemove(viewModel.Id, out obj);

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

            return await this.Navigate(stackEntry.ViewModelType, stackEntry.Parameters, XamlNav.NavigationMode.Forward, NavigationType.Code);
        }

        /// <summary>
        /// Tries to close all <see cref="ContentDialog"/>s and <see cref="Flyout"/>s
        /// </summary>
        /// <returns>Returns true if there are open dialogs, otherwise false</returns>
        public bool TryClose()
        {
            if (this.dialogs.Count > 0)
            {
                foreach (var dialog in this.dialogs.ToArray() /* So that we can change the collection while we are looping through */)
                {
                    dialog.As<ContentDialog>()?.Hide();
                    dialog.As<FlyoutBase>()?.Hide();
                }
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Tries to close a view model associated <see cref="ContentDialog"/> or <see cref="Flyout"/>
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
            d.As<ContentDialog>()?.Hide();
            d.As<FlyoutBase>()?.Hide();

            return true;
        }

        internal async Task<bool> Navigate(Type viewModelType, object[] arguments, XamlNav.NavigationMode navigationMode, NavigationType navigationType)
        {
            if (!await this.CanChangePage(viewModelType, navigationMode, navigationType))
                return false;

            FrameworkElement view = null;
            IViewModel viewModel = null;

            // Create a view
            if (navigationMode == XamlNav.NavigationMode.Back || navigationMode == XamlNav.NavigationMode.Forward || navigationMode == XamlNav.NavigationMode.New)
                view = GetView(viewModelType);

            // Create a view model
            if (navigationMode == XamlNav.NavigationMode.Back || navigationMode == XamlNav.NavigationMode.Forward || navigationMode == XamlNav.NavigationMode.Refresh)
                viewModel = Factory.Create(viewModelType, arguments).As<IViewModel>(); // Use Factory create here so that the factory extensions are invoked

            // Bind the IsLoading if implemented
            if (view != null)
            {
                view.SetBinding(Control.IsEnabledProperty, new Binding
                {
                    Path = new PropertyPath(nameof(IViewModel.IsLoading)),
                    Converter = new BooleanInvertConverter()
                });
            }

            if (navigationMode == XamlNav.NavigationMode.Back || navigationMode == XamlNav.NavigationMode.Forward)
            {
                // Setup the page transition animations
                if (view.Transitions != null && view.Transitions.Count > 0)
                    this.ContentTransitions = view.Transitions;
                else
                    this.ContentTransitions = this.DefaultChildrenTransitions;

                // we need to keep our old datacontext to dispose them properly later on
                var oldDataContext = this.ContentDataContext;
                var oldView = this.Content.As<FrameworkElement>();
                // Assign our new content
                this.Content = view;
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => view.DataContext = viewModel);
                // Invoke the Navigation stuff in the current datacontext
                await AsyncHelper.NullGuard(viewModel.As<INavigable>()?.OnNavigatedTo(new NavigationInfo(navigationMode, navigationType, oldDataContext?.GetType())));
                // Remove the reference of the old vm from the old view
                oldView.IsNotNull(x => x.DataContext = null);
                // Now invoke the navigated from on our old data context
                await AsyncHelper.NullGuard(oldDataContext?.As<INavigable>()?.OnNavigatedFrom(new NavigationInfo(navigationMode, navigationType, viewModelType)));
                // dispose the old viewmodel
                oldDataContext?.TryDispose();
            }
            else if (navigationMode == XamlNav.NavigationMode.New)
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
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => view.DataContext = currentDataContext);
                // Invoke the Navigation stuff in the current datacontext
                await AsyncHelper.NullGuard(currentDataContext.As<INavigable>()?.OnNavigatedTo(new NavigationInfo(navigationMode, navigationType, currentDataContext.GetType())));
                // Remove the reference of the current viewmodel from the old view
                oldView.IsNotNull(x => x.DataContext = null);
            }
            else if (navigationMode == XamlNav.NavigationMode.Refresh)
            {
                // we need to keep our old datacontext to dispose them properly later on
                var oldDataContext = this.ContentDataContext;
                // NavigationMode.Refresh means recreate the view model but preserve the view
                viewModel = Factory.Create(viewModelType, arguments).As<IViewModel>();
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => view.DataContext = viewModel);
                // Invoke the Navigation stuff in the current datacontext
                await AsyncHelper.NullGuard(viewModel.As<INavigable>()?.OnNavigatedTo(new NavigationInfo(navigationMode, navigationType, viewModelType)));
                // dispose the old viewmodel
                oldDataContext?.TryDispose();
            }

            // Refresh the stacks
            if (navigationMode == XamlNav.NavigationMode.Forward)
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

            if (navigationMode == XamlNav.NavigationMode.Back || navigationMode == XamlNav.NavigationMode.Forward)
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

        private static FrameworkElement GetViewInstance(string viewTypeName, Type viewType = null)
        {
            var newViewTypeName = viewTypeName;

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == DeviceFamilies.DeviceFamily_WindowsMobile)
                newViewTypeName += "Mobile";
            else if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == DeviceFamilies.DeviceFamily_WindowsDesktop)
                newViewTypeName += "Desktop";
            else if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == DeviceFamilies.DeviceFamily_WindowsXbox)
                newViewTypeName += "Xbox";
            else if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == DeviceFamilies.DeviceFamily_WindowsIoT)
                newViewTypeName += "IoT";

            // Example: public class MainViewMobile

            var newViewType = Assemblies.GetTypeFromName(newViewTypeName);

            if (newViewType == null)
            {
                var currentView = ApplicationView.GetForCurrentView();

                if (currentView.Orientation == ApplicationViewOrientation.Landscape)
                    newViewTypeName += "Landscape";
                else if (currentView.Orientation == ApplicationViewOrientation.Portrait)
                    newViewTypeName += "Portrait";

                // Example: public class MainViewMobileLandscape

                newViewType = Assemblies.GetTypeFromName(newViewTypeName);
            }

            if (newViewType == null)
            {
                newViewTypeName = viewTypeName;
                var currentView = ApplicationView.GetForCurrentView();

                if (currentView.Orientation == ApplicationViewOrientation.Landscape)
                    newViewTypeName += "Landscape";
                else if (currentView.Orientation == ApplicationViewOrientation.Portrait)
                    newViewTypeName += "Portrait";

                // Example: public class MainViewLandscape

                newViewType = Assemblies.GetTypeFromName(newViewTypeName);
            }

            if (newViewType == null && viewType == null)
            {
                var textBlock = new TextBlock();
                textBlock.Text = viewTypeName;
                textBlock.Foreground = new SolidColorBrush(Colors.Tomato);
                textBlock.TextWrapping = TextWrapping.NoWrap;
                textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                textBlock.FontSize = 18;
                return textBlock;
            }

            if (newViewType == null)
                // Use createinstance because we dont need anything other than creating an object
                // The Factory uses a compile expression to create objects which is faster than Activator.CreateInstance.
                // First create is always slow
                return Factory.CreateInstance(viewType).As<FrameworkElement>();
            else
                return Factory.CreateInstance(newViewType).As<FrameworkElement>();
        }

        private async Task<bool> CanChangePage(Type requestingViewModel, XamlNav.NavigationMode navigationMode, NavigationType navigationType)
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

        #region Dependency Property BackButtonVisible

        /// <summary>
        /// Identifies the <see cref="BackButtonVisible" /> dependency property
        /// </summary>
        public static readonly DependencyProperty BackButtonVisibleProperty = DependencyProperty.Register(nameof(BackButtonVisible), typeof(bool), typeof(NavigationFrame), new PropertyMetadata(false, NavigationFrame.OnBackButtonVisibleChanged));

        /// <summary>
        /// Gets or sets the <see cref="BackButtonVisible" /> Property
        /// </summary>
        public bool BackButtonVisible
        {
            get { return (bool)this.GetValue(BackButtonVisibleProperty); }
            set { this.SetValue(BackButtonVisibleProperty, value); }
        }

        private static void OnBackButtonVisibleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                (bool)args.NewValue ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        #endregion Dependency Property BackButtonVisible

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

        #region Dependency Property DefaultChildrenTransistions

        /// <summary>
        /// Identifies the DefaultChildrenTransistions dependency property
        /// </summary>
        public static readonly DependencyProperty DefaultChildrenTransitionsProperty = DependencyProperty.Register(nameof(DefaultChildrenTransitions), typeof(TransitionCollection), typeof(NavigationFrame), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the DefaultChildrenTransistions Property
        /// </summary>
        public TransitionCollection DefaultChildrenTransitions
        {
            get { return (TransitionCollection)this.GetValue(DefaultChildrenTransitionsProperty); }
            set { this.SetValue(DefaultChildrenTransitionsProperty, value); }
        }

        #endregion Dependency Property DefaultChildrenTransistions

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
                this.ContentDataContext?.As<IFrameAware>()?.Deactivated();
            else
                this.ContentDataContext?.As<IFrameAware>()?.Activated();
        }

        private void Current_Closed(object sender, CoreWindowEventArgs e) =>
            this.ContentDataContext?.TryDispose();

        private async void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            var currentView = ApplicationView.GetForCurrentView();

            if (this.currentViewOrientation != currentView.Orientation)
            {
                this.currentViewOrientation = currentView.Orientation;
                await this.Navigate(this.ContentDataContext?.GetType(), null, XamlNav.NavigationMode.New, NavigationType.Code);
            }

            this.ContentDataContext?.As<ISizeAware>()?.SizeChanged(e.Size.Width, e.Size.Height);
        }

        private void Current_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (e.Visible)
                this.ContentDataContext?.As<IPrelaunchAware>()?.AppIsVisible();
        }

        private FrameworkElement GetView(Type viewModelType)
        {
            var attrib = viewModelType.GetTypeInfo().GetCustomAttribute<ViewAttribute>();

            if (attrib != null)
                return GetViewInstance(attrib.ViewType.Name, attrib.ViewType);
            else
            {
                // The datatemplate has a higher priority than a control
                var dt = this.selector.SelectTemplate(viewModelType, this);

                // if we have no datatemplate for the viewmodel then we try getting the view based on the name of the viewmodel
                if (dt == null && viewModelType.Name.EndsWith("Model"))
                    return GetViewInstance(viewModelType.Name.Left(viewModelType.Name.Length - "Model".Length));

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

        private void NavigationFrame_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            e.Handled = true;

            // Close the App if you are on the startpage
            if (!this.CanGoBack)
                Application.Current.Exit();

            // Navigate back
            if (this.CanGoBack)
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.GoBack(NavigationType.BackButton);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void NavigationFrame_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= NavigationFrame_Unloaded;
            Window.Current.Activated -= Current_Activated;
            Window.Current.SizeChanged -= Current_SizeChanged;
            Window.Current.Closed -= Current_Closed;
            Window.Current.VisibilityChanged -= Current_VisibilityChanged;

            SystemNavigationManager.GetForCurrentView().BackRequested -= NavigationFrame_BackRequested;
            ApplicationView.GetForCurrentView().VisibleBoundsChanged -= NavigationFrame_VisibleBoundsChanged;
        }

        private void NavigationFrame_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            var visible = ApplicationView.GetForCurrentView().VisibleBounds;

            var isOpen = (visible.Height != Window.Current.Bounds.Height || visible.Width != Window.Current.Bounds.Width);

            if (isOpen)
            {
                this.Margin = new Thickness(
                    visible.Left,
                    visible.Top,
                    Window.Current.Bounds.Width - visible.Width - visible.Left,
                    Window.Current.Bounds.Height - visible.Height - visible.Top);
            }
            else
            {
                this.Margin = new Thickness(0);
            }
        }
    }
}