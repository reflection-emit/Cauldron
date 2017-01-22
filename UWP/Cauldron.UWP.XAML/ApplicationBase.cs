using Cauldron.Activator;
using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Localization;
using Cauldron.Potions;
using Cauldron.XAML.Controls;
using Cauldron.XAML.Navigation;
using Cauldron.XAML.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Cauldron.XAML
{
    /// <summary>
    /// Encapsulates the app and its available services.
    /// </summary>
    public abstract class ApplicationBase : Application, IViewModel
    {
        private DispatcherEx _dispatcher;
        private Guid? _id;
        private bool _isLoading = true;
        private IMessageDialog _messageDialog;
        private INavigator _navigator;
        private bool resourceLoaded = false;

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationBase"/>
        /// </summary>
        public ApplicationBase()
        {
            this.Suspending += OnSuspending;

            Assemblies.AddAssembly(typeof(ApplicationBase).GetTypeInfo().Assembly);
            Assemblies.AddAssembly(typeof(Locale).GetTypeInfo().Assembly);
            Assemblies.AddAssembly(typeof(Serializer).GetTypeInfo().Assembly);
            Assemblies.AddAssembly(typeof(Factory).GetTypeInfo().Assembly);
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
        /// Gets the <see cref="Dispatcher"/> this <see cref="DispatcherEx "/> is associated with.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DispatcherEx Dispatcher
        {
            get
            {
                if (this._dispatcher == null)
                    this._dispatcher = DispatcherEx.Current;

                return this._dispatcher;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to display frame-rate and per-frame
        /// CPU usage info. These display as an overlay of counters in the window chrome
        /// while the app runs.
        /// </summary>
        public bool EnableFrameRateCounter
        {
            get { return this.DebugSettings.EnableFrameRateCounter; }
            set { this.DebugSettings.EnableFrameRateCounter = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates, that back button should be handled automatically
        /// </summary>
        public bool HandleBackButton { get; set; } = true;

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
        /// <returns>Returns true if <see cref="RaisePropertyChanged(string)"/> should be cancelled. Otherwise false</returns>
        protected virtual bool BeforeRaiseNotifyPropertyChanged(string propertyName) => false;

        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                try
                {
                    var eventArgs = args as ProtocolActivatedEventArgs;
                    this.OnActivationProtocol(eventArgs.Uri);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            base.OnActivated(args);
        }

        /// <summary>
        /// Occures if the application is activated by a URI whose scheme name this app is registered to handle.
        /// </summary>
        /// <param name="uri">Gets the Uniform Resource Identifier (URI) for which the app was activated.</param>
        protected virtual void OnActivationProtocol(Uri uri)
        {
        }

        /// <summary>
        /// Occures if the app is launched after prelaunch has occured
        /// </summary>
        protected virtual Task OnAppVisible() => Task.FromResult(0);

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            this.LoadResources();

            if (e.PrelaunchActivated)
                await this.OnPrelaunchActivation();

            if (Assemblies.IsDebugging)
                this.EnableFrameRateCounter = true;

            var rootFrame = Window.Current.Content as NavigationFrame;

            if (rootFrame == null)
            {
                using (var p = new ExecutionTimer("Preload"))
                {
                    var contentControl = new ContentControl();
                    contentControl.ContentTemplateSelector = new CauldronTemplateSelector();
                    contentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
                    contentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    Window.Current.Content = contentControl;
                    contentControl.Content = this;
                    Window.Current.Activate();
                    await this.OnPreload();
                }
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                Window.Current.VisibilityChanged -= Current_VisibilityChanged;
                Window.Current.VisibilityChanged += Current_VisibilityChanged;

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new NavigationFrame();
                rootFrame.ContentTemplateSelector = new CauldronTemplateSelector();

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                rootFrame.Navigated += RootFrame_Navigated;
            }

            if (rootFrame.DataContext == null)
                rootFrame.DataContext = this;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                await this.OnLoadStateAsync(e);
                await rootFrame.Reload();
            }

            if (rootFrame.Content == null)
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation parameter
                await this.OnStartup(e);

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Load state from previously suspended application
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected virtual async Task OnLoadStateAsync(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as NavigationFrame;

            if (rootFrame == null)
                return;

            var backStack = await Serializer.DeserializeAsync(rootFrame.BackStack.GetType(), "State1") as IEnumerable<Navigation.PageStackEntry>;
            var forwardStack = await Serializer.DeserializeAsync(rootFrame.ForwardStack.GetType(), "State2") as IEnumerable<Navigation.PageStackEntry>;

            rootFrame.BackStack.AddRange(backStack);
            rootFrame.ForwardStack.AddRange(forwardStack);
        }

        /// <summary>
        /// Occures on prelaunch
        /// </summary>
        protected virtual Task OnPrelaunchActivation() => Task.FromResult(0);

        /// <summary>
        /// Occures on preload
        /// </summary>
        protected virtual Task OnPreload() => Task.FromResult(0);

        /// <summary>
        /// Occures before loading XAML resources
        /// </summary>
        protected virtual void OnResourceLoad()
        {
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="e">Details about the suspend request.</param>
        protected virtual async Task OnSaveStateAsync(SuspendingEventArgs e)
        {
            var rootFrame = Window.Current.Content as NavigationFrame;

            if (rootFrame == null)
                return;

            try
            {
                await Serializer.SerializeAsync(rootFrame.BackStack, "State1");
                await Serializer.SerializeAsync(rootFrame.ForwardStack, "State2");
            }
            catch (Exception ex)
            {
                Output.WriteLineError(ex.Message);
            }
        }

        /// <summary>
        /// Occures if the application is launched
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected abstract Task OnStartup(LaunchActivatedEventArgs e);

        private async void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
        {
            if (e.Visible)
                await this.OnAppVisible();

            this.LoadResources();
        }

        private void LoadResources()
        {
            using (var m = new ExecutionTimer())
            {
                if (this.resourceLoaded)
                    return;

                this.resourceLoaded = true;
                this.OnResourceLoad();

                // Add the custom template selector to the resources
                this.Resources.Add(typeof(CauldronTemplateSelector).Name, new CauldronTemplateSelector());

                // Add all Value converters to the dictionary
                foreach (var valueConverter in Assemblies.ExportedTypes.Where(x => !x.ContainsGenericParameters && !x.IsAbstract && x.ImplementsInterface<IValueConverter>()))
                    if (!Application.Current.Resources.ContainsKey(valueConverter.Name))
                        Application.Current.Resources.Add(valueConverter.Name, System.Activator.CreateInstance(valueConverter.AsType()));

                // find all resourcedictionaries and add them to the existing resources
                var resourceDictionaries = Assemblies.ExportedTypes.Where(x => x.IsSubclassOf(typeof(ResourceDictionary)));
                var cauldronDictionaries = resourceDictionaries.Where(x => x.Assembly.FullName.StartsWith("Cauldron.")).OrderBy(x => x.Name);
                var otherDictionaries = resourceDictionaries.Where(x => !x.Assembly.FullName.StartsWith("Cauldron.")).OrderBy(x => x.Name);

                // add all cauldron dictionaries first
                foreach (var item in cauldronDictionaries)
                {
                    var dictionary = System.Activator.CreateInstance(item.AsType()) as ResourceDictionary;
                    Application.Current.Resources.MergedDictionaries.Add(dictionary);
                }
                // Them then others
                foreach (var item in otherDictionaries)
                {
                    var dictionary = System.Activator.CreateInstance(item.AsType()) as ResourceDictionary;
                    Application.Current.Resources.MergedDictionaries.Add(dictionary);
                }
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await this.OnSaveStateAsync(e);
            deferral.Complete();
        }

        private void RootFrame_Navigated(object sender, EventArgs e)
        {
            if (!this.HandleBackButton)
                return;

            var rootFrame = sender as NavigationFrame;
            rootFrame.BackButtonVisible = rootFrame.CanGoBack;
        }
    }
}