using Couldron.Aspects;
using Couldron.Behaviours;
using Couldron.Collections;
using Couldron.Core;
using Couldron.ViewModels;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Couldron
{
    public static class Navigator
    {
        private static ConcurrentList<ViewModelWindow> viewModelWindow = new ConcurrentList<ViewModelWindow>();

        // The navigator always knows every window that it has created
        private static ConcurrentList<Window> windows = new ConcurrentList<Window>();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller"></param>
        /// <param name="args"></param>
        /// <exception cref="ArgumentException">Methodname specified in <see cref="NavigatingAttribute"/> does not exist</exception>
        /// <exception cref="NotSupportedException"><paramref name="caller"/> viewmodel was not created by the <see cref="Navigator"/></exception>
        [ExecutionSpeedDebug]
        public static async void Navigate<T>(IViewModel caller, params object[] args) where T : IViewModel
        {
            // create the new viewmodel
            var viewModel = Factory.Create<T>();
            var viewModelType = viewModel.GetType();

            (viewModel as IChangeAwareViewModel).IsNotNull(x => x.IsLoading = true);
            (viewModel as IDisposableObject).IsNotNull(x => x.Disposed += (s, e) => viewModelWindow.Remove(o => o.viewModelId == viewModel.Id));

            // add the viewmodel to the list

            // get the navigation methods
            await NavigatingTo(viewModelType, viewModel, args);

            // Check if the view model has a defined view
            var viewAttrib = viewModelType.GetCustomAttribute<ViewAttribute>(false);
            if (viewAttrib != null)
            {
                // Create the view - use the activator, since we dont expect any code in the view
                var view = Activator.CreateInstance(viewAttrib.ViewType) as FrameworkElement;
                // check if we have to create a new window
                Interaction.GetBehaviour<WindowConfigurationBehaviour>(view).IsNotNull(x =>
                {
                    if (x.Length > 0)
                        CreateWindow(caller, x[0], view, viewModel);
                    else
                        AssignViewToExistingView(caller, view, viewModel);
                });
            }
            else
            {
                AssignViewToExistingView(caller, null, viewModel);
            }

            (viewModel as IChangeAwareViewModel).IsNotNull(x => x.IsLoading = false);
        }

        [ExecutionSpeedDebug]
        private static void AssignViewToExistingView(IViewModel caller, FrameworkElement view, object viewModel)
        {
            // try to get the current window
            var callersWindow = viewModelWindow.FirstOrDefault(x => x.viewModelId == caller.Id);
            if (callersWindow == null)
                throw new NotSupportedException("The calling viewmodel must be created by the Navigator");

            // dispose and get rid of the old window content
            var window = callersWindow.window;
            window.BeginInit();
            window.Content.DisposeAll();

            if (view == null)
                window.DataContext = viewModel;
            else
            {
                window.Content = view;
                view.DataContext = viewModel;
            }

            window.EndInit();
            window.Activate();
        }

        [ExecutionSpeedDebug]
        private static void CreateWindow(IViewModel caller, WindowConfigurationBehaviour windowConfig, FrameworkElement view, object viewModel)
        {
            var window = new Window();
            window.BeginInit();
            // Add this new window to the dictionary
            windows.Add(window);

            // set the configs
            window.Width = windowConfig.Width;
            window.Height = windowConfig.Height;
            window.MaxHeight = windowConfig.MaximumHeight;
            window.MinHeight = windowConfig.MinimumHeight;
            window.MaxWidth = windowConfig.MaximumWidth;
            window.MinWidth = windowConfig.MinimumWidth;
            window.ResizeMode = windowConfig.ResizeMode;
            window.ShowInTaskbar = windowConfig.ShowInTaskbar;
            window.Topmost = windowConfig.Topmost;
            window.WindowStartupLocation = windowConfig.WindowStartupLocation;
            window.WindowState = windowConfig.WindowState;
            window.WindowStyle = windowConfig.WindowStyle;

            // binds these properties so that we can change them using the viewmodels
            window.SetBinding(Window.IconProperty, windowConfig, nameof(WindowConfigurationBehaviour.Icon));
            window.SetBinding(Window.TitleProperty, windowConfig, nameof(WindowConfigurationBehaviour.Title));

            if (windowConfig.Template != null)
                window.Template = windowConfig.Template;

            (viewModel as IWindowViewModel).IsNotNull(x =>
            {
                window.Closing += (s, e) => e.Cancel = !x.CanClose();
                window.GotFocus += (s, e) => x.GotFocus();
                window.SizeChanged += (s, e) => x.SizeChanged(e.NewSize.Width, e.NewSize.Height);
                window.LostFocus += (s, e) => x.LostFocus();
                window.Closed += (s, e) =>
                {
                    windows.Remove(s);
                    window.Content.DisposeAll();
                    window.Content = null;
                    // Also remove the viewmodel ids from the list
                    viewModelWindow.Remove(o => o.window == window);
                };
            });

            // assign the window's owner
            if (caller != null)
                viewModelWindow.FirstOrDefault(x => x.viewModelId == caller.Id).IsNotNull(x => window.Owner = x.window);

            window.Content = view;
            view.DataContext = viewModel;
            window.EndInit();

            if (windowConfig.IsDialog)
                window.ShowDialog();
            else
                window.Show();

            window.Activate();
        }

        [ExecutionSpeedDebug]
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

        [ExecutionSpeedDebug]
        private static async Task NavigatingTo(Type viewModelType, object viewModel, object[] args)
        {
            var navigatingAttrib = viewModelType.GetCustomAttribute<NavigatingAttribute>();

            if (navigatingAttrib != null)
            {
                foreach (var methodName in navigatingAttrib.MethodNames)
                {
                    var methodInfo = viewModelType.GetMethod(methodName, BindingFlags.Public);
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

        private class ViewModelWindow
        {
            public Guid viewModelId = Guid.Empty;
            public Window window = null;
        }
    }
}