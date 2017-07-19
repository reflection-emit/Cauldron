using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Internal;
using Cauldron.XAML.Controls;
using Cauldron.XAML.Navigation;
using Cauldron.XAML.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Storage;

namespace Cauldron.XAML
{
    /// <summary>
    /// Encapsulates a Windows Presentation Foundation (WPF) application.
    /// </summary>
    public abstract class ApplicationBase : Application, IViewModel
    {
        private readonly string applicationHash;
        private DispatcherEx _dispatcher;
        private Guid? _id;
        private bool _isLoading = true;
        private bool _isSingleInstance;
        private bool _isSinglePage;
        private IMessageDialog _messageDialog;
        private INavigator _navigator;
        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBase"/>
        /// </summary>
        public ApplicationBase()
        {
            this.Startup += ApplicationBase_Startup;

            DispatcherEx dispatcher = Application.Current == null ? System.Windows.Threading.Dispatcher.CurrentDispatcher : Application.Current.Dispatcher;
            this.OnConstruction();
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            //Assemblies.LoadAssembly();
            this.OnResourceLoad();

            // Add the custom template selector to the resources
            this.Resources.Add(typeof(CauldronTemplateSelector).Name, new CauldronTemplateSelector());

            // Add all Value converters to the dictionary
            Factory.CreateMany<IValueConverter>().Foreach(x => this.Resources.Add(x.GetType().Name, x));

            // find all resourcedictionaries and add them to the existing resources
            Assemblies.CauldronObjects
                .Select(x => x as IFactoryCache)
                .Where(x => x != null)
                .SelectMany(x => x.GetComponents())
                .Where(x => x.ContractName == typeof(ResourceDictionary).FullName).Select(x =>
                {
                    var type = x.Type;
                    return type.FullName.StartsWith("Cauldron.") ? new { Index = 0, FactoryInfo = x, Type = type } : new { Index = 1, FactoryInfo = x, Type = type };
                })
                .OrderBy(x => x.Index)
                .ThenByDescending(x => x.FactoryInfo.Priority)
                .ThenBy(x => x.Type.FullName)
                .Foreach(x =>
                {
                    this.Resources.MergedDictionaries.Add(x.FactoryInfo.CreateInstance() as ResourceDictionary);
                    Output.WriteLineInfo($"Adding ResourceDictionary: {x.Type.FullName}");
                });

            this.applicationHash = (ApplicationInfo.ApplicationName + ApplicationInfo.ApplicationPublisher + ApplicationInfo.ApplicationVersion.ToString()).GetHash(HashAlgorithms.Md5);
        }

        /// <summary>
        /// Occures if a behaviour should be invoked
        /// </summary>
        public event EventHandler<BehaviourInvocationArgs> BehaviourInvoke;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the application splash screen image. This is only neccessary if the property
        /// <see cref="IsSinglePage"/> is set to true
        /// </summary>
        public ImageSource ApplicationSplash { get; set; }

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="DispatcherEx "/> is associated with.
        /// </summary>
        public new DispatcherEx Dispatcher
        {
            get
            {
                if (this._dispatcher == null)
                    this._dispatcher = DispatcherEx.Current;

                return this._dispatcher;
            }
        }

