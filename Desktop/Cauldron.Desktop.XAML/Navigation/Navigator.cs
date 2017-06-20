using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Collections;
using Cauldron.Core.Extensions;
using Cauldron.XAML.ViewModels;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Cauldron.XAML.Navigation
{
    /// <summary>
    /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
    /// </summary>
    [Component(typeof(INavigator), FactoryCreationPolicy.Singleton)]
    public sealed class Navigator : Singleton<INavigator>, INavigator
    {
        private static readonly object MainWindowTag = new object();

        // The navigator always knows every window that it has created
        private ConcurrentList<WindowViewModelObject> windows = new ConcurrentList<WindowViewModelObject>();

        private WindowType windowType;

        /// <exclude/>
        [ComponentConstructor]
        public Navigator()
        {
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history.
        /// <para/>
        /// Only relevant for UWP and Desktop single page application
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanGoBack
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in forward navigation history.
        /// <para/>
        /// Only relevant for UWP and Desktop single page application
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanGoForward
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a Frame manages its own navigation history.
        /// <para/>
        /// Only relevant for UWP and Desktop single page application
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> GoBack()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a Frame manages its own navigation history.
        /// <para/>
        /// Only relevant for UWP and Desktop single page application
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> GoForward()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>.
        /// </summary>
        /// <param name="viewModelType">The type of the viewmodel to construct</param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> NavigateAsync(Type viewModelType) =>
            await NavigateInternal<bool>(viewModelType, null, null);

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <param name="viewModelType">The type of the viewmodel to construct</param>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model; must have a basic type (string, char, numeric, or GUID) to support parameter serialization.
        /// </param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> NavigateAsync(Type viewModelType, params object[] parameters) =>
            await NavigateInternal<bool>(viewModelType, null, null, parameters);

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> NavigateAsync<T>() where T : IViewModel =>
            await NavigateInternal<bool>(typeof(T), null, null);

        /// <summary>
        /// Create a new popup with the view defined by the view model, depending on the views definition.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task NavigateAsync<T, TResult>(Func<TResult, Task> callback) where T : class, IDialogViewModel<TResult> =>
            await NavigateInternal<TResult>(typeof(T), callback, null, null);

        /// <summary>
        /// Create a new popup with the view defined by the view model, depending on the views definition.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task NavigateAsync<T>(Func<Task> callback) where T : class, IDialogViewModel =>
            await NavigateInternal<bool>(typeof(T), null, callback, null);

        /// <summary>
        /// Causes the window or page to load content represented by the specified <see cref="IViewModel"/>, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model; must have a basic type (string, char, numeric, or GUID) to support parameter serialization.
        /// </param>
        /// <returns>true if the navigation attempt was successful; otherwise, false</returns>
        public async Task<bool> NavigateAsync<T>(params object[] parameters) where T : IViewModel =>
            await NavigateInternal<bool>(typeof(T), null, null, parameters);

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
        public async Task NavigateAsync<T, TResult>(Func<TResult, Task> callback, params object[] parameters) where T : class, IDialogViewModel<TResult> =>
            await NavigateInternal<TResult>(typeof(T), callback, null, parameters);

        /// <summary>
        /// Create a new popup with the view defined by the view model, depending on the views definition, also passing a parameter to be used to construct the view model.
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel to construct</typeparam>
        /// <param name="callback">A delegate that is called after the popup has been closed</param>
        /// <param name="parameters">
        /// The navigation parameter to pass to the target view model.
        /// </param>
        /// <permission cref="NotSupportedException">The is already an open ContentDialog. Multiple ContentDialogs are not supported</permission>
        public async Task NavigateAsync<T>(Func<Task> callback, params object[] parameters) where T : class, IDialogViewModel =>
            await NavigateInternal<bool>(typeof(T), null, callback, parameters);

        /// <summary>
        /// Tries to close a view model associated popup
        /// </summary>
        /// <param name="viewModel">The viewmodel to that was assigned to the window's data context</param>
        /// <returns>Returns true if successfully closed, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="viewModel"/> is null</exception>
        public bool TryClose(IViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            var window = windows.FirstOrDefault(x => x.viewModelId == viewModel.Id);

            if (window == null)
                return false;

            // Close the window
            window.window.Close();

            return true;
        }

        /// <summary>
        /// Closes the current focused <see cref="Window"/>.
        /// </summary>
        /// <returns>Returns true if successfully closed, otherwise false</returns>
        public bool TryCloseFocusedWindow()
        {
            var windowObject = windows.FirstOrDefault(x => x.window.IsActive);

            if (windowObject == null)
                return false;

            if (Close(windowObject.window))
                windowObject.window.Close();

            return true;
        }

        private bool Close(Window window)
        {
            if (window == Application.Current.MainWindow)
            {
                foreach (var windowObject in windows)
                {
                    if (windowObject.window != Application.Current.MainWindow)
                        windowObject.window.Close();
                }

                if (windows.Count == 1)
                    return true;
            }
            else
                return true;

            return false;
        }

        private async Task<Tuple<Window, bool>> CreateDefaultWindow<TResult>(Func<TResult, Task> callback1, Func<Task> callback2, FrameworkElement view, IViewModel viewModel) =>
            new Tuple<Window, bool>(await CreateWindow(callback1, callback2, view, viewModel), WindowConfiguration.GetIsModal(view));

        private async Task<Window> CreateWindow<TResult>(Func<TResult, Task> callback1, Func<Task> callback2, FrameworkElement view, IViewModel viewModel)
        {
            var window = Common.CreateWindow(ref windowType);
            window.BeginInit();

            // Add this new window to the dictionary
            windows.Add(new WindowViewModelObject { window = window, viewModelId = viewModel.Id });

            // set the configs
            if (windowType.IsCutomWindow)
                window.ResizeMode = WindowConfiguration.GetResizeMode(view);
            else
            {
                window.ResizeMode = WindowConfiguration.GetResizeMode(view);
                window.WindowStyle = WindowConfiguration.GetWindowStyle(view);
            }

            window.Width = WindowConfiguration.GetWidth(view);
            window.Height = WindowConfiguration.GetHeight(view);
            window.MaxHeight = WindowConfiguration.GetMaxHeight(view);
            window.MinHeight = WindowConfiguration.GetMinHeight(view);
            window.MaxWidth = WindowConfiguration.GetMaxWidth(view);
            window.MinWidth = WindowConfiguration.GetMinWidth(view);
            window.ShowInTaskbar = WindowConfiguration.GetShowInTaskbar(view);
            window.Topmost = WindowConfiguration.GetTopmost(view);
            window.WindowStartupLocation = WindowConfiguration.GetWindowStartupLocation(view);
            window.WindowState = WindowConfiguration.GetWindowState(view);
            window.SizeToContent = WindowConfiguration.GetSizeToContent(view);

            // Special stuff for splashscreens
            if (WindowConfiguration.GetIsMainWindow(view))
                window.Tag = MainWindowTag;

            if (Application.Current.MainWindow != null && window.Tag == MainWindowTag)
                Application.Current.MainWindow = window;

            // Add the inputbindings to the window
            // we have to recreate the binding here because the sources are all wrong

            foreach (InputBinding inputBinding in view.InputBindings)
            {
                var oldBinding = BindingOperations.GetBinding(inputBinding, InputBinding.CommandProperty);
                var newBinding = oldBinding.Clone();
                newBinding.Source = viewModel;
                BindingOperations.ClearBinding(inputBinding, InputBinding.CommandProperty);
                BindingOperations.SetBinding(inputBinding, InputBinding.CommandProperty, newBinding);

                window.InputBindings.Add(inputBinding);
            }
            // remove them from the view
            view.InputBindings.Clear();

            if (WindowConfiguration.GetIsWindowPersistent(view))
                PersistentWindowInformation.Load(window, viewModel.GetType());

            // set the window owner
            if (window.Tag != MainWindowTag && WindowConfiguration.GetHasOwner(view))
                windows.FirstOrDefault(x => x.window.IsActive).IsNotNull(x => window.Owner = x.window);

            window.SetBinding(Window.IconProperty, new Binding { Path = new PropertyPath(WindowConfiguration.IconProperty), Source = view });
            window.SetBinding(Window.TitleProperty, new Binding { Path = new PropertyPath(WindowConfiguration.TitleProperty), Source = view });

            if (window.Icon == null)
                window.Icon = await Win32Api.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location).ToBitmapImageAsync();

            (viewModel as IFrameAware).IsNotNull(x =>
            {
                window.Activated += (s, e) => x.Activated();
                window.Deactivated += (s, e) => x.Deactivated();
            });
            (viewModel as ISizeAware).IsNotNull(x => window.SizeChanged += (s, e) => x.SizeChanged(e.NewSize.Width, e.NewSize.Height));
            window.SizeChanged += (s, e) =>
            {
                if (window.WindowState == WindowState.Normal)
                {
                    PersistentWindowProperties.SetHeight(window, e.NewSize.Height);
                    PersistentWindowProperties.SetWidth(window, e.NewSize.Width);
                }
            };
            window.Closing += (s, e) =>
            {
                if (WindowConfiguration.GetIsWindowPersistent(view))
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    PersistentWindowInformation.Save(window, viewModel.GetType());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                var canClose = viewModel as IFrameAware;

                if (canClose == null)
                    e.Cancel |= !Close(window);
                else
                    e.Cancel |= !canClose.CanClose() | !Close(window);
            };
            window.Closed += (s, e) =>
            {
                windows.Remove(x => x.window == s);

                if (callback1 != null)
                {
                    (viewModel as IDialogViewModel<TResult>).IsNotNull(async x => await callback1(x.Result));
                    (viewModel as IDialogViewModel).IsNotNull(async x => await callback2());
                }

                window.Content.As<FrameworkElement>()?.DataContext.TryDispose();

                window.Content.TryDispose();
                window.Content = null;
                window.TryDispose(); // some custom windows have implemented the IDisposable interface
            };

            // make sure the datacontext of the window is correct
            view.DataContextChanged += (s, e) => window.DataContext = view.DataContext;

            window.Content = view;
            view.DataContext = viewModel;

            Common.AddTransistionStoryboard(view);
            window.EndInit();

            return window;
        }

        private async Task<bool> NavigateInternal<TResult>(Type type, Func<TResult, Task> callback1, Func<Task> callback2, params object[] args)
        {
            try
            {
                // create the new viewmodel
                var viewModel = Factory.Create(type, args) as IViewModel;
                var viewModelType = viewModel.GetType();
                Tuple<Window, bool> windowInfo;

                // Check if the view model has a defined view
                var viewAttrib = viewModelType.GetCustomAttribute<ViewAttribute>(false);
                if (viewAttrib != null)
                    // Create the view - use the activator, since we dont expect any code in the view
                    windowInfo = await CreateDefaultWindow(callback1, callback2, viewAttrib.ViewType.CreateInstance() as FrameworkElement, viewModel);
                else // The viewmodel does not have a defined view... Maybe we have a data template instead
                {
                    // we always prefer our selector, because it rocks
                    var templateSelector = Application.Current.Resources[typeof(CauldronTemplateSelector).Name] as DataTemplateSelector;
                    var dataTemplate = templateSelector.SelectTemplate(viewModel, null);

                    // If we dont have a dataTemplate... we try to find a matching FrameworkElement
                    if (dataTemplate == null)
                    {
                        var possibleViewName = viewModelType.Name.Left(viewModelType.Name.Length - "Model".Length);
                        var possibleView = Factory.Create(possibleViewName) ?? Factory.Create(Assemblies.GetTypeFromName(possibleViewName));

                        // On such case we create a dummy View
                        if (possibleView == null)
                        {
                            var textBlock = new TextBlock();
                            textBlock.Text = viewModelType.FullName;
                            textBlock.Foreground = new SolidColorBrush(Colors.Tomato);
                            textBlock.TextWrapping = TextWrapping.NoWrap;
                            textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                            textBlock.FontSize = 18;

                            windowInfo = await CreateDefaultWindow(callback1, callback2, textBlock, viewModel);
                        }
                        else
                            windowInfo = await CreateDefaultWindow(callback1, callback2, possibleView as FrameworkElement, viewModel);
                    }
                    else
                        // try to get a WindowConfiguration attach in the datatemplate
                        windowInfo = await CreateDefaultWindow(callback1, callback2, dataTemplate.LoadContent() as FrameworkElement, viewModel);
                }

                Window window = windowInfo.Item1;
                (viewModel as IDisposableObject).IsNotNull(x => x.Disposed += async (s, e) => await viewModel.Dispatcher.RunAsync(() => window.Close()));

                // if this is not a dialog... we show the window first and then invoke the navigation method
                if (!windowInfo.Item2)
                    window.Show();

                // This only applies to windows that are not maximized
                if (window.WindowState != WindowState.Maximized)
                {
                    // Check if the window is visible for the user
                    // If the user has for example undocked his laptop (which means he lost a monitor) and the application
                    // was running on the secondary monitor, we can't just start the window with that configuration
                    if (!MonitorInfo.WindowIsInAnyMonitor(window.GetWindowHandle()))
                    {
                        var primaryBounds = MonitorInfo.PrimaryMonitorBounds;
                        window.Height = Math.Min(window.Height, primaryBounds.Height);
                        window.Width = Math.Min(window.Width, primaryBounds.Width);
                        window.Left = Math.Max(0, (primaryBounds.Width / 2) - (window.Width / 2));
                        window.Top = Math.Max(0, (primaryBounds.Height / 2) - (window.Height / 2));
                    }
                    else
                    {
                        // we have to make sure, that the title bar of the window is visible for the user
                        var monitorBounds = MonitorInfo.GetMonitorBounds(window.GetWindowHandle());

                        if (monitorBounds.HasValue)
                        {
                            window.Height = Math.Min(window.Height, monitorBounds.Value.Height);
                            window.Width = Math.Min(window.Width, monitorBounds.Value.Width);
                            window.Left = window.Left >= monitorBounds.Value.Left && window.Left <= monitorBounds.Value.Right ? window.Left : monitorBounds.Value.Left;
                            window.Top = window.Top >= monitorBounds.Value.Top && window.Top <= monitorBounds.Value.Bottom ? window.Top : monitorBounds.Value.Top;
                        }
                        else // set the left and top to 0
                        {
                            window.Left = 0;
                            window.Top = 0;
                        }
                    }
                }

                if (windowInfo.Item2)
                    window.ShowDialog();

                return true;
            }
            catch (XamlParseException)
            {
                throw;
            }
            catch
            {
                return false;
            }
        }

        private class WindowViewModelObject
        {
            public Guid viewModelId;
            public Window window;
        }
    }
}