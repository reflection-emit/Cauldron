using Cauldron.Controls;
using Cauldron.Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Cauldron
{
    /// <summary>
    /// Encapsulates the app and its available services
    /// </summary>
    public abstract class CauldronApplication : Application, INotifyPropertyChanged, INotifyBehaviourInvocation
    {
        private Visibility _statusBarVisibility;

        /// <summary>
        /// Initializes a new instance of the <see cref="CauldronApplication"/>
        /// </summary>
        public CauldronApplication()
        {
            this.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            this.Resources.Add(nameof(ThemeAccentColor), Colors.SteelBlue);
            this.OnConstruction();

            // Add the custom template selector to the resources
            this.Resources.Add(typeof(CauldronTemplateSelector).Name, new CauldronTemplateSelector());

            // Add all Value converters to the dictionary
            foreach (var valueConverter in AssemblyUtil.ExportedTypes.Where(x => x.ImplementsInterface<IValueConverter>()))
                this.Resources.Add(valueConverter.Name, Activator.CreateInstance(valueConverter.AsType()));

            // find all resourcedictionaries and add them to the existing resources
            foreach (var resourceDictionary in AssemblyUtil.ExportedTypes
                    .Where(x => x.IsSubclassOf(typeof(ResourceDictionary)))
                    .OrderByDescending(x => x.Assembly.FullName.StartsWith("Cauldron.")))
                this.Resources.MergedDictionaries.Add(Activator.CreateInstance(resourceDictionary.AsType()) as ResourceDictionary);
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
        /// Gets the Application object for the current application.
        /// </summary>
        public static new CauldronApplication Current
        {
            get { return Application.Current as CauldronApplication; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a back button is shown in the system UI.
        /// </summary>
        public AppViewBackButtonVisibility AppViewBackButtonVisibility
        {
            get { return SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility; }
            set { SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = value; }
        }

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="CauldronDispatcher "/> is associated with.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced), JsonIgnore]
        public CauldronDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the status bar is shown or hidden
        /// </summary>
        public Visibility StatusBarVisibility
        {
            get { return this._statusBarVisibility; }
            set
            {
                if (value == Visibility.Visible)
                    this.StatusBarShowAsync();
                else
                    this.StatusBarHideAsync();

                this._statusBarVisibility = value;
            }
        }

        /// <summary>
        /// Gets or sets the Cauldron theme accent color
        /// <para/>
        /// There is no garantee that the used theme supports the accent color
        /// </summary>
        public Color ThemeAccentColor
        {
            get { return (Color)this.Resources[nameof(ThemeAccentColor)]; }
            set { this.Resources[nameof(ThemeAccentColor)] = value; }
        }

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        public async void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
                await this.Dispatcher.RunAsync(() => this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
        }

        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
        }

        /// <summary>
        /// Occures on initialization of <see cref="CauldronApplication"/>
        /// </summary>
        protected virtual void OnConstruction()
        {
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = Windows.UI.Xaml.Window.Current.Content as NavigationFrame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new NavigationFrame();

                // Place the frame in the current Window
                Windows.UI.Xaml.Window.Current.Content = rootFrame;
                // assign datacontext to rootframe
                rootFrame.DataContext = this;
            }

            base.OnLaunched(args);
        }

        /// <summary>
        /// Invokes the <see cref="BehaviourInvoke"/> event
        /// </summary>
        /// <param name="behaviourName">The name of the behaviour to invoke</param>
        protected async void RaiseNotifyBehaviourInvoke(string behaviourName)
        {
            if (this.BehaviourInvoke != null)
                await this.Dispatcher.RunAsync(() => this.BehaviourInvoke(this, new BehaviourInvocationArgs(behaviourName)));
        }

        private async void StatusBarHideAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }
        }

        private async void StatusBarShowAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }
        }
    }
}