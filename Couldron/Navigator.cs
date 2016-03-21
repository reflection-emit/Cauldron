using Couldron.Behaviours;
using Couldron.Collections;
using Couldron.Core;
using Couldron.ViewModels;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Couldron
{
    /// <summary>
    /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
    /// </summary>
    public static class Navigator
    {
        // The navigator always knows every window that it has created
        private static ConcurrentList<Window> windows = new ConcurrentList<Window>();

        /// <summary>
        /// Closes the current focused <see cref="Window"/>.
        /// </summary>
        public static void Close()
        {
            var window = windows.FirstOrDefault(x => x.IsActive);

            if (window == null)
                return;

            if (Close(window))
                window.Close();
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public static async void Navigate<T>() where T : IViewModel
        {
            await NavigateInternal<T, bool>(null, null);
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <typeparam name="TResult">The result type of the dialog</typeparam>
        /// <param name="callback">A delegate that is called after the <see cref="Window"/> has been closed</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public static async void Navigate<T, TResult>(Action<TResult> callback) where T : IViewModel
        {
            await NavigateInternal<T, TResult>(callback);
        }

        /// <summary>
        /// Handles creation of a new <see cref="Window"/> and association of the viewmodel
        /// </summary>
        /// <typeparam name="T">The viewModel type to create</typeparam>
        /// <param name="args">Parameters of the <see cref="NavigatingAttribute"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        public static async void Navigate<T>(params object[] args) where T : IViewModel
        {
            await NavigateInternal<T, bool>(null, args);
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
        public static async void Navigate<T, TResult>(Action<TResult> callback, params object[] args) where T : IViewModel
        {
            await NavigateInternal<T, TResult>(callback, args);
        }

        private static bool Close(Window window)
        {
            if (window == Application.Current.MainWindow)
            {
                foreach (var w in windows)
                {
                    if (w != Application.Current.MainWindow)
                        w.Close();
                }

                if (windows.Count == 1)
                    return true;
            }
            else
                return true;

            return false;
        }

        private static Window CreateDefaultWindow<TResult>(Action<TResult> callback, FrameworkElement view, object viewModel)
        {
            return CreateWindow(callback, new WindowConfigurationBehaviour(), view, viewModel);
        }

        private static Window CreateWindow<TResult>(Action<TResult> callback, FrameworkElement view, object viewModel, out bool isDialog)
        {
            Window window = null;

            var windowConfig = Interaction.GetBehaviour<WindowConfigurationBehaviour>(view);

            if (windowConfig != null && windowConfig.Length > 0)
            {
                isDialog = windowConfig[0].IsDialog;
                window = CreateWindow(callback, windowConfig[0], view, viewModel);
            }
            else
            {
                isDialog = false;
                window = CreateDefaultWindow(callback, view, viewModel);
            }

            return window;
        }

        private static Window CreateWindow<TResult>(Action<TResult> callback, WindowConfigurationBehaviour windowConfig, FrameworkElement view, object viewModel)
        {
            var window = new Window();
            window.BeginInit();
            // Add this new window to the dictionary
            windows.Add(window);

            // set the configs
            window.Width = windowConfig.Width;
            window.Height = windowConfig.Height;
            window.MaxHeight = windowConfig.MaxHeight;
            window.MinHeight = windowConfig.MinHeight;
            window.MaxWidth = windowConfig.MaxWidth;
            window.MinWidth = windowConfig.MinWidth;
            window.ResizeMode = windowConfig.ResizeMode;
            window.ShowInTaskbar = windowConfig.ShowInTaskbar;
            window.Topmost = windowConfig.Topmost;
            window.WindowStartupLocation = windowConfig.WindowStartupLocation;
            window.WindowState = windowConfig.WindowState;
            window.WindowStyle = windowConfig.WindowStyle;
            window.Icon = windowConfig.Icon;
            window.Title = windowConfig.Title;

            // set the window owner
            window.Owner = windows.FirstOrDefault(x => x.IsActive);

            windowConfig.IconChanged += (s, e) => window.Icon = windowConfig.Icon;
            windowConfig.TitleChanged += (s, e) => window.Title = windowConfig.Title;

            (viewModel as IWindowViewModel).IsNotNull(x =>
            {
                window.Closing += (s, e) => e.Cancel |= !x.CanClose();
                window.GotFocus += (s, e) => x.GotFocus();
                window.SizeChanged += (s, e) => x.SizeChanged(e.NewSize.Width, e.NewSize.Height);
                window.LostFocus += (s, e) => x.LostFocus();
            });
            window.Closing += (s, e) =>
            {
                e.Cancel |= !Close(window);
            };
            window.Closed += (s, e) =>
            {
                windows.Remove(s);
                window.Content.DisposeAll();
                window.Content = null;
            };

            if (callback != null)
                (viewModel as IDialogViewModel<TResult>).IsNotNull(x => window.Closing += (s, e) =>
                {
                    if (!e.Cancel)
                        callback(x.Result);
                });

            window.Content = view;
            view.DataContext = viewModel;
            window.EndInit();

            return window;
        }

        private static bool IsParameterMatch(object[] args, ParameterInfo[] types)
        {
            if (args.Length != types.Length)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].GetType() != types[i].ParameterType)
                    return false;
            }

            return true;
        }

        private static async Task NavigateInternal<T, TResult>(Action<TResult> callback, params object[] args) where T : IViewModel
        {
            // create the new viewmodel
            var viewModel = Factory.Create<T>();
            var viewModelType = viewModel.GetType();
            var isDialog = false;
            Window window = null;

            (viewModel as IChangeAwareViewModel).IsNotNull(x => x.IsLoading = true);

            // Check if the view model has a defined view
            var viewAttrib = viewModelType.GetCustomAttribute<ViewAttribute>(false);
            if (viewAttrib != null)
                // Create the view - use the activator, since we dont expect any code in the view
                window = CreateWindow(callback, Activator.CreateInstance(viewAttrib.ViewType) as FrameworkElement, viewModel, out isDialog);
            else // The viewmodel does not have a defined view... Maybe we have a data template instead
            {
                // we always prefer our selector, because it rocks
                var templateSelector = Application.Current.Resources[typeof(CouldronTemplateSelector)] as DataTemplateSelector;
                var dataTemplate = templateSelector.SelectTemplate(viewModel, null);

                // On such case we just use the default window
                if (dataTemplate == null)
                    window = CreateDefaultWindow(callback, null, viewModel);
                else
                    // try to get a WindowConfigurationBehaviour attach in the datatemplate
                    window = CreateWindow(callback, dataTemplate.LoadContent() as FrameworkElement, viewModel, out isDialog);
            }

            (viewModel as IDisposableObject).IsNotNull(x => x.Disposed += (s, e) => window.Close());

            // if this is not a dialog... we show the window first and then invoke the navigation method
            if (!isDialog)
                window.Show();

            // get the navigation methods
            await NavigatingTo(viewModelType, viewModel, args);

            (viewModel as IChangeAwareViewModel).IsNotNull(x => x.IsLoading = false);

            if (isDialog)
                window.ShowDialog();
        }

        private static async Task NavigatingTo(Type viewModelType, object viewModel, object[] args)
        {
            var navigatingAttrib = viewModelType.GetCustomAttribute<NavigatingAttribute>();

            if (navigatingAttrib != null)
            {
                foreach (var methodName in navigatingAttrib.MethodNames)
                {
                    var methodInfo = viewModelType.GetMethod(methodName);
                    if (methodInfo == null)
                        throw new ArgumentException("The method '" + methodName + "' does not exist in " + viewModelType.FullName);

                    // Check if the args matches with the method info param types
                    if (IsParameterMatch(args, methodInfo.GetParameters()))
                    {
                        if (methodInfo.ReturnParameter.ParameterType.IsSubclassOf(typeof(Task)))
                            await (methodInfo.Invoke(viewModel, args) as Task);
                        else
                            methodInfo.Invoke(viewModel, args);

                        break;
                    }
                }
            }
        }
    }
}