using Cauldron;
using Cauldron.Activator;
using Cauldron.Core.Reflection;
using Cauldron.Core.Threading;
using Cauldron.XAML.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Cauldron.Cryptography;
using Cauldron.XAML.Navigation;

namespace Cauldron.XAML
{
    using Cauldron.Core.Diagnostics;
    using Cauldron.XAML.Controls;

    /// <summary>
    /// Encapsulates a Windows Presentation Foundation (WPF) application.
    /// </summary>
    public abstract class ApplicationBase : Application, IViewModel
    {
        private readonly string applicationHash;
        private IDispatcher _dispatcher;
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

            this.OnConstruction();
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            //Assemblies.LoadAssembly();
            this.OnResourceLoad();

            // Add the custom template selector to the resources
            this.Resources.Add(typeof(CauldronTemplateSelector).Name, new CauldronTemplateSelector());
            // Add some application information also as resources
            this.Resources.Add(nameof(ApplicationInfo.ApplicationName), ApplicationInfo.ApplicationName);
            this.Resources.Add(nameof(ApplicationInfo.ApplicationPublisher), ApplicationInfo.ApplicationPublisher);
            this.Resources.Add(nameof(ApplicationInfo.ApplicationVersion), ApplicationInfo.ApplicationVersion);
            this.Resources.Add(nameof(ApplicationInfo.ProductName), ApplicationInfo.ProductName);
            this.Resources.Add(nameof(ApplicationInfo.Description), ApplicationInfo.Description);

            // Add all Value converters to the dictionary
            Assemblies.CauldronObjects
                .Select(x => x as IFactoryCache)
                .Where(x => x != null)
                .SelectMany(x => x.GetComponents())
                .Where(x => x.ContractName == typeof(IValueConverter).FullName || x.ContractName == typeof(IMultiValueConverter).FullName)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.Type.FullName)
                .Foreach(x =>
                {
                    var name = x.Type.Name;

                    if (!this.Resources.Contains(name))
                        this.Resources.Add(name, x.CreateInstance());
                    else
                        Debug.WriteLine("ERROR: Multiple ValueConverters with the same name found: " + name);
                });

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
                    if (x.FactoryInfo.CreateInstance() is ResourceDictionary instance)
                    {
                        this.Resources.MergedDictionaries.Add(instance);
                        Debug.WriteLine($"Adding ResourceDictionary: {x.Type.FullName}");
                    }
                });

            this.applicationHash = (ApplicationInfo.ApplicationName + ApplicationInfo.ApplicationPublisher + ApplicationInfo.ApplicationVersion.ToString()).GetHash(HashAlgorithms.Md5);
        }

        /// <summary>
        /// Occures if a behaviour should be invoked
        /// </summary>
        public event EventHandler<BehaviourInvocationArgs> BehaviourInvoke;

        /// <summary>
        /// Occures if the <see cref="IsLoading"/> property has changed.
        /// </summary>
        public event EventHandler IsLoadingChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the application splash screen image. This is only neccessary if the property <see cref="IsSinglePage"/> is set to true
        /// </summary>
        public ImageSource ApplicationSplash { get; set; }

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="DispatcherEx "/> is associated with.
        /// </summary>
        public new IDispatcher Dispatcher
        {
            get
            {
                if (this._dispatcher == null)
                    this._dispatcher = Factory.Create<IDispatcher>();

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
                this.IsLoadingChanged?.Invoke(this, EventArgs.Empty);
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
        /// Occures on initialization of <see cref="ApplicationBase"/>
        /// </summary>
        protected virtual void OnConstruction()
        {
        }

        /// <summary>
        /// Occures on preload. Will only occures if <see cref="IsSingleInstance"/> is true or if the inheriting class has a View.
        /// The view can be added by the <see cref="ViewAttribute"/> or as a DataTemplate with the correct naming nomenclature.
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

        private async void ApplicationBase_Startup(object sender, StartupEventArgs e)
        {
            this.isInitialized = true;

            ParamPassing.Configure(e.Args, x =>
            {
                x.IsSingleInstance = this.IsSingleInstance;
                x.BringToFront = this.ShouldBringToFront;
                x.RandomSelectInstance = true;
                x.DataSeparator = '\n';

                x.ParameterPassedCallback = new Action<string[]>(args => this.OnActivated(args));

                x.ExitDelegate = () =>
                {
                    this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    this.Shutdown();
                };
            });

            if (this.IsSingleInstance && ParamPassing.AreOtherInstanceActive)
            {
                if (ParamPassing.BringToFront())
                {
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
                    window.Icon = await UnsafeNative.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location).ToBitmapImageAsync();
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
                    var oldShutdownMode = Application.Current.ShutdownMode;
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    this.Navigator.As<Navigator>()?.NavigateInternal<ApplicationBase>(this, null, null);
                    await this.OnPreload();

                    if (Application.Current.MainWindow == null)
                    {
                        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                        Application.Current.Shutdown();
                        return;
                    }

                    Application.Current.MainWindow.Activate();
                    this.Navigator.TryClose(this);
                    Application.Current.ShutdownMode = oldShutdownMode;
                }

                await this.OnStartup(new LaunchActivatedEventArgs(e.Args));

                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.AddHookParameterPassing();
                    Application.Current.MainWindow.Activate();
                }
            }
        }
    }
}