        /// <summary>
        /// Gets the unique Id of the view model
        /// </summary>
        public Guid Id
        {
            get
            {
                if (!this._id.HasValue)
                    this._id = Guid.NewGuid();

                return this._id.Value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the viewmodel is loading
        /// </summary>
        public bool IsLoading
        {
            get { return this._isLoading; }
            set
            {
                if (this._isLoading == value)
                    return;
                this._isLoading = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates that the application is single instanced. Default is false.
        /// </summary>
        public bool IsSingleInstance
        {
            get { return this._isSingleInstance; }
            set
            {
                if (this.isInitialized)
                    throw new NotSupportedException("The application is already initialized. This value cannot be changed");

                this._isSingleInstance = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates that the application is a single page application.
        /// This application will behave almost like a UWP app. Default is false.
        /// </summary>
        public bool IsSinglePage
        {
            get { return this._isSinglePage; }
            set
            {
                if (this.isInitialized)
                    throw new NotSupportedException("The application is already initialized. This value cannot be changed");

                this._isSinglePage = value;
            }
        }

        /// <summary>
        /// Gets the message dialog
        /// </summary>
        public IMessageDialog MessageDialog
        {
            get
            {
                if (this._messageDialog == null)
                    this._messageDialog = Factory.Create<IMessageDialog>();

                return this._messageDialog;
            }
        }

        /// <summary>
        /// Gets the window navigator
        /// </summary>
        public INavigator Navigator
        {
            get
            {
                if (this._navigator == null)
                    this._navigator = Factory.Create<INavigator>();

                return this._navigator;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the main window should be brought to front if a
        /// second instance of the application sends arguments. Default is true.
        /// </summary>
        public bool ShouldBringToFront { get; set; } = true;

        /// <summary>
        /// Gets or sets the URL protocol names which is used to register the Application to a URI Scheme
        /// </summary>
        public string[] UrlProtocolNames { get; set; }

        /// <summary>
        /// Centralized error handling
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> that occured</param>
        public virtual void OnException(Exception e)
        {
            throw e;
        }

        /// <summary>
        /// Invokes the <see cref="BehaviourInvoke"/> event
        /// </summary>
        /// <param name="behaviourName">The name of the behaviour to invoke</param>
        public async void RaiseNotifyBehaviourInvoke(string behaviourName) =>
            await this.Dispatcher.RunAsync(() => this.BehaviourInvoke?.Invoke(this, new BehaviourInvocationArgs(behaviourName)));

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        public async void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if ((propertyName != null && propertyName.EndsWith("Command")) || this.BeforeRaiseNotifyPropertyChanged(propertyName))
                return;

            await this.Dispatcher.RunAsync(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

            this.AfterRaiseNotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Occures after the event <see cref="PropertyChanged"/> has been invoked
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        protected virtual void AfterRaiseNotifyPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Occured before the <see cref="PropertyChanged"/> event is invoked.
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        /// <returns>
        /// Returns true if <see cref="RaisePropertyChanged(string)"/> should be cancelled. Otherwise false
        /// </returns>
        protected virtual bool BeforeRaiseNotifyPropertyChanged(string propertyName) => false;

        /// <summary>
        /// Occures if the application is activated by passing arguments from a second app instance.
        /// Will only occures if <see cref="IsSingleInstance"/> is true
        /// </summary>
        /// <param name="argument">The parameters passed from the second instance</param>
        protected virtual void OnActivated(string[] argument)
        {
        }

        /// <summary>
        /// Occures if the application is activated by a URI whose scheme name this app is registered
        /// to handle.
        /// </summary>
        /// <param name="uri">
        /// Gets the Uniform Resource Identifier (URI) for which the app was activated.
        /// </param>
        protected virtual void OnActivationProtocol(Uri uri)
        {
        }

        /// <summary>
        /// Occures on initialization of <see cref="ApplicationBase"/>
        /// </summary>
        protected virtual void OnConstruction()
        {
        }

        /// <summary>
        /// Occures on preload. Will only occures if <see cref="IsSingleInstance"/> is true
        /// </summary>
        protected virtual Task OnPreload() => Task.FromResult(0);

        /// <summary>
        /// Occures before loading XAML resources
        /// </summary>
        protected virtual void OnResourceLoad()
        {
        }

        /// <summary>
        /// Occures if the application is launched
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected abstract Task OnStartup(LaunchActivatedEventArgs e);

        /// <summary>
        /// Occures if the url protocoll requires registration.
        /// </summary>
        /// <param name="name">The application uri e.g. exampleApplication://</param>
        protected virtual void OnUrlProtocolRegistration(string name)
        {
            try
            {
                UrlProtocol.Register(name);
            }
            catch (UnauthorizedAccessException)
            {
                var location = Assembly.GetEntryAssembly().Location;

                var processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = Path.Combine(Path.GetDirectoryName(location), Path.GetFileName(location).Replace(".vshost.exe", ".exe"));
                processInfo.Arguments = "registerUriScheme";
                Process.Start(processInfo);
            }
        }

        private async void ApplicationBase_Startup(object sender, StartupEventArgs e)
        {
            this.isInitialized = true;

            if (this.UrlProtocolNames != null && this.UrlProtocolNames.Length > 0)
            {
                for (int i = 0; i < this.UrlProtocolNames.Length; i++)
                {
                    if (UrlProtocol.RequiresRegistration(this.UrlProtocolNames[i]))
                        this.OnUrlProtocolRegistration(this.UrlProtocolNames[i]);
                }

                if (e.Args.Length > 0 && e.Args[0] == "registerUriScheme")
                {
                    this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    this.Shutdown(0);
                    return;
                }
            }

            // If an application is being run by VS then it will have the .vshost suffix to its proc
            // name. We have to also check them
            var proc = Process.GetCurrentProcess();
            var processName = proc.ProcessName.Replace(".vshost", "");
            var processes = Process.GetProcesses().Where(x => (x.ProcessName == processName || x.ProcessName == proc.ProcessName || x.ProcessName == proc.ProcessName + ".vshost") && x.Id != proc.Id).ToArray();

            // Special case ... If we recieve a call from an uri protocol we will be passing this to
            // all instances of the application
            if (this.UrlProtocolNames != null &&
                this.UrlProtocolNames.Length > 0 &&
                e.Args.Length > 0 &&
                processes.Length > 0 &&
                this.IsUrlProtocol(e.Args))
            {
                foreach (var process in processes.Where(x => x.Id != proc.Id))
                    Win32Api.SendMessage(process.MainWindowHandle, $"{e.Args.Join("\n")}");

                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                this.Shutdown();
            }

            if (this.IsSingleInstance && processes.Length > 0)
            {
                var hwnd = processes[0].MainWindowHandle;

                if (hwnd != IntPtr.Zero)
                {
                    Win32Api.SendMessage(hwnd, $"{e.Args.Join("\n")}");
                    this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    this.Shutdown();
                }
            }
            else
            {
                if (this.IsSinglePage)
                {
                    WindowType windowType = null;
                    var window = Common.CreateWindow(ref windowType);

                    window.ContentTemplateSelector = new CauldronTemplateSelector();

                    window.MinHeight = 353;
                    window.MinWidth = 502;
                    window.ShowInTaskbar = true;
                    window.Topmost = false;
                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                    window.WindowState = WindowState.Normal;
                    window.Icon = await Win32Api.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location).ToBitmapImageAsync();
                    window.Title = ApplicationInfo.ApplicationName;

                    PersistentWindowInformation.Load(window, this.GetType());

                    window.SizeChanged += (s, e1) =>
                    {
                        if (window.WindowState == WindowState.Normal)
                        {
                            PersistentWindowProperties.SetHeight(window, e1.NewSize.Height);
                            PersistentWindowProperties.SetWidth(window, e1.NewSize.Width);
                        }
                    };
                    window.Closing += (s, e1) => PersistentWindowInformation.Save(window, this.GetType());

                    window.Show();

                    window.Content = this;
                    window.Activate();
                    await this.OnPreload();

                    var rootFrame = new NavigationFrame();
                    rootFrame.DataContext = this;
                    window.Content = rootFrame;
                    window.InputBindings.Add(new KeyBinding(new RelayCommand(async () => { await rootFrame.GoBack(); }, () => rootFrame.CanGoBack), Key.Back, ModifierKeys.None));
                }
                else if (this.GetType().GetCustomAttribute<ViewAttribute>() != null || Application.Current.Resources.Contains($"View_{this.GetType().Name}"))
                {
                    this.Navigator.As<Navigator>()?.NavigateInternal<ApplicationBase>(this, null, null);
                    await this.OnPreload();
                    Application.Current.MainWindow.Activate();
                }

                await this.OnStartup(new LaunchActivatedEventArgs(e.Args));
                this.Navigator.TryClose(this);

                if (Application.Current.MainWindow != null)
                {
                    HwndSource.FromHwnd(Application.Current.MainWindow.GetWindowHandle())?.AddHook(new HwndSourceHook(HandleMessages));
                    Application.Current.MainWindow.Activate();
                }
            }
        }

        private IntPtr HandleMessages(IntPtr handle, int message, IntPtr wParameter, IntPtr lParameter, ref Boolean handled)
        {
            var data = Win32Api.GetMessage(message, lParameter);

            if (data != null)
            {
                if (Application.Current.MainWindow == null)
                    return IntPtr.Zero;

                if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;

                if (this.ShouldBringToFront)
                    Win32Api.ActivateWindow(Application.Current.MainWindow.GetWindowHandle());

                var args = data.Split('\n');

                if (this.UrlProtocolNames != null &&
                    this.UrlProtocolNames.Length > 0 &&
                    args.Length > 0 &&
                    this.IsUrlProtocol(args))
                {
                    try
                    {
                        if (!WebAuthenticationBrokerWrapper.CallBack(args[0]))
                            this.OnActivationProtocol(new Uri(args[0]));
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else if (this.IsSingleInstance)
                    this.OnActivated(args);
                handled = true;
            }

            return IntPtr.Zero;
        }

        private bool IsUrlProtocol(string[] args)
        {
            for (int i = 0; i < this.UrlProtocolNames.Length; i++)
                if (args[0].StartsWith(this.UrlProtocolNames[i], StringComparison.CurrentCultureIgnoreCase))
                    return true;

            return false;
        }
    }
}