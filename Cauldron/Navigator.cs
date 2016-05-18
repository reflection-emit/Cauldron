using Cauldron.Attached;
using Cauldron.Behaviours;
using Cauldron.Collections;
using Cauldron.Core;
using Cauldron.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cauldron
{
    /// <summary>
    /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
    /// </summary>
    [Factory(typeof(INavigator), FactoryCreationPolicy.Singleton)]
    public sealed class Navigator : Singleton<Navigator>, INavigator
    {
        private static readonly object MainWindowTag = new object();
        private static bool isCustomWindow = false;

        // The navigator always knows every window that it has created
        private ConcurrentList<WindowViewModelObject> windows = new ConcurrentList<WindowViewModelObject>();

        /// <summary>
        /// Closes the current focused <see cref="Window"/>.
        /// </summary>
        public void CloseFocusedWindow()
        {
            var windowObject = windows.FirstOrDefault(x => x.window.IsActive);

            if (windowObject == null)
                return;

            if (Close(windowObject.window))
                windowObject.window.Close();
        }

        /// <summary>
        /// Closes the window to where the given viewmodel was directly assigned to.
        /// </summary>
        /// <param name="viewModel">The viewmodel to that was assigned to the window's data context</param>
        /// <returns>Returns true if <see cref="Window.Close"/> was triggered, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="viewModel"/> is null</exception>
        public bool CloseWindowOf(IViewModel viewModel)
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
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public async void Navigate<T>() where T : IViewModel
        {
            await NavigateInternal<bool>(typeof(T), null, null);
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the <see cref="Window"/> has been closed</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public async void Navigate<T, TResult>(Action<TResult> callback) where T : IViewModel
        {
            await NavigateInternal<TResult>(typeof(T), callback);
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <param name="args">Parameters of the <see cref="NavigatingAttribute"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public async void Navigate<T>(params object[] args) where T : IViewModel
        {
            await NavigateInternal<bool>(typeof(T), null, args);
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the <see cref="Window"/> has been closed</param>
        /// <param name="args">Parameters of the <see cref="NavigatingAttribute"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        /// <exception cref="ResourceReferenceKeyNotFoundException">View of a viewmodel not found</exception>
        public async void Navigate<T, TResult>(Action<TResult> callback, params object[] args) where T : IViewModel
        {
            await NavigateInternal<TResult>(typeof(T), callback, args);
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <param name="viewModelType">The viewModel type to create</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public async void Navigate(Type viewModelType)
        {
            await NavigateInternal<bool>(viewModelType, null, null);
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

        private Window CreateDefaultWindow<TResult>(Action<TResult> callback, FrameworkElement view, IViewModel viewModel)
        {
            return CreateWindow(callback, new WindowConfiguration(), view, viewModel);
        }

        private Window CreateWindow<TResult>(Action<TResult> callback, FrameworkElement view, IViewModel viewModel, out bool isDialog)
        {
            Window window = null;

            var windowConfig = Interaction.GetBehaviour<WindowConfiguration>(view);

            if (windowConfig != null && windowConfig.Length > 0)
            {
                isDialog = windowConfig[0].IsModal;
                window = CreateWindow(callback, windowConfig[0], view, viewModel);
            }
            else
            {
                isDialog = false;
                window = CreateDefaultWindow(callback, view, viewModel);
            }

            return window;
        }

        private Window CreateWindow()
        {
            var window = AssemblyUtil.ExportedTypes.FirstOrDefault(x => x.IsSubclassOf(typeof(Window)));
            if (window == null)
                return new Window();

            isCustomWindow = true;

            return Activator.CreateInstance(window.AsType()) as Window;
        }

        private Window CreateWindow<TResult>(Action<TResult> callback, WindowConfiguration windowConfig, FrameworkElement view, IViewModel viewModel)
        {
            var window = CreateWindow();
            window.BeginInit();
            // Add this new window to the dictionary
            windows.Add(new WindowViewModelObject { window = window, viewModelId = viewModel.Id });

            // set the configs
            if (isCustomWindow)
                window.ResizeMode = windowConfig.ResizeMode;
            else
            {
                window.ResizeMode = windowConfig.ResizeMode;
                window.WindowStyle = windowConfig.WindowStyle;
            }

            window.Width = windowConfig.Width;
            window.Height = windowConfig.Height;
            window.MaxHeight = windowConfig.MaxHeight;
            window.MinHeight = windowConfig.MinHeight;
            window.MaxWidth = windowConfig.MaxWidth;
            window.MinWidth = windowConfig.MinWidth;
            window.ShowInTaskbar = windowConfig.ShowInTaskbar;
            window.Topmost = windowConfig.Topmost;
            window.WindowStartupLocation = windowConfig.WindowStartupLocation;
            window.WindowState = windowConfig.WindowState;
            window.Icon = windowConfig.Icon;
            window.Title = windowConfig.Title;
            window.SizeToContent = windowConfig.SizeToContent;

            if (windowConfig.IsMainWindow)
                window.Tag = MainWindowTag;

            if (Application.Current.MainWindow != null && window.Tag == MainWindowTag)
                Application.Current.MainWindow = window;

            // Add the inputbindings to the window
            window.InputBindings.AddRange(windowConfig.InputBindings);
            // remove them from the windowConfig
            windowConfig.InputBindings.Clear();

            if (windowConfig.IsWindowPersistent)
                PersistentWindowInformation.Load(window, viewModel.GetType());

            // set the window owner
            if (window.Tag != MainWindowTag)
                windows.FirstOrDefault(x => x.window.IsActive).IsNotNull(x => window.Owner = x.window);

            // Set the toolbar template
            WindowToolbar.SetTemplate(window, windowConfig.ToolbarTemplate);

            windowConfig.IconChanged += (s, e) => window.Icon = windowConfig.Icon;
            windowConfig.TitleChanged += (s, e) => window.Title = windowConfig.Title;

            (viewModel as IWindowViewModel).IsNotNull(x =>
            {
                window.Closing += (s, e) => e.Cancel |= !x.CanClose();
                window.Activated += (s, e) => x.Activated();
                window.SizeChanged += (s, e) => x.SizeChanged(e.NewSize.Width, e.NewSize.Height);
                window.Deactivated += (s, e) => x.Deactivated();
            });
            window.Closing += (s, e) =>
            {
                e.Cancel |= !Close(window);
            };
            window.Closed += (s, e) =>
            {
                if (windowConfig.IsWindowPersistent)
                    PersistentWindowInformation.Save(window, viewModel.GetType());

                windows.Remove(x => x.window == s);

                if (callback != null)
                    (viewModel as IDialogViewModel<TResult>).IsNotNull(x => callback(x.Result));

                window.Content.DisposeAll();
                window.Content = null;
                window.DisposeAll(); // some custom windows have implemented the IDisposable interface
            };

            // make sure the datacontext of the behaviour is correct
            view.DataContextChanged += (s, e) =>
            {
                windowConfig.DataContext = view.DataContext;
                window.DataContext = view.DataContext;
            };

            window.Content = view;
            view.DataContext = viewModel;
            window.EndInit();

            return window;
        }

        private async Task NavigateInternal<TResult>(Type type, Action<TResult> callback, params object[] args)
        {
            // create the new viewmodel
            var viewModel = Factory.Create(type) as IViewModel;
            var viewModelType = viewModel.GetType();
            var isModal = false;
            Window window = null;

            (viewModel as IChangeAwareViewModel).IsNotNull(x => x.IsLoading = true);

            // Check if the view model has a defined view
            var viewAttrib = viewModelType.GetCustomAttribute<ViewAttribute>(false);
            if (viewAttrib != null)
                // Create the view - use the activator, since we dont expect any code in the view
                window = CreateWindow(callback, Activator.CreateInstance(viewAttrib.ViewType) as FrameworkElement, viewModel, out isModal);
            else // The viewmodel does not have a defined view... Maybe we have a data template instead
            {
                // we always prefer our selector, because it rocks
                var templateSelector = Application.Current.Resources[typeof(CauldronTemplateSelector).Name] as DataTemplateSelector;
                var dataTemplate = templateSelector.SelectTemplate(viewModel, null);

                // If we dont have a dataTemplate... we try to find a matching FrameworkElement
                if (dataTemplate == null)
                {
                    var possibleViewName = viewModelType.Name.Left(viewModelType.Name.Length - "Model".Length);
                    var possibleViewType = AssemblyUtil.GetTypeFromName(possibleViewName);

                    // On such case we throw an exception
                    if (possibleViewType == null)
                        throw new ResourceReferenceKeyNotFoundException("Unable to find the view for a viewmodel", "View_" + viewModelType.Name);

                    window = CreateWindow(callback, Factory.Create(possibleViewType) as FrameworkElement, viewModel, out isModal);
                }
                else
                    // try to get a WindowConfiguration attach in the datatemplate
                    window = CreateWindow(callback, dataTemplate.LoadContent() as FrameworkElement, viewModel, out isModal);
            }

            (viewModel as IDisposableObject).IsNotNull(x => x.Disposed += (s, e) => window.Close());

            // if this is not a dialog... we show the window first and then invoke the navigation method
            if (!isModal)
                window.Show();

            // This only applies to windows that are not maximized
            if (window.WindowState != WindowState.Maximized)
            {
                // Check if the window is visible for the user
                // If the user has for example undocked his laptop (which means he lost a monitor) and the application
                // was running on the secondary monitor, we can't just start the window with that configuration
                IntPtr monitor = UnsafeNative.MonitorFromWindow(window.GetWindowHandle(), UnsafeNative.MonitorOptions.MONITOR_DEFAULTTONULL);

                // If MonitorFromWindow has return zero, we are sure that the window is not in any of our monitors
                if (monitor == IntPtr.Zero)
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
                    var monitorBounds = MonitorInfo.GetMonitorBounds(window);

                    if (monitorBounds.HasValue)
                    {
                        window.Height = Math.Min(window.Height, monitorBounds.Value.Height);
                        window.Width = Math.Min(window.Width, monitorBounds.Value.Width);
                        window.Left = Math.Max(window.Left, monitorBounds.Value.Left);
                        window.Top = Math.Max(window.Top, monitorBounds.Value.Top);
                    }
                    else // set the left and top to 0
                    {
                        window.Left = 0;
                        window.Top = 0;
                    }
                }
            }

            // get the navigation methods and execute them if neccessary
            await NavigatingTo(viewModelType, viewModel, args);

            (viewModel as IChangeAwareViewModel).IsNotNull(x => x.IsLoading = false);

            if (isModal)
                window.ShowDialog();
        }

        private async Task NavigatingTo(Type viewModelType, object viewModel, object[] args)
        {
            var navigatingAttrib = viewModelType.GetCustomAttribute<NavigatingAttribute>();

            if (navigatingAttrib != null)
            {
                var parameterTypes = args.GetTypes();

                foreach (var methodName in navigatingAttrib.MethodNames)
                {
                    var methodInfo = viewModelType.GetMethod(methodName, parameterTypes, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
                    if (methodInfo != null)
                    {
                        if (methodInfo.ReturnParameter.ParameterType.IsSubclassOf(typeof(Task)))
                            await (methodInfo.Invoke(viewModel, args) as Task);
                        else
                            methodInfo.Invoke(viewModel, args);
                    }
                }
            }
        }

        private class PersistentWindowInformation
        {
            [JsonProperty("height")]
            public double Height { get; set; }

            [JsonProperty("left")]
            public double Left { get; set; }

            [JsonProperty("state")]
            public WindowState State { get; set; }

            [JsonProperty("top")]
            public double Top { get; set; }

            [JsonProperty("width")]
            public double Width { get; set; }

            public static void Load(Window window, Type viewModelType)
            {
                var path = Path.Combine(ApplicationData.Current.RoamingFolder, "Navigator");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var name = viewModelType.FullName.GetMD5HashString() + ".json";
                var filename = Path.Combine(path, name);

                if (!File.Exists(filename))
                    return;

                var obj = JsonConvert.DeserializeObject<PersistentWindowInformation>(File.ReadAllText(filename));

                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Left = obj.Left;
                window.Top = obj.Top;
                window.Height = obj.Height;
                window.Width = obj.Width;
                window.WindowState = obj.State;
            }

            public static void Save(Window window, Type viewModelType)
            {
                var path = Path.Combine(ApplicationData.Current.RoamingFolder, "Navigator");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var obj = new PersistentWindowInformation();
                obj.Width = window.Width;
                obj.Height = window.Height;
                obj.Top = window.Top;
                obj.Left = window.Left;
                obj.State = window.WindowState;

                var filename = Path.Combine(path, viewModelType.FullName.GetMD5HashString() + ".json");
                var content = JsonConvert.SerializeObject(obj);
                File.WriteAllText(filename, content);
            }
        }

        private class WindowViewModelObject
        {
            public Guid viewModelId;
            public Window window;
        }
    }
